using System.Text;

namespace Cube.Core;

/// <summary>
/// Turns the game's UTF-8 name bytes into strings, once per name: a string is only built when the bytes in an
/// object slot change (PROJECT.md, "Efficiency and memory": name labels are cached and only rebuilt when a name
/// changes). Reading the same names every frame allocates nothing.
/// </summary>
/// <remarks>
/// One entry per object table slot, allocated up front, so memory is fixed (PROJECT.md: caches have fixed caps).
/// A slot reused by a different player simply has different bytes, so it can never show a stale name. Names
/// live only in memory and are never written anywhere (PROJECT.md, "Privacy").
/// </remarks>
public sealed class NameCache
{
    /// <summary>The longest name the game stores, in bytes (FFXIVClientStructs: <c>GameObject.Name</c> is 64 bytes).</summary>
    public const int MaxNameBytes = 64;

    /// <summary>The cached bytes per slot.</summary>
    private readonly byte[][] bytes;

    /// <summary>How many of each slot's bytes are in use.</summary>
    private readonly int[] lengths;

    /// <summary>The cached string per slot (null until first seen).</summary>
    private readonly string?[] names;

    /// <summary>Creates an empty cache.</summary>
    /// <param name="slots">How many object slots to cache, e.g. 200 character slots (plus any Debug fake players).</param>
    public NameCache(int slots)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(slots, 1);
        this.bytes = new byte[slots][];
        for (var i = 0; i < slots; i++)
        {
            this.bytes[i] = new byte[MaxNameBytes];
        }

        this.lengths = new int[slots];
        this.names = new string?[slots];
    }

    /// <summary>How many slots the cache holds.</summary>
    public int Slots => this.names.Length;

    /// <summary>How many strings have been built so far; a test and the bridge use it to prove names aren't rebuilt every frame.</summary>
    public long Builds { get; private set; }

    /// <summary>
    /// Returns the name for a slot, building a new string only if the bytes differ from last time.
    /// </summary>
    /// <param name="slot">Object table index.</param>
    /// <param name="utf8">The name bytes; anything from the first zero byte on is ignored (the game's buffer is zero-terminated).</param>
    public string Get(int slot, ReadOnlySpan<byte> utf8)
    {
        var end = utf8.IndexOf((byte)0);
        if (end >= 0)
        {
            utf8 = utf8[..end];
        }

        if (utf8.Length > MaxNameBytes)
        {
            utf8 = utf8[..MaxNameBytes];
        }

        var cached = this.names[slot];
        if (cached is not null && utf8.SequenceEqual(this.bytes[slot].AsSpan(0, this.lengths[slot])))
        {
            return cached;
        }

        utf8.CopyTo(this.bytes[slot]);
        this.lengths[slot] = utf8.Length;
        var name = Encoding.UTF8.GetString(utf8);
        this.names[slot] = name;
        this.Builds++;
        return name;
    }
}
