#region Using Statements
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Wheat.Environment;
using Wheat.Manager;
#endregion

namespace Wheat
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Texture2D _fireBall;
        Camera _camera;

        // World objects
        Terrain _terrain;
        Monkey _monkey;

        public Main() : base() 
        {
            _graphics = new GraphicsDeviceManager(this);
            _camera = new Camera();

            _graphics.PreferredBackBufferWidth = _camera.BackBufferWidth;
            _graphics.PreferredBackBufferHeight = _camera.BackBufferHeight;
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fireBall = Content.Load<Texture2D>("Textures/FireBall");

            _terrain = new Terrain(GraphicsDevice);
            _terrain.LoadContent(Content);

            _monkey = new Monkey(GraphicsDevice);
            _monkey.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_fireBall, new Vector2(100.0f, 100.0f));
            _spriteBatch.End();

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            _terrain.Draw(GraphicsDevice, _camera);
            _monkey.Draw(GraphicsDevice, _camera);

            base.Draw(gameTime);
        }
    }
}
