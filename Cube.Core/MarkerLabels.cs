namespace Cube.Core;

/// <summary>
/// Builds the label text with its markers (#111): <c>[T]</c> in front of the player's current target, <c>[!]</c> in
/// front of an enemy that is attacking the player, both as <c>[T] [!] Name</c> with the target marker first.
/// </summary>
/// <remarks>
/// A prefixed string is only built when a slot's name or markers change, and is then reused every frame (PROJECT.md,
/// "Efficiency and memory": no per-frame allocation). With no markers the name itself is returned, so the common case
/// costs nothing. One entry per object slot, allocated up front, like <see cref="NameCache"/>.
/// </remarks>
public sealed class MarkerLabels
{
    /// <summary>Prefix for the current target.</summary>
    public const string TargetMarker = "[T] ";

    /// <summary>Prefix for an enemy attacking the player.</summary>
    public const string AttackingMarker = "[!] ";

    /// <summary>The name each slot's label was last built from (compared by reference: names are cached strings).</summary>
    private readonly string?[] names;

    /// <summary>The markers each slot's label was last built with.</summary>
    private readonly byte[] flags;

    /// <summary>The built label per slot.</summary>
    private readonly string?[] labels;

    /// <summary>Creates an empty cache.</summary>
    /// <param name="slots">How many object slots to cache; matches <see cref="NameCache.Slots"/>.</param>
    public MarkerLabels(int slots)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(slots, 1);
        this.names = new string?[slots];
        this.flags = new byte[slots];
        this.labels = new string?[slots];
    }

    /// <summary>How many prefixed strings have been built; a test uses it to prove they aren't rebuilt every frame.</summary>
    public long Builds { get; private set; }

    /// <summary>The label text for a name and its markers.</summary>
    /// <param name="isTarget">Whether this is the player's current target (any kind of object).</param>
    /// <param name="isAttacking">Whether this is an enemy attacking the player.</param>
    /// <param name="name">The cached name.</param>
    /// <returns>The text with its markers.</returns>
    public static string Compose(bool isTarget, bool isAttacking, string name) => (isTarget, isAttacking) switch
    {
        (true, true) => TargetMarker + AttackingMarker + name,
        (true, false) => TargetMarker + name,
        (false, true) => AttackingMarker + name,
        _ => name,
    };

    /// <summary>Returns the label for a slot, rebuilding it only if the name or markers changed.</summary>
    /// <param name="slot">Object table index (below the slot count).</param>
    /// <param name="isTarget">Whether this is the player's current target.</param>
    /// <param name="isAttacking">Whether this is an enemy attacking the player.</param>
    /// <param name="name">The cached name for the slot.</param>
    /// <returns>The label text.</returns>
    public string Get(int slot, bool isTarget, bool isAttacking, string name)
    {
        if (!isTarget && !isAttacking)
        {
            return name;
        }

        var key = (byte)((isTarget ? 1 : 0) | (isAttacking ? 2 : 0));
        if (this.labels[slot] is { } cached && ReferenceEquals(this.names[slot], name) && this.flags[slot] == key)
        {
            return cached;
        }

        var label = Compose(isTarget, isAttacking, name);
        this.names[slot] = name;
        this.flags[slot] = key;
        this.labels[slot] = label;
        this.Builds++;
        return label;
    }
}
