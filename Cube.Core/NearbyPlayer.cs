using System.Numerics;

namespace Cube.Core;

/// <summary>
/// One player inside the cube this frame: where they are, how they relate to the local player, and their
/// (cached) name for the label.
/// </summary>
/// <param name="Slot">Object table index the player was read from; keys the name cache.</param>
/// <param name="EntityId">The game's id for the player, used by the bridge to compare with an independent read.</param>
/// <param name="Position">World position in yalms.</param>
/// <param name="Relation">Self, party/alliance or other.</param>
/// <param name="Name">The player's name, shared from <see cref="NameCache"/>: only drawn on screen, never stored or sent (PROJECT.md, "Privacy").</param>
public readonly record struct NearbyPlayer(int Slot, uint EntityId, Vector3 Position, PlayerRelation Relation, string Name);
