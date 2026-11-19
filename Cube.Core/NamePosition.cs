namespace Cube.Core;

/// <summary>Where a name label sits relative to its cube (#112; <see cref="CubeSettings.NamePosition"/>).</summary>
public enum NamePosition : byte
{
    /// <summary>Beside the cube, to its right: the original behaviour and the default.</summary>
    Side = 0,

    /// <summary>Centred horizontally above the cube.</summary>
    Above = 1,
}
