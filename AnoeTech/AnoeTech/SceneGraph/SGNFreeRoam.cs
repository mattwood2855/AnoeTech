using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnoeTech
{
    public class SGNFreeRoam : SceneGraphNode
    {
        public override void Update()
        {
            _position.X += _localVelocities.X;
            _position.Y += _localVelocities.Y;
            _position.Z += _localVelocities.Z;

            if (_localVelocities.X + _localVelocities.Y + _localVelocities.Z > 0)
                _moving = true;
            else
                _moving = false;

            _localVelocities *= _friction;
        }

    }
}
