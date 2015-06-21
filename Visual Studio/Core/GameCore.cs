using GrassRendering.Components;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Input;

namespace GrassRendering.Core
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit.Graphics;

    /// <summary>
    /// Level of detail distances
    /// </summary>
    public enum LevelOfDetail
    {
        Level1 = 0,
        Level2 = 50,
        Level3 = 60,
        Level4 = 70
    }

    class GameCore
    {
        #region Properties

        /// <summary>
        /// The graphics device
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// The content manager
        /// </summary>
        public ContentManager ContentManager { get; private set; }

        /// <summary>
        /// The camera
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// The shadow camera
        /// </summary>
        public ShadowCamera ShadowCamera { get; private set; }

        /// <summary>
        /// The current keyboard state
        /// </summary>
        public KeyboardState KeyboardState;

        /// <summary>
        /// The current mouse state
        /// </summary>
        public MouseState MouseState;

        private readonly KeyboardManager keyboard;
        private readonly MouseManager mouse;

        #endregion

        #region Public Methods

        public GameCore(GraphicsDevice graphicsDevice, ContentManager contentManager, Camera camera, ShadowCamera shadowCamera, KeyboardManager keyboardManager, MouseManager mouseManager)
        {
            this.GraphicsDevice = graphicsDevice;
            this.ContentManager = contentManager;
            this.Camera = camera;
            this.ShadowCamera = shadowCamera;

            this.keyboard = keyboardManager;
            this.mouse = mouseManager;
        }

        public void Update()
        {
            this.KeyboardState = this.keyboard.GetState();
            this.MouseState = this.mouse.GetState();
        }

        #endregion
    }
}
