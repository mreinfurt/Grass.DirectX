using System;
using SharpDX;
using SharpDX.Toolkit.Input;

namespace Wheat.Components
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    class Camera
    {
        #region Fields

        // Camera Movement
        const float rotationSpeed = 0.5f;
        const float movementSpeed = 30.0f;
        float horizontalRotation = MathUtil.PiOverTwo;
        float verticalRotation = -MathUtil.Pi / 10.0f;
        MouseState originalMouseState;

        #endregion

        #region Properties
        public Matrix World { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Vector3 Position { get; private set; }

        public int BackBufferWidth { get; set; }
        public int BackBufferHeight { get; set; }

        private KeyboardManager keyboard;
        private MouseManager mouse;
        #endregion

        #region Public Methods

        public Camera(GraphicsDevice graphicsDevice, int backBufferWidth, int backBufferHeight, KeyboardManager keyboard, MouseManager mouse)
        {
            this.BackBufferWidth = backBufferWidth;
            this.BackBufferHeight = backBufferHeight;

            this.keyboard = keyboard;
            this.mouse = mouse;

            // Create default camera position
            this.Position = new Vector3(0, 10, 10);
            this.World = Matrix.Identity;

            // Calculates the world and the view based on the model size
            this.View = Matrix.LookAtRH(this.Position, new Vector3(0, 0, 0), Vector3.UnitY);
            this.Projection = Matrix.PerspectiveFovRH(0.9f, (float)graphicsDevice.BackBuffer.Width / graphicsDevice.BackBuffer.Height, 0.01f, 100.0f);

            mouse.SetPosition(new Vector2(0.5f, 0.5f));
            originalMouseState = mouse.GetState();
        }

        public void Update(GameTime gameTime)
        {
            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            // Handle mouse input
            MouseState currentMouseState = mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = (currentMouseState.X * BackBufferWidth) - (originalMouseState.X * BackBufferWidth);
                float yDifference = (currentMouseState.Y * BackBufferHeight) - (originalMouseState.Y * BackBufferHeight);
                horizontalRotation -= rotationSpeed * xDifference * amount;
                verticalRotation -= rotationSpeed * yDifference * amount;

                mouse.SetPosition(new Vector2(0.5f, 0.5f));
                UpdateViewMatrix();
            }

            // Handle keyboard input
            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                moveVector += new Vector3(0, 0, -1);
            }

            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
            {
                moveVector += new Vector3(0, 0, 1);                
            }

            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                moveVector += new Vector3(1, 0, 0);                
            }

            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                moveVector += new Vector3(-1, 0, 0);                
            }
            if (keyState.IsKeyDown(Keys.Q))
            {
                moveVector += new Vector3(0, 1, 0);                
            }
            if (keyState.IsKeyDown(Keys.Z))
            {
                moveVector += new Vector3(0, -1, 0);                
            }

            AddToCameraPosition(moveVector * amount);
        }

        #endregion

        #region Private Methods

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.RotationX(verticalRotation) * Matrix.RotationY(horizontalRotation);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector4 convertVectorTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraRotatedTarget = new Vector3(convertVectorTarget.X, convertVectorTarget.Y, convertVectorTarget.Z);
            Vector3 cameraFinalTarget = this.Position + cameraRotatedTarget;

            Vector4 convertVectorUp = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
            Vector3 cameraRotatedUpVector = new Vector3(convertVectorUp.X, convertVectorUp.Y, convertVectorUp.Z);

            this.View = Matrix.LookAtRH(this.Position, cameraFinalTarget, cameraRotatedUpVector);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.RotationX(verticalRotation) * Matrix.RotationY(horizontalRotation);

            Vector4 convertVectorRotatoin = Vector3.Transform(vectorToAdd, cameraRotation);
            Vector3 rotatedVector = new Vector3(convertVectorRotatoin.X, convertVectorRotatoin.Y, convertVectorRotatoin.Z);
            this.Position += movementSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        #endregion
    }
}
