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

        const float SKYRATIO = 2f / 3f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int score = -1;

        SpriteClass spikeBall;
        SpriteClass balloon;
        Texture2D startGameSplash;
        Texture2D gameOverTexture;

        SpriteFont scoreFont;
        SpriteFont stateFont;

        bool spaceDown;
        bool gameStarted;
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
            // TODO: Add your initialization logic here

            base.Initialize();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            /*graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;*/
            //System.Diagnostics.Debug.WriteLine("aaa" + screenWidth.ToString());
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            spaceDown = false;
            gameStarted = false;
            gameOver = false;
            upToDown = false;
            spinSpeed = 7f;
            previousSpeed = spikeBall.dY;

            ballBounceSpeed = ScaleToHighDPI(-1200f);
            gravitySpeed = ScaleToHighDPI(30f);
            score = -1;
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

            spikeBall = new SpriteClass(GraphicsDevice, "Content/character_sprite.png", ScaleToHighDPI(1f));
            balloon = new SpriteClass(GraphicsDevice, "Content/character_sprite.png", ScaleToHighDPI(1f));
            
            //
            
            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");
            _camera = new Camera();
            


            // TODO: use this.Content to load your game content here
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
                spikeBall.angle = 100;
                spikeBall.dA = 0;
                balloon.dX = 0;
                balloon.dY = 0;
            }

            if(previousSpeed <= 0 && spikeBall.dY > 0)
            {
                upToDown = true;
            }

            spikeBall.Update(elapsedTime);
            balloon.Update(elapsedTime);

            spikeBall.dY += gravitySpeed;

            if (spikeBall.y > groundHeight)
            {
                spikeBall.dY = 0;
                spikeBall.y = groundHeight;
            }
            
            if (spikeBall.RectangleCollision(balloon))
            {
                float tempAngle = ((float)RadianToDegree(spikeBall.angle) % 360);
                if((tempAngle > 179 || tempAngle == 0) && !gameOver && gameStarted)
                {
                    bounce(tempAngle);
                }
                else
                {
                    gameOver = true;
                }
            }
            _camera.Follow(spikeBall);
            base.Update(gameTime);
        }


        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.Red });
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

            if (!gameStarted)
            {
                spriteBatch.Begin();
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);

                String title = "PARTY HOPPER";
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
            
            System.Diagnostics.Debug.WriteLine(_camera.Transform);
            spriteBatch.Begin(transformMatrix: _camera.Transform);


            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), backgroundScaleRatio, SpriteEffects.None, 1);
            spikeBall.Draw(spriteBatch);

            
            //DrawLine(spriteBatch, //draw line
            //         new Vector2(0, groundHeight - spikeBall.texture.Height * spikeBall.scale / 2), //start of line
            //         new Vector2(screenWidth, groundHeight - spikeBall.texture.Height * spikeBall.scale / 2) //end of line
            //);

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
            backgroundScaleRatio = screenWidth / backgroundTexture.Width;
            groundHeight = (backgroundTexture.Height - 240) * backgroundScaleRatio;
            System.Diagnostics.Debug.WriteLine(backgroundTexture.Height);
            System.Diagnostics.Debug.WriteLine(backgroundScaleRatio);
            System.Diagnostics.Debug.WriteLine(groundHeight);
            
            spikeBall.x = screenWidth / 2;

            spikeBall.y = groundHeight;
            spikeBall.angle = 100;
            spikeBall.dA = 0;

            balloon.x = screenWidth / 2;

            balloon.y = groundHeight;
            score = -1;
            spinSpeed = 7f;
            ballBounceSpeed = ScaleToHighDPI(-1200f);
        }

        void bounce(float angle)
        {
            if(upToDown)
            {
                if (angle > 240 && angle < 300)
                {
                    score += 3;
                    ballBounceSpeed += -200;
                }
                else
                {
                    score++;
                    ballBounceSpeed += -100;
                }
                upToDown = false;
            }

            spikeBall.dY = ballBounceSpeed;
            if(score > 0 && (score % 2 == 0))
            {
                spinSpeed *= (float)1.15;
            }
            spikeBall.dA = spinSpeed;
        }

        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)) Exit();

            // Start the game if Space is pressed.
            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
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

    }
}
