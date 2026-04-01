using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using backend.Models;
using backend.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace backend.Services;

public sealed class RabbitChatBus : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitChatBus> _logger;
    private readonly SemaphoreSlim _connectionGate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private IConnection? _connection;

    public RabbitChatBus(IOptions<RabbitMqOptions> options, ILogger<RabbitChatBus> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using var channel = await CreateChannelAsync(cancellationToken);
        await channel.ExchangeDeclareAsync(
            _options.ExchangeName,
            ExchangeType.Fanout,
            durable: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public async Task EnsureSessionQueueAsync(string queueName, CancellationToken cancellationToken)
    {
        await using var channel = await CreateChannelAsync(cancellationToken);

        await channel.ExchangeDeclareAsync(
            _options.ExchangeName,
            ExchangeType.Fanout,
            durable: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queueName,
            _options.ExchangeName,
            string.Empty,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteQueueAsync(string queueName, CancellationToken cancellationToken)
    {
        await using var channel = await CreateChannelAsync(cancellationToken);

        try
        {
            await channel.QueueDeleteAsync(
                queueName,
                ifUnused: false,
                ifEmpty: false,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Queue {QueueName} could not be deleted cleanly.", queueName);
        }
    }

    public async Task PublishAsync(ChatEnvelope message, CancellationToken cancellationToken)
    {
        await using var channel = await CreateChannelAsync(cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message, _jsonOptions);
        var properties = new BasicProperties
        {
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            _options.ExchangeName,
            string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async Task StreamQueueToSseAsync(
        string queueName,
        HttpResponse response,
        CancellationToken cancellationToken)
    {
        await using var channel = await CreateChannelAsync(cancellationToken);
        await channel.QueueDeclarePassiveAsync(queueName, cancellationToken: cancellationToken);

        var deliveries = Channel.CreateUnbounded<QueuedDelivery>();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var copiedBody = ea.Body.ToArray();
            await deliveries.Writer.WriteAsync(
                new QueuedDelivery(copiedBody, ea.DeliveryTag),
                cancellationToken);
        };

        var consumerTag = await channel.BasicConsumeAsync(
            queueName,
            autoAck: false,
            consumer,
            cancellationToken);

        await WriteSseEventAsync(response, "ready", "{\"ok\":true}", cancellationToken);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var ready = await deliveries.Reader
                        .WaitToReadAsync(cancellationToken)
                        .AsTask()
                        .WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);

                    if (!ready)
                    {
                        break;
                    }

                    while (deliveries.Reader.TryRead(out var delivery))
                    {
                        var payload = Encoding.UTF8.GetString(delivery.Body);
                        await WriteSseEventAsync(response, "message", payload, cancellationToken);
                        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, cancellationToken);
                    }
                }
                catch (TimeoutException)
                {
                    await response.WriteAsync(": keep-alive\n\n", cancellationToken);
                    await response.Body.FlushAsync(cancellationToken);
                }
            }
        }
        finally
        {
            try
            {
                await channel.BasicCancelAsync(consumerTag, noWait: false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Consumer {ConsumerTag} cancellation was not clean.", consumerTag);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _connectionGate.Dispose();
    }

    private async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionGate.WaitAsync(cancellationToken);

        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = _options.NetworkRecoveryInterval,
                ConsumerDispatchConcurrency = 1,
                ClientProvidedName = "wacky-chat-backend"
            };

            const int maxAttempts = 20;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken);
                    return _connection;
                }
                catch when (attempt < maxAttempts)
                {
                    _logger.LogInformation(
                        "RabbitMQ connection attempt {Attempt}/{MaxAttempts} failed. Retrying...",
                        attempt,
                        maxAttempts);

                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
            }

            throw new InvalidOperationException("RabbitMQ connection retries were exhausted.");
        }
        finally
        {
            _connectionGate.Release();
        }
    }

    private static async Task WriteSseEventAsync(
        HttpResponse response,
        string eventName,
        string data,
        CancellationToken cancellationToken)
    {
        await response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await response.WriteAsync($"data: {data}\n\n", cancellationToken);
        await response.Body.FlushAsync(cancellationToken);
    }

    private sealed record QueuedDelivery(byte[] Body, ulong DeliveryTag);
}
