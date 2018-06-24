using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        spriteClass avatar;
        spriteClass fire;

        bool spaceDown;
        bool gameStarted;
        bool gameOver;

        float fireSpeedMultiplier;
        float gravitySpeed;
        float AvatarSpeedX;
        float AvatarJumpY;
        float score;

        Random random;
        const float SKYRATIO = 2f / 3f;
        float screenWidth;
        float screenHeight;
        Texture2D grass;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Texture2D texture2;

        Texture2D startGameSplash;
        SpriteFont scoreFont;
        SpriteFont stateFont;
        
        Texture2D gameOverTexture;
        KeyboardState previousState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            // Get screen height and width, scaling them up if running on a high-DPI monitor.
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            fireSpeedMultiplier = 0.5f;
            spaceDown = false;
            gameStarted = false;
            gameOver = false;
            random = new Random();
            AvatarSpeedX = (1000f);
            AvatarJumpY = (-1200f);
            gravitySpeed = (30f);
            this.IsMouseVisible = false; // Hide the mouse within the app window
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            grass = Content.Load<Texture2D>("grass");
            texture = this.Content.Load<Texture2D>("tinyWalk");
            texture2 = this.Content.Load<Texture2D>("fire");
           
            avatar = new spriteClass( texture,  1f);
            fire = new spriteClass( texture2, 0.2f);

            startGameSplash = Content.Load<Texture2D>("start-splash");
            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");

            gameOverTexture = Content.Load<Texture2D>("brown");
        }

        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time elapsed since last Update iteration

            KeyboardHandler(); // Handle keyboard input
            if (gameOver)
            {
                avatar.dX = 0;
                avatar.dY = 0;
                fire.dX = 0;
                fire.dY = 0;
                fire.dA = 0;
            }

            // Stop all movement when the game ends
            if (gameOver)
            {
                avatar.dX = 0;
                avatar.dY = 0;
                fire.dX = 0;
                fire.dY = 0;
                fire.dA = 0;
            }

            // Update animated SpriteClass objects based on their current rates of change
            avatar.Update(elapsedTime);
            fire.Update(elapsedTime);

            // Accelerate the Avatar downward each frame to simulate gravity.
            avatar.dY += gravitySpeed;

            // Set game floor
            if (avatar.y > screenHeight * SKYRATIO)
            {
                avatar.dY = 0;
                avatar.y = screenHeight * SKYRATIO;
            }

            // Set right edge
            if (avatar.x > screenWidth - avatar.texture.Width / 2)
            {
                avatar.x = screenWidth - avatar.texture.Width / 2;
                avatar.dX = 0;
            }

            // Set left edge
            if (avatar.x < 0 + avatar.texture.Width / 2)
            {
                avatar.x = 0 + avatar.texture.Width / 2;
                avatar.dX = 0;
            }

            // If the fire goes offscreen, spawn a new one and iterate the score
            if (fire.y > screenHeight + 100 || fire.y < -100 || fire.x > screenWidth + 100 || fire.x < -100)
            {
                SpawnFire();
                score++;
            }

            if (avatar.RectangleCollision(fire)) gameOver = true;

            //if (avatar.RectangleCollision(fire)) gameOver = true; // End game if the Avatar collides with the fire
            base.Update(gameTime);
        }
    

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumPurple); // Clear the screen

            spriteBatch.Begin(); // Begin drawing
            // Draw grass
            spriteBatch.Draw(grass, new Rectangle(0, (int)(screenHeight * SKYRATIO), (int)screenWidth, (int)screenHeight), Color.White);


            if (gameOver)
            {
                // Draw game over texture
                spriteBatch.Draw(gameOverTexture, new Vector2(screenWidth / 2 - gameOverTexture.Width / 2, screenHeight / 4 - gameOverTexture.Height / 2), Color.White);
                String pressEnter = "Press Enter to restart!";
                // Measure the size of text in the given font
                Vector2 pressEnterSize = stateFont.MeasureString(pressEnter);
                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, pressEnter, new Vector2(screenWidth / 2 - pressEnterSize.X / 2, screenHeight - 200), Color.White);
            }

            // Draw fire and Avatar with the SpriteClass method
            fire.Draw(spriteBatch);
            avatar.Draw(spriteBatch);

            spriteBatch.DrawString(scoreFont, score.ToString(),
            new Vector2(screenWidth - 100, 50), Color.Black);

            if (!gameStarted)
            {
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0,
                (int)screenWidth, (int)screenHeight), Color.White);

                String title = "FLAME DODGE";
                String pressSpace = "Press Space to start";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title,
                new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3),
                Color.Maroon);
                spriteBatch.DrawString(stateFont, pressSpace,
                new Vector2(screenWidth / 2 - pressSpaceSize.X / 2,
                screenHeight / 2), Color.White);
            }

            spriteBatch.End(); // Stop drawing



            base.Draw(gameTime);
        }


        // Spawn the fire object in a random location offscreen
        public void SpawnFire()
        {
            // Spawn fire either left (1), above (2), right (3), or below (4) the screen
            int direction = random.Next(1, 5);
            switch (direction)
            {
                case 1:
                    fire.x = -100;
                    fire.y = random.Next(0, (int)screenHeight);
                    break;
                case 2:
                    fire.y = -100;
                    fire.x = random.Next(0, (int)screenWidth);
                    break;
                case 3:
                    fire.x = screenWidth + 100;
                    fire.y = random.Next(0, (int)screenHeight);
                    break;
                case 4:
                    fire.y = screenHeight + 100;
                    fire.x = random.Next(0, (int)screenWidth);
                    break;
            }

            // Increase  fire speed for every five points scored
            if (score % 5 == 0) fireSpeedMultiplier += 0.2f; 

            // Orient the fire sprite towards the Avatar sprite and set angular velocity
            fire.dX = (avatar.x - fire.x) * fireSpeedMultiplier;
            fire.dY = (avatar.y - fire.y) * fireSpeedMultiplier;
            fire.dA = 7f;
        }


        // Start a new game, on start up or after game over
        public void StartGame()
        {
            // Reset Avatar position
            avatar.x = screenWidth / 2;
            avatar.y = screenHeight * SKYRATIO;
            // Reset fire speed and respawn it
            fireSpeedMultiplier = 0.5f;
            SpawnFire();

            score = 0; // Reset score
        }



        //Keyboard
        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)) Exit();

            // Start the game if Space is pressed.
            // Exit the keyboard handler method early, preventing the Avatar from jumping on the same keypress.
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

            // Restart game if Enter is pressed
            if (gameOver)
            {
                if (state.IsKeyDown(Keys.Enter))
                {
                    StartGame();
                    gameOver = false;
                }
            }

            // Jump  
            if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
            {
                // Jump if Space is pressed but not held and the Avatar is  on the floor
                if (!spaceDown && avatar.y >= screenHeight * SKYRATIO - 1) avatar.dY = AvatarJumpY;
                spaceDown = true;
            }
            else spaceDown = false;

            // left and right
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A)) avatar.dX = AvatarSpeedX * -1;
            else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D)) avatar.dX = AvatarSpeedX;
            else avatar.dX = 0;
        }
    }
}