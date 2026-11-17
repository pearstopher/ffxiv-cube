using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace Cube.Snapshot;

/// <summary>
/// Writes a <see cref="Canvas"/> as a PNG file (8-bit RGBA, no filtering), using only the .NET base library so
/// the tool needs no image package.
/// </summary>
public static class PngWriter
{
    /// <summary>The 8-byte PNG file signature.</summary>
    private static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>CRC-32 lookup table (polynomial 0xEDB88320), as PNG chunks require.</summary>
    private static readonly uint[] CrcTable = BuildCrcTable();

    /// <summary>Encodes a canvas as PNG bytes.</summary>
    /// <param name="canvas">The image.</param>
    public static byte[] Encode(Canvas canvas)
    {
        using var output = new MemoryStream();
        output.Write(Signature);

        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, canvas.Width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], canvas.Height);
        header[8] = 8;  // bits per channel
        header[9] = 6;  // colour type: RGBA
        header[10] = 0; // compression: deflate
        header[11] = 0; // filter method
        header[12] = 0; // no interlace
        WriteChunk(output, "IHDR", header);

        // Each row starts with a filter-type byte (0 = none).
        var stride = canvas.Width * 4;
        var raw = new byte[(stride + 1) * canvas.Height];
        for (var y = 0; y < canvas.Height; y++)
        {
            canvas.Pixels.Slice(y * stride, stride).CopyTo(raw.AsSpan((y * (stride + 1)) + 1));
        }

        using (var compressed = new MemoryStream())
        {
            using (var zlib = new ZLibStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
            {
                zlib.Write(raw);
            }

            WriteChunk(output, "IDAT", compressed.ToArray());
        }

        WriteChunk(output, "IEND", []);
        return output.ToArray();
    }

    /// <summary>Writes one chunk: length, type, data, CRC of type and data.</summary>
    /// <param name="output">Where to write.</param>
    /// <param name="type">Four-letter chunk type.</param>
    /// <param name="data">Chunk contents.</param>
    private static void WriteChunk(Stream output, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> four = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(four, data.Length);
        output.Write(four);

        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        var crc = Crc(Crc(0xFFFFFFFF, typeBytes), data) ^ 0xFFFFFFFF;
        BinaryPrimitives.WriteUInt32BigEndian(four, crc);
        output.Write(four);
    }

    /// <summary>Continues a CRC-32 over more bytes.</summary>
    /// <param name="crc">The running value.</param>
    /// <param name="bytes">The bytes to add.</param>
    private static uint Crc(uint crc, ReadOnlySpan<byte> bytes)
    {
        foreach (var b in bytes)
        {
            crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        }

        return crc;
    }

    /// <summary>Builds the CRC-32 table.</summary>
    private static uint[] BuildCrcTable()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            var c = n;
            for (var k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            }

            table[n] = c;
        }

        return table;
    }
}
