namespace Cube.Core;

/// <summary>
/// Remembers the pieces each collision mesh node was split into (<see cref="NodeSplitter"/>), so the split is worked out
/// once per node and a gather only transforms the kept pieces (#127). Bounded and dropped on zone change, like the box
/// sets (PROJECT.md, "Bounded memory").
/// </summary>
/// <remarks>
/// <para>
/// <b>Keyed by node address.</b> A node's triangles don't change while its mesh is loaded, so its split is fixed. The
/// address alone could be reused by a different mesh after the old one unloads, so each entry also stores a
/// <c>check</c> number (the reader makes it from the node's triangle count and bounds); a lookup whose check differs is
/// a miss, and the stale entry is replaced.
/// </para>
/// <para>
/// <b>Fixed size.</b> Pieces live in one array of <see cref="DefaultCapacity"/> pieces (24 bytes each, about 96 KB) and
/// the index is a dictionary created once at that size, so nothing grows with time in game. A node that can't be split
/// cheaply (too many triangles) is stored with no pieces, meaning "use the node's own box", so it isn't tried again.
/// When the cache is full <see cref="Add"/> says so, and the reader drops everything and refines again from what is
/// near the player (the region has a fixed box cap, so that is bounded too).
/// </para>
/// </remarks>
public sealed class NodePieceCache
{
    /// <summary>Most pieces kept (the #125 recommendation: 1,042 nodes made 1,183 pieces in a house).</summary>
    public const int DefaultCapacity = 4000;

    /// <summary>The kept pieces, one entry's pieces after another.</summary>
    private readonly LocalBox[] pieces;

    /// <summary>Where each node's pieces are.</summary>
    private readonly Dictionary<ulong, Entry> entries;

    /// <summary>How many slots of <see cref="pieces"/> are used.</summary>
    private int used;

    /// <summary>Creates an empty cache.</summary>
    /// <param name="capacity">Most pieces (and so most nodes) kept.</param>
    public NodePieceCache(int capacity = DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.pieces = new LocalBox[capacity];
        this.entries = new Dictionary<ulong, Entry>(capacity);
    }

    /// <summary>How many nodes are remembered.</summary>
    public int Nodes => this.entries.Count;

    /// <summary>How many pieces are kept.</summary>
    public int Pieces => this.used;

    /// <summary>The most pieces kept.</summary>
    public int Capacity => this.pieces.Length;

    /// <summary>Drops everything (zone change). The buffers are kept for reuse.</summary>
    public void Clear()
    {
        this.entries.Clear();
        this.used = 0;
    }

    /// <summary>Looks a node up.</summary>
    /// <param name="key">The node's address.</param>
    /// <param name="check">The node's check number; must match what was stored.</param>
    /// <param name="found">The node's pieces; empty for a node that is to be drawn as its own box.</param>
    /// <returns>True if the node has been refined (even to "no pieces").</returns>
    public bool TryGet(ulong key, uint check, out ReadOnlySpan<LocalBox> found)
    {
        if (this.entries.TryGetValue(key, out var entry) && entry.Check == check)
        {
            found = this.pieces.AsSpan(entry.Start, entry.Count);
            return true;
        }

        found = default;
        return false;
    }

    /// <summary>Stores a node's pieces.</summary>
    /// <param name="key">The node's address.</param>
    /// <param name="check">The node's check number.</param>
    /// <param name="nodePieces">The pieces; empty means "draw the node's own box".</param>
    /// <returns>False if there is no room (the node stays unrefined).</returns>
    public bool Add(ulong key, uint check, ReadOnlySpan<LocalBox> nodePieces)
    {
        if (this.used + nodePieces.Length > this.pieces.Length || (this.entries.Count >= this.pieces.Length && !this.entries.ContainsKey(key)))
        {
            return false;
        }

        nodePieces.CopyTo(this.pieces.AsSpan(this.used));
        this.entries[key] = new Entry(this.used, nodePieces.Length, check);
        this.used += nodePieces.Length;
        return true;
    }

    /// <summary>Where one node's pieces are, and which node they were made for.</summary>
    /// <param name="Start">Index of the first piece.</param>
    /// <param name="Count">How many pieces.</param>
    /// <param name="Check">The node's check number.</param>
    private readonly record struct Entry(int Start, int Count, uint Check);
}
