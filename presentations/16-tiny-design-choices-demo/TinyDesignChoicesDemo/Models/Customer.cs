namespace TinyDesignChoicesDemo.Models;

/// <summary>
/// Customer with correct Equals/GetHashCode based on immutable Id.
/// </summary>
public class Customer : IEquatable<Customer>
{
    public int Id { get; init; }
    public string Name { get; set; } = "";

    public override bool Equals(object? obj) => Equals(obj as Customer);

    public bool Equals(Customer? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Customer? left, Customer? right)
        => Equals(left, right);

    public static bool operator !=(Customer? left, Customer? right)
        => !Equals(left, right);

    public override string ToString() => $"Customer(Id={Id}, Name={Name})";
}

/// <summary>
/// Customer with BROKEN GetHashCode that includes the mutable Name field.
/// Demonstrates the "lost in HashSet" bug.
/// </summary>
public class BrokenCustomer
{
    public int Id { get; init; }
    public string Name { get; set; } = "";

    public override bool Equals(object? obj)
    {
        if (obj is not BrokenCustomer other) return false;
        return Id == other.Id && Name == other.Name;
    }

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    public override string ToString() => $"BrokenCustomer(Id={Id}, Name={Name})";
}
