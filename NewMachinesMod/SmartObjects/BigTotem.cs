using System;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public class BigTotem : SmartObjectBase
    {
        protected static int TileSize => Game1.tileSize;

        protected Rectangle SourceRect { get; }

        public BigTotem() : base(ClassMapperService.Instance.GetItemID<BigTotem>())
        {
            SourceRect = GetSourceRect();
        }

        protected override int TileHeight { get; } = 2;
        protected override int TileWidth { get; } = 2;

        protected override bool CanDoAction(Farmer farmer)
        {
            return farmer.IsMainPlayer;
        }

        protected override bool DoAction(Farmer farmer)
        {
            Game1.player.jitterStrength = 1;
            Game1.playSound("warrior");
            Game1.player.faceDirection(2);
            Game1.player.CanMove = false;
            Game1.player.temporarilyInvincible = true;
            Game1.player.temporaryInvincibilityTimer = -4000;
            Game1.changeMusicTrack("none");
            Game1.player.FarmerSprite.animateOnce(new[]
            {
                new FarmerSprite.AnimationFrame(57, 2000, false, false),
                new FarmerSprite.AnimationFrame(Game1.player.FarmerSprite.currentFrame, 0, false, false, TotemWarpBegin, true)
            });

            var playerPosition = Game1.player.position;
            AddAnimatedSprite(new Vector2(0, -1), 0.01f, 1, 0, 1, playerPosition + new Vector2(0, -TileSize * 3 / 2));
            AddAnimatedSprite(new Vector2(0, -0.5f), 0.005f, 0.5f, 10, 0.9999f, playerPosition + new Vector2(-TileSize, -TileSize * 3 / 2));
            AddAnimatedSprite(new Vector2(0, -0.5f), 0.005f, 0.5f, 20, 0.9988f, playerPosition + new Vector2(TileSize, -TileSize * 3 / 2));
            Game1.screenGlowOnce(Color.LightBlue, false);
            Utility.addSprinklesToLocation(Game1.currentLocation, Game1.player.getTileX(), Game1.player.getTileY(), 16, 16, 1300, 20, Color.White, null, true);       
            return true;
        }

        private void AddAnimatedSprite(Vector2 motion, float scaleChange, float scale, int delay, float depth, Vector2 position)
        {
            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(parentSheetIndex, 9999, 1, 999, position, false, false, false, 0)
            {
                motion = motion,
                scaleChange = scaleChange,
                scale = scale / 2,
                alpha = 1,
                alphaFade = 0.0075f,
                shakeIntensity = 1,
                delayBeforeAnimationStart = delay,
                initialPosition = position,
                xPeriodic = true,
                xPeriodicLoopTime = 1000,
                xPeriodicRange = TileSize / 16,
                layerDepth = depth,
                sourceRect = SourceRect,
                Texture = Game1.objectSpriteSheet,
            });
        }

        private void TotemWarpBegin(Farmer farmer)
        {
            var farmerX = (int)farmer.position.X;
            var farmerY = (int)farmer.position.Y;
            for (var index = 0; index < 12; ++index)
            {
                farmer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next(farmerX - TileSize * 4, farmerX + TileSize * 3), Game1.random.Next(farmerY - TileSize * 4, farmerY + TileSize * 3)), false, Game1.random.NextDouble() < 0.5));
            }
            PlaySound(Sound.Wand);
            Game1.displayFarmer = false;
            Game1.player.freezePause = 1000;
            Game1.flashAlpha = 1;
            DelayedAction.fadeAfterDelay(TotemWarpEnd, 1000);
            var num = 0;
            for (var index = farmer.getTileX() + 8; index >= farmer.getTileX() - 8; --index)
            {
                farmer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(index, farmer.getTileY()) * TileSize, Color.White, 8, false, 50)
                {
                    layerDepth = 1,
                    delayBeforeAnimationStart = num * 25,
                    motion = new Vector2(-0.25f, 0),
                });
                ++num;
            }
        }

        private void TotemWarpEnd()
        {
            Game1.warpFarmer("Beach", 20, 4, false);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }

        private Rectangle GetSourceRect()
        {
            var source = TexturesService.GetSourceRectForObject(ParentSheetIndex, Game1.objectSpriteSheet, 16, 16);
            source.Width *= TileWidth;
            source.Height *= TileHeight;
            return source;
        }

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
            spriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(x / TileSize * TileSize - Game1.viewport.X, y / TileSize * TileSize - Game1.viewport.Y),
                new Rectangle(Utility.playerCanPlaceItemHere(location, this, x, y, Game1.player) ? 194 : 210, 388, 16, 16),
                Color.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                0.01f);
            draw(spriteBatch, x / TileSize, y / TileSize, 0.5f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            spriteBatch.Draw(
                Game1.objectSpriteSheet,
                location + new Vector2(TileSize / 2, TileSize / 2),
                SourceRect,
                Color.White * transparency,
                0,
                new Vector2(8 * TileWidth, 8 * TileHeight),
                Game1.pixelZoom * (scaleSize < 0.2 ? scaleSize : scaleSize / 2),
                SpriteEffects.None,
                layerDepth);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            spriteBatch.Draw(
                Game1.objectSpriteSheet,
                objectPosition,
                SourceRect,
                Color.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                Math.Max(0.0f, (f.getStandingY() + 2) / 10000f));
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var dest = Game1.GlobalToLocal(Game1.viewport,
                new Vector2(TileSize * x + TileSize * TileWidth / 2, TileSize * y + TileSize * TileHeight / 2));

            spriteBatch.Draw(
                Game1.objectSpriteSheet,
                dest,
                SourceRect,
                Color.White * alpha,
                0,
                new Vector2(8 * TileWidth, 8 * TileHeight),
                Game1.pixelZoom,
                SpriteEffects.None,
                getBoundingBox(new Vector2(x, y)).Bottom / 10000f);
        }
    }
}
