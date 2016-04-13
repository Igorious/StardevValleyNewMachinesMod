using System.Globalization;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class Color
    {
        public Color() {}

        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public static Color FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            return new Color
            {
                R = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                G = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                B = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
            };
        }

        public override string ToString()
        {
            return $"{R} {G} {B}";
        }

        public string ToHex()
        {
            return $"{R:X2}{G:X2}{B:X2}";
        }

        public static implicit operator Microsoft.Xna.Framework.Color(Color c)
        {
            return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B);
        }
    }
}