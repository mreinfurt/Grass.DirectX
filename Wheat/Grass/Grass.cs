using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Content;
using Wheat.Components;

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

        private GraphicsDevice graphicsDevice;
        private Texture2D texture;
        private Effect effect;
        private Buffer<VertexPositionNormalTexture> vertexBuffer;

        #endregion

        #region Properties

        public int RootCount;

        #endregion

        #region Public Methods

        public GrassController(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice;
            this.effect = content.Load<Effect>("Effects/Grass");
            this.texture = content.Load<Texture2D>("Textures/grassBlade");

            // 1. Create lots of independent vertices
            int rows = 100;
            this.RootCount = rows * rows;
            int rootsPerRow = this.RootCount / rows;

            Vector3 startPosition = new Vector3(-20, 0, -20);
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[this.RootCount];

            int currentVertex = 0;

            Random rnd = new Random();
            float randomizedDistance = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < rootsPerRow; j++)
                {
                    randomizedDistance = (float)rnd.NextDouble(0.3, 2);
                    var currentPosition = new Vector3(startPosition.X + (j * randomizedDistance), startPosition.Y, startPosition.Z);
                    vertices[currentVertex] = new VertexPositionNormalTexture(currentPosition, Vector3.Up, new Vector2(0, 0));
                    currentVertex++;
                }

                randomizedDistance = (float)rnd.NextDouble(0, 2);
                startPosition.Z += randomizedDistance;
            }

            this.vertexBuffer = Buffer.Vertex.New(graphicsDevice, vertices);
        }

        public void Draw(GameTime gameTime, Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.Identity);
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);
            this.effect.Parameters["Time"].SetValue(new Vector2((float)gameTime.TotalGameTime.TotalMilliseconds / 1000, gameTime.ElapsedGameTime.Milliseconds));

            this.graphicsDevice.SetVertexBuffer(this.vertexBuffer);

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.graphicsDevice.Draw(PrimitiveType.PointList, this.RootCount);
            }
        }

        #endregion
    }
}
