using System.Buffers.Binary;
using System.IO.Compression;

namespace FractalBlazor.Components.Tools
{
    /// <summary>
    /// Minimal PNG decoder for the palette images: 8-bit RGB or RGBA, not interlaced. It only
    /// needs the framework's zlib stream, so the tool has no third-party dependency.
    /// </summary>
    internal sealed class PngReader
    {
        private static ReadOnlySpan<byte> Signature => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

        private readonly byte[] _pixels;
        private readonly int _channels;

        private PngReader(int width, int height, int channels, byte[] pixels)
        {
            Width = width;
            Height = height;
            _channels = channels;
            _pixels = pixels;
        }

        public int Width { get; }

        public int Height { get; }

        public (byte R, byte G, byte B) this[int x, int y]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfNegative(x);
                ArgumentOutOfRangeException.ThrowIfNegative(y);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, Width);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, Height);

                var offset = ((y * Width) + x) * _channels;
                return (_pixels[offset], _pixels[offset + 1], _pixels[offset + 2]);
            }
        }

        public static PngReader Load(string path)
        {
            using var file = File.OpenRead(path);
            var signature = new byte[8];
            file.ReadExactly(signature);
            if (!signature.AsSpan().SequenceEqual(Signature))
                throw new InvalidDataException("The file is not a PNG image.");

            int width = 0, height = 0, channels = 0;
            using var compressed = new MemoryStream();
            var header = new byte[8];

            while (true)
            {
                file.ReadExactly(header);
                var length = BinaryPrimitives.ReadInt32BigEndian(header);
                var type = System.Text.Encoding.ASCII.GetString(header, 4, 4);
                var data = new byte[length];
                file.ReadExactly(data);
                file.ReadExactly(new byte[4]); // CRC

                if (type == "IHDR")
                {
                    width = BinaryPrimitives.ReadInt32BigEndian(data);
                    height = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(4));
                    var bitDepth = data[8];
                    var colorType = data[9];
                    var interlace = data[12];
                    channels = colorType switch { 2 => 3, 6 => 4, _ => 0 };
                    if (bitDepth != 8 || channels == 0 || interlace != 0)
                        throw new NotSupportedException("Only 8-bit, non-interlaced RGB or RGBA PNG images are supported.");
                }
                else if (type == "IDAT")
                {
                    compressed.Write(data);
                }
                else if (type == "IEND")
                {
                    break;
                }
            }

            if (channels == 0)
                throw new InvalidDataException("The PNG image has no IHDR chunk.");

            compressed.Position = 0;
            using var inflater = new ZLibStream(compressed, CompressionMode.Decompress);
            var stride = width * channels;
            var raw = new byte[(stride + 1) * height];
            inflater.ReadExactly(raw);

            return new PngReader(width, height, channels, Unfilter(raw, width, height, channels));
        }

        private static byte[] Unfilter(byte[] raw, int width, int height, int channels)
        {
            var stride = width * channels;
            var pixels = new byte[stride * height];

            for (var y = 0; y < height; y++)
            {
                var filter = raw[y * (stride + 1)];
                var line = raw.AsSpan((y * (stride + 1)) + 1, stride);
                var current = pixels.AsSpan(y * stride, stride);
                var previous = y > 0 ? pixels.AsSpan((y - 1) * stride, stride) : Span<byte>.Empty;

                for (var i = 0; i < stride; i++)
                {
                    int left = i >= channels ? current[i - channels] : 0;
                    int up = y > 0 ? previous[i] : 0;
                    int upLeft = y > 0 && i >= channels ? previous[i - channels] : 0;

                    current[i] = (byte)(line[i] + filter switch
                    {
                        0 => 0,
                        1 => left,
                        2 => up,
                        3 => (left + up) / 2,
                        4 => Paeth(left, up, upLeft),
                        _ => throw new InvalidDataException($"Unknown PNG filter {filter}.")
                    });
                }
            }

            return pixels;
        }

        private static int Paeth(int left, int up, int upLeft)
        {
            var estimate = left + up - upLeft;
            var toLeft = Math.Abs(estimate - left);
            var toUp = Math.Abs(estimate - up);
            var toUpLeft = Math.Abs(estimate - upLeft);
            if (toLeft <= toUp && toLeft <= toUpLeft)
                return left;
            return toUp <= toUpLeft ? up : upLeft;
        }
    }
}
