using System;
using SharpDX;

namespace Wheat.Components
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    class Camera
    {
        #region Fields

        private double _rotation;

        #endregion

        #region Properties
        public Matrix World { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Vector3 Position { get; private set; }

        public int BackBufferWidth { get; set; }
        public int BackBufferHeight { get; set; }
        #endregion

        #region Public Methods

        public Camera(GraphicsDevice graphicsDevice, float backBufferWidth = 800, float backBufferHeight = 600)
        {
            this.BackBufferWidth = (int)backBufferWidth;
            this.BackBufferHeight = (int)backBufferHeight;

            // Create default camera position
            this.Position = new Vector3(0, 10, 10);
            this.World = Matrix.Identity;

            // Calculates the world and the view based on the model size
            this.View = Matrix.LookAtRH(this.Position, new Vector3(0, 0, 0), Vector3.UnitY);
            this.Projection = Matrix.PerspectiveFovRH(0.9f, (float)graphicsDevice.BackBuffer.Width / graphicsDevice.BackBuffer.Height, 0.01f, 100.0f);
        }

        public void Update(GameTime gameTime)
        {
            _rotation += gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            this.View = Matrix.LookAtRH(new Vector3(this.Position.X * (float)Math.Cos(_rotation), this.Position.Y, this.Position.Y * (float)Math.Sin(_rotation)), new Vector3(0, 2, 0), Vector3.Up);
        }

        #endregion
    }
}
