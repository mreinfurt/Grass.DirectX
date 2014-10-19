using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Content;
using Wheat.Components;

namespace Wheat.Environment
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    class Terrain
    {
        #region Fields

        private GeometricPrimitive plane; 
        private Texture2D texture;
        private Effect effect;
        private SharpDX.Toolkit.Graphics.Buffer<VertexPositionNormalTexture> vertexBuffer;

        private GraphicsDevice graphicsDevice;
        #endregion

        #region Public Methods

        public Terrain(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.effect = content.Load<Effect>("Effects/Terrain");
            this.plane = GeometricPrimitive.Plane.New(graphicsDevice, 50, 50);
            this.texture = content.Load<Texture2D>("Textures/planeGrass");
            this.graphicsDevice = graphicsDevice;

            float size = 20.0f;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-size, size, 0), Vector3.Up, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(size, size, 0), Vector3.Up, new Vector2(1, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-size, -size, 0), Vector3.Up, new Vector2(0, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(size, -size, 0), Vector3.Up, new Vector2(1, 1));

            this.vertexBuffer = Buffer.Vertex.New(graphicsDevice, vertices);
            //this.vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), 4, BufferUsage.WriteOnly);
            //this.vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        }

        public void Draw(Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.RotationX(-90));
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);

            this.graphicsDevice.SetVertexBuffer(this.vertexBuffer);

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.graphicsDevice.Draw(PrimitiveType.TriangleStrip, 4);
            }
        }

        #endregion
    }
}
