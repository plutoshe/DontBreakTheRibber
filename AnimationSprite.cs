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
    public class Animation
    {
        float elapsedTime;
        float delayTimeAfterStart = 0;
        bool isPassDelay;
        float frameTime = (float)0.02;
        int currentFrame;
        int frameCount;
        public int frameHeight;
        public int frameWidth;


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


        List<Texture2D> AnimationFrames;

        public Animation(ContentManager contentManager, string contentFolder)
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
            frameHeight = AnimationFrames[0].Height;
            frameWidth = AnimationFrames[0].Width;
        }
        public Animation(GraphicsDevice graphicsDevice, String dir)
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
            frameHeight = AnimationFrames[0].Height;
            frameWidth = AnimationFrames[0].Width;
        }

        public void Start()
        {
            currentFrame = 0;
            active = true;
        }

        public void Draw(SpriteBatch spriteBatch, float x, float y, float angle, float scale)
        {
            // Determine the position vector of the sprite
            Vector2 spritePosition = new Vector2(x, y);

            // Draw the sprite
            spriteBatch.Draw(AnimationFrames[currentFrame],
                spritePosition,
                null,
                Color.White,
                angle,
                new Vector2(AnimationFrames[currentFrame].Width / 2, AnimationFrames[currentFrame].Height / 2),
                new Vector2(scale, scale), SpriteEffects.None, 0f);

        }

        public void Update(float _elapsedTime)
        {
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
    }

    public class AnimationSprite: SpriteClass
    {
        public Animation animation;
        
        public AnimationSprite(Animation _animation, float _scale) {
            this.animation = _animation;
            textureHeight = _animation.frameHeight;
            textureWidth = _animation.frameWidth;
            scale = _scale;
        }


        public void Update(float _elapsedTime)
        {
            System.Diagnostics.Debug.WriteLine("animation update");
            this.x += this.dX * _elapsedTime;
            this.y += this.dY * _elapsedTime;
            this.angle += this.dA * _elapsedTime;
            animation.Update(_elapsedTime);
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, this.x, this.y, this.angle, this.scale);
        }

    }
}
