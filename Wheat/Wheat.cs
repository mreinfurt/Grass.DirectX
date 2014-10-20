using System;
using System.Drawing;
using System.Text;
using Ninject;
using Ninject.Extensions.Logging.Log4net;
using Ninject.Modules;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.XInput;
using Wheat.Components;
using Wheat.Core;
using Wheat.Environment;
using Wheat.Grass;
using Color = SharpDX.Color;

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

        // Engine
        private GameCore gameCore;

        // Camera
        private Camera camera;
        private ShadowCamera shadowCamera;

        // Objects
        private Terrain terrain;
        private GrassController grass;

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
            this.graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            this.graphicsDeviceManager.PreferredBackBufferHeight = 800;
            this.graphicsDeviceManager.PreferMultiSampling = true;
            Content.RootDirectory = "Content";

            // Input
            this.keyboard = new KeyboardManager(this);
            this.mouse = new MouseManager(this);
        }

        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Wheat";
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
            camera = new Camera(this.GraphicsDevice, this.graphicsDeviceManager.PreferredBackBufferWidth, this.graphicsDeviceManager.PreferredBackBufferHeight, keyboard, mouse);
            shadowCamera = new ShadowCamera(this.GraphicsDevice, this.graphicsDeviceManager.PreferredBackBufferWidth, this.graphicsDeviceManager.PreferredBackBufferHeight);

            gameCore = new GameCore(this.GraphicsDevice, this.Content, this.camera, this.shadowCamera);

            terrain = new Terrain(this.gameCore);
            grass = new GrassController(this.gameCore);

            base.LoadContent();
        }

        /// <summary>
        /// Reference page contains links to related conceptual articles.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            keyboardState = keyboard.GetState();
            mouseState = mouse.GetState();
            camera.Update(gameTime);
        }

        /// <summary>
        /// Reference page contains code sample.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        protected override void Draw(GameTime gameTime)
        {
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.Black);

            this.SetUpBlendState();
            this.SetUpRasterizerState();

            // Input
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Objects
            terrain.Draw(camera);
            grass.Draw(gameTime, camera);

            // Draw string (Mouse position and FPS)
            spriteBatch.Begin();
            var text = new StringBuilder("");
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            text.AppendFormat("Mouse ({0},{1}); FPS: {2}", mouseState.X, mouseState.Y, frameRate).AppendLine();
            spriteBatch.DrawString(arial16Font, text.ToString(), new Vector2(16, 16), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the blending state.
        /// </summary>
        private void SetUpBlendState()
        {
            var blendStateDesc = new BlendStateDescription();
            blendStateDesc.AlphaToCoverageEnable = true;
            blendStateDesc.IndependentBlendEnable = false;
            blendStateDesc.RenderTarget[0].IsBlendEnabled = true;
            blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
            blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.DestinationAlpha;
            blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

            BlendState blendState = BlendState.New(this.GraphicsDevice, blendStateDesc);
            this.GraphicsDevice.SetBlendState(blendState);
        }

        /// <summary>
        /// Sets the state of the rasterizer.
        /// </summary>
        private void SetUpRasterizerState()
        {
            RasterizerStateDescription stateDescription = new RasterizerStateDescription();
            stateDescription.FillMode = FillMode.Solid;
            stateDescription.CullMode = CullMode.None;
            this.GraphicsDevice.SetRasterizerState(RasterizerState.New(this.GraphicsDevice, stateDescription));
        }

        #endregion
    }
}
