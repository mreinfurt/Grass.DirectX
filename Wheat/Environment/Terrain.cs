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
        private VertexPositionNormalTexture[] terrainVertices;

        private Texture2D heightMap;
        private float[,] heightData;
        private int[] indices;
        private Buffer indexBuffer;

        private VertexInputLayout vertexInputLayout;


        #endregion

        #region Public Methods

        public Terrain(GameCore core)
        {
            this.core = core;
            this.effect = this.core.ContentManager.Load<Effect>("Effects/Terrain");
            this.plane = GeometricPrimitive.Plane.New(this.core.GraphicsDevice, 50, 50);
            this.texture = this.core.ContentManager.Load<Texture2D>("Textures/planeGrass");
            this.heightMap = this.core.ContentManager.Load<Texture2D>("Textures/heightMap");

            LoadHeightData(this.heightMap);
            SetUpVertices();
            SetUpIndices();

           this.vertexBuffer = Buffer.Vertex.New(this.core.GraphicsDevice, this.terrainVertices);
           this.vertexInputLayout = VertexInputLayout.FromBuffer(0, this.vertexBuffer);
       }

        private void LoadHeightData(Texture2D heightMap)
        {
            int width = heightMap.Width;
            int height = heightMap.Height;

            Image image = heightMap.GetDataAsImage();
            heightData = new float[width,height];


            for (int x = 0; x < width ; x++)
            {
                for (int y = 0; y < height ; y++)
                {
                    PixelData.R8G8B8A8 pixel = image.PixelBuffer[0].GetPixel<PixelData.R8G8B8A8>(x, y);
                    heightData[x, y] = pixel.R / 2f;
                }
            }

        }

        private void SetUpVertices()
        {
            int width = heightMap.Width;
            int height = heightMap.Height;
            terrainVertices = new VertexPositionNormalTexture[width * height];
            for (int x = 0; x < width ; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    terrainVertices[x + y * width].Position = new Vector3(x, heightData[x, y], -y);
                    //terrainVertices[x + y * width].Color = new SharpDX.Color(1.0f, 1.0f, 1.0f, 1.0f); ;
                }
            }

        }

        private void SetUpIndices()
        {
            int width = heightMap.Width;
            int height = heightMap.Height;

            indices = new int[(width - 1) * (height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y * width;
                    int lowerRight = (x + 1) + y * width;
                    int topLeft = x + (y + 1) * width;
                    int topRight = (x + 1) + (y + 1) * width;

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


        public void Draw(Camera camera)
        {
            this.effect.Parameters["World"].SetValue(Matrix.Identity);
            this.effect.Parameters["View"].SetValue(camera.View);
            this.effect.Parameters["Projection"].SetValue(camera.Projection);
            this.effect.Parameters["Texture"].SetResource(this.texture);
            this.effect.Parameters["LightPosition"].SetValue(this.core.ShadowCamera.Position);

            this.core.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            this.core.GraphicsDevice.SetIndexBuffer(this.indexBuffer, true, 0);
            this.core.GraphicsDevice.SetVertexInputLayout(this.vertexInputLayout);
 

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
               // this.core.GraphicsDevice.Draw(PrimitiveType.TriangleStrip, 4);
                this.core.GraphicsDevice.DrawIndexed(PrimitiveType.TriangleList, indexBuffer.ElementCount, 0, 0);
            }
        }

        #endregion
    }
}
