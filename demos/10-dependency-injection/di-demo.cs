#!/usr/bin/dotnet run
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.DependencyInjection.Abstractions@10.0.0

using System;
using Microsoft.Extensions.DependencyInjection;

public interface IAuditClock
{
    DateTime UtcNow { get; }
}

public sealed class SystemAuditClock : IAuditClock
{
    public SystemAuditClock()
    {
        Console.WriteLine("Constructed: SystemAuditClock (Singleton)");
    }

    public DateTime UtcNow => DateTime.UtcNow;
}

public interface IAuditLogger
{
    void Write(string message);
}

public sealed class ConsoleAuditLogger : IAuditLogger
{
    private readonly IAuditClock _clock;

    public ConsoleAuditLogger(IAuditClock clock)
    {
        _clock = clock;
        Console.WriteLine("Constructed: ConsoleAuditLogger (depends on IAuditClock)");
    }

    public void Write(string message)
    {
        Console.WriteLine($"[{_clock.UtcNow:O}] AUDIT {message}");
    }
}

public interface IComplianceReportGenerator
{
    string Generate(string orderId);
}

public sealed class ComplianceReportGenerator : IComplianceReportGenerator
{
    private readonly IAuditLogger _auditLogger;

    public ComplianceReportGenerator(IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
        Console.WriteLine("Constructed: ComplianceReportGenerator (depends on IAuditLogger)");
    }

    public string Generate(string orderId)
    {
        string reportId = $"RPT-{orderId}";
        _auditLogger.Write($"Generated compliance report {reportId}");
        return reportId;
    }
}

public sealed class NightlyComplianceJob
{
    private readonly IComplianceReportGenerator _generator;

    public NightlyComplianceJob(IComplianceReportGenerator generator)
    {
        _generator = generator;
        Console.WriteLine("Constructed: NightlyComplianceJob (depends on IComplianceReportGenerator)");
    }

    public void Run(string orderId)
    {
        string reportId = _generator.Generate(orderId);
        Console.WriteLine($"Job completed with {reportId}");
    }
}

public static class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IAuditClock, SystemAuditClock>();
        services.AddTransient<IAuditLogger, ConsoleAuditLogger>();
        services.AddTransient<IComplianceReportGenerator, ComplianceReportGenerator>();
        services.AddTransient<NightlyComplianceJob>();

        using ServiceProvider provider = services.BuildServiceProvider();

        Console.WriteLine("Resolving NightlyComplianceJob...");
        NightlyComplianceJob job = provider.GetRequiredService<NightlyComplianceJob>();

        Console.WriteLine("Running job...");
        job.Run("ORD-2026-00042");
    }
}