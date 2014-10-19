using System;
using System.Text;
using SharpDX;

using Wheat.Components;

namespace Wheat
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    /// <summary>
    /// The application's main class.
    /// </summary>
    public class Wheat : Game
    {
        #region Fields

        // DirectX
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private Texture2D ballsTexture;
        private SpriteFont arial16Font;

        // Camera
        private Camera _camera;

        // Objects
        private BasicEffect basicEffect;
        private GeometricPrimitive primitive;

        // Input
        private KeyboardManager keyboard;
        private KeyboardState keyboardState;
        private MouseManager mouse;
        private MouseState mouseState;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Wheat" /> class.
        /// </summary>
        public Wheat()
        {
            this.graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Input
            this.keyboard = new KeyboardManager(this);
            this.mouse = new MouseManager(this);
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Wheat";
            _camera = new Camera(this.GraphicsDevice, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);

            base.Initialize();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            // Instantiate a SpriteBatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            ballsTexture = Content.Load<Texture2D>("Textures/Balls");
            arial16Font = Content.Load<SpriteFont>("Fonts/Arial16");

            // Creates a basic effect
            basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            basicEffect.PreferPerPixelLighting = true;
            basicEffect.EnableDefaultLighting();

            // Creates torus primitive
            primitive = ToDisposeContent(GeometricPrimitive.GeoSphere.New(GraphicsDevice));

            base.LoadContent();
        }

        /// <summary>
        /// Reference page contains links to related conceptual articles.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update basic effect for rendering the Primitive
            basicEffect.View = _camera.View;
            basicEffect.Projection = _camera.Projection;

            keyboardState = keyboard.GetState();
            mouseState = mouse.GetState();

            _camera.Update(gameTime);
        }

        /// <summary>
        /// Reference page contains code sample.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        protected override void Draw(GameTime gameTime)
        {
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.Black);

            // Constant used to translate 3d models
            float translateX = 0.0f;

            basicEffect.World = Matrix.Scaling(2.0f, 2.0f, 2.0f) *
                                Matrix.RotationX(0.8f * (float)Math.Sin(time * 1.45)) *
                                Matrix.RotationY(time * 2.0f) *
                                Matrix.RotationZ(0) *
                                Matrix.Translation(translateX, -1.0f, 0);

            primitive.Draw(basicEffect);

            // ------------------------------------------------------------------------
            // Draw the some 2d text
            // ------------------------------------------------------------------------
            spriteBatch.Begin();
            var text = new StringBuilder("");

            // Display pressed keys
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Display mouse coordinates and mouse button status
            text.AppendFormat("Mouse ({0},{1}) Left: {2}, Right {3}", mouseState.X, mouseState.Y, mouseState.LeftButton, mouseState.RightButton).AppendLine();
            
            spriteBatch.DrawString(arial16Font, text.ToString(), new Vector2(16, 16), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
