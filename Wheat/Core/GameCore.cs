using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Content;
using Wheat.Components;

namespace Wheat.Core
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    class GameCore
    {
        #region Properties

        public GraphicsDevice GraphicsDevice { get; private set; }
        public ContentManager ContentManager { get; private set; }

        public Camera Camera { get; private set; }
        public ShadowCamera ShadowCamera { get; private set; }

        #endregion

        #region Public Methods

        public GameCore(GraphicsDevice graphicsDevice, ContentManager contentManager, Camera camera, ShadowCamera shadowCamera )
        {
            this.GraphicsDevice = graphicsDevice;
            this.ContentManager = contentManager;
            this.Camera = camera;
            this.ShadowCamera = shadowCamera;
        }

        #endregion
    }
}
