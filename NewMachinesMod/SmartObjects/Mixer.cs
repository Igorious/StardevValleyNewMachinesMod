using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Mixer : CustomMachineBase
    {
        public Mixer() : base(ClassMapperService.Instance.GetID<Mixer>()) { }

        private Object FirstDroppedItem { get; set; }
        private Object SecondDroppedItem { get; set; } 

        protected override bool OnPickaxeAction(Pickaxe pickaxe)
        {
            if (FirstDroppedItem != null)
            {
                Game1.currentLocation.debris.Add(new Debris(FirstDroppedItem, TileLocation, TileLocation));
                FirstDroppedItem = null;
            }
            return base.OnPickaxeAction(pickaxe);
        }

        protected override bool OnAxeAction(Axe axe)
        {
            if (FirstDroppedItem != null)
            {
                Game1.currentLocation.debris.Add(new Debris(FirstDroppedItem, TileLocation));
                FirstDroppedItem = null;
            }
            return base.OnAxeAction(axe);
        }

        protected override MachineInformation MachineInformation => NewMachinesMod.Config.Mixer;

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            var result = base.minutesElapsed(minutes, environment);
            if (readyForHarvest)
            {
                FirstDroppedItem = null;
                SecondDroppedItem = null;
            }
            return result;
        }

        protected override bool PerformDropIn(Object item, Farmer farmer)
        {
            if (FirstDroppedItem == null)
            {
                FirstDroppedItem = item;
            }
            else
            {
                SecondDroppedItem = item;
                PlaySound(Sound.Ship);
                var q1 = FirstDroppedItem.quality;
                var q2 = SecondDroppedItem.quality;

                var itemQuality = Math.Max(0, q1 + q2 - 1);
                var itemPrice = (FirstDroppedItem.price * (4 + q1) + SecondDroppedItem.price * (4 + q2)) / 4;

                var random = GetRandom();
                var postfix = $"{FirstDroppedItem.Name.First()}{SecondDroppedItem.Name.First()}-{(char)random.Next('A', 'Z')}{random.Next(1, 9)}{random.Next(0, 9)}";
                PutItem(911, 1, itemQuality, "{0} " + postfix, itemPrice, true);

                var hue = random.Next(0, 359); // TODO: Not random color.
                ((ColoredObject)heldObject).color = RawColor.FromHSL(hue, 0.75, 0.55).ToXnaColor();
                minutesUntilReady = GetMinutesUntilReady(SecondDroppedItem); // TODO: From first?
            }
            
            PlaySound(Sound.Ship);
            return true;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y, alpha);

            var v1 = getScale() * Game1.pixelZoom;
            var v2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize));
            var destinationX = v2.X - v1.X / 2 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0);
            var destinationY = v2.Y - v1.Y / 2 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0);
            var width = Game1.tileSize + v1.X;
            var height = Game1.tileSize * 2 + v1.Y / 2;
            var destinationRectangle = new Rectangle((int)destinationX, (int)destinationY, (int)width, (int)height);

            var color1 = (FirstDroppedItem is ColoredObject)? ((ColoredObject)FirstDroppedItem).color : Color.White;
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, getSourceRectForBigCraftable(ParentSheetIndex + 1), color1 * alpha, 0, Vector2.Zero, SpriteEffects.None, Math.Max(0, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f + 0.001f));
            var color2 = (SecondDroppedItem is ColoredObject)? ((ColoredObject)SecondDroppedItem).color : Color.White;
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, getSourceRectForBigCraftable(ParentSheetIndex + 2), color2 * alpha, 0, Vector2.Zero, SpriteEffects.None, Math.Max(0, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f + 0.002f));           
        }
    }
}