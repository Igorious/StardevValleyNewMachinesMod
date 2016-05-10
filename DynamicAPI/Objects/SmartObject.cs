using System;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public class SmartObject : Object, ISmartObject
    {
        protected static Rectangle Rectangle(float x, float y, float width, float height)
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        protected bool UsedPreviewIcon => (SpriteHeight > 2) || (SpriteWidth > 1);

        protected static int TileSize => Game1.tileSize;
        public XColor? Color { get; set; }
        public int ID => ParentSheetIndex;

        public SmartObject() { }

        public SmartObject(int id) : base(Vector2.Zero, id) { }

        public SmartObject(int id, int count) : base(Vector2.Zero, id, count) { }

        #region Bounding

        protected virtual int BoundingTileWidth { get; } = 1;
        protected virtual int BoundingTileHeight { get; } = 1;

        public sealed override Rectangle getBoundingBox(Vector2 tile)
        {
            boundingBox.X = (int)tile.X * Game1.tileSize;
            boundingBox.Y = (int)tile.Y * Game1.tileSize;
            boundingBox.Height = BoundingTileHeight * Game1.tileSize;
            boundingBox.Width = BoundingTileWidth * Game1.tileSize;
            return boundingBox;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            for (var w = 0; w < BoundingTileWidth; ++w)
                for (var h = 0; h < BoundingTileHeight; ++h)
                {
                    if (!base.canBePlacedHere(l, new Vector2(tile.X + w, tile.Y + h))) return false;
                }
            return true;
        }

        #endregion

        #region Draw

        protected virtual TextureType TextureType => TextureType.Items;
        protected Texture2D Texture => TextureInfo.Default[TextureType].Texture;
        protected virtual int SpriteWidth { get; } = 1;
        protected virtual int SpriteHeight { get; } = 1;
        protected virtual int VerticalShift { get; } = 0;

        protected virtual Rectangle SourceRect => GetSourceRect(ParentSheetIndex);

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            if (!isPlaceable()) return;

            var x = Game1.getOldMouseX() + Game1.viewport.X;
            var y = Game1.getOldMouseY() + Game1.viewport.Y;
            var grabbedTile = Game1.player.GetGrabTile();

            if (Game1.mouseCursorTransparency == 0)
            {
                x = (int)grabbedTile.X * TileSize;
                y = (int)grabbedTile.Y * TileSize;
            }
            if (grabbedTile.Equals(Game1.player.getTileLocation()) && Game1.mouseCursorTransparency == 0)
            {
                var translatedVector2 = Utility.getTranslatedVector2(grabbedTile, Game1.player.facingDirection, 1);
                x = (int)translatedVector2.X * TileSize;
                y = (int)translatedVector2.Y * TileSize;
            }

            DrawPlacementMarker(spriteBatch, location, x + 0 * TileSize, y + 0 * TileSize);
            draw(spriteBatch, x / TileSize, y / TileSize, 0.5f);
        }

        private void DrawPlacementMarker(SpriteBatch spriteBatch, GameLocation location, int x, int y)
        {
            var markerSourceRect = Rectangle(Utility.playerCanPlaceItemHere(location, this, x, y, Game1.player) ? 194 : 210, 388, 16, 16);
            spriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(x / TileSize * TileSize - Game1.viewport.X, y / TileSize * TileSize - Game1.viewport.Y),
                markerSourceRect,
                XColor.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                0.01f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            if (isRecipe)
            {
                transparency = 0.5f;
                scaleSize *= 0.75f;
            }

            DrawMenuItem(spriteBatch, location, transparency, scaleSize, layerDepth);
            if (Color != null)
            {
                DrawMenuItem(spriteBatch, location, transparency, scaleSize, layerDepth, Color);
            }

            if (!bigCraftable)
            {
                DrawShadow(spriteBatch, location, scaleSize, layerDepth);
                DrawStackNumber(location, scaleSize, drawStackNumber);
                DrawQualityStar(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
                DrawTackleBar(spriteBatch, location, scaleSize);
            }

            DrawRecipe(spriteBatch, location, layerDepth);
        }

        private void DrawMenuItem(SpriteBatch spriteBatch, Vector2 location, float transparency, float scaleSize, float layerDepth, Color? color = null)
        {
            var sourceRect = GetSourceRect(ParentSheetIndex + (color.HasValue ? 1 : 0) + (UsedPreviewIcon ? -1 : 0), UsedPreviewIcon ? 1 : (int?)null, UsedPreviewIcon ? 1 : (int?)null);
            var scaleX = scaleSize * 16 / sourceRect.Height;
            var scaleY = scaleSize * 16 / sourceRect.Width;

            spriteBatch.Draw(
                Texture,
                Rectangle(
                    location.X + TileSize * (1 - scaleX) / 2,
                    location.Y + TileSize * (1 - scaleY) / 2,
                    TileSize * scaleX,
                    TileSize * scaleY),
                sourceRect,
                (color ?? XColor.White) * transparency,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth + (color.HasValue ? 0.00002f : 0));
        }

        protected Rectangle GetSourceRect(int index, int? length = null, int? height = null)
        {
            return TextureInfo.Default[TextureType].GetSourceRect(index + (UsedPreviewIcon ? 1 : 0) + (showNextIndex ? 1 : 0), length ?? SpriteWidth, height ?? SpriteHeight);
        }

        private void DrawRecipe(SpriteBatch spriteBatch, Vector2 location, float layerDepth)
        {
            if (!isRecipe) return;

            spriteBatch.Draw(
                Game1.objectSpriteSheet,
                location + new Vector2(TileSize / 4f, TileSize / 4f),
                Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16),
                XColor.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom * 3f / 4,
                SpriteEffects.None,
                layerDepth + 0.0001f);
        }

        private void DrawTackleBar(SpriteBatch spriteBatch, Vector2 location, float scaleSize)
        {
            if (category != tackleCategory || scale.Y == 1) return;

            spriteBatch.Draw(
                Game1.staminaRect,
                Rectangle(
                    location.X,
                    location.Y + (TileSize - 2 * Game1.pixelZoom) * scaleSize,
                    TileSize * scaleSize * scale.Y,
                    2 * Game1.pixelZoom * scaleSize),
                Utility.getRedToGreenLerpColor(scale.Y));
        }

        private void DrawQualityStar(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            if (!drawStackNumber || quality == 0) return;

            var blinkScale = (float)(Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512) + 1) * 0.05f;
            spriteBatch.Draw(
                Game1.mouseCursors,
                location + new Vector2(12, Game1.tileSize - 12 + blinkScale),
                Rectangle(330 + quality * 8, 400, 8, 8),
                XColor.White * transparency,
                0,
                new Vector2(4, 4),
                3 * scaleSize * (1 + blinkScale),
                SpriteEffects.None,
                layerDepth + 0.0003f);
        }

        private void DrawStackNumber(Vector2 location, float scaleSize, bool drawStackNumber)
        {
            if (!drawStackNumber || maximumStackSize() <= 1 || scaleSize <= 0.3 || Stack == int.MaxValue || Stack <= 1) return;

            var fontScale = 0.5f + scaleSize;
            var message = stack.ToString();
            var measure = Game1.tinyFont.MeasureString(message);
            Game1.drawWithBorder(
                message,
                XColor.Black,
                XColor.White,
                location + new Vector2(TileSize - measure.X * fontScale, TileSize - measure.Y * 3 / 4 * fontScale),
                0,
                fontScale,
                1,
                true);
        }

        protected void DrawShadow(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float layerDepth)
        {
            spriteBatch.Draw(
                Game1.shadowTexture,
                location + new Vector2(Game1.tileSize / 2f, Game1.tileSize * 3 / 4f),
                Game1.shadowTexture.Bounds,
                XColor.White * 0.5f,
                0,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                3,
                SpriteEffects.None,
                layerDepth - 0.00001f);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer farmer)
        {
            DrawHeld(spriteBatch, objectPosition, farmer);
            if (Color != null)
            {
                DrawHeld(spriteBatch, objectPosition, farmer, Color);
            }
        }

        private void DrawHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer farmer, XColor? color = null)
        {
            var sourceRect = GetSourceRect(ParentSheetIndex + (color.HasValue ? SpriteWidth : 0));
            spriteBatch.Draw(
                Texture,
                objectPosition - new Vector2((sourceRect.Width - 16) / 32f * TileSize, (sourceRect.Height / 16f - 1 + VerticalShift) * TileSize),
                sourceRect,
                color ?? XColor.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                Math.Max(0, (farmer.getStandingY() + 2 + (color.HasValue ? 1 : 0)) / 10000f));
        }

        protected Vector2 GetScale(bool change = true)
        {
            if (category == -22) return Vector2.Zero;
            if (!bigCraftable)
            {
                scale.Y = Math.Max(Game1.pixelZoom, scale.Y - Game1.pixelZoom / 100f);
                return scale;
            }
            if (heldObject == null && minutesUntilReady <= 0 || readyForHarvest) return Vector2.Zero;
            if (ID == (int)CraftableID.BeeHouse || name.Contains("Table") || ID == (int)CraftableID.Tapper) return Vector2.Zero;
            if (ID == (int)CraftableID.Loom)
            {
                scale.X = (float)((scale.X + Game1.pixelZoom / 100.0) % (2.0 * Math.PI));
                return Vector2.Zero;
            }
            if (change)
            {
                scale.X -= 0.1f;
                scale.Y += 0.1f;
                if (scale.X <= 0) scale.X = 10;
                if (scale.Y >= 10) scale.Y = 0;
            }
            return new Vector2(Math.Abs(scale.X - 5), Math.Abs(scale.Y - 5));
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            DrawObject(spriteBatch, x, y, alpha);
            if (Color != null)
            {
                DrawObject(spriteBatch, x, y, alpha, Color);
            }
            DrawHeldObject(spriteBatch, x, y);
        }

        protected void DrawObject(SpriteBatch spriteBatch, int x, int y, float alpha, XColor? color = null, int sheetIndexDelta = 0)
        {
            var currentScale = GetScale(color == null) * Game1.pixelZoom;
            var destVector = Game1.GlobalToLocal(Game1.viewport, new Vector2(TileSize * x, TileSize * (y + VerticalShift)));
            if (shakeTimer > 0)
            {
                destVector.X += Game1.random.Next(-1, 2);
                destVector.Y += Game1.random.Next(-1, 2);
            }

            var destRect = Rectangle(
                destVector.X - currentScale.X / 4,
                destVector.Y - currentScale.Y / 4,
                TileSize * SourceRect.Width / 16f + currentScale.X / 2,
                TileSize * SourceRect.Height / 16f + currentScale.Y / 2);

            var depth = getBoundingBox(new Vector2(x, y)).Bottom / 10000f;

            spriteBatch.Draw(
                Texture,
                destRect,
                GetSourceRect(ParentSheetIndex + sheetIndexDelta * SpriteWidth),
                (color ?? XColor.White) * alpha,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                depth + (color.HasValue ? 0.0001f : 0));
        }

        protected void DrawHeldObject(SpriteBatch spriteBatch, int x, int y)
        {
            if (!readyForHarvest || heldObject == null) return;

            var deltaY = (float)(4 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250), 2));
            var depth = (y + 1) * Game1.tileSize / 10000f + tileLocation.X / 10000f;

            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(x * TileSize - 8, y * TileSize - TileSize * 3 / 2 - 16 + deltaY)),
                Rectangle(141, 465, 20, 24),
                XColor.White * 0.75f,
                0,
                Vector2.Zero,
                4,
                SpriteEffects.None,
                depth - 0.0002f);

            var baloonCenterAbsolute = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * TileSize + TileSize / 2, y * TileSize - TileSize - TileSize / 8 + deltaY));
            var itemCenterRelative = new Vector2(8, 8);

            var itemsTextureInfo = TextureInfo.Default[TextureType.Items];
            spriteBatch.Draw(
                itemsTextureInfo.Texture,
                baloonCenterAbsolute,
                itemsTextureInfo.GetSourceRect(heldObject.parentSheetIndex),
                XColor.White * 0.75f,
                0,
                itemCenterRelative,
                Game1.pixelZoom,
                SpriteEffects.None,
                depth - 0.0001f);

            var color = heldObject.GetColor();
            if (color != null)
            {
                spriteBatch.Draw(
                    itemsTextureInfo.Texture,
                    baloonCenterAbsolute,
                    itemsTextureInfo.GetSourceRect(heldObject.parentSheetIndex + 1),
                    color.Value,
                    0,
                    itemCenterRelative,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    depth);
            }
        }

        protected void PlayAnimation(Farmer farmer, Animation animation)
        {
            var animationSprites = farmer.currentLocation.temporarySprites;

            switch (animation)
            {
                case Animation.Steam:
                    animationSprites.Add(new TemporaryAnimatedSprite(
                        27, 
                        tileLocation * Game1.tileSize + new Vector2(-TileSize / 4f, -TileSize * 2), 
                        XColor.White, 
                        4, 
                        false, 
                        50, 
                        10, 
                        TileSize, 
                        (tileLocation.Y + 1) * TileSize / 10000 + 0.0001f)
                        {
                            alphaFade = 0.005f,
                        });
                    break;
            }
        }

        #endregion

        #region Action

        private static readonly Object ProbeObject = new Object();

        public sealed override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            return justCheckingForActivity ? CanDoAction(who) : DoAction(who);
        }

        protected virtual bool CanDoAction(Farmer farmer) => base.checkForAction(farmer, true);

        protected virtual bool DoAction(Farmer farmer) => base.checkForAction(farmer);

        public sealed override bool performObjectDropInAction(Object item, bool isProbe, Farmer farmer)
        {
            if (!CanPerformDropIn(item, farmer)) return false;

            if (isProbe)
            {
                heldObject = ProbeObject;
                return true;
            }
            else
            {
                return PerformDropIn(item, farmer);
            }
        }

        protected void PutItem(int itemID, int count, int itemQuality = 0, string overridedName = null, int? overridedPrice = null, XColor? color = null)
        {
            heldObject = new SmartObject(itemID, count)
            {
                quality = itemQuality,
                Color = color,
            };
            if (overridedName != null) heldObject.Name = string.Format(overridedName, heldObject.Name);
            if (overridedPrice != null) heldObject.Price = overridedPrice.Value;
        }

        protected virtual bool CanPerformDropIn(Object item, Farmer farmer) => CanPerformDropInRaw(item, farmer);

        protected virtual bool PerformDropIn(Object item, Farmer farmer) => PerformDropInRaw(item, farmer);

        protected bool PerformDropInRaw(Object item, Farmer farmer) => base.performObjectDropInAction(item, false, farmer);

        protected bool CanPerformDropInRaw(Object item, Farmer farmer)
        {
            base.performObjectDropInAction(item, true, farmer);
            var result = (heldObject != null);
            heldObject = null;
            return result;
        }

        #endregion

        #region Tool Action

        public sealed override bool performToolAction(Tool tool)
        {
            if (tool is Pickaxe) return OnPickaxeAction((Pickaxe)tool);
            if (tool is Axe) return OnAxeAction((Axe)tool);
            if (tool is WateringCan)
            {
                OnWateringCanAction((WateringCan)tool);
                return false;
            }
            return base.performToolAction(tool);
        }

        protected virtual bool OnPickaxeAction(Pickaxe pickaxe)
        {
            return base.performToolAction(pickaxe);
        }

        protected virtual bool OnAxeAction(Axe axe)
        {
            return base.performToolAction(axe);
        }

        protected virtual void OnWateringCanAction(WateringCan wateringCan)
        {
            base.performToolAction(wateringCan);
        }

        #endregion

        public override Item getOne()
        {
            var clone = (SmartObject)MemberwiseClone();
            clone.Stack = 1;
            return clone;
        }

        protected Random GetRandom()
        {
            return new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay + (int)tileLocation.X * 200 + (int)tileLocation.Y);
        }

        protected void PlaySound(Sound sound) => Game1.playSound(sound.GetDescription());

        protected void ShowRedMessage(Farmer farmer, string message)
        {
            if (!farmer.IsMainPlayer) return;
            Game1.showRedMessage(message);
        }
    }
}
