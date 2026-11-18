using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the collision detail levels (#32, collision detail setting; PROJECT.md, "Collision (phase 2)": simplified
/// boxes): Full walks the whole node tree, Medium stops a few levels down, Coarse keeps one box per collider, and every
/// level is its own stamp so a change gathers again at once.
/// </summary>
public class CollisionDetailsTests
{
    [Fact]
    public void Full_detail_walks_the_whole_tree()
    {
        Assert.Equal(int.MaxValue, CollisionDetails.MaxNodeDepth(CollisionDetail.Full));
    }

    [Fact]
    public void Coarse_detail_keeps_only_the_root_box_of_each_mesh()
    {
        Assert.Equal(0, CollisionDetails.MaxNodeDepth(CollisionDetail.Coarse));
    }

    [Fact]
    public void Medium_detail_is_between_coarse_and_full()
    {
        var medium = CollisionDetails.MaxNodeDepth(CollisionDetail.Medium);

        Assert.InRange(medium, CollisionDetails.MaxNodeDepth(CollisionDetail.Coarse) + 1, CollisionDetails.MaxNodeDepth(CollisionDetail.Full) - 1);
    }

    [Fact]
    public void Each_detail_level_has_its_own_stamp_for_the_same_scene()
    {
        var stamps = Enum.GetValues<CollisionDetail>().Select(d => CollisionDetails.Stamp(12345, d)).ToArray();

        Assert.Equal(stamps.Length, stamps.Distinct().Count());
    }

    [Fact]
    public void Different_scenes_have_different_stamps_at_the_same_detail()
    {
        Assert.NotEqual(CollisionDetails.Stamp(1, CollisionDetail.Full), CollisionDetails.Stamp(2, CollisionDetail.Full));
        Assert.NotEqual(CollisionDetails.Stamp(1, CollisionDetail.Coarse), CollisionDetails.Stamp(2, CollisionDetail.Coarse));
    }
}
