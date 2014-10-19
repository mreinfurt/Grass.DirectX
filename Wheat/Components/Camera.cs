using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Wheat.Components
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

        private float _rotation;

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

        public Camera(int backBufferWidth, int backBufferHeight)
        {
            // Prepare matrices
            _position = new Vector3(0, 10, -10);
            _viewMatrix = Matrix.LookAtLH(_position, new Vector3(0, 0, 0), Vector3.UnitY);
            _projectionMatrix = Matrix.Identity;

            _backBufferWidth = backBufferWidth;
            _backBufferHeight = backBufferHeight;
        }

        public void Update(float time)
        {
            _rotation += 0.000005f;
            //_viewMatrix = Matrix.LookAtLH(new Vector3(_position.X * (float)Math.Cos(_rotation), _position.Y, _position.Z * (float)Math.Sin(_rotation)), new Vector3(0, 0, 0), Vector3.Up);
        }

        /// <summary>
        /// Resizes this instance.
        /// </summary>
        public void Resize(int backBufferWidth, int backBufferHeight)
        {
            _projectionMatrix = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, backBufferWidth / (float)backBufferHeight, 0.1f, 100.0f);
        }

        #endregion
    }
}
