using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints.SingleBody;
using Jitter.Dynamics.Joints;
using Jitter.LinearMath;

namespace AnoeTech
{
    class SGNUnitTank : SGNUnit
    {

        SoundEffect engineSound;
        public SoundEffectInstance tankEngine;

        public override void Initiate(InitPacket initPacket)
        {
            IsSelectable = true;
            models = new Model[initPacket.modelFiles.Length];
            for (int tmp = 0; tmp < initPacket.modelFiles.Length; tmp++)
            {
                models[tmp] = GameState.contentManager.Load<Model>(initPacket.modelFiles[tmp]);
            }
            rigidBodies = new RigidBody[initPacket.rigidBodies.Length];
            for (int tmp = 0; tmp < rigidBodies.Length; tmp++)
            {
                rigidBodies[tmp] = initPacket.rigidBodies[tmp];
                rigidBodies[tmp].Mass = initPacket.mass;
                GameState.anoetech.sceneGraph.world.AddBody(rigidBodies[tmp]);
            }

            _position = initPacket.position;
            
            cameraPositions = new Vector3[initPacket.cameraPositions.Length];
            for( int tmp = 0; tmp < initPacket.cameraPositions.Length; tmp++)
                cameraPositions[tmp] = initPacket.cameraPositions[tmp];
            

            textures = new Texture2D[initPacket.textures.Length];
            for( int tmp = 0; tmp < initPacket.textures.Length; tmp++ )
            {
                textures[tmp] = GameState.contentManager.Load<Texture2D>(initPacket.textures[tmp]);
            }
            moveSpeed = 3.5f;
            engineSound = GameState.contentManager.Load<SoundEffect>("SFX/dieselEngine");
            tankEngine = engineSound.CreateInstance();
            tankEngine.IsLooped = true;
            
        }

        public override void Draw(Object obj)
        {
            // Draw the model. A model can have multiple meshes, so loop.

            Matrix[] transforms = new Matrix[models[0].Bones.Count];
            models[0].CopyAbsoluteBoneTransformsTo(transforms);
            Matrix wMatrix = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY( leftrightRot + MathHelper.Pi ) * Matrix.CreateTranslation(_position);
            bool tmp = false;
            foreach (ModelMesh mesh in models[0].Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = transforms[mesh.ParentBone.Index] * wMatrix;

                    currentEffect.World = worldMatrix;
                    currentEffect.View = GraphicsEngine.camera.ViewMatrix;
                    currentEffect.Projection = GraphicsEngine.projectionMatrix;
                    currentEffect.TextureEnabled = true;
                    currentEffect.Texture = textures[1];
                    if (tmp)
                        currentEffect.Texture = textures[0];
                    tmp = !tmp;
                }
                mesh.Draw();
            }
        }

        public override void Update()
        {

            if (updownRot > MathHelper.Pi * 2)
                updownRot -= MathHelper.Pi * 2;
            if (updownRot < 0)
                updownRot += MathHelper.Pi * 2;
            if (leftrightRot > MathHelper.Pi * 2)
                leftrightRot -= MathHelper.Pi * 2;
            if (leftrightRot < 0)
                leftrightRot += MathHelper.Pi * 2;
            
            rigidBodies[0].LinearVelocity = (rigidBodies[0].LinearVelocity + ToJVector(localVelocities*30));
            _position.X = rigidBodies[0].Position.X;
            _position.Y = rigidBodies[0].Position.Y;
            _position.Z = rigidBodies[0].Position.Z;
            currentSpeed = rigidBodies[0].LinearVelocity.Length()/20 - 1;
            _localVelocities.X = 0; _localVelocities.Y = 0; _localVelocities.Z = 0;

            if (tankEngine != null)
                if (tankEngine.State == SoundState.Playing)
                {
                    float xd = GraphicsEngine.camera.Position.X - _position.X;
                    float yd = GraphicsEngine.camera.Position.Y - _position.Y;
                    float zd = GraphicsEngine.camera.Position.Z - _position.Z;
                    float distance = (float)Math.Sqrt((double)(xd * xd + yd * yd + zd * zd));
                    distance = 1 - (distance / 250);
                    if (distance < 0) distance = 0;
                    tankEngine.Volume = distance;
                    tankEngine.Pitch = MathHelper.Clamp(currentSpeed, -1, 1);
                }
                else
                    tankEngine.Play();

        }
    }
}
