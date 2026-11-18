using System.Numerics;

namespace Cube.Core;

/// <summary>
/// One player or named NPC inside the cube this frame: where they are, how they relate to the local player, and
/// their (cached) name for the label. NPCs use the same record so they share the players' culling, draw order and
/// label code (PROJECT.md, "NPCs": shown the same way as players).
/// </summary>
/// <param name="Slot">Object table index the player was read from; keys the name cache.</param>
/// <param name="EntityId">The game's id for the player, used by the bridge to compare with an independent read.</param>
/// <param name="Position">World position in yalms.</param>
/// <param name="Relation">Self, party/alliance, other, NPC or enemy.</param>
/// <param name="Name">The name, shared from <see cref="NameCache"/>: only drawn on screen, never stored or sent (PROJECT.md, "Privacy").</param>
/// <param name="Label">The text drawn: <paramref name="Name"/> with any <c>[T]</c>/<c>[!]</c> markers (#111), cached by <see cref="MarkerLabels"/>.</param>
public readonly record struct NearbyPlayer(int Slot, uint EntityId, Vector3 Position, PlayerRelation Relation, string Name, string Label = "");
