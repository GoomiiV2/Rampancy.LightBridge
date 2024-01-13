using System;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RampantC20
{
    public partial class Utils
    {
        // Just grab and decompress the color plate from a bitmap
        public static (int width, int height, byte[] pixels)? GetColorPlateFromBitMap(AssetDb.TagInfo tagInfo)
        {
            using var br = tagInfo.GetReader();
            var (width, height) = GetH1BitmapDimensions(br);
            var byteSize = (int) SwapEndianness((uint) br.ReadInt32());
            br.BaseStream.Seek(80, SeekOrigin.Current);
            var plateBytes = br.ReadBytes(byteSize);

            var outSize = (width * height) * 4; // RGBA
            using (var stream = new BinaryReader(new ZLibStream(new MemoryStream(plateBytes), CompressionMode.Decompress, true))) {
                var pixels = stream.ReadBytes(outSize);
                return (width, height, pixels);
            }
        }

        public static (int width, int height) GetH1BitmapDimensions(BinaryReader br)
        {
            br.BaseStream.Seek(88, SeekOrigin.Begin);
            var width  = SwapEndianness(br.ReadUInt16());
            var height = SwapEndianness(br.ReadUInt16());

            return (width, height);
        }

        // Convert a height map to a normal map with a sobel edge filter like
        public static byte[] HeightmapToNormal(int width, int height, byte[] pixels, float normalStrength)
        {
            var norm = new byte[pixels.Length];

            for (int i = 0; i < pixels.Length; i += 4) {
                var upIdx                           = i - (width * 4);
                var downIdx                         = i + (width * 4);
                var leftIdx                         = i - 1;
                var rightIdx                        = i + 1;
                
                var up    = upIdx   > 0 ? RGBToGreyscale(pixels[upIdx], pixels[upIdx                 + 1], pixels[upIdx    + 2]) : 0;
                var down  = downIdx < pixels.Length ? RGBToGreyscale(pixels[downIdx], pixels[downIdx + 1], pixels[downIdx  + 2]) : 0;
                var left  = leftIdx > 0 ? RGBToGreyscale(pixels[leftIdx], pixels[leftIdx             + 1], pixels[leftIdx  + 2]) : 0;
                var right = rightIdx < pixels.Length ? RGBToGreyscale(pixels[rightIdx], pixels[rightIdx + 1], pixels[rightIdx + 2]) : 0;

                var dx     = new Vector3(1, 0, (right - left) * normalStrength);
                var dy     = new Vector3(0, 1, (down  - up)   * normalStrength);
                var normal = Vector3.Cross(dx, dy);
                normal = Vector3.Normalize(normal);

                byte r = (byte)((normal.X + 1.0f) * 127.5f);
                byte g = (byte)(255 - ((normal.Y + 1.0f) * 127.5f));

                norm[i]     = 255;                                                                                  // b
                norm[i + 1] = g;                                                                                    // g
                norm[i + 2] = r;                                                                                    // r
                norm[i + 3] = (byte) (256 / RGBToGreyscale(pixels[i], pixels[i + 1], pixels[i + 2]));          // a
            }

            return norm;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int To1DIndex(int x, int y, int width) => x + width * y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RGBToGreyscale(byte r, byte g, byte b) => ((r + b + g) / 3) / 255f;

        public static uint SwapEndianness(uint x)
        {
            x = (x >> 16) | (x << 16);
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        public static float SwapEndianness(float val) => SwapEndianness((uint) val);

        public static int SwapEndianness(int val) => (int) SwapEndianness((uint) val);

        public static ushort SwapEndianness(ushort x)
        {
            return (ushort) ((ushort) ((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcAreaOfTri(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var l1 = Vector3.Distance(v1, v2);
            var l2 = Vector3.Distance(v1, v3);
            var l3 = Vector3.Distance(v2, v3);

            var semiPerm = (l1 + l2 + l3) / 2;
            var areaToBeSqrRooted = semiPerm * (semiPerm - l1) * (semiPerm - l2) * (semiPerm - l3);
            var area = (float)Math.Sqrt(areaToBeSqrRooted);

            return area;
        }

        // https://i.imgflip.com/7fgjfe.jpg
        public static bool IsDegenerateTri(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            const float TOLERANCE = 0.0001f;
            var area = CalcAreaOfTri(v1, v2, v3);

            if (area <= TOLERANCE ||
                Math.Abs(Vector3.Distance(v1, v2)) <= TOLERANCE ||
                Math.Abs(Vector3.Distance(v1, v3)) <= TOLERANCE ||
                Math.Abs(Vector3.Distance(v2, v3)) <= TOLERANCE)
                return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetTriNormal(Vector3 vi1, Vector3 vi2, Vector3 vi3)
        {
            var v0 = vi2 - vi1;
            var v1 = vi3 - vi1;
            var n  = Vector3.Cross(v0, v1);

            return Vector3.Normalize(n);
        }

        public static Vector3 ProjectPointToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 relativePoint = point - lineStart;
            Vector3 lineDirection = lineEnd - lineStart;
            float length = lineDirection.Length();
            Vector3 normalizedLineDirection = lineDirection;
            if (length > .000001f)
            {
                normalizedLineDirection /= length;
            }

            float dot = Vector3.Dot(normalizedLineDirection, relativePoint);
            dot = Math.Clamp(dot, 0.0F, length);

            return lineStart + normalizedLineDirection * dot;
        }

        public static float DistancePointToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return (ProjectPointToLine(point, lineStart, lineEnd) - point).Length();
        }

        public static string Halo1MatFlagsToString(uint flags)
        {
            var str = "";

            if ((flags & (1 << 10)) != 0)
                str += "%";

            if ((flags & (1 << 11)) != 0)
                str += "#";

            if ((flags & (1 << 12)) != 0)
                str += "!";

            if ((flags & (1 << 13)) != 0)
                str += "*";

            if ((flags & (1 << 14)) != 0)
                str += "@";

            if ((flags & (1 << 15)) != 0)
                str += "$";

            if ((flags & (1 << 16)) != 0)
                str += "^";

            if ((flags & (1 << 17)) != 0)
                str += "-";

            if ((flags & (1 << 18)) != 0)
                str += "&";

            if ((flags & (1 << 19)) != 0)
                str += ".";

            return str;
        }
    }
}