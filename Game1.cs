using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.Graphics.Display;
using System;
using Windows.UI.ViewManagement;

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


        SpriteClass spikeBall;
        SpriteClass balloon;

        bool spaceDown;
        bool gameStarted;
        bool gameOver;

        float screenWidth;
        float screenHeight;

        float ballBounceSpeed;
        float gravitySpeed;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;

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

            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            spaceDown = false;
            gameStarted = false;

            ballBounceSpeed = ScaleToHighDPI(-1200f);
            gravitySpeed = ScaleToHighDPI(30f);
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
            

            spikeBall = new SpriteClass(GraphicsDevice, "Content/characterSprite.png", ScaleToHighDPI(1f));
            balloon = new SpriteClass(GraphicsDevice, "Content/characterSprite.png", ScaleToHighDPI(1f));
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
                balloon.dX = 0;
                balloon.dY = 0;
            }

            spikeBall.Update(elapsedTime);
            balloon.Update(elapsedTime);

            spikeBall.dY += gravitySpeed;

            if (spikeBall.y > screenHeight * SKYRATIO)
            {
                spikeBall.dY = 0;
                spikeBall.y = screenHeight * SKYRATIO;
            }

 //           spikeBall.dA = 7f;

            if (spikeBall.RectangleCollision(balloon))
            {
                bounce();
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue); // Clear the screen

            spriteBatch.Begin();

            spikeBall.Draw(spriteBatch);
            balloon.Draw(spriteBatch);

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
    
            spikeBall.x = screenWidth / 2;
            spikeBall.y = screenHeight * SKYRATIO;
            balloon.x = screenWidth / 2;
            balloon.y = screenHeight * SKYRATIO;
            gameStarted = true;
        }

        void bounce()
        {
            spikeBall.dY = ballBounceSpeed;
            spikeBall.dA = 7f;
            //ballBounceSpeed *= (float)1.1;
        }

        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)) Exit();

            // Start the game if Space is pressed.
            // Exit the keyboard handler method early, preventing the dino from jumping on the same keypress.
            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    StartGame();
                    gameStarted = true;
                    spaceDown = true;
                    gameOver = false;
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

            // Jump if Space (or another jump key) is pressed
            if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
            {
                // Jump if Space is pressed but not held and the dino is on the floor
                if (!spaceDown)
                {
                    spikeBall.dA = 0;
                    spikeBall.angle = spikeBall.angle;
                }

                spaceDown = true;
            }
            else spaceDown = false;
        }
    }
}
