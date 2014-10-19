using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Content;
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

        private Texture2D texture;
        private Effect effect;
        private Buffer<VertexPositionNormalTexture> vertexBuffer;

        private GameCore core;

        #endregion

        #region Properties

        public int RootCount;

        #endregion

        #region Public Methods

        public GrassController(GameCore core)
        {
            this.core = core;
            this.effect = this.core.ContentManager.Load<Effect>("Effects/Grass");
            this.texture = this.core.ContentManager.Load<Texture2D>("Textures/grassBlade");

            // 1. Create lots of independent vertices (roots)
            int rows = 400;
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
                    // The Y position should be a bit randomized too, but we have to remain in the grid
                    float randomizedYOffset = (float) rnd.NextDouble(-0.25, 0.25);

                    randomizedDistance = (float)rnd.NextDouble(0.3, 2);
                    var currentPosition = new Vector3(startPosition.X + (j * randomizedDistance), startPosition.Y + randomizedYOffset, startPosition.Z);
                    vertices[currentVertex] = new VertexPositionNormalTexture(currentPosition, Vector3.Up, new Vector2(0, 0));
                    currentVertex++;
                }

                randomizedDistance = (float)rnd.NextDouble(0, 0.25);
                startPosition.Z += randomizedDistance;
            }

            this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, vertices);
        }

        public void Draw(GameTime gameTime, Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.Identity);
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);
            this.effect.Parameters["Time"].SetValue(new Vector2((float)gameTime.TotalGameTime.TotalMilliseconds / 1000, gameTime.ElapsedGameTime.Milliseconds));
            this.effect.Parameters["LightPosition"].SetValue(this.core.ShadowCamera.Position);

            this.core.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.core.GraphicsDevice.Draw(PrimitiveType.PointList, this.RootCount);
            }
        }

        #endregion
    }
}
