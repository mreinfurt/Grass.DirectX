using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wheat.Manager
{
    class Camera
    {
        #region Fields

        private Matrix _worldMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private Vector3 _position;
        private int _backBufferWidth;
        private int _backBufferHeight;

        private double _rotation;
        
        #endregion

        #region Properties
        public Matrix WorldMatrix
        {
            get { return _worldMatrix; }
        }

        public Matrix ViewMatrix
        {
            get { return _viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return _projectionMatrix; }
        }

        public Vector3 Position
        {
            get { return _position; }
        }

        public int BackBufferWidth
        {
            get { return _backBufferWidth; }
            set { _backBufferWidth = value; }
        }

        public int BackBufferHeight
        {
            get { return _backBufferHeight; }
            set { _backBufferHeight = value; }
        }
        #endregion

        #region Public Methods

        public Camera(GraphicsDevice graphicsDevice, int backBufferWidth = 800, int backBufferHeight = 600)
        {
            BackBufferWidth = backBufferWidth;
            BackBufferHeight = backBufferHeight;

            // Create default camera position
            _position = new Vector3(0, 10, 20);
            _worldMatrix = Matrix.Identity;
            _viewMatrix = Matrix.CreateLookAt(_position, Vector3.Zero, Vector3.Up);
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), graphicsDevice.Viewport.AspectRatio, 1f, 1000f);
        }

        public void Update(GameTime gameTime)
        {
            _rotation += gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            _viewMatrix = Matrix.CreateLookAt(new Vector3(5.0f * (float)Math.Cos(_rotation), 2, 5.0f * (float)Math.Sin(_rotation)), new Vector3(0, 2, 0), Vector3.Up);
        }

        #endregion
    }
}
