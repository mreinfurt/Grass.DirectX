using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Wheat.Manager;

namespace Wheat.Environment
{
    class Monkey
    {
        #region Fields

        Model _mesh;
        Effect _effect;
        Texture2D _texture;

        EffectParameter _projectionParameter;
        EffectParameter _viewParameter;
        EffectParameter _worldParameter;
        EffectParameter _ambientIntensityParameter;
        EffectParameter _ambientColorParameter;
        EffectParameter _diffuseIntensityParameter;
        EffectParameter _diffuseColorParameter;

        #endregion

        #region Public Methods

        public Monkey(GraphicsDevice graphicsDevice)
        {
        }

        public void LoadContent(ContentManager content)
        {
            _mesh = content.Load<Model>("Models/Object");
            _texture = content.Load<Texture2D>("Models/model_diff");
            _effect = content.Load<Effect>("Effects/Object");

            // Bind the parameters with the shader.
            _worldParameter = _effect.Parameters["World"];
            _viewParameter = _effect.Parameters["View"];
            _projectionParameter = _effect.Parameters["Projection"];

            _ambientColorParameter = _effect.Parameters["AmbientColor"];
            _ambientIntensityParameter = _effect.Parameters["AmbientIntensity"];
            _diffuseColorParameter = _effect.Parameters["DiffuseColor"];
            _diffuseIntensityParameter = _effect.Parameters["DiffuseIntensity"];
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            // Prepare shader
            _projectionParameter.SetValue(camera.ProjectionMatrix);
            _viewParameter.SetValue(camera.ViewMatrix);
            _worldParameter.SetValue(camera.WorldMatrix);
            
            _ambientIntensityParameter.SetValue(0.3f);
            _ambientColorParameter.SetValue(new Vector3(1, 1, 1));

            _diffuseIntensityParameter.SetValue(1.0f);
            _diffuseColorParameter.SetValue(new Vector3(0, 1, 1));

            //_effect.Parameters["cTexture"].SetValue(_texture);

            // Draw mesh
            ModelMesh mesh = _mesh.Meshes[0];
            ModelMeshPart meshPart = mesh.MeshParts[0];

            graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
            graphicsDevice.Indices = meshPart.IndexBuffer;
            _effect.CurrentTechnique = _effect.Techniques["Technique1"];
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
            }
        }

        #endregion
    }
}
