using System;
using SharpDX;
using SharpDX.Toolkit.Input;
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
        private Effect effect;
        private Buffer<VertexPositionNormalTexture> vertexBuffer;
        private VertexInputLayout vertexInputLayout;
        private VertexPositionNormalTexture[] vertices;

        private Texture2D heightMap;
        private float[,] heightData;


        private BoundingFrustum boundingFrustum;
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
        public int NumberOfPatchRows { get; private set; }

        /// <summary>
        /// Returns the number of roots.
        /// </summary>
        public int NumberOfRoots { get; private set; }

        /// <summary>
        /// Returns the number of patches.
        /// </summary>
        public int NumberOfPatches { get; private set; }

        /// <summary>
        /// Returns the number of roots in in one patch.
        /// </summary>
        public int NumberOfRootsInPatch { get; private set; }

        /// <summary>
        /// Returns the number of rows in in one patch.
        /// </summary>
        public int NumberOfRowsInPatch { get; private set; }

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
            this.heightMap = this.core.ContentManager.Load<Texture2D>("Textures/heightMap");
            this.LoadHeightData(this.heightMap);
            this.GenerateRoots();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (this.core.KeyboardState.IsKeyPressed(Keys.F5))
            {
                this.effect = this.core.ContentManager.Load<Effect>("Effects/Grass");
            }

            this.boundingFrustum = new BoundingFrustum(this.core.Camera.View * this.core.Camera.Projection);
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

            // Draw patches
            int startRoot = 0;
            
            for (int i = 0; i < this.NumberOfPatches; i++)
            {
                BoundingSphere boundingSphere = new BoundingSphere(this.vertices[startRoot + this.NumberOfRootsInPatch / 2].Position, 3.0f);

                if (!this.boundingFrustum.Intersects(ref boundingSphere))
                {
                    startRoot += this.NumberOfRootsInPatch;
                }
                else
                {
                    Vector3 cameraPosition = this.core.Camera.Position;
                    cameraPosition.Y = 0;
                    Vector3 difference = cameraPosition - this.vertices[startRoot].Position;
                    float distance = difference.Length();

                    if (distance > (int)LevelOfDetail.Level4)
                    {
                        this.effect.Techniques["LevelOfDetail4"].Passes[0].Apply();
                    }
                    else if (distance > (int)LevelOfDetail.Level3)
                    {
                        this.effect.Techniques["LevelOfDetail3"].Passes[0].Apply();
                    }
                    else if (distance > (int)LevelOfDetail.Level2)
                    {
                        this.effect.Techniques["LevelOfDetail2"].Passes[0].Apply();
                    }
                    else
                    {
                        this.effect.Techniques["LevelOfDetail1"].Passes[0].Apply();
                    }

                    this.core.GraphicsDevice.Draw(PrimitiveType.PointList, this.NumberOfRootsInPatch, startRoot);
                    startRoot += this.NumberOfRootsInPatch;
                }
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
            this.NumberOfPatchRows = 50;
            this.NumberOfPatches = this.NumberOfPatchRows * this.NumberOfPatchRows;
            this.NumberOfRootsInPatch = 100;
            this.NumberOfRowsInPatch = 10;
            this.NumberOfRoots = this.NumberOfPatches * this.NumberOfRootsInPatch;

            this.DistanceSpaceX = new Vector2(0.3f, 0.5f);
            this.DistanceSpaceZ = new Vector2(0.3f, 0.5f);
            this.vertices = new VertexPositionNormalTexture[this.NumberOfRoots];

            Random rnd = new Random();
            int currentVertex = 0;

            Vector3 startPosition = new Vector3(0, 0, 0);
            int rootsPerRow = this.NumberOfRootsInPatch / this.NumberOfRowsInPatch;

            // Generate grid of patches
            for (int x = 0; x < this.NumberOfPatchRows; x++)
            {
                for (int y = 0; y < this.NumberOfPatchRows; y++)
                {
                    currentVertex = this.AddPatch(this.NumberOfRowsInPatch, rootsPerRow, startPosition, currentVertex, rnd);
                    startPosition.X += rootsPerRow * 0.5f;
                }

                startPosition.X = 0;
                startPosition.Z += rootsPerRow * 0.4f;
            }


            this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, this.vertices);
            this.vertexInputLayout = VertexInputLayout.FromBuffer(0, this.vertexBuffer);
        }

        /// <summary>
        /// Generates all the roots for one patch.
        /// </summary>
        /// <param name="numberOfRoots">Number of roots in total for this patch</param>
        /// <param name="rootsPerRow">How many roots are in one row of the patch</param>
        /// <param name="startPosition">Start of the roots in object space</param>
        /// <param name="currentVertex">Index of the vertex</param>
        /// <param name="rnd">Random seed</param>
        /// <returns></returns>
        private int AddPatch(int numberOfRoots, int rootsPerRow, Vector3 startPosition, int currentVertex, Random rnd)
        {
            double maxDistance = rootsPerRow * 0.5;

            for (var i = 0; i < this.NumberOfRowsInPatch; i++)
            {
                float randomizedDistance;
                for (var j = 0; j < rootsPerRow; j++)
                {
                    // The Z position should be a bit randomized too, but we have to remain in the grid
                    float randomizedZOffset = (float)rnd.NextDouble(DistanceSpaceZ.X, DistanceSpaceZ.Y);

                    randomizedDistance = (float)rnd.NextDouble(0, maxDistance);
   
                    int indexX = (int)((startPosition.X + randomizedDistance));
                    int indexZ = (int)((startPosition.Z + randomizedZOffset ));


                    var currentPosition = new Vector3(startPosition.X + (randomizedDistance), heightData[indexX, indexZ], startPosition.Z + randomizedZOffset);
                    this.vertices[currentVertex] = new VertexPositionNormalTexture(currentPosition, Vector3.Up, new Vector2(0, 0));
                    currentVertex++;
                }

                randomizedDistance = (float)rnd.NextDouble(this.DistanceSpaceX.X, this.DistanceSpaceX.Y);
                startPosition.Z += randomizedDistance;
            }

            return currentVertex;
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            int width = heightMap.Width;
            int height = heightMap.Height;

            Image image = heightMap.GetDataAsImage();
            heightData = new float[width, height];


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    PixelData.R8G8B8A8 pixel = image.PixelBuffer[0].GetPixel<PixelData.R8G8B8A8>(x, y);
                    heightData[x, y] = (pixel.R - 225f)/5f;
                }
            }

        }

        #endregion
    }
}
