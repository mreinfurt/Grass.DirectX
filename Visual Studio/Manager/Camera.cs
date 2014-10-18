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

        private int _backBufferWidth;
        private int _backBufferHeight;
        
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

        public Camera(int backBufferWidth = 800, int backBufferHeight = 600)
        {
            BackBufferWidth = backBufferWidth;
            BackBufferHeight = backBufferHeight;

            // Create default camera position
            _worldMatrix = Matrix.Identity;
            _viewMatrix = Matrix.CreateLookAt(new Vector3(0, -2, 8), Vector3.Zero, Vector3.Up);
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), BackBufferWidth / BackBufferHeight, 0.01f, 100f);
        }

        #endregion

    }
}
