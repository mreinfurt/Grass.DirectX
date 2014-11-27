using System.Runtime.CompilerServices;
using SharpDX;
using SharpDX.Toolkit;
using Wheat.Components;
using Wheat.Core;


namespace Wheat.Environment
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit.Graphics;

    internal class Terrain
    {
        #region Fields

        private GameCore core;
        private Texture2D texture;
        private Effect effect;

        private VertexPositionNormalTexture[] vertices;
        private int[] indices;

        private int nIndices;
        private Buffer<VertexPositionNormalTexture> vertexBuffer;
        private Buffer indexBuffer;
        private VertexInputLayout vertexInputLayout;

        private Texture2D heightMap;
        private float[,] heightData;


        #endregion

        #region Public Methods

        public Terrain(GameCore core)
        {
            this.core = core;
            this.effect = this.core.ContentManager.Load<Effect>("Effects/Terrain");
            this.texture = this.core.ContentManager.Load<Texture2D>("Textures/grass");
            this.heightMap = this.core.ContentManager.Load<Texture2D>("Textures/heightMap");

            nIndices = (this.heightMap.Width - 1) * (this.heightMap.Height - 1) * 6;

            LoadHeightData(this.heightMap);
            SetUpVertices();
            SetUpIndices();
            GenNormals();

            this.vertexInputLayout = VertexInputLayout.FromBuffer(0, this.vertexBuffer);
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
                    heightData[x, y] = (pixel.R - 225f)/5f ;
                }
            }

        }

        private void SetUpVertices()
        {
            int width = heightMap.Width;
            int height = heightMap.Height;
            vertices = new VertexPositionNormalTexture[width*height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 uv = new Vector2((float) x/width, (float) y/height);
                    vertices[x + y*width] = new VertexPositionNormalTexture(new Vector3(x, heightData[x, y], y),
                        Vector3.Up, uv);
                }
            }

        }

        private void SetUpIndices()
        {
            int width = heightMap.Width;
            int height = heightMap.Height;

            indices = new int[(width - 1)*(height - 1)*6];
            int counter = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y*width;
                    int lowerRight = (x + 1) + y*width;
                    int topLeft = x + (y + 1)*width;
                    int topRight = (x + 1) + (y + 1)*width;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;

                }
            }

            indexBuffer = Buffer.New(this.core.GraphicsDevice, indices, BufferFlags.IndexBuffer);


        }

        private void GenNormals()
        {
            for (int i = 0; i < nIndices; i += 3)
            {

                // Find the position of each corner of the triangle
                Vector3 v1 = vertices[indices[i]].Position;
                Vector3 v2 = vertices[indices[i + 1]].Position;
                Vector3 v3 = vertices[indices[i + 2]].Position;

                // Cross the vectors between the corners to get the normal
                Vector3 normal = Vector3.Cross(v1 - v2, v1 - v3);
                normal.Normalize();

                // Add the influence of the normal to each vertex in the
                // triangle
                vertices[indices[i]].Normal += normal;
                vertices[indices[i + 1]].Normal += normal;
                vertices[indices[i + 2]].Normal += normal;
            }

            // Average the influences of the triangles touching each
            // vertex
            for (int i = 0; i < vertices.Length; i++){
                vertices[i].Normal.Normalize();
        }

        this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, vertices);

    }



public void Draw(Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.Identity);
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);
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

        #endregion
    }
}
