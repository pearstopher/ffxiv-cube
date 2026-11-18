namespace Cube.Core;

/// <summary>
/// The fields of a game collider that decide whether the player can bump into it, copied out of the game's struct so
/// the decision (<see cref="CollisionFilter.Blocks"/>) is a pure function that can be unit tested.
/// </summary>
/// <param name="VisibilityFlags">The collider's <c>VisibilityFlags</c>; bit 1 is set when the game's raycasts use it (#28).</param>
/// <param name="LayerMask">The collider's <c>LayerMask</c>; bit 0 is the layer movement queries use (#28, vnavmesh notes).</param>
/// <param name="ObjectMaterialValue">The collider's <c>ObjectMaterialValue</c>; bit 0x4000 marks primitives that block (#107).</param>
/// <param name="IsPrimitive">True for box, cylinder, sphere and plane colliders; false for mesh colliders.</param>
public readonly record struct ColliderFields(byte VisibilityFlags, ulong LayerMask, ulong ObjectMaterialValue, bool IsPrimitive);

/// <summary>
/// Decides which of the game's colliders are drawn (PROJECT.md, "Phase 2": collision means "what the player would
/// bump into, walk on, or be stopped by"). Colliders that exist only for something else are dropped, because drawing
/// them shows walls the player can walk through (#107).
/// </summary>
/// <remarks>
/// <para>
/// Evidence (<c>docs/research-notes.md</c>, "Phantom boxes (#107)"): the game's own movement raycast was cast through
/// every collider box within 150 yalms. Every mesh collider kind it was ever asked about was hit (materials 0 and
/// 0x7000), but among primitive colliders only those with material bit 0x4000 (<c>4000</c>, <c>4005</c>, <c>6004</c>,
/// <c>6406</c>) were ever hit; the roughly 200 primitives with only bit 0x2000 (<c>2004</c>, <c>2005</c>, <c>2006</c>,
/// <c>200A</c>, <c>3005</c> and so on) were never hit by any ray. Those are the invisible trigger and event volumes
/// the player walks through.
/// </para>
/// <para>
/// Mesh node boxes that are larger than their triangles are a separate matter (box coarseness, not filtering) and are
/// left to the real-triangle work in #100.
/// </para>
/// </remarks>
public static class CollisionFilter
{
    /// <summary>The bit of <see cref="ColliderFields.VisibilityFlags"/> marking colliders the game's raycasts use.</summary>
    public const byte RaycastActive = 1;

    /// <summary>The bit of <see cref="ColliderFields.LayerMask"/> for the layer movement queries use.</summary>
    public const ulong MovementLayer = 1;

    /// <summary>The bit of <see cref="ColliderFields.ObjectMaterialValue"/> that a primitive collider needs to block.</summary>
    public const ulong BlockingMaterial = 0x4000;

    /// <summary>
    /// Whether a collider is something the player could bump into: active for raycasts, on the movement layer, and,
    /// if it is a primitive, carrying the blocking material bit.
    /// </summary>
    /// <param name="fields">The collider's fields.</param>
    public static bool Blocks(ColliderFields fields) =>
        (fields.VisibilityFlags & RaycastActive) != 0
        && (fields.LayerMask & MovementLayer) != 0
        && (!fields.IsPrimitive || (fields.ObjectMaterialValue & BlockingMaterial) != 0);
}
