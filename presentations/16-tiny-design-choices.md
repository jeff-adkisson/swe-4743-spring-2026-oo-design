# Tiny Design Choices, Massive Consequences

### Four Habits That Separate Fragile Code from Professional-Grade Systems

##### [Powerpoint Presentation](16-tiny-design-choices.pptx) | [PDF](16-tiny-design-choices.pdf) | [Video](16-tiny-design-choices.mp4)

---

Every production outage has a root cause. Sometimes it is a missing feature or a broken algorithm, but surprisingly often the root cause is something small — a raw `string` where a type should have been, a default `ToString` that made a log unreadable, a hash code that silently shifted after insertion, or a swallowed exception that hid the real failure for months.

These are not exotic design problems. They are **tiny choices** that developers make dozens of times per day, usually on autopilot. When made well, the codebase stays clean, debuggable, and predictable. When made poorly, the consequences are disproportionately large: data corruption, phantom bugs, security leaks, and hours of wasted debugging time.

This lecture examines four of these choices:

```mermaid
flowchart LR
    A["Raw Primitives\n(Primitive Obsession)"] --> BUG["Production\nBug 💥"]
    B["Missing ToString\nOverride"] --> BUG
    C["Broken Hash Code /\nEquality Contract"] --> BUG
    D["Swallowed or Leaked\nExceptions"] --> BUG
```

Each topic is self-contained, but they reinforce each other. Value objects solve primitive obsession **and** give you a natural place for `ToString` and `GetHashCode`. Proper equality contracts prevent phantom collection bugs. Good exception handling keeps error information where it belongs — in the server log, not in the client response.

> The difference between a junior developer and a senior one is not the patterns they know — it is the small habits they have internalized.

---
## Table of Contents

