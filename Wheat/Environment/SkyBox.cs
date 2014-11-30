using SharpDX;
using Wheat.Components;
using Wheat.Core;

namespace Wheat.Environment
{
    using SharpDX.Toolkit.Graphics;

    class SkyBox
    {
        private GameCore core;
        //private Texture2D texture;
        private TextureCube texture;
        private Effect effect;
        private VertexInputLayout vertexInputLayout;

        private Buffer<VertexPositionNormalTexture> vertexBuffer;
        private Buffer indexBuffer;

        public SkyBox(GameCore core)
        {
            this.core = core;
            this.effect = this.core.ContentManager.Load<Effect>("Effects/Sky");
            this.texture = this.core.ContentManager.Load<TextureCube>("Textures/skyBox");
            SetUpVertices();
            SetUpIndices();
            this.vertexInputLayout = VertexInputLayout.FromBuffer(0, this.vertexBuffer);
       }

        private void SetUpVertices()
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[8];
            
            vertices[0].Position = new Vector3(-1.0f, -1.0f, -1.0f);
            vertices[1].Position = new Vector3(-1.0f, -1.0f, 1.0f);
            vertices[2].Position = new Vector3(1.0f, -1.0f, 1.0f);
            vertices[3].Position = new Vector3(1.0f, -1.0f, -1.0f);
            vertices[4].Position = new Vector3(-1.0f, 1.0f, -1.0f);
            vertices[5].Position = new Vector3(-1.0f, 1.0f, 1.0f);
            vertices[6].Position = new Vector3(1.0f, 1.0f, 1.0f);
            vertices[7].Position = new Vector3(1.0f, 1.0f, -1.0f);

            this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, vertices);
        }

        private void SetUpIndices()
        {

            int[] cubeIndices = new int[36];

            //bottom face
            cubeIndices[0] = 0;
            cubeIndices[1] = 2;
            cubeIndices[2] = 3;
            cubeIndices[3] = 0;
            cubeIndices[4] = 1;
            cubeIndices[5] = 2;

            //top face
            cubeIndices[6] = 4;
            cubeIndices[7] = 6;
            cubeIndices[8] = 5;
            cubeIndices[9] = 4;
            cubeIndices[10] = 7;
            cubeIndices[11] = 6;

            //front face
            cubeIndices[12] = 5;
            cubeIndices[13] = 2;
            cubeIndices[14] = 1;
            cubeIndices[15] = 5;
            cubeIndices[16] = 6;
            cubeIndices[17] = 2;

            //back face
            cubeIndices[18] = 0;
            cubeIndices[19] = 7;
            cubeIndices[20] = 4;
            cubeIndices[21] = 0;
            cubeIndices[22] = 3;
            cubeIndices[23] = 7;

            //left face
            cubeIndices[24] = 0;
            cubeIndices[25] = 4;
            cubeIndices[26] = 1;
            cubeIndices[27] = 1;
            cubeIndices[28] = 4;
            cubeIndices[29] = 5;

            //right face
            cubeIndices[30] = 2;
            cubeIndices[31] = 6;
            cubeIndices[32] = 3;
            cubeIndices[33] = 3;
            cubeIndices[34] = 6;
            cubeIndices[35] = 7;

            this.indexBuffer = Buffer.New(this.core.GraphicsDevice, cubeIndices, BufferFlags.IndexBuffer);
        }

        public void Draw(Camera camera)
        {
            const float size = 1000.0f;

            this.effect.Parameters["World"].SetValue(Matrix.Scaling(size, size, size));
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["SkyBoxTexture"].SetResource(this.texture);
            this.effect.Parameters["LightPosition"].SetValue(this.core.ShadowCamera.Position);
            this.effect.Parameters["CameraPosition"].SetValue(this.core.Camera.Position);

            this.core.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            this.core.GraphicsDevice.SetIndexBuffer(this.indexBuffer, true);
            this.core.GraphicsDevice.SetVertexInputLayout(this.vertexInputLayout);


            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.core.GraphicsDevice.DrawIndexed(PrimitiveType.TriangleList, indexBuffer.ElementCount);
            }
        }

    }
}
