using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wheat.Manager;

namespace Wheat.Environment
{
    class Terrain
    {
        #region Fields

        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;

        #endregion

        #region Public Methods

        public Terrain(GraphicsDevice graphicsDevice)
        {
            _basicEffect = new BasicEffect(graphicsDevice);

            VertexPositionColor[] vertices = new VertexPositionColor[3];
            vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(+0.5f, 0, 0), Color.Green);
            vertices[2] = new VertexPositionColor(new Vector3(-0.5f, 0, 0), Color.Blue);

            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            _basicEffect.World = camera.WorldMatrix;
            _basicEffect.View = camera.ViewMatrix;
            _basicEffect.Projection = camera.ProjectionMatrix;
            _basicEffect.VertexColorEnabled = true;

            graphicsDevice.SetVertexBuffer(_vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
        }

        #endregion
    }
}
