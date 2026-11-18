using System.Globalization;
using System.Numerics;
using Cube.Core;
using Cube.Snapshot;

// Dev-only snapshot tool (PROJECT.md, "Automated testing": snapshot renderer).
//   Cube.Snapshot <drawlist.json> <snapshot.png>   render a draw list dumped by the in-game test bridge
//   Cube.Snapshot --demo <snapshot.png> [yaw] [viewYaw° viewPitch°]
//                                                  render Core's output for a sample scene, no game needed
if (args.Length >= 2 && args[0] == "--demo")
{
    var yaw = args.Length >= 3 ? float.Parse(args[2], CultureInfo.InvariantCulture) : 0.5f;
    var view = args.Length >= 5
        ? ViewOrientation.FromDegrees(float.Parse(args[3], CultureInfo.InvariantCulture), float.Parse(args[4], CultureInfo.InvariantCulture))
        : ViewOrientation.Default;
    var size = new Vector2(400, 360);
    var space = new CubeSpace(Vector3.Zero, CubeSpace.DefaultEdge, yaw);
    var projection = ViewProjection.Fit(Vector2.Zero, size, space.HalfEdge, view);
    var list = new DrawList(256);
    CubeGeometry.AddWireCube(list, space, projection, Colour.Pack(0.85f, 0.85f, 0.85f, 0.8f));
    CubeGeometry.AddSolidCube(list, space, projection, Vector3.Zero, 0.5f, Colour.Pack(0.2f, 0.6f, 1f, 1f));
    CubeGeometry.AddSolidCube(list, space, projection, space.RotateOffset(new Vector3(8, 0, -12)), 0.5f, Colour.Pack(0.3f, 1f, 0.4f, 1f));
    CubeGeometry.AddSolidCube(list, space, projection, space.RotateOffset(new Vector3(-10, 2, 6)), 0.5f, Colour.Pack(1f, 0.6f, 0.2f, 1f));
    list.SortBackToFront();
    File.WriteAllBytes(args[1], PngWriter.Encode(SnapshotRenderer.Paint(Vector2.Zero, size, list.Items)));
    Console.WriteLine($"Wrote {args[1]} (demo, yaw {yaw}, view rotation {view.Rotation})");
    return 0;
}

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: Cube.Snapshot <drawlist.json> <snapshot.png>  |  Cube.Snapshot --demo <snapshot.png> [yaw] [viewYaw viewPitch]");
    return 2;
}

var canvas = SnapshotRenderer.Render(File.ReadAllText(args[0]));
File.WriteAllBytes(args[1], PngWriter.Encode(canvas));
Console.WriteLine($"Wrote {args[1]} ({canvas.Width}x{canvas.Height})");
return 0;
