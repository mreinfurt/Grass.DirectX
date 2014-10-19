using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Content;
using Wheat.Components;
using Wheat.Core;

namespace Wheat.Environment
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;

    class Terrain
    {
        #region Fields

        private GameCore core;
        private GeometricPrimitive plane; 
        private Texture2D texture;
        private Effect effect;
        private Buffer<VertexPositionNormalTexture> vertexBuffer;

        #endregion

        #region Public Methods

        public Terrain(GameCore core)
        {
            this.core = core;
            this.effect = this.core.ContentManager.Load<Effect>("Effects/Terrain");
            this.plane = GeometricPrimitive.Plane.New(this.core.GraphicsDevice, 50, 50);
            this.texture = this.core.ContentManager.Load<Texture2D>("Textures/planeGrass");

            float size = 20.0f;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-size, size, 0), Vector3.Up, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(size, size, 0), Vector3.Up, new Vector2(1, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-size, -size, 0), Vector3.Up, new Vector2(0, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(size, -size, 0), Vector3.Up, new Vector2(1, 1));

            this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, vertices);
        }

        public void Draw(Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.RotationX(MathUtil.DegreesToRadians(90)));
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);

            this.core.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.core.GraphicsDevice.Draw(PrimitiveType.TriangleStrip, 4);
            }
        }

        #endregion
    }
}
