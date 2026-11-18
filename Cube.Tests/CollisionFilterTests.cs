using Cube.Core;
using Xunit;

namespace Cube.Tests;

/// <summary>
/// Checks which colliders are drawn: PROJECT.md, "Phase 2", says collision is "what the player would bump into, walk
/// on, or be stopped by", so colliders the player walks through (#107) must be dropped.
/// </summary>
public class CollisionFilterTests
{
    /// <summary>Visible to raycasts, on the movement layer.</summary>
    private const byte Active = 1;

    /// <summary>The movement layer bit.</summary>
    private const ulong Layer0 = 1;

    [Theory]
    [InlineData(0UL)]
    [InlineData(0x7000UL)]
    public void MeshOnMovementLayer_IsDrawn_WhateverItsMaterial(ulong material) =>
        Assert.True(CollisionFilter.Blocks(new ColliderFields(Active, Layer0, material, IsPrimitive: false)));

    [Theory]
    [InlineData(0x4000UL)]
    [InlineData(0x4005UL)]
    [InlineData(0x6004UL)]
    [InlineData(0x6406UL)]
    public void PrimitiveWithBlockingMaterial_IsDrawn(ulong material) =>
        Assert.True(CollisionFilter.Blocks(new ColliderFields(Active, Layer0, material, IsPrimitive: true)));

    [Theory]
    [InlineData(0x2004UL)]
    [InlineData(0x2005UL)]
    [InlineData(0x2006UL)]
    [InlineData(0x200AUL)]
    [InlineData(0x3005UL)]
    [InlineData(0x2000UL)]
    [InlineData(0UL)]
    public void PrimitiveWithoutBlockingMaterial_IsNotDrawn(ulong material) =>
        Assert.False(CollisionFilter.Blocks(new ColliderFields(Active, Layer0, material, IsPrimitive: true)));

    [Fact]
    public void ColliderRaycastsIgnore_IsNotDrawn() =>
        Assert.False(CollisionFilter.Blocks(new ColliderFields(0, Layer0, 0x4000, IsPrimitive: true)));

    [Fact]
    public void ColliderOffTheMovementLayer_IsNotDrawn() =>
        Assert.False(CollisionFilter.Blocks(new ColliderFields(Active, 0xA, 0x7000, IsPrimitive: true)));

    [Fact]
    public void MeshOffTheMovementLayer_IsNotDrawn() =>
        Assert.False(CollisionFilter.Blocks(new ColliderFields(Active, 0xA, 0, IsPrimitive: false)));
}
