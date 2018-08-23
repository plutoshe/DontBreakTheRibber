using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WaterPump
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    public class StrokeItem
    {
        Texture2D texture;
        Vector2 pos;
    }
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector2 ScreenSize = new Vector2(1024, 5012);
        Vector2 GraphicSize = new Vector2(1024, 1024);

        Texture2D ballTexture;
        Rectangle ballRectangle;
        Vector2 ballOrigin;
        Vector2 ballPosition;

        float ballYSpeed = 0;
        bool DirectionYCouldChange = true;

        float rotation = 0;
   
        

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
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ballTexture = Content.Load<Texture2D>("character_sprite");
            ballPosition = new Vector2(300, 250);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ballRectangle = new Rectangle((int)ballPosition.X, (int)ballPosition.Y, ballTexture.Width, ballTexture.Height);
            ballOrigin = new Vector2(ballRectangle.Width / 2, ballRectangle.Height / 2);
            ballPosition.Y += ballYSpeed;
            rotation += 0.1f;
            if (Keyboard.GetState().IsKeyUp(Keys.B)) DirectionYCouldChange = true;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && DirectionYCouldChange)
            {
                ballYSpeed = -ballYSpeed; DirectionYCouldChange = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotation += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotation -= 0.1f;
            base.Update(gameTime);
        }

       
        protected override void Draw(GameTime gameTime)
        {
            /*
            var screenWidth = Window.ClientBounds.Width;
            var screenHeight = Window.ClientBounds.Height;
            System.Console.WriteLine(Window.ClientBounds.Width);
            System.Console.WriteLine(Window.ClientBounds.Height);*/
            GraphicsDevice.Clear(Color.CornflowerBlue);
            /*
            var initialPositionX = screenWidth / 2;
            var ínitialPositionY = (int)(screenHeight * 0.8);
            var ballDimension = 10;
            var ballRectangle = new Rectangle(initialPositionX, ínitialPositionY, ballDimension, ballDimension);
            Texture2D texture = new Texture2D(this.GraphicsDevice, 100, 100);
            Color[] colorData = new Color[100 * 100];
            for (int i = 0; i < 10000; i++)
                colorData[i] = Color.White;
            texture.SetData<Color>(colorData);*/
            spriteBatch.Begin();
            spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, rotation, ballOrigin, 1f, SpriteEffects.None, 0);
            
            spriteBatch.End();


            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }


    }
}
