using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The settings window's zoom slider (#94; PROJECT.md, "Zoom"): one value, the box's largest extent in yalms, that
/// scales width, height and depth together keeping their ratios. It saves nothing of its own: its value always comes
/// from the current extents, so moving a width/height/depth slider or a gizmo handle moves it too.
/// </summary>
/// <remarks>
/// <para>
/// While the slider is dragged, every value is applied to the size the drag started from (see <see cref="Drag"/>),
/// not to the previous frame's result. Dragging back and forth therefore never lets rounding build up: returning to
/// the starting value gives exactly the starting box.
/// </para>
/// <para>
/// The value and range are cached against the size they were worked out for and only recomputed when it changes, so
/// an open settings window does no work for the slider on frames where nothing moves. A plain struct with no
/// allocations; the window keeps one as a field.
/// </para>
/// </remarks>
public struct ZoomSlider
{
    /// <summary>The size <see cref="Value"/>, <see cref="Low"/> and <see cref="High"/> were worked out for.</summary>
    private Vector3 cachedFor;

    /// <summary>The size a drag started from; every value of the drag scales this, see <see cref="Drag"/>.</summary>
    private Vector3 dragBase;

    /// <summary>Whether a drag is in progress, so <see cref="dragBase"/> holds its starting size.</summary>
    private bool dragging;

    /// <summary>The slider's value for the last size given to <see cref="Update"/>: its largest extent.</summary>
    public float Value { get; private set; }

    /// <summary>The slider's lower limit: where the smallest extent would be 10 (<see cref="BoxExtents.ZoomRange"/>).</summary>
    public float Low { get; private set; }

    /// <summary>The slider's upper limit: where the largest extent would be 100.</summary>
    public float High { get; private set; }

    /// <summary>
    /// Brings <see cref="Value"/>, <see cref="Low"/> and <see cref="High"/> up to date with the current size, working
    /// them out again only if it changed since the last call.
    /// </summary>
    /// <param name="size">The cube's current width, height and depth.</param>
    public void Update(Vector3 size)
    {
        if (size == this.cachedFor && this.High > 0f)
        {
            return;
        }

        this.cachedFor = size;
        var clamped = BoxExtents.Clamp(size);
        this.Value = BoxExtents.Largest(clamped);
        (this.Low, this.High) = BoxExtents.ZoomRange(clamped);
    }

    /// <summary>
    /// Applies a slider value: the size whose largest extent is <paramref name="value"/>, with the ratios of the size
    /// the drag started from. The first call of a drag records <paramref name="currentSize"/> as that start.
    /// </summary>
    /// <param name="currentSize">The cube's size now; used only on the first call of a drag.</param>
    /// <param name="value">The slider's new value, the wanted largest extent in yalms.</param>
    /// <returns>The new size, clamped so no extent leaves 10–100.</returns>
    public Vector3 Drag(Vector3 currentSize, float value)
    {
        if (!this.dragging)
        {
            this.dragBase = currentSize;
            this.dragging = true;
        }

        return BoxExtents.ZoomTo(this.dragBase, value);
    }

    /// <summary>Ends a drag, so the next <see cref="Drag"/> starts from the size at that point.</summary>
    public void EndDrag() => this.dragging = false;
}
