using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontBreakTheRubber
{
    public class SpriteClass
    {
        const float HITBOXSCALE = .5f; // experiment with this value to make the collision detection more or less forgiving

        public int textureHeight, textureWidth;

        // sprite texture
        public Texture2D texture
        {
            get;
            set;
        }

        // x coordinate of the center of the sprite
        public float x;

        // y coordinate of the center of the sprite
        public float y;
        // Angle of the sprite around central axis
        public float angle;

        // Rate of change of x per second
        public float dX;

        // Rate of change of y per second
        public float dY;

        // Rate of change of angle per second
        public float dA;
        public float scale;


        // Constructor
        public SpriteClass() { }

        public SpriteClass(GraphicsDevice graphicsDevice, string textureName, float scale)
        {
            this.scale = scale;

            // Load the specified texture
            var stream = TitleContainer.OpenStream(textureName);
            texture = Texture2D.FromStream(graphicsDevice, stream);
            textureHeight = texture.Height;
            textureWidth = texture.Width;
        }

        // Update the position and angle of the sprite based on each rate of change and the time elapsed
        public void Update(float elapsedTime)
        {
            System.Diagnostics.Debug.WriteLine("spriteClass update");
            this.x += this.dX * elapsedTime;
            this.y += this.dY * elapsedTime;
            this.angle += this.dA * elapsedTime;
        }

        // Draw the sprite with the given SpriteBatch
        public void Draw(SpriteBatch spriteBatch)
        {
            // Determine the position vector of the sprite
            Vector2 spritePosition = new Vector2(this.x, this.y);

            // Draw the sprite
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.angle, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }

        // Detect collision between two rectangular sprites
        public bool RectangleCollision(SpriteClass otherSprite)
        {
            if (this.x + this.textureWidth * this.scale * HITBOXSCALE / 2 < otherSprite.x - otherSprite.textureWidth * otherSprite.scale / 2) return false;
            if (this.y + this.textureHeight * this.scale * HITBOXSCALE / 2 < otherSprite.y - otherSprite.textureHeight * otherSprite.scale / 2) return false;
            if (this.x - this.textureWidth * this.scale * HITBOXSCALE / 2 > otherSprite.x + otherSprite.textureWidth * otherSprite.scale / 2) return false;
            if (this.y - this.textureHeight * this.scale * HITBOXSCALE / 2 > otherSprite.y + otherSprite.textureHeight * otherSprite.scale / 2) return false;
            return true;
        }

    }

   
}
