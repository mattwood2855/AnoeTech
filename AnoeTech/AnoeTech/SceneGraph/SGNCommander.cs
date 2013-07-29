using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace AnoeTech
{
    public class SGNCommander : SceneGraphNode
    {
        public override void Initiate(InitPacket initPacket)
        {
            _position = initPacket.position;
            cameraPositions = initPacket.cameraPositions;
            updownRot = 3 * MathHelper.PiOver2;
        }

        public override void Update()
        {
            
            if ( _position.X - (GraphicsEngine.camera.FarFustrum.Width/2) < 0 )
                if (_localVelocities.X < 0)
                    _localVelocities.X = 0;
            if (_position.X + (GraphicsEngine.camera.FarFustrum.Width / 2) > GameState.anoetech.sceneGraph.terrainWidthTotal)
                if (_localVelocities.X > 0)
                    _localVelocities.X = 0;
            if (_position.Z - (GraphicsEngine.camera.FarFustrum.Height / 2) < -GameState.anoetech.sceneGraph.terrainHeightTotal)
                if (_localVelocities.Z < 0)
                    _localVelocities.Z = 0;
            if (_position.Z + (GraphicsEngine.camera.FarFustrum.Height / 2) > 0)
                if (_localVelocities.Z > 0)
                    _localVelocities.Z = 0;
            if (_position.Y > 2500)
                if (_localVelocities.Y > 0)
                    _localVelocities.Y = 0;
            if (_position.Y < 250)
                if (_localVelocities.Y < 0)
                    _localVelocities.Y = 0;

            _position.X += _localVelocities.X;
            _position.Y += _localVelocities.Y;
            _position.Z += _localVelocities.Z;
            localVelocities *= _friction;
        }
    }
}
