using System;
using SharpDX;
using Wheat.Components;
using Wheat.Core;

namespace Wheat.Grass
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    /// <summary>
    /// Main class for creating and rendering the grass
    /// </summary>
    class GrassController
    {
        #region Fields

        private readonly Texture2D texture;
        private readonly Effect effect;
        private Buffer<VertexPositionNormalTexture> vertexBuffer;
        private VertexInputLayout vertexInputLayout;

        private readonly GameCore core;

        #endregion

        #region Properties

        /// <summary>
        /// The offset where the grid starts. 
        /// Used so the roots are distributed fairly equal around the object space origin.
        /// </summary>
        public float StartPositionOffset { get; private set;  }

        /// <summary>
        /// The number of rows for x and y of the grid
        /// </summary>
        public int NumberOfRows { get; private set; }

        /// <summary>
        /// Returns the number of roots.
        /// </summary>
        public int NumberOfRoots { get; private set; }

        /// <summary>
        /// Space for the random displacement (distance between roots) for the X axis.
        /// X is minimum, Y is maximum.
        /// </summary>
        public Vector2 DistanceSpaceX { get; private set; }

        /// <summary>
        /// Space for the random displacement (distance between roots) for the Z axis.
        /// X is minimum, Y is maximum.
        /// </summary>
        public Vector2 DistanceSpaceZ { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassController"/> class.
        /// </summary>
        /// <param name="core">The core.</param>
        public GrassController(GameCore core)
        {
            this.core = core;
            this.effect = this.core.ContentManager.Load<Effect>("Effects/Grass");
            this.texture = this.core.ContentManager.Load<Texture2D>("Textures/grassBladeDrawn");
            
            this.GenerateRoots();
        }

        /// <summary>
        /// Draws grass field.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="camera">The camera.</param>
        public void Draw(GameTime gameTime, Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.Identity);
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            if (this.effect.Parameters["Texture"] != null) this.effect.Parameters["Texture"].SetResource(this.texture);
            this.effect.Parameters["Time"].SetValue(new Vector2((float)gameTime.TotalGameTime.TotalMilliseconds / 1000, gameTime.ElapsedGameTime.Milliseconds));
            this.effect.Parameters["LightPosition"].SetValue(this.core.ShadowCamera.Position);
            this.effect.Parameters["CameraPosition"].SetValue(this.core.Camera.Position);

            this.core.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            this.core.GraphicsDevice.SetVertexInputLayout(this.vertexInputLayout);

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.core.GraphicsDevice.Draw(PrimitiveType.PointList, this.NumberOfRoots);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the roots.
        /// </summary>
        private void GenerateRoots()
        {
            // Initialize parameters
            this.NumberOfRows = 100;
            this.NumberOfRoots = this.NumberOfRows * this.NumberOfRows;
            this.StartPositionOffset = -0.15f;
            this.DistanceSpaceX = new Vector2(0.2f, 0.4f);
            this.DistanceSpaceZ = new Vector2(0.2f, 0.4f);

            Random rnd = new Random();
            Vector3 startPosition = new Vector3(this.NumberOfRows * this.StartPositionOffset, 0, this.NumberOfRows * this.StartPositionOffset);
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[this.NumberOfRoots];

            // Generate roots in a grid
            int currentVertex = 0;
            int rootsPerRow = this.NumberOfRoots / this.NumberOfRows;

            for (var i = 0; i < rootsPerRow; i++)
            {
                float randomizedDistance;
                for (var j = 0; j < rootsPerRow; j++)
                {
                    // The Z position should be a bit randomized too, but we have to remain in the grid
                    float randomizedZOffset = (float)rnd.NextDouble(DistanceSpaceZ.X, DistanceSpaceZ.Y); 

                    randomizedDistance = (float)rnd.NextDouble(this.DistanceSpaceX.X, this.DistanceSpaceX.Y);
                    var currentPosition = new Vector3(startPosition.X + (j * randomizedDistance), startPosition.Y, startPosition.Z + randomizedZOffset);
                    vertices[currentVertex] = new VertexPositionNormalTexture(currentPosition, Vector3.Up, new Vector2(0, 0));
                    currentVertex++;
                }

                randomizedDistance = (float)rnd.NextDouble(this.DistanceSpaceX.X, this.DistanceSpaceX.Y);
                startPosition.Z += randomizedDistance;
            }

            this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, vertices);
            this.vertexInputLayout = VertexInputLayout.FromBuffer(0, this.vertexBuffer);
        }

        #endregion
    }
}
