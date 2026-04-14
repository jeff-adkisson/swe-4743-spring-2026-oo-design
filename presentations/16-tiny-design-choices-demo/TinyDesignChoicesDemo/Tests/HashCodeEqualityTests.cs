using TinyDesignChoicesDemo.Models;

namespace TinyDesignChoicesDemo.Tests;

public class HashCodeEqualityTests
{
    [Fact]
    public void Customer_EqualById()
    {
        var a = new Customer { Id = 1, Name = "Alice" };
        var b = new Customer { Id = 1, Name = "Alice Smith" };

        // Equal because equality is based on Id only
        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void Customer_SameHashCodeWhenEqual()
    {
        var a = new Customer { Id = 1, Name = "Alice" };
        var b = new Customer { Id = 1, Name = "Alice Smith" };

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Customer_StableInHashSet_AfterMutation()
    {
        var set = new HashSet<Customer>();
        var alice = new Customer { Id = 1, Name = "Alice" };
        set.Add(alice);

        // Mutate the name — hash code is based on Id, so it does NOT change
        alice.Name = "Alice Smith";

        // Still found in the set because GetHashCode uses only Id
        Assert.Contains(alice, set);
    }

    [Fact]
    public void Customer_FoundInDictionary_AfterMutation()
    {
        var dict = new Dictionary<Customer, string>();
        var alice = new Customer { Id = 1, Name = "Alice" };
        dict[alice] = "VIP";

        alice.Name = "Alice Smith";

        // Still found because hash code is based on immutable Id
        Assert.Equal("VIP", dict[alice]);
    }

    [Fact]
    public void BrokenCustomer_LostInHashSet_AfterMutation()
    {
        var set = new HashSet<BrokenCustomer>();
        var alice = new BrokenCustomer { Id = 1, Name = "Alice" };
        set.Add(alice);

        // Mutate the name — hash code CHANGES because it includes Name
        alice.Name = "Alice Smith";

        // The object is physically in the set, but Contains returns false!
        // This is the "phantom lost object" bug.
        Assert.DoesNotContain(alice, set);
    }

    [Fact]
    public void BrokenCustomer_LostInDictionary_AfterMutation()
    {
        var dict = new Dictionary<BrokenCustomer, string>();
        var alice = new BrokenCustomer { Id = 1, Name = "Alice" };
        dict[alice] = "VIP";

        alice.Name = "Alice Smith";

        // Key lookup fails — the object is in the wrong bucket
        Assert.False(dict.ContainsKey(alice));
    }

    [Fact]
    public void BrokenCustomer_SetCountStillShowsObject()
    {
        var set = new HashSet<BrokenCustomer>();
        var alice = new BrokenCustomer { Id = 1, Name = "Alice" };
        set.Add(alice);

        alice.Name = "Alice Smith";

        // The set still has 1 element, but you cannot find it by lookup
        Assert.Single(set);
        Assert.DoesNotContain(alice, set);
    }

    [Fact]
    public void Record_AutoGeneratesCorrectEquality()
    {
        // C# records generate Equals and GetHashCode automatically
        var a = new RecordCustomer(1, "Alice");
        var b = new RecordCustomer(1, "Alice");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}

public record RecordCustomer(int Id, string Name);
