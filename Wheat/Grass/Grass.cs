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

        #region Public Methods

        public GrassController(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice;
            this.effect = content.Load<Effect>("Effects/Grass");
            this.texture = content.Load<Texture2D>("Textures/billboardFlowers");

            // TODO
            // 1. Create lots of independent vertices
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[1];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Up, new Vector2(0, 0));
            this.vertexBuffer = Buffer.Vertex.New(graphicsDevice, vertices);

            // 2. Create geometry shader and make those vertices to quads

        }

        public void Draw(Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.Translation(0, 0, 0) * Matrix.RotationX(0));
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);

            this.graphicsDevice.SetVertexBuffer(this.vertexBuffer);

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.graphicsDevice.Draw(PrimitiveType.PointList, 1);
            }
        }

        #endregion
    }
}
