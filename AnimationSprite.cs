using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontBreakTheRubber
{
    public class AnimationSprite : SpriteClass
    {
        float elapsedTime;
        float delayTimeAfterStart = 0;
        bool isPassDelay;
        float frameTime = (float)0.02;
        int frameCount;
        

        private bool _active = false;
        public bool active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                if (active)
                {
                    isPassDelay = false;
                }
            }
        }
        int currentFrame;

        List<Texture2D> AnimationFrames;


        public AnimationSprite(ContentManager contentManager, string contentFolder, float _scale)
        {

            //Load directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            //Init the resulting list
            AnimationFrames = new List<Texture2D>();
            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                System.Diagnostics.Debug.WriteLine(contentFolder + "/" + key);
                AnimationFrames.Add(
                    contentManager.Load<Texture2D>(contentFolder + "/" + key));
            }
            frameCount = AnimationFrames.Count();
            textureHeight = AnimationFrames[0].Height;
            textureWidth = AnimationFrames[0].Width;
            scale = _scale;
        }

        public AnimationSprite(GraphicsDevice graphicsDevice, String dir, float scale)
        {
            string[] files = System.IO.Directory.GetFiles(dir);
            frameCount = files.Count();
            AnimationFrames = new List<Texture2D>();
            for (int i = 0; i < frameCount; i++)
            {
                System.Diagnostics.Debug.WriteLine(files[i]);
                System.Diagnostics.Debug.WriteLine(files[i].Remove(0, "Content/".Length));
                var stream = TitleContainer.OpenStream(files[i].Remove(0, "Content/".Length));
                AnimationFrames.Add(Texture2D.FromStream(graphicsDevice, stream));
            }
            textureHeight = AnimationFrames[0].Height;
            textureWidth = AnimationFrames[0].Width;
            this.texture = AnimationFrames[0];

        }

        public void Update(float _elapsedTime)
        {
            System.Diagnostics.Debug.WriteLine("animation update");
            this.x += this.dX * _elapsedTime;
            this.y += this.dY * _elapsedTime;
            this.angle += this.dA * _elapsedTime;
            elapsedTime += _elapsedTime;
            if (active)
            {
                if (elapsedTime > frameTime)
                {
                    currentFrame++;
                    if (currentFrame == frameCount)
                    {
                        currentFrame = 0;
                        active = false;
                    }
                    elapsedTime = 0;
                }
            }
            else currentFrame = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Determine the position vector of the sprite
            Vector2 spritePosition = new Vector2(this.x, this.y);

            // Draw the sprite
            spriteBatch.Draw(AnimationFrames[currentFrame],
                spritePosition,
                null,
                Color.White,
                this.angle,
                new Vector2(AnimationFrames[currentFrame].Width / 2, AnimationFrames[currentFrame].Height / 2),
                new Vector2(scale, scale), SpriteEffects.None, 0f);

        }


    }
}
