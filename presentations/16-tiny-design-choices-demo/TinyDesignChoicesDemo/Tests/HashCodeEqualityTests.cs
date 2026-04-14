using TinyDesignChoicesDemo.Models;
using Xunit.Abstractions;

namespace TinyDesignChoicesDemo.Tests;

public class HashCodeEqualityTests
{
    private readonly ITestOutputHelper _out;

    public HashCodeEqualityTests(ITestOutputHelper output) => _out = output;

    [Fact]
    public void Customer_EqualById()
    {
        var a = new Customer { Id = 1, Name = "Alice" };
        var b = new Customer { Id = 1, Name = "Alice Smith" };

        _out.WriteLine("Customer.Equals is based on Id only — Name is ignored.");
        _out.WriteLine($"  a = {a}");
        _out.WriteLine($"  b = {b}");
        _out.WriteLine($"  a == b?  {a == b}   (true — same Id)");
        _out.WriteLine("For entities, equality usually means 'same identity,' not 'same state.'");

        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void Customer_SameHashCodeWhenEqual()
    {
        var a = new Customer { Id = 1, Name = "Alice" };
        var b = new Customer { Id = 1, Name = "Alice Smith" };

        _out.WriteLine("Contract: equal objects MUST have equal hash codes.");
        _out.WriteLine($"  a.GetHashCode() = {a.GetHashCode()}");
        _out.WriteLine($"  b.GetHashCode() = {b.GetHashCode()}");
        _out.WriteLine($"  Match? {a.GetHashCode() == b.GetHashCode()}");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Customer_StableInHashSet_AfterMutation()
    {
        var set = new HashSet<Customer>();
        var alice = new Customer { Id = 1, Name = "Alice" };
        set.Add(alice);

        var hashBefore = alice.GetHashCode();
        _out.WriteLine($"Inserted {alice} (hash = {hashBefore}).");

        alice.Name = "Alice Smith";
        var hashAfter = alice.GetHashCode();
        _out.WriteLine($"Mutated Name → now {alice} (hash = {hashAfter}).");
        _out.WriteLine($"Hash unchanged: {hashBefore == hashAfter}   (GetHashCode only uses the immutable Id).");
        _out.WriteLine($"set.Contains(alice)? {set.Contains(alice)}   (expected: True — still findable)");

        Assert.Contains(alice, set);
    }

    [Fact]
    public void Customer_FoundInDictionary_AfterMutation()
    {
        var dict = new Dictionary<Customer, string>();
        var alice = new Customer { Id = 1, Name = "Alice" };
        dict[alice] = "VIP";

        _out.WriteLine($"Stored dict[{alice}] = \"VIP\".");

        alice.Name = "Alice Smith";
        _out.WriteLine($"Mutated Name → {alice}.");
        _out.WriteLine($"dict[alice] = \"{dict[alice]}\"   (still findable — hash based on immutable Id)");

        Assert.Equal("VIP", dict[alice]);
    }

    [Fact]
    public void BrokenCustomer_LostInHashSet_AfterMutation()
    {
        var set = new HashSet<BrokenCustomer>();
        var alice = new BrokenCustomer { Id = 1, Name = "Alice" };
        set.Add(alice);

        var hashBefore = alice.GetHashCode();
        _out.WriteLine("*** DEMONSTRATING THE 'PHANTOM LOST OBJECT' BUG ***");
        _out.WriteLine($"Inserted {alice} into HashSet (hash = {hashBefore}).");
        _out.WriteLine($"Set count: {set.Count}");

        alice.Name = "Alice Smith";
        var hashAfter = alice.GetHashCode();

        _out.WriteLine($"Mutated Name → now {alice} (hash = {hashAfter}).");
        _out.WriteLine($"Hash CHANGED: {hashBefore} → {hashAfter}   (BrokenCustomer includes Name in GetHashCode).");
        _out.WriteLine($"Set still has {set.Count} item — the object is physically there.");
        _out.WriteLine($"But set.Contains(alice)? {set.Contains(alice)}   (FALSE — looking in wrong bucket!)");
        _out.WriteLine("This is the silent bug: the object exists in the set but cannot be found.");

        Assert.DoesNotContain(alice, set);
    }

    [Fact]
    public void BrokenCustomer_LostInDictionary_AfterMutation()
    {
        var dict = new Dictionary<BrokenCustomer, string>();
        var alice = new BrokenCustomer { Id = 1, Name = "Alice" };
        dict[alice] = "VIP";

        _out.WriteLine($"Stored dict[{alice}] = \"VIP\".");
        alice.Name = "Alice Smith";
        _out.WriteLine($"Mutated Name → {alice}.");
        _out.WriteLine($"dict.ContainsKey(alice)? {dict.ContainsKey(alice)}   (FALSE — bucket lookup failed)");
        _out.WriteLine("The entry is still in the dictionary, but the new hash points to the wrong bucket.");

        Assert.False(dict.ContainsKey(alice));
    }

    [Fact]
    public void BrokenCustomer_SetCountStillShowsObject()
    {
        var set = new HashSet<BrokenCustomer>();
        var alice = new BrokenCustomer { Id = 1, Name = "Alice" };
        set.Add(alice);

        alice.Name = "Alice Smith";

        _out.WriteLine("Paradox that makes this bug hard to diagnose:");
        _out.WriteLine($"  set.Count           = {set.Count}   (the object IS in the set)");
        _out.WriteLine($"  set.Contains(alice) = {set.Contains(alice)}  (but Contains says no)");
        _out.WriteLine("Symptoms look like 'my object disappeared' — the real cause is a violated hash code contract.");

        Assert.Single(set);
        Assert.DoesNotContain(alice, set);
    }

    [Fact]
    public void Record_AutoGeneratesCorrectEquality()
    {
        var a = new RecordCustomer(1, "Alice");
        var b = new RecordCustomer(1, "Alice");

        _out.WriteLine("C# records auto-generate Equals and GetHashCode from ALL declared fields.");
        _out.WriteLine($"  a = {a}");
        _out.WriteLine($"  b = {b}");
        _out.WriteLine($"  a.Equals(b)                = {a.Equals(b)}");
        _out.WriteLine($"  a.GetHashCode() == b.GetHashCode()? {a.GetHashCode() == b.GetHashCode()}");
        _out.WriteLine("For immutable value objects, this is exactly what you want — no manual overrides needed.");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}

public record RecordCustomer(int Id, string Name);
