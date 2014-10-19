using System;
using SharpDX;

namespace Wheat.Components
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    class ShadowCamera
    {
        #region Fields

        #endregion

        #region Properties

        public Vector3 Position { get; private set; }

        public int BackBufferWidth { get; set; }
        public int BackBufferHeight { get; set; }
        #endregion

        #region Public Methods

        public ShadowCamera(GraphicsDevice graphicsDevice, float backBufferWidth = 800, float backBufferHeight = 600)
        {
            this.BackBufferWidth = (int)backBufferWidth;
            this.BackBufferHeight = (int)backBufferHeight;
            this.Position = new Vector3(0, 20, 0);
        }

        public void Update(GameTime gameTime)
        {

        }

        #endregion
    }
}
