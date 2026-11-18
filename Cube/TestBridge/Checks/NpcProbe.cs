using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Dalamud.Plugin.Services;
using BattleNpcSubKind = FFXIVClientStructs.FFXIV.Client.Game.Object.BattleNpcSubKind;
using CsObjectKind = FFXIVClientStructs.FFXIV.Client.Game.Object.ObjectKind;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Running totals for the NPC research in <see cref="GameStateProbeCheck"/> (#73): per-frame counts by
/// category, the largest count seen in any frame, the slot range each kind used, and name changes.
/// </summary>
/// <remarks>
/// The maximum per frame, not the total, is what sizes the NPC cap for #74: the cap bounds how many NPCs one
/// frame can hold. A check may allocate (it isn't the draw path), so plain dictionaries are used.
/// </remarks>
internal sealed class NpcProbe
{
    /// <summary>This frame's count per category, reset by <see cref="BeginFrame"/>.</summary>
    private readonly Dictionary<string, int> current = [];

    /// <summary>The largest per-frame count seen so far, per category.</summary>
    private readonly Dictionary<string, int> max = [];

    /// <summary>Lowest and highest object table index each kind was seen at.</summary>
    private readonly Dictionary<string, (int Min, int Max)> slots = [];

    /// <summary>Last name bytes seen per NPC (entity id and slot), to count renames without logging names.</summary>
    private readonly Dictionary<ulong, byte[]> names = [];

    /// <summary>Gets or sets the length of the game's index-sorted object table.</summary>
    public int TableLength { get; set; }

    /// <summary>Gets the most NPCs passing the proposed #74 filter seen in one frame, anywhere in the table.</summary>
    public int MaxNamed => this.max.GetValueOrDefault("named");

    /// <summary>Gets the most NPCs passing the filter seen in one frame within the largest cube.</summary>
    public int MaxNamedInMaxCube => this.max.GetValueOrDefault("namedInMaxCube");

    /// <summary>Gets how many times an NPC still in the same slot showed a different name than before.</summary>
    public int NameChanges { get; private set; }

    /// <summary>Gets the number of frames sampled.</summary>
    public int Frames { get; private set; }

    /// <summary>Starts a frame: clears this frame's counts.</summary>
    public void BeginFrame() => this.current.Clear();

    /// <summary>Ends a frame: folds this frame's counts into the maxima.</summary>
    public void EndFrame()
    {
        this.Frames++;
        foreach (var (key, value) in this.current)
        {
            if (value > this.max.GetValueOrDefault(key))
            {
                this.max[key] = value;
            }
        }
    }

    /// <summary>Counts an object that is neither a player nor an NPC (minions, mounts, event objects...).</summary>
    /// <param name="kind">Its object kind.</param>
    /// <param name="slot">Its object table index.</param>
    public void CountOther(CsObjectKind kind, int slot)
    {
        var key = "other" + kind;
        this.Add(key);
        this.Slot(key, slot);
    }

    /// <summary>Counts one event or battle NPC under every category it belongs to.</summary>
    /// <param name="slot">Its object table index.</param>
    /// <param name="kind"><c>EventNpc</c> or <c>BattleNpc</c>.</param>
    /// <param name="subKind">Its battle NPC sub-kind (ignored for event NPCs).</param>
    /// <param name="owned">Whether <c>OwnerId</c> names an owner.</param>
    /// <param name="nameLength">Bytes of name before the terminator; 0 means no name.</param>
    /// <param name="inMaxCube">Whether it is inside the largest cube the settings allow.</param>
    public void CountNpc(int slot, CsObjectKind kind, BattleNpcSubKind subKind, bool owned, int nameLength, bool inMaxCube)
    {
        var kindKey = kind == CsObjectKind.EventNpc ? "event" : "battle" + subKind;
        this.Add(kindKey);
        this.Slot(kindKey, slot);
        this.Slot(kind == CsObjectKind.EventNpc ? "eventAll" : "battleAll", slot);

        if (nameLength == 0)
        {
            this.Add(kindKey + "NoName");
        }

        if (owned)
        {
            this.Add(kindKey + "Owned");
        }

        var petOrBuddy = kind == CsObjectKind.BattleNpc && (subKind == BattleNpcSubKind.Pet || subKind == BattleNpcSubKind.Buddy);
        if (nameLength > 0 && !owned && !petOrBuddy)
        {
            this.Add("named");
            this.Add(kind == CsObjectKind.EventNpc ? "namedEvent" : "namedBattle");
            if (inMaxCube)
            {
                this.Add("namedInMaxCube");
            }
        }
    }

    /// <summary>Remembers an NPC's name and counts a change if the same NPC had a different name before.</summary>
    /// <param name="key">Entity id and slot together, so a reused slot isn't mistaken for a rename.</param>
    /// <param name="name">The name bytes, without the terminator.</param>
    public void TrackName(ulong key, ReadOnlySpan<byte> name)
    {
        if (this.names.TryGetValue(key, out var previous))
        {
            if (!name.SequenceEqual(previous))
            {
                this.NameChanges++;
                this.names[key] = name.ToArray();
            }

            return;
        }

        this.names[key] = name.ToArray();
    }

    /// <summary>Writes every total as a metric and one summary line to the log (counts only, no names).</summary>
    /// <param name="context">Where the metrics go.</param>
    /// <param name="log">Where the summary goes, at Information.</param>
    public void Record(BridgeContext context, IPluginLog log)
    {
        context.Metric("npcFrames", this.Frames);
        context.Metric("objectTableLength", this.TableLength);
        context.Metric("npcDistinctNamed", this.names.Count);
        context.Metric("npcNameChanges", this.NameChanges);

        var summary = new StringBuilder();
        summary.Append(CultureInfo.InvariantCulture, $"Probe #73: {this.Frames} frames, table length {this.TableLength}, {this.names.Count} distinct named NPCs, {this.NameChanges} name changes; max per frame:");
        foreach (var (key, value) in this.max)
        {
            context.Metric("npcMax_" + key, value);
            summary.Append(CultureInfo.InvariantCulture, $" {key}={value}");
        }

        summary.Append("; slots:");
        foreach (var (key, (min, max)) in this.slots)
        {
            context.Metric("npcSlotMin_" + key, min);
            context.Metric("npcSlotMax_" + key, max);
            summary.Append(CultureInfo.InvariantCulture, $" {key}={min}-{max}");
        }

        log.Information(summary.ToString());
    }

    /// <summary>Adds one to this frame's count for a category.</summary>
    /// <param name="key">The category.</param>
    private void Add(string key) => this.current[key] = this.current.GetValueOrDefault(key) + 1;

    /// <summary>Widens a category's slot range to include a slot.</summary>
    /// <param name="key">The category.</param>
    /// <param name="slot">The object table index seen.</param>
    private void Slot(string key, int slot)
    {
        this.slots[key] = this.slots.TryGetValue(key, out var range)
            ? (Math.Min(range.Min, slot), Math.Max(range.Max, slot))
            : (slot, slot);
    }
}
