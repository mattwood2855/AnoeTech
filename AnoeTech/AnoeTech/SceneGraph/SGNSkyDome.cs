using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace AnoeTech
{
    public class SGNSkyDome : SceneGraphNode
    {
        private Model  _skyDome;
        Texture2D      _cloudMap;
        Texture2D      _starMap;

        private float   _ambientLighting;
        public float    AmbientLight { get { return _ambientLighting; } }
        private Vector3 _lightDirection;
        public Vector3  LightDirection { get { return _lightDirection; } }
        public float[] lightColor = new float[4] { 1, 1, 1, 1 };
        Color[] lightGradient = new Color[1440];
        float transperency;

        public SGNSkyDome()
        {
            _skyDome = GameState.contentManager.Load<Model>("SkyMaps/dome");
            _cloudMap = GameState.contentManager.Load<Texture2D>("SkyMaps/heavyclouds");
            _starMap = GameState.contentManager.Load<Texture2D>("SkyMaps/starmap");
            Effect effect = GameState.contentManager.Load<Effect>("effects");
            _skyDome.Meshes[0].MeshParts[0].Effect = effect.Clone();
        }

        public void Initiate( Object obj )
        {
            
            
        }

        public override void Draw( Object timeOfDay )
        {


            GraphicsEngine.graphicsDevice.DepthStencilState = DepthStencilState.None;

            Matrix[] modelTransforms = new Matrix[_skyDome.Bones.Count];
            _skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(0, -0.3f, 0) * Matrix.CreateScale(100) * Matrix.CreateTranslation(GraphicsEngine.camera.Position);
            foreach (ModelMesh mesh in _skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(GraphicsEngine.camera.ViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(GraphicsEngine.projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(_starMap);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xAmbient"].SetValue(_ambientLighting);
                    currentEffect.Parameters["xTransperency"].SetValue(transperency);
                }
                mesh.Draw();
            }
            foreach (ModelMesh mesh in _skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(GraphicsEngine.camera.ViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(GraphicsEngine.projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(_cloudMap);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xAmbient"].SetValue(_ambientLighting);
                    currentEffect.Parameters["xTransperency"].SetValue(1-transperency);
                }
                mesh.Draw();
            }





            GraphicsEngine.graphicsDevice.BlendState = BlendState.Opaque;
            GraphicsEngine.graphicsDevice.DepthStencilState = DepthStencilState.Default;

        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

         
        ///<summary>
        ///Updates the SkyDome
        ///</summary>
        public void Update(Object timeOfDay)
        {
            //int tod = (int)Math.Floor((double)timeOfDay);
            transperency = (float)((((double)timeOfDay / 60 - 5) * ((double)timeOfDay / 60 - 17))) / 60;
            // Ambient light is calculated at sin of the time of day with a max of pi to avoid negative
            _ambientLighting = (float)-(Math.Cos(MathHelper.Pi * ((double)timeOfDay / 720))) + 0.5f;
            if (_ambientLighting < 0.55f)
                _ambientLighting = 0.55f;
            if (_ambientLighting > 0.9f)
                _ambientLighting = 0.9f;
            _lightDirection.Y = (float)Math.Cos(MathHelper.Pi * ((double)timeOfDay / 720));
            _lightDirection.Z = (float)Math.Sin(MathHelper.Pi * ((double)timeOfDay / 720));
        }
    }
}