- [1. Value Objects and the Primitive Obsession Anti-Pattern](#1-value-objects-and-the-primitive-obsession-anti-pattern)
- [2. Overriding ToString (and Its Equivalents)](#2-overriding-tostring-and-its-equivalents)
- [3. GetHashCode, hashCode, and the Equality Contract](#3-gethashcode-hashcode-and-the-equality-contract)
- [4. Exception Handling Done Right](#4-exception-handling-done-right)
- [5. Connecting the Dots](#5-connecting-the-dots)
- [Appendix A: Language Comparison Quick Reference](#appendix-a-language-comparison-quick-reference)
- [Appendix B: Logging Client-Side Errors on the Server](#appendix-b-logging-client-side-errors-on-the-server)

---
## 1. Value Objects and the Primitive Obsession Anti-Pattern

### The Bug

A support ticket arrives: *"Customer received someone else's order confirmation email."*

After hours of investigation, the team finds this method signature:

```csharp
// C#
public void SendConfirmation(string email, string orderId, string customerId)
{
    // ...
}
```

Somewhere upstream, a caller swapped the arguments:

```csharp
SendConfirmation(customer.Id, order.Id, customer.Email); // compiles fine, runs wrong
```

The compiler cannot help. Every parameter is a `string`. The type system sees no difference between an email address, an order ID, and a customer ID. This is **primitive obsession**.

### What Is Primitive Obsession?

Primitive obsession is a code smell where domain concepts are represented using built-in primitive types (`string`, `int`, `double`) instead of small, purpose-built types. Symptoms include:

- **Method signatures full of strings**: `void Register(string name, string email, string phone)`
- **Validation scattered everywhere**: The same email regex appears in six different files
- **Meaningless comparisons**: Nothing stops you from comparing an email to a phone number
- **Silent corruption**: Invalid values propagate unchecked until they cause damage far from the source

```mermaid
flowchart LR
    RAW["Raw string email"] --> V1["Validate in Controller"]
    RAW --> V2["Validate in Service"]
    RAW --> V3["Validate in Repository"]
    RAW --> V4["Forget to validate somewhere"]
    V4 --> BUG["Invalid data in database"]
```

### Value Objects as the Cure

A **value object** is a small, immutable type that:

1. **Wraps** a primitive (or a small group of primitives)
2. **Validates** at construction — if it exists, it is valid
3. **Compares by value** — two `EmailAddress` objects with the same address are equal
4. **Is immutable** — once created, it cannot change

> If it exists, it is valid. If two instances hold the same data, they are equal. If you need a different value, create a new instance.

### EmailAddress — Three Languages

**C#** — using a `record` for automatic value equality:

```csharp
public sealed record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        if (!value.Contains('@'))
            throw new ArgumentException($"'{value}' is not a valid email.", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;
}
```

**Java** — using a `record` (Java 16+):

```java
public record EmailAddress(String value) {

    public EmailAddress {
        if (value == null || value.isBlank())
            throw new IllegalArgumentException("Email cannot be empty.");
        if (!value.contains("@"))
            throw new IllegalArgumentException("'" + value + "' is not a valid email.");
        value = value.trim().toLowerCase();
    }

    @Override
    public String toString() {
        return value;
    }
}
```

**TypeScript** — using an immutable class with a factory method:

```typescript
export class EmailAddress {
    private constructor(public readonly value: string) {}

    static create(raw: string): EmailAddress {
        if (!raw || !raw.trim())
            throw new Error("Email cannot be empty.");
        if (!raw.includes("@"))
            throw new Error(`'${raw}' is not a valid email.`);
        return new EmailAddress(raw.trim().toLowerCase());
    }

    equals(other: EmailAddress): boolean {
        return this.value === other.value;
    }

    toString(): string {
        return this.value;
    }
}
```

Now the original bug is impossible:

```csharp
public void SendConfirmation(EmailAddress email, OrderId orderId, CustomerId customerId)
{
    // swapping arguments is now a compile-time error
}
```

### Money — A Value Object With Multiple Fields

Value objects are not limited to wrapping a single primitive. `Money` wraps an amount and a currency, and its invariant prevents mixing currencies:

```csharp
// C#
public sealed record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot add {Currency} to {other.Currency}.");
        return this with { Amount = Amount + other.Amount };
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
```

```java
// Java
public record Money(BigDecimal amount, String currency) {

    public Money add(Money other) {
        if (!currency.equals(other.currency()))
            throw new IllegalArgumentException(
                "Cannot add " + currency + " to " + other.currency());
        return new Money(amount.add(other.amount()), currency);
    }

    @Override
    public String toString() {
        return amount.setScale(2) + " " + currency;
    }
}
```

```typescript
// TypeScript
export class Money {
    private constructor(
        public readonly amount: number,
        public readonly currency: string
    ) {}

    static of(amount: number, currency: string): Money {
        return new Money(amount, currency);
    }

    add(other: Money): Money {
        if (this.currency !== other.currency)
            throw new Error(`Cannot add ${this.currency} to ${other.currency}`);
        return Money.of(this.amount + other.amount, this.currency);
    }

    toString(): string {
        return `${this.amount.toFixed(2)} ${this.currency}`;
    }
}
```

### DateRange — An Invariant Between Two Fields

```csharp
// C#
public sealed record DateRange(DateOnly Start, DateOnly End)
{
    public DateRange
    {
        if (Start > End)
            throw new ArgumentException(
                $"Start ({Start}) must not be after End ({End}).");
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;

    public override string ToString() => $"{Start} to {End}";
}
```

### Validation at the Boundary

The key insight is that validation happens **once**, at construction. Everything downstream can trust the type:

```mermaid
flowchart LR
    INPUT["Raw input\n(string, int, JSON)"] -->|"validate + wrap"| VO["Value Object"]
    VO -->|"trusted everywhere"| SVC["Service Layer"]
    VO --> REPO["Repository"]
    VO --> LOG["Logger"]
```

Compare this to the earlier diagram where validation was scattered. The value object creates a **trust boundary**: raw input enters on the left, validated domain types flow to the right.

### When NOT to Wrap

Not every primitive needs a value object. Guidelines:

| Wrap it | Leave it primitive |
|---|---|
| It has validation rules | It is a loop counter or array index |
| It appears in method signatures alongside other same-typed values | It is purely internal to a single method |
| It has domain meaning (email, currency, temperature) | It is a simple flag or configuration toggle |
| Getting it wrong causes a bug far from the source | Misuse would be caught immediately |

### Library Recommendations

You do not always need to build value objects from scratch:

- **C#**: [ValueOf](https://github.com/mcintyre321/ValueOf) — a lightweight base class that handles equality, `ToString`, and comparison boilerplate
- **Java**: [Immutables](https://immutables.github.io/) for day-to-day productivity with annotation-driven code generation; [jMolecules](https://github.com/xmolecules/jmolecules) if your team wants to emphasize DDD vocabulary and design intent
- **TypeScript**: No dominant library exists. Use **branded types** for compile-time safety with zero runtime overhead, and **immutable classes** with `readonly` fields and factory methods for runtime validation

#### TypeScript Branded Types

TypeScript's structural type system means two types with the same shape are interchangeable. Branded types add a phantom property to create nominal typing at compile time:

```typescript
type EmailAddress = string & { readonly __brand: unique symbol };
type CustomerId = string & { readonly __brand: unique symbol };

function createEmail(raw: string): EmailAddress {
    if (!raw.includes("@")) throw new Error("Invalid email");
    return raw as EmailAddress;
}

function createCustomerId(raw: string): CustomerId {
    return raw as CustomerId;
}

// Now the compiler catches swapped arguments:
function sendConfirmation(email: EmailAddress, id: CustomerId): void { /* ... */ }

const email = createEmail("alice@example.com");
const id = createCustomerId("CUST-42");

sendConfirmation(email, id);  // ✅ compiles
sendConfirmation(id, email);  // ❌ compile error
```

This costs nothing at runtime — the brand exists only in the type system.

### SOLID Connection

- **SRP**: Validation logic lives inside the value object, not scattered across controllers, services, and repositories
- **OCP**: Adding a new validation rule (e.g., blocking disposable email domains) changes only the `EmailAddress` class, not every caller
- **DIP**: Higher-level code depends on the abstraction (`EmailAddress`) rather than the raw primitive (`string`)

---
## 2. Overriding ToString (and Its Equivalents)

### The Bug

A developer is debugging an order processing failure. The log file reads:

```
[ERROR] Failed to process order: MyApp.Models.Order
```

That is the **entire** error message. The default `ToString` returned the type name and nothing else. The developer now has to reproduce the failure, attach a debugger, and inspect the object manually. If this happened in production at 3 AM, that is not an option.

### Why ToString Matters

`ToString` (C#), `toString` (Java), and `toString` (TypeScript/JavaScript) are called in more places than most developers realize:

```mermaid
flowchart LR
    TS["ToString / toString"] --> LOG["Logging frameworks\nlogger.Info(order)"]
    TS --> DBG["Debugger watch windows"]
    TS --> INTERP["String interpolation\n$'Processing {order}'"]
    TS --> EXC["Exception messages\nthrow new Ex($'Failed: {order}')"]
    TS --> ASSERT["Test assertions\nAssert.Equal(expected, actual)\nshows ToString on failure"]
    TS --> CONSOLE["Console output\nConsole.WriteLine(order)"]
```

A good `ToString` override improves **every one** of these scenarios for free.

### The Default Is Almost Always Useless

| Language | Default ToString Output |
|---|---|
| C# (class) | `MyApp.Models.Order` (fully qualified type name) |
| C# (record) | `Order { Id = 42, Status = Pending }` (auto-generated, actually useful) |
| Java (class) | `Order@3f2a1b7c` (type name + hash code in hex) |
| Java (record) | `Order[id=42, status=Pending]` (auto-generated, actually useful) |
| TypeScript | `[object Object]` (completely useless) |

Records in C# and Java generate a useful `ToString` automatically. For everything else, you must override it yourself.

### Examples in All Three Languages

**C#**:

```csharp
public class Order
{
    public int Id { get; init; }
    public string Status { get; set; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }

    public override string ToString()
        => $"Order(Id={Id}, Status={Status}, Total={Total:C}, Created={CreatedAt:yyyy-MM-dd})";
}
```

**Java**:

```java
public class Order {
    private final int id;
    private String status;
    private final BigDecimal total;
    private final LocalDate createdAt;

    @Override
    public String toString() {
        return String.format("Order(Id=%d, Status=%s, Total=%s, Created=%s)",
            id, status, total, createdAt);
    }
}
```

**TypeScript**:

```typescript
class Order {
    constructor(
        public readonly id: number,
        public status: string,
        public readonly total: number,
        public readonly createdAt: Date
    ) {}

    toString(): string {
        return `Order(Id=${this.id}, Status=${this.status}, ` +
               `Total=${this.total.toFixed(2)}, Created=${this.createdAt.toISOString().slice(0, 10)})`;
    }
}
```

Now the log reads:

```
[ERROR] Failed to process order: Order(Id=42, Status=Pending, Total=$125.00, Created=2026-04-10)
```

The developer can immediately see which order failed, its status, and when it was created — without attaching a debugger.

### What to Include and What to Exclude

| Include | Exclude |
|---|---|
| Identity fields (ID, name, key) | Passwords, tokens, API keys |
| Current state (status, phase) | Full credit card numbers (mask to last 4) |
| Small distinguishing attributes | Large collections (show count instead) |
| Timestamps relevant to debugging | Entire nested object graphs |

**Rule of thumb**: Include enough to identify the object in a log line. Exclude anything sensitive or anything that would make the output span multiple lines.

### Sensitive Data Warning

```csharp
// ❌ DANGEROUS — leaks credentials to logs
public override string ToString()
    => $"User(Email={Email}, Password={Password}, SSN={Ssn})";

// ✅ SAFE — masks sensitive fields
public override string ToString()
    => $"User(Email={Email}, Password=***, SSN=***-**-{Ssn[^4..]})";
```

> Never put secrets in ToString. Anything in ToString **will** end up in a log file eventually.

### Connection to Value Objects

Value objects from Section 1 especially benefit from good `ToString` overrides. When an `EmailAddress` appears in a log, you want to see `alice@example.com`, not `MyApp.ValueObjects.EmailAddress`. All of the value object examples in Section 1 included a `ToString` override for exactly this reason.

---
## 3. GetHashCode, hashCode, and the Equality Contract

### The Bug

A developer adds a `Customer` object to a `HashSet`, then updates the customer's name. Later, a lookup reports the customer is not in the set — even though it was never removed:

```csharp
var customers = new HashSet<Customer>();
var alice = new Customer { Id = 1, Name = "Alice" };
customers.Add(alice);

alice.Name = "Alice Smith"; // mutate after insertion

Console.WriteLine(customers.Contains(alice)); // false! 😱
```

The object is still **in** the set. It is sitting in a bucket determined by its **original** hash code. But the lookup uses the **new** hash code, which maps to a different bucket. The object is effectively lost.

```mermaid
flowchart TB
    subgraph "HashSet Buckets"
        B0["Bucket 0"]
        B1["Bucket 1"]
        B2["Bucket 2\n(Alice was placed here\nbased on original hash)"]
        B3["Bucket 3"]
    end
    LOOKUP["Contains(alice)\nhash now maps to Bucket 0"] --> B0
    B0 --> MISS["Not found! ❌"]
    B2 --> GHOST["Alice is here,\nbut nobody looks"]
```

This is one of the most insidious bugs in object-oriented programming because it is completely silent. No exception, no warning, no crash — just wrong behavior.

### What Is a Hash Code?

A hash code is an integer computed from an object's data. Its purpose is to quickly sort objects into **buckets** so that hash-based collections (`Dictionary`, `HashMap`, `HashSet`) can find them in near-constant time instead of scanning every element.

Think of it like a library filing system. Instead of searching every shelf for a book, you compute a shelf number from the book's title. To find the book later, you recompute the shelf number and go directly there. The shelf number is the hash code.

```mermaid
flowchart TB
    OBJ["Object\n(Id=42, Name='Alice')"] -->|"GetHashCode()"| HC["Hash code\n(integer, e.g. 42)"]
    HC -->|"bucket = hash % arraySize"| BUCKET["Bucket 2\n(of a 20-bucket array)"]
    BUCKET --> FIND["Direct lookup\nO(1) average"]
```

#### A Naive Hash Code Example

At its simplest, a hash function takes the fields that define equality and combines them into a single integer. Here is a deliberately simple implementation to illustrate the mechanics:

```csharp
// C# — naive hash code for a Point with two fields
public class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y) { X = x; Y = y; }

    public override int GetHashCode()
    {
        // Multiply the first field by a prime, then add the second.
        // The prime (31) spreads values across the integer range
        // and reduces the chance that (3,7) and (7,3) hash the same.
        return X * 31 + Y;
    }

    public override bool Equals(object? obj)
        => obj is Point other && X == other.X && Y == other.Y;
}
```

```java
// Java — same idea
public class Point {
    private final int x, y;

    public Point(int x, int y) { this.x = x; this.y = y; }

    @Override
    public int hashCode() {
        return x * 31 + y;   // same prime-multiply-and-add approach
    }

    @Override
    public boolean equals(Object obj) {
        if (!(obj instanceof Point other)) return false;
        return x == other.x && y == other.y;
    }
}
```

**Why 31?** It is a small odd prime. Multiplying by a prime before adding the next field reduces the number of collisions — cases where two different objects land in the same bucket. The number 31 is a convention (Java's `String.hashCode()` uses it), but any small prime works.

**In practice, do not write your own hash function.** Use the built-in combiners:

| Language | Recommended Approach |
|---|---|
| C# | `HashCode.Combine(field1, field2, ...)` |
| Java | `Objects.hash(field1, field2, ...)` |
| TypeScript | No built-in — use primitive keys or serialize to a string |

```csharp
// C# — production-quality hash code
public override int GetHashCode() => HashCode.Combine(X, Y);
```

```java
// Java — production-quality hash code
@Override
public int hashCode() { return Objects.hash(x, y); }
```

These built-in methods handle null fields, use better bit-mixing algorithms, and are tested against real-world collision rates. The naive example above is for understanding only.

#### How a Hash-Based Collection Uses the Hash Code

When you call `dictionary[key] = value`, the collection does two things:

1. **Compute the bucket**: `bucket = key.GetHashCode() % bucketCount`
2. **Store the entry** in that bucket (along with the key and value)

When you later call `dictionary[key]` to retrieve:

1. **Recompute the bucket**: `bucket = key.GetHashCode() % bucketCount`
2. **Search that bucket** using `Equals` to find the exact match

This is why the equality contract and the hash code contract are inseparable. The hash code narrows the search to one bucket; `Equals` confirms the exact match within that bucket.

```mermaid
flowchart TB
    subgraph "dictionary[key] — lookup"
        direction TB
        B1["key.GetHashCode()"] --> B2["bucket = hash % size"]
        B2 --> B3["Scan bucket using\nkey.Equals(candidate)"]
        B3 --> B4["Return value"]
    end
    subgraph "dictionary[key] = value"
        direction TB
        A1["key.GetHashCode()"] --> A2["bucket = hash % size"]
        A2 --> A3["Store (key, value)\nin bucket"]
    end

```

If two different objects produce different hash codes but are considered `Equal`, the lookup will search the **wrong bucket** and never find the stored entry. This is why the contract exists.

### The Equality Contract

Every object-oriented language defines a contract for equality that implementations must follow. The rules are the same across languages:

| Rule | Meaning |
|---|---|
| **Reflexive** | `a.equals(a)` is always `true` |
| **Symmetric** | If `a.equals(b)` then `b.equals(a)` |
| **Transitive** | If `a.equals(b)` and `b.equals(c)` then `a.equals(c)` |
| **Consistent** | Calling `equals` multiple times returns the same result (if the object has not changed) |
| **Null-safe** | `a.equals(null)` is always `false` |

### The Hash Code Contract

The hash code contract sits on top of the equality contract:

1. **If two objects are equal, they MUST have the same hash code.** Violating this breaks every hash-based collection (HashMap, HashSet, Dictionary).
2. **If two objects are NOT equal, they SHOULD have different hash codes.** This is not required, but identical hash codes for different objects cause collisions that degrade performance from O(1) to O(n).
3. **The hash code MUST NOT change while the object is in a hash-based collection.** This is the bug from the opening example.

> Equal objects **must** hash the same. Unequal objects **should** hash differently. Hash codes **must not** change while the object is stored in a hash-based collection.

### C# — Equals and GetHashCode

```csharp
public class Customer : IEquatable<Customer>
{
    public int Id { get; init; }
    public string Name { get; set; }

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
}
```

Key decisions:
- Equality is based on `Id` only, not on mutable fields like `Name`
- `GetHashCode` uses the same field(s) as `Equals`
- `IEquatable<T>` avoids boxing for value type comparisons
- Operator overloads (`==`, `!=`) delegate to `Equals` for consistency

### Java — equals and hashCode

```java
public class Customer {
    private final int id;
    private String name;

    @Override
    public boolean equals(Object obj) {
        if (this == obj) return true;
        if (!(obj instanceof Customer other)) return false;
        return id == other.id;
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}
```

Java's `Objects.hash()` is a convenience method that handles null-safe hashing of multiple fields. For a single `int` field, `Integer.hashCode(id)` is slightly more efficient.

### TypeScript — No Built-In Contract

TypeScript (and JavaScript) has no `hashCode` equivalent and no overridable `equals`. Object identity uses reference equality (`===`), and there is no way to override it for `Map` or `Set`:

```typescript
const map = new Map<{id: number}, string>();

const key1 = { id: 1 };
const key2 = { id: 1 }; // same data, different reference

map.set(key1, "Alice");
console.log(map.get(key2)); // undefined — different reference!
```

**Workarounds** for TypeScript:
1. Use primitive keys (string or number) instead of object keys
2. Serialize to a string key: `map.set(JSON.stringify(key), value)`
3. Implement a custom collection that uses an `equals` method

```typescript
// Use a string-keyed Map for value-based lookup
class CustomerMap {
    private map = new Map<string, Customer>();

    set(customer: Customer): void {
        this.map.set(String(customer.id), customer);
    }

    get(id: number): Customer | undefined {
        return this.map.get(String(id));
    }
}
```

This is a fundamental limitation of JavaScript's object model. The language simply does not support value-based hashing or equality for object keys.

### Records Solve This Automatically

Both C# and Java `record` types generate correct `Equals`, `GetHashCode` / `equals`, `hashCode` implementations automatically based on all declared fields:

**C#**:
```csharp
public record Customer(int Id, string Name);

var a = new Customer(1, "Alice");
var b = new Customer(1, "Alice");

Console.WriteLine(a == b);          // true (value equality)
Console.WriteLine(a.GetHashCode() == b.GetHashCode()); // true
```

**Java**:
```java
public record Customer(int id, String name) {}

var a = new Customer(1, "Alice");
var b = new Customer(1, "Alice");

System.out.println(a.equals(b));    // true
System.out.println(a.hashCode() == b.hashCode()); // true
```

> If your class is a value object (immutable, compared by value), use a `record`. You get correct equality, hash codes, and `ToString` for free.

### When to Override

| Type of Class | Override? | Equality Based On |
|---|---|---|
| **Value Object** (EmailAddress, Money) | Always | All fields (they are all immutable) |
| **Entity** (Customer, Order) | Usually | Identity field (ID) only |
| **DTO / Data Transfer Object** | Rarely | Typically not needed — use records if you want value equality |
| **Service / Controller** | Never | Reference equality is correct — there should be only one instance |

### The Immutability Connection

This ties directly back to Section 1. Value objects are immutable, which means:
- Their hash codes never change after construction
- They are safe to use as dictionary keys and set members
- The bug from the opening of this section is **impossible** with immutable value objects

```mermaid
flowchart TB
    VO["Value Object\n(immutable)"] -->|"hash code stable"| SAFE["Safe as\nDictionary key"]
    MUT["Mutable Entity"] -->|"hash code can change"| DANGER["Dangerous as\nDictionary key"]
    DANGER -->|"use ID-only hash"| MITIGATE["Mitigated if hash\nis based on\nimmutable ID field"]
```

### Common Mistakes

1. **Overriding `Equals` without `GetHashCode`**: The compiler warns you (C#) or your IDE flags it (Java), but developers sometimes ignore the warning. This **will** break hash-based collections.

2. **Including mutable fields in `GetHashCode`**: The opening bug of this section. Only include fields that do not change after construction.

3. **Inconsistent `Equals` and `GetHashCode`**: If `Equals` uses `Id` and `Name`, but `GetHashCode` uses only `Id`, then two objects with the same `Id` but different `Name` will hash the same but not be equal. This is technically legal but causes confusing behavior and poor performance.

4. **Forgetting operator overloads in C#**: If you override `Equals` but not `==`, then `a.Equals(b)` returns `true` but `a == b` returns `false` (reference comparison). This is a common source of test failures.

5. **Treating hash codes as stable identifiers**: Hash codes for the same data are **not** guaranteed to be the same across different CPU architectures, runtime versions, or even different process runs. In .NET, `string.GetHashCode()` is deliberately randomized per process by default (since .NET Core). Java's `String.hashCode()` algorithm is contractually fixed, but most other types make no such promise. **Never use hash codes as database keys, cache keys persisted to disk, or any identifier that must survive beyond the current process.** They are ephemeral, in-memory optimization values — nothing more.

---
## 4. Exception Handling Done Right

### The Bug

A user reports that their payment was charged but the order was never created. The team searches the logs and finds — nothing. No error, no warning, no trace. After days of investigation, they discover this code:

```csharp
try
{
    var order = CreateOrder(cart);
    chargePayment(order);
    sendConfirmation(order);
}
catch (Exception)
{
    // TODO: handle this later
}
```

The exception was **swallowed**. The payment succeeded, the confirmation threw an exception, and the catch block silently ate it. The order was never saved. The customer was charged for nothing.

> Swallowing an exception is not "handling" it. It is **hiding** it. And hidden bugs do not go away — they compound.

### The Golden Rule

Exceptions are for **exceptional** circumstances — situations that the normal flow of the program cannot or should not handle. They are not a control flow mechanism.

```mermaid
flowchart TB
    Q1{"Is this situation\nexpected?"}
    Q1 -->|"Yes"| FLOW["Use normal control flow\n(if/else, return codes,\nResult types)"]
    Q1 -->|"No"| Q2{"Can this layer\nrecover?"}
    Q2 -->|"Yes"| HANDLE["Catch, recover,\nand continue"]
    Q2 -->|"No"| PROPAGATE["Let it propagate\n(do NOT catch)"]
```

### Anti-Pattern 1: Swallowing Exceptions

```csharp
// ❌ C# — exception swallowed
try { Process(order); }
catch (Exception) { }
```

```java
// ❌ Java — exception swallowed
try { process(order); }
catch (Exception e) { }
```

```typescript
// ❌ TypeScript — exception swallowed
try { process(order); }
catch (e) { }
```

**Why it is dangerous**: The failure is completely invisible. No log, no alert, no trace. The system continues in a corrupted state.

**The fix**: At minimum, log the exception. Better yet, only catch what you can actually handle.

### Anti-Pattern 2: Exceptions as Control Flow

```csharp
// ❌ C# — using exceptions for expected cases
public int ParseAge(string input)
{
    try
    {
        return int.Parse(input);
    }
    catch (FormatException)
    {
        return -1; // using exception as a fancy if-statement
    }
}

// ✅ C# — using TryParse for expected cases
public int ParseAge(string input)
{
    return int.TryParse(input, out var age) ? age : -1;
}
```

```java
// ❌ Java — using exceptions for expected cases
public int parseAge(String input) {
    try {
        return Integer.parseInt(input);
    } catch (NumberFormatException e) {
        return -1;
    }
}

// ✅ Java — checking before parsing
public OptionalInt parseAge(String input) {
    if (input == null || !input.matches("\\d+"))
        return OptionalInt.empty();
    return OptionalInt.of(Integer.parseInt(input));
}
```

```typescript
// ❌ TypeScript — using exceptions for expected cases
function parseAge(input: string): number {
    try {
        const age = JSON.parse(input);
        if (typeof age !== "number") throw new Error();
        return age;
    } catch {
        return -1;
    }
}

// ✅ TypeScript — checking before parsing
function parseAge(input: string): number | undefined {
    const age = Number(input);
    return Number.isFinite(age) ? age : undefined;
}
```

**Why it is dangerous**: Exceptions are expensive (stack trace capture), make control flow hard to follow, and hide the difference between expected and unexpected situations.

> If you can check for a condition before it happens, do not use exceptions to detect it after it happens.

### Anti-Pattern 3: Catching Too Broadly

```csharp
// ❌ C# — catches everything, including OutOfMemoryException
try { Process(order); }
catch (Exception ex) { logger.Error(ex, "Processing failed"); }

// ✅ C# — catches only what you can handle
try { Process(order); }
catch (PaymentGatewayException ex) { logger.Error(ex, "Payment failed for order {OrderId}", order.Id); }
catch (InventoryException ex) { logger.Error(ex, "Stock unavailable for order {OrderId}", order.Id); }
```

```java
// ❌ Java — catches everything
try { process(order); }
catch (Exception e) { logger.error("Processing failed", e); }

// ✅ Java — catches specific exceptions
try { process(order); }
catch (PaymentGatewayException e) { logger.error("Payment failed for order {}", order.getId(), e); }
catch (InventoryException e) { logger.error("Stock unavailable for order {}", order.getId(), e); }
```

**Why it is dangerous**: Catching `Exception` catches everything — including `NullPointerException`, `StackOverflowException`, and other bugs that you should **not** be suppressing.

### Anti-Pattern 4: Losing the Stack Trace

```csharp
// ❌ C# — loses the original stack trace
try { Process(order); }
catch (Exception ex) { throw new ApplicationException("Processing failed: " + ex.Message); }

// ✅ C# — preserves the original stack trace
try { Process(order); }
catch (Exception ex) { throw new ApplicationException("Processing failed", ex); }

// ✅ C# — rethrow without wrapping (preserves everything)
try { Process(order); }
catch (Exception ex)
{
    logger.Error(ex, "Processing failed");
    throw; // rethrows with original stack trace
}
```

```java
// ❌ Java — loses the original stack trace
try { process(order); }
catch (Exception e) { throw new RuntimeException("Processing failed: " + e.getMessage()); }

// ✅ Java — preserves the original stack trace
try { process(order); }
catch (Exception e) { throw new RuntimeException("Processing failed", e); }
```

```typescript
// ❌ TypeScript — loses the original stack trace
try { process(order); }
catch (e) { throw new Error("Processing failed"); }

// ✅ TypeScript — preserves the original error as cause (ES2022+)
try { process(order); }
catch (e) { throw new Error("Processing failed", { cause: e }); }
```

> When rethrowing, always pass the original exception as the inner exception or cause. The stack trace is the most valuable debugging information you have.

### Best Practice: Resource Cleanup

All three languages have mechanisms for guaranteed cleanup:

**C#** — `using` statements (implements `IDisposable`):

```csharp
// The connection is automatically closed/disposed even if an exception is thrown
using var connection = new SqliteConnection(connectionString);
connection.Open();
// use the connection...
```

**Java** — `try-with-resources` (implements `AutoCloseable`):

```java
// The connection is automatically closed even if an exception is thrown
try (var connection = DriverManager.getConnection(connectionString)) {
    // use the connection...
}
```

**TypeScript** — `using` declarations (implements `Disposable`, stage 3 TC39 proposal, supported in TypeScript 5.2+):

```typescript
// Modern approach — requires TypeScript 5.2+
function readFile(path: string) {
    using handle = openFile(path); // auto-disposed at end of scope
    return handle.read();
}

// Traditional approach — try/finally
const handle = openFile(path);
try {
    return handle.read();
} finally {
    handle.close();
}
```

### Best Practice: Custom Exception Types

Create domain-specific exception types when the caller needs to distinguish between different failure modes:

```csharp
// C#
public class OrderProcessingException : Exception
{
    public string OrderId { get; }

    public OrderProcessingException(string orderId, string message, Exception? inner = null)
        : base(message, inner)
    {
        OrderId = orderId;
    }
}
```

```java
// Java
public class OrderProcessingException extends RuntimeException {
    private final String orderId;

    public OrderProcessingException(String orderId, String message, Throwable cause) {
        super(message, cause);
        this.orderId = orderId;
    }

    public String getOrderId() { return orderId; }
}
```

```typescript
// TypeScript
export class OrderProcessingError extends Error {
    constructor(
        public readonly orderId: string,
        message: string,
        options?: { cause?: unknown }
    ) {
        super(message, options);
        this.name = "OrderProcessingError";
    }
}
```

### Never Return Exceptions to Untrusted Clients

This is a security concern as much as a design concern. A raw exception sent to a client leaks implementation details:

```json
{
    "error": "System.Data.SqlClient.SqlException: Invalid column name 'user_pwd'.\n   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception)...",
    "stackTrace": "   at MyApp.Repositories.UserRepository.FindByEmail(String email) in /src/Repos/UserRepository.cs:line 42\n   at MyApp.Controllers.AuthController.Login(LoginRequest req) in /src/Controllers/AuthController.cs:line 18"
}
```

This response tells an attacker your database schema, your file paths, your class names, and which line of code failed. That is a roadmap for exploitation.

**The rule**: Log the full exception server-side. Return a sanitized message to the client.

```mermaid
sequenceDiagram
    participant Client
    participant API as API / Controller
    participant Service
    participant Log as Server Log

    Client->>API: POST /orders
    API->>Service: CreateOrder()
    Service-->>API: 💥 SqlException
    API->>Log: Log full exception\n(stack trace, details, context)
    API-->>Client: { "error": "Order processing failed",\n"code": "ORDER_ERR_001" }
```

**C# — ASP.NET Exception-Handling Middleware**:

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        // Log the full exception server-side
        _logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            context.Request.Method, context.Request.Path);

        // Return a sanitized response to the client
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An internal error occurred. Please try again later.",
            code = "INTERNAL_ERROR",
            traceId = context.TraceIdentifier // safe to share for support tickets
        }, ct);

        return true;
    }
}
```

**Java — Spring @ControllerAdvice**:

```java
@ControllerAdvice
public class GlobalExceptionHandler {
    private static final Logger log = LoggerFactory.getLogger(GlobalExceptionHandler.class);

    @ExceptionHandler(Exception.class)
    public ResponseEntity<Map<String, String>> handleAll(Exception ex, HttpServletRequest req) {

        // Log the full exception server-side
        log.error("Unhandled exception for {} {}", req.getMethod(), req.getRequestURI(), ex);

        // Return a sanitized response to the client
        return ResponseEntity
            .status(HttpStatus.INTERNAL_SERVER_ERROR)
            .body(Map.of(
                "error", "An internal error occurred. Please try again later.",
                "code", "INTERNAL_ERROR"
            ));
    }
}
```

### Structured Error Handling Alternatives

For expected failure cases (not truly exceptional situations), consider **Result types** that make success and failure explicit in the type system:

```csharp
// C# — a simple Result type
public record Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T? value, string? error) => (Value, Error) = (value, error);

    public static Result<T> Ok(T value) => new(value, null);
    public static Result<T> Fail(string error) => new(default, error);
}

// Usage:
public Result<Order> CreateOrder(Cart cart)
{
    if (cart.Items.Count == 0)
        return Result<Order>.Fail("Cart is empty");

    var order = new Order(cart);
    return Result<Order>.Ok(order);
}
```

Result types make failure **visible** in the type signature. The caller cannot forget to check for failure because the type system forces it.

### Connection to Value Objects

Value objects that validate at construction prevent many exceptions downstream. If an `EmailAddress` cannot exist in an invalid state, then no downstream code needs to catch `InvalidEmailException`. The exception is thrown at the boundary, once, when the raw input is first converted to a value object. Everything after that point can trust the type.

```mermaid
flowchart LR
    INPUT["Raw input"] -->|"try to create\nvalue object"| BOUNDARY["Boundary\n(Controller, API)"]
    BOUNDARY -->|"valid"| DOMAIN["Domain layer\n(no validation exceptions\nneeded)"]
    BOUNDARY -->|"invalid"| REJECT["Return 400\n(structured error)"]
```

---
## 5. Connecting the Dots

These four topics are not isolated. They form a reinforcing cycle of good design habits:

```mermaid
flowchart TB
    VO["Value Objects\n(Section 1)"] -->|"always immutable\n→ stable hash codes"| HASH["Correct Hash & Equality\n(Section 3)"]
    VO -->|"natural place for"| TS["Good ToString\n(Section 2)"]
    VO -->|"validate at construction\n→ fewer exceptions downstream"| EX["Good Exception Handling\n(Section 4)"]
    TS -->|"meaningful error messages"| EX
    HASH -->|"immutability enforced\nby value objects"| VO
    EX -->|"boundary validation\nproduces value objects"| VO

    style VO fill:#e1f5fe
    style TS fill:#f3e5f5
    style HASH fill:#e8f5e9
    style EX fill:#fff3e0
```

1. **Value objects eliminate primitive obsession** and move validation to construction. This reduces the number of exception paths in your domain layer.

2. **Value objects are immutable**, which means their hash codes are stable. They are always safe to use as dictionary keys and set members.

3. **Value objects benefit most from ToString overrides** because they appear frequently in logs, error messages, and debugger displays.

4. **Good exception handling at the boundary** is where you reject invalid input and create value objects. Everything downstream can trust the types it receives.

5. **Sanitized exception responses** protect your implementation details from clients, while server-side logging with good `ToString` output makes debugging possible.

6. **Client-side errors are invisible unless you log them back to the server.** Section 4 covers sanitizing server errors sent to clients, but the reverse problem is equally important: runtime errors in the browser never reach your server logs unless you explicitly capture and forward them. [Appendix B](#appendix-b-logging-client-side-errors-on-the-server) walks through a practical Angular + ASP.NET Core approach.

> Professional code validates at the boundary, communicates through ToString, respects the equality contract, and reserves exceptions for the truly exceptional.

---
## Appendix A: Language Comparison Quick Reference

### Value Objects

| Feature | C# | Java | TypeScript |
|---|---|---|---|
| Recommended type | `record` | `record` (16+) | Immutable class + factory |
| Equality | Auto-generated by `record` | Auto-generated by `record` | Manual `equals()` method |
| Immutability | `init` properties, `readonly` | Fields are final in records | `readonly` properties |
| Compile-time type safety | Strong (nominal typing) | Strong (nominal typing) | Branded types for nominal |
| Library | ValueOf | Immutables, jMolecules | None dominant |

### ToString

| Feature | C# | Java | TypeScript |
|---|---|---|---|
| Override syntax | `override string ToString()` | `@Override public String toString()` | `toString(): string` |
| Default for classes | Fully qualified type name | `ClassName@hashHex` | `[object Object]` |
| Default for records | All properties shown | All components shown | N/A (no built-in records) |
| String interpolation | `$"text {obj}"` calls ToString | `"text " + obj` calls toString | `` `text ${obj}` `` calls toString |

### Equality and Hash Codes

| Feature | C# | Java | TypeScript |
|---|---|---|---|
| Equality method | `Equals(object)` + `IEquatable<T>` | `equals(Object)` | No standard protocol |
| Hash code method | `GetHashCode()` | `hashCode()` | No built-in equivalent |
| Auto-generated by records | Yes | Yes | N/A |
| Hash-based collections | `Dictionary<K,V>`, `HashSet<T>` | `HashMap<K,V>`, `HashSet<T>` | `Map<K,V>`, `Set<T>` (reference-based for objects) |
| Operator overload | `==` and `!=` can be overridden | Not available | Not available |

### Exception Handling

| Feature | C# | Java | TypeScript |
|---|---|---|---|
| Resource cleanup | `using` / `IDisposable` | `try-with-resources` / `AutoCloseable` | `using` (TS 5.2+) / `try-finally` |
| Checked exceptions | No | Yes (controversial) | No |
| Chained cause | `new Ex(msg, innerException)` | `new Ex(msg, cause)` | `new Error(msg, { cause })` (ES2022) |
| Rethrow preserving stack | `throw;` (no argument) | `throw e;` (rethrows same object) | `throw e;` |
| Global handler (web) | `IExceptionHandler` middleware | `@ControllerAdvice` | Express error middleware |
| Result type | Custom or `OneOf` library | Custom or `vavr.Either` | Discriminated union |

---
## Appendix B: Logging Client-Side Errors on the Server

This appendix walks through a practical approach to forwarding client-side runtime errors to the server log. The example uses Angular for the SPA and ASP.NET Core for the API, but the pattern applies to any frontend/backend combination. A working demo of everything described below is available in the companion project: [Client-Side Error Logging Demo](16-tiny-design-choices-client-error-logging-demo/).

### The Blind Spot

Section 4 covered how to sanitize server exceptions before returning them to clients. But there is an equally dangerous blind spot running in the opposite direction: **runtime errors that occur in the browser never appear in your server logs.** A `TypeError`, a failed API deserialization, or an unhandled promise rejection crashes the user's experience — and you never know it happened.

If your error logging strategy only covers the server, you are flying blind on the client.

```mermaid
flowchart TB
    subgraph "Server"
    	direction TB
        API["API Controller"]
        LOG["Server Log ✅"]
        API -->|"errors logged"| LOG
    end
    subgraph "Browser"
    direction TB
        APP["Angular App"]
        ERR["Runtime Error 💥"]
        APP --> ERR
        ERR -->|"❌ invisible to server"| VOID["Lost forever"]
    end
```

The fix is straightforward: capture client-side errors, package them with enough context to be actionable, and forward them to a lightweight server endpoint that writes them to the same log infrastructure.

```mermaid
flowchart LR
    subgraph "Browser"
        APP["Angular App"]
        ERR["Runtime Error"]
        HANDLER["Global\nErrorHandler"]
        APP --> ERR --> HANDLER
    end
    subgraph "Server"
        CTRL["ClientLog\nController"]
        LOG["Server Log ✅"]
        HANDLER -->|"POST /api/client-log"| CTRL
        CTRL -->|"log as\nclient-side error"| LOG
    end
```

### Step 1: Capture Errors in Angular

Angular provides a global `ErrorHandler` interface. By replacing the default handler, you can intercept every unhandled error in the application — component errors, service errors, unhandled promise rejections — in one place.

```typescript
// client-error.model.ts
export interface ClientErrorReport {
    message: string;
    stack?: string;
    url: string;           // browser path when the error occurred
    user?: string;         // authenticated user, if known
    timestamp: string;     // ISO 8601
    userAgent: string;     // browser and OS for reproduction
}
```

```typescript
// global-error-handler.ts
import { ErrorHandler, Injectable, Injector } from "@angular/core";
import { Router } from "@angular/router";
import { ClientLogService } from "./client-log.service";
import { AuthService } from "./auth.service";
import { ClientErrorReport } from "./client-error.model";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

    // Use Injector to avoid circular dependency with Router and HTTP
    constructor(private injector: Injector) {}

    handleError(error: unknown): void {
        // Still log to the browser console for local debugging
        console.error(error);

        const router = this.injector.get(Router);
        const auth = this.injector.get(AuthService);
        const logService = this.injector.get(ClientLogService);

        const report: ClientErrorReport = {
            message: error instanceof Error ? error.message : String(error),
            stack: error instanceof Error ? error.stack : undefined,
            url: router.url,
            user: auth.currentUser?.email,
            timestamp: new Date().toISOString(),
            userAgent: navigator.userAgent,
        };

        logService.reportError(report);
    }
}
```

```typescript
// client-log.service.ts
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ClientErrorReport } from "./client-error.model";

@Injectable({ providedIn: "root" })
export class ClientLogService {

    private readonly endpoint = "/api/client-log";

    constructor(private http: HttpClient) {}

    reportError(report: ClientErrorReport): void {
        // Fire-and-forget — do not let a logging failure cascade
        this.http.post(this.endpoint, report).subscribe({
            error: () => console.warn("Failed to report client error to server."),
        });
    }
}
```

Register the handler in your application config:

```typescript
// app.config.ts (or app.module.ts)
import { ErrorHandler } from "@angular/core";
import { GlobalErrorHandler } from "./global-error-handler";

export const appConfig = {
    providers: [
        { provide: ErrorHandler, useClass: GlobalErrorHandler },
        // ... other providers
    ],
};
```

### Step 2: Receive and Log on the Server (ASP.NET Core)

The server endpoint receives the error report and writes it to the application log. Two important design decisions:

1. **Tag the log entry clearly as a client-side error** so it is not confused with server exceptions during triage.
2. **Rate-limit the endpoint** to prevent a misbehaving client (or an attacker) from flooding your log infrastructure.

```csharp
// ClientErrorReport.cs
public sealed record ClientErrorReport(
    string Message,
    string? Stack,
    string Url,
    string? User,
    string Timestamp,
    string UserAgent
);
```

```csharp
// ClientLogController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/client-log")]
public class ClientLogController : ControllerBase
{
    private readonly ILogger<ClientLogController> _logger;

    public ClientLogController(ILogger<ClientLogController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [EnableRateLimiting("client-log")]
    public IActionResult Post([FromBody] ClientErrorReport report)
    {
        // Tag clearly as CLIENT-SIDE so server log searches can distinguish it
        _logger.LogError(
            "[CLIENT-SIDE ERROR] Message={Message} | Url={Url} | User={User} | " +
            "Timestamp={Timestamp} | UserAgent={UserAgent} | Stack={Stack}",
            report.Message,
            report.Url,
            report.User ?? "(anonymous)",
            report.Timestamp,
            report.UserAgent,
            report.Stack ?? "(no stack trace)");

        // Return 204 — no content needed
        return NoContent();
    }
}
```

#### Rate Limiting

A basic fixed-window rate limiter prevents abuse without blocking legitimate error reports:

```csharp
// In Program.cs
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("client-log", limiter =>
    {
        limiter.PermitLimit = 10;              // max 10 requests
        limiter.Window = TimeSpan.FromMinutes(1); // per 1-minute window
        limiter.QueueLimit = 0;                // reject immediately when full
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ... later in the pipeline
app.UseRateLimiter();
```

This allows 10 error reports per minute per client. In a real production system, you might partition by IP address or authenticated user.

### Step 3: Include Context That Makes Errors Actionable

A stack trace alone is rarely enough to reproduce a client-side bug. The error report should include contextual data that answers the questions a developer will ask during triage:

| Field | Why It Matters |
|---|---|
| `message` | What went wrong |
| `stack` | Where in the code it happened |
| `url` | What page/route the user was on — critical for reproducing the error |
| `user` | Which user hit the bug — is it one user or widespread? |
| `timestamp` | When it happened — correlate with server logs and deployments |
| `userAgent` | Which browser and OS — is it browser-specific? |

You can extend the report with additional fields as needed:

```typescript
export interface ClientErrorReport {
    message: string;
    stack?: string;
    url: string;
    user?: string;
    timestamp: string;
    userAgent: string;
    // Optional extensions:
    appVersion?: string;       // correlate with deployments
    componentName?: string;    // which Angular component threw
    additionalContext?: Record<string, unknown>; // route params, feature flags, etc.
}
```

### Source Maps: Readable Stack Traces

By default, production Angular builds are minified and bundled. A stack trace from production looks like this:

```
TypeError: Cannot read properties of undefined (reading 'name')
    at main.3f2a1b.js:1:28473
    at main.3f2a1b.js:1:31290
```

This is nearly useless. **Source maps** are files that map minified code back to the original TypeScript source. With source maps, the same error becomes:

```
TypeError: Cannot read properties of undefined (reading 'name')
    at OrderSummaryComponent.getCustomerName (order-summary.component.ts:42:18)
    at OrderSummaryComponent.ngOnInit (order-summary.component.ts:28:12)
```

#### Source Map Best Practices

```mermaid
flowchart TB
    BUILD["ng build --source-map"] --> MAPS["*.js.map files\ngenerated alongside bundles"]
    MAPS --> OPTION1["Option A:\nUpload maps to error\ntracking service\n(Sentry, Datadog, etc.)"]
    MAPS --> OPTION2["Option B:\nKeep maps on server,\nnever serve publicly"]
    MAPS --> OPTION3["Option C:\nGenerate hidden maps\n(--source-map=hidden)"]
    OPTION1 --> DECODED["Decoded stack traces\nin your dashboard"]
    OPTION2 --> DECODED
    OPTION3 --> DECODED
```

- **Generate source maps in production builds**: Use `ng build --source-map` or set `"sourceMap": true` in `angular.json`. Angular 15+ also supports `"hidden"` source maps that are generated but not linked from the bundle, preventing public access.
- **Do NOT serve source maps publicly**: They expose your original source code. Upload them to your error tracking service or store them on the server for internal use only.
- **Map stack traces server-side or in your error dashboard**: Services like Sentry, Datadog, and Rollbar automatically apply uploaded source maps to incoming error reports, giving you readable stack traces without exposing source maps to end users.

### Responsibility Split

The client and server each have a distinct role. The client is responsible for capturing the error and its context. The server is responsible for durable logging, rate limiting, and integration with alerting.

```mermaid
flowchart TB
    subgraph "Client Responsibilities"
        C1["Capture unhandled errors\n(global ErrorHandler)"]
        C2["Collect context\n(url, user, timestamp, userAgent)"]
        C3["Forward to server\n(fire-and-forget POST)"]
        C1 --> C2 --> C3
    end
    subgraph "Server Responsibilities"
        S1["Rate-limit incoming reports"]
        S2["Tag as CLIENT-SIDE ERROR"]
        S3["Write to application log\n(structured logging)"]
        S4["Trigger alerts if threshold exceeded"]
        S1 --> S2 --> S3 --> S4
    end
    C3 -->|"POST /api/client-log"| S1
```

| Concern | Client | Server |
|---|---|---|
| Error capture | Global `ErrorHandler` intercepts all unhandled errors | — |
| Context collection | Browser path, user, timestamp, userAgent | — |
| Transport | HTTP POST, fire-and-forget | Receives and validates payload |
| Rate limiting | — | Fixed-window limiter (e.g., 10/min) |
| Durable logging | — | Writes to structured log with `[CLIENT-SIDE ERROR]` tag |
| Alerting | — | Threshold-based alerts (e.g., spike in client errors after a deploy) |
| Source map decoding | — | Map minified stacks to original source (via uploaded maps) |
