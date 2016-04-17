using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Services;
using Microsoft.Xna.Framework.Graphics;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public sealed class DominantColorFinder
    {
        private static readonly Dictionary<int, XColor> ColorCache = new Dictionary<int, XColor>();

        public static XColor GetDominantColor(int spriteIndex, Texture2D texture, int width, int height)
        {
            XColor cachedColor;
            if (ColorCache.TryGetValue(spriteIndex, out cachedColor)) return cachedColor;

            var rect = TexturesService.GetSourceRectForObject(spriteIndex, texture, width, height);
            var data = new XColor[width * height];
            texture.GetData(0, rect, data, 0, data.Length);

            var colors = new Dictionary<int, int>();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var xColor = data[y * width + x];
                    if (xColor.A == 0) continue;

                    double h, s, l;
                    var c = new RawColor(xColor.R, xColor.G, xColor.B);
                    c.ToHSL(out h, out s, out l);
                    if (l <= 0.15 || 0.85 <= l || s <= 0.2) continue;

                    var kh = (int)((h + 5) / 10); // 0..36

                    var ks = (int)(s * 3); // 0..3
                    var kl = (int)(l * 3); // 0..3
                    var key = kh * 100 + ks * 10 + kl;
                    if (colors.ContainsKey(key))
                    {
                        colors[key]++;
                    }
                    else
                    {
                        colors.Add(key, 1);
                    }
                }
            }

            if (!colors.Any())
            {
                cachedColor = XColor.Gray;
            }
            else
            {
                var dominantColors = colors.OrderByDescending(x => x.Value).Take(2).ToList();
                var dominantColor1 = dominantColors.First();
                var dominantColor2 = dominantColors.Last();

                var dominantColorKey = dominantColor1.Key;
                if (900 <= dominantColorKey && dominantColorKey <= 1500 && dominantColor2.Value * 4 / 3 >= dominantColor1.Value)
                {
                    dominantColorKey = dominantColor2.Key;
                }

                var realH = Math.Max(dominantColorKey / 100 * 10 - 5, 0);
                cachedColor = RawColor.FromHSL(realH, 0.8, 0.5).ToXnaColor();
            }

            ColorCache.Add(spriteIndex, cachedColor);
            return cachedColor;
        }
    }
}
