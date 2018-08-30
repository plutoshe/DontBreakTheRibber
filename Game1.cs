using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.Graphics.Display;
using System;
using Windows.UI.ViewManagement;
using DontBreakTheRubber.Core;

namespace DontBreakTheRubber
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int score = 0;

        SpriteClass spikeBall;
        SpriteClass balloon;
        AnimationSprite tramp;
        Texture2D startGameSplash;
        Texture2D gameOverTexture;

        SpriteFont scoreFont;
        SpriteFont stateFont;

        bool spaceDown;
        bool gameStarted;
        bool instructionsViewed;
        bool controlsViewed;
        bool titleViewed;
        bool gameOver;
        bool upToDown;

        public static float screenWidth;
        public static float screenHeight;
        public static float groundHeight;

        float ballBounceSpeed;
        float gravitySpeed;
        float spinSpeed;
        float previousSpeed;

        Texture2D backgroundTexture;
        float backgroundScaleRatio;
        Camera _camera;
        //private object console;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            spaceDown = false;
            gameStarted = false;
            instructionsViewed = false;
            controlsViewed = false;
            titleViewed = false;
            gameOver = false;
            upToDown = false;
            spinSpeed = 7f;
            previousSpeed = spikeBall.dY;

            backgroundScaleRatio = screenWidth / backgroundTexture.Width;

            ballBounceSpeed = ScaleToHighDPI(-1200f);
            gravitySpeed = ScaleToHighDPI(30f);
            score = 0;
            this.IsMouseVisible = false;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startGameSplash = Content.Load<Texture2D>("start-splash");
            gameOverTexture = Content.Load<Texture2D>("game-over");
            backgroundTexture = Content.Load<Texture2D>("Background_compressed");

            spikeBall = new SpriteClass(GraphicsDevice, "Content/character_test.png", ScaleToHighDPI(1f));
            balloon = new SpriteClass(GraphicsDevice, "Content/character_sprite.png", ScaleToHighDPI(1f));
            tramp = new AnimationSprite(Content, "tramp_animation", ScaleToHighDPI(1f));
            
            // font
            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");
            _camera = new Camera();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time elapsed since last Update iteration


            KeyboardHandler();

            if (gameOver)
            {
                spikeBall.dX = 0;
                spikeBall.dY = 0;
                spikeBall.angle = DegreeToRadian(0);
                spikeBall.dA = 0;
                balloon.dX = 0;
                balloon.dY = 0;
            }

            
            
            spikeBall.Update(elapsedTime);
            balloon.Update(elapsedTime);
            tramp.Update(elapsedTime);

            //TODO Fix Score
            previousSpeed = spikeBall.dY;
            spikeBall.dY += gravitySpeed;

            if (previousSpeed < 0 && spikeBall.dY >= 0)
            {
                upToDown = true;
            }

            if ((int)((backgroundTexture.Height * backgroundScaleRatio) - (backgroundTexture.Height - spikeBall.texture.Height) * backgroundScaleRatio) > score)
            {
                score = (int)((backgroundTexture.Height * backgroundScaleRatio) - (backgroundTexture.Height - spikeBall.texture.Height) * backgroundScaleRatio);
            }

            if (spikeBall.y > groundHeight)
            {
                spikeBall.dY = 0;
                spikeBall.y = groundHeight;
            }
            
            if (spikeBall.RectangleCollision(balloon) && spikeBall.dY >= 0)
            {
              //  System.Diagnostics.Debug.WriteLine(spikeBall.angle);
                float tempAngle = ((float)RadianToDegree(spikeBall.angle) % 360);
                if((tempAngle > 240 || tempAngle <= 120) && !gameOver && gameStarted)
                {
                    bounce(tempAngle);
                    tramp.active = true;
                }
                else
                {
                    gameOver = true;
                }
            }
            _camera.Follow(spikeBall);
            base.Update(gameTime);
        }


        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { color });
            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    10), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); // Clear the screen
            
            if (!titleViewed)
            {
                spriteBatch.Begin();
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);

                String title = "PARTY HOPPER";
                String pressSpace = "Press Space";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3), Color.CornflowerBlue);
                spriteBatch.DrawString(stateFont, pressSpace, new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2), Color.White);
                spriteBatch.End();
                return;
            }
            if (!controlsViewed)
            {
                spriteBatch.Begin();
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);

                String title = "Pressing Space Bar will stop you from spinning";
                String pressSpace = "Press Space";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3), Color.CornflowerBlue);
                spriteBatch.DrawString(stateFont, pressSpace, new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2), Color.White);
                spriteBatch.End();
                return;
            }
            if (!instructionsViewed)
            {
                spriteBatch.Begin();
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);

                String title = "Don't ruin your party hat!";
                String pressSpace = "Press Space to start";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3), Color.CornflowerBlue);
                spriteBatch.DrawString(stateFont, pressSpace, new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2), Color.White);
                spriteBatch.End();
                return;
            }
            spriteBatch.Begin(transformMatrix: _camera.Transform);

            
            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), backgroundScaleRatio, SpriteEffects.None, 1);
            tramp.Draw(spriteBatch);
            spikeBall.Draw(spriteBatch);

            if (gameOver)
            {
                 
                
                // Draw game over texture over camera
                spriteBatch.Draw(gameOverTexture, new Vector2(screenWidth / 2 - gameOverTexture.Width / 2, spikeBall.y - 400), Color.White);

                String pressEnter = "Press Enter to restart!";

                // Measure the size of text in the given font
                Vector2 pressEnterSize = stateFont.MeasureString(pressEnter);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, pressEnter, new Vector2(screenWidth / 2 - pressEnterSize.X / 2, spikeBall.y - 100), Color.White);

                // If the game is over, draw the score in red
                spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, spikeBall.y - 300), Color.Red);
            }
            else
            {
                spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, spikeBall.y - 300), Color.Black);
            }

            
            //balloon.Draw(spriteBatch);

            

            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public float ScaleToHighDPI(float f)
        {
            DisplayInformation d = DisplayInformation.GetForCurrentView();
            f *= (float)d.RawPixelsPerViewPixel;
            return f;
        }


        public void StartGame()
        {
            const int originOfTramp = 400;
            groundHeight = (backgroundTexture.Height - originOfTramp) * backgroundScaleRatio;
            
            spikeBall.x = screenWidth / 2;
            spikeBall.y = groundHeight - balloon.texture.Height * balloon.scale / 2;
            spikeBall.angle = DegreeToRadian(0);
            spikeBall.dA = 0;

            tramp.x = screenWidth / 2;
            tramp.y = groundHeight - balloon.texture.Height * balloon.scale / 2 - 120;
            tramp.active = true;
            tramp.scale = screenWidth / tramp.frameWidth;


            balloon.x = screenWidth / 2;
            balloon.y = groundHeight;
            score = 0;
            spinSpeed = 7f;
            ballBounceSpeed = ScaleToHighDPI(-1200f);
        }



        void bounce(float angle)
        {
            if(upToDown)
            {
                if ((angle > 340 && angle <= 360) || (angle >= 0 && angle < 20))
                {
                    //score += 4;
                    ballBounceSpeed += -250;
                }
                else if((angle > 305 && angle <= 340) || (angle >= 20 && angle < 55))
                {
                    //score += 3;
                    ballBounceSpeed += -100;
                    spinSpeed *= (float)1.05;
                }
                else if ((angle > 270 && angle <= 305) || (angle >= 55 && angle < 90))
                {
                    //score += 2;
                    ballBounceSpeed += -50;
                    spinSpeed *= (float)1.1;
                }
                else
                {
                    //score++;
                    ballBounceSpeed += 50;
                    spinSpeed *= (float)1.2;
                }
                upToDown = false;
            }

            spikeBall.dY = ballBounceSpeed;
            spikeBall.dA = spinSpeed;
        }

        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)) Exit();

            // Start the game if Space is pressed.
            if (!titleViewed)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    titleViewed = true;
                    spaceDown = true;
                }
                return;
            }
            if (!controlsViewed && !spaceDown)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    controlsViewed = true;
                    spaceDown = true;
                }
                return;
            }
            if (!instructionsViewed && !spaceDown)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    instructionsViewed = true;
                    StartGame();
                    gameStarted = true;
                    spaceDown = true;
                    gameOver = false;
                    upToDown = false;
                }
                return;
            }




            // Restart the game if Enter is pressed
            if (gameOver)
            {
                if (state.IsKeyDown(Keys.Enter))
                {
                    StartGame();
                    gameOver = false;
                }
            }

            // stop spinning if space is pressed
            if (state.IsKeyDown(Keys.Space))
            {
                // stop spinning once space is released
                if (!spaceDown)
                {
                    spikeBall.dA = 0;
                    spikeBall.angle = spikeBall.angle;
                }

                spaceDown = true;
            }
            else spaceDown = false;
        }

        double RadianToDegree(float angle)
        {
            return angle * (180.0 / Math.PI);
        }

        float DegreeToRadian(double radian)
        {
            return (float)(radian * Math.PI / 180.0);
        }

    }
}
