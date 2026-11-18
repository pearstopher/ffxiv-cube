using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Decides where the cube is anchored each frame (PROJECT.md, "Exact behaviour rules", Cube, "Jump"; #105): on the
/// local player, except that while the player is jumping its height is held at the take-off height, so the world
/// stays still in the cube and the player's marker visibly rises and lands.
/// </summary>
/// <remarks>
/// <para>
/// A small two-state machine, <i>following</i> and <i>held</i>. While following, <see cref="Update"/> returns the
/// player's position unchanged, so everything behaves exactly as before #105. While held, x and z still follow the
/// player every frame and only y is replaced by the take-off height. Landing ends the hold in the frame the flag
/// clears, so landing on a ledge higher or lower than the take-off point snaps the cube to the new height, as walking
/// up or down does.
/// </para>
/// <para>
/// Walking off a ledge, falling, swimming, mounts, flying and being moved by the game never set the jump flag, so they
/// never hold. The take-off height is the previous frame's height, because the game may raise the jump flag a frame
/// after the character leaves the ground. The anchor feeds <see cref="CubeSpace"/> as the player position, so the
/// player-height offset (<see cref="PlayerHeight"/>) is applied to the held height the same way. Pure maths with no
/// game references and no allocations, so it is unit tested without the game.
/// </para>
/// </remarks>
public sealed class CubeAnchor
{
    /// <summary>The previous frame's player height, used as the take-off height when a jump starts.</summary>
    private float lastY;

    /// <summary>Whether <see cref="lastY"/> holds a real frame (false at the start and after <see cref="Reset"/>).</summary>
    private bool hasLast;

    /// <summary>The height the cube is held at while <see cref="IsHeld"/>.</summary>
    private float heldY;

    /// <summary>Whether the cube is currently held at the take-off height.</summary>
    public bool IsHeld { get; private set; }

    /// <summary>The height the cube is held at; only meaningful while <see cref="IsHeld"/>.</summary>
    public float HeldY => this.heldY;

    /// <summary>
    /// Gives the position the cube is anchored on this frame.
    /// </summary>
    /// <param name="player">The local player's world position this frame.</param>
    /// <param name="isJumping">Whether the game says the local player is jumping.</param>
    /// <returns>The player's position, with y replaced by the take-off height while the jump lasts.</returns>
    public Vector3 Update(Vector3 player, bool isJumping)
    {
        if (!isJumping)
        {
            this.IsHeld = false;
        }
        else if (!this.IsHeld)
        {
            this.IsHeld = true;
            this.heldY = this.hasLast ? this.lastY : player.Y;
        }

        this.lastY = player.Y;
        this.hasLast = true;
        return this.IsHeld ? new Vector3(player.X, this.heldY, player.Z) : player;
    }

    /// <summary>
    /// Forgets the previous frame and any hold. Called whenever the cube isn't being built (hidden, no local player),
    /// so a jump can't carry a stale height across a zone change or a loading screen.
    /// </summary>
    public void Reset()
    {
        this.IsHeld = false;
        this.hasLast = false;
    }
}
