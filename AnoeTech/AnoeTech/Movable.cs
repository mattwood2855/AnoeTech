using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
    public class Movable
    {
        
        public float  leftrightRot = MathHelper.PiOver2;
        public float  updownRot = -MathHelper.Pi / 10.0f;
        public float rotationSpeed = 0.3f;
        public float moveSpeed = 10.0f;
        protected float _friction = 0.9f;
        public Vector3 _position, _look, _up, _localVelocities, _axisVelocities;
        public float currentSpeed;

        protected bool _moving = false;

        public virtual void Update()
        {
            _position.X += _localVelocities.X;
            _position.Y += _localVelocities.Y;
            _position.Z += _localVelocities.Z;

            _position.X += _axisVelocities.X;
            _position.Y += _axisVelocities.Y;
            _position.Z += _axisVelocities.Z;

            if (_localVelocities.X + _localVelocities.Y + _localVelocities.Z > 0)
            {
                _moving = true;
                currentSpeed = _localVelocities.Length();
            }
            else
                _moving = false;

            _localVelocities *= _friction;
        }

        public void AddToPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            _position += 1 * rotatedVector;

        }

        public void AddToVelocity(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            _localVelocities += rotatedVector;
        }

        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public void SetY(float y)
        {
            _position.Y = y;
        }

        public Vector3 look
        {
            get
            {
                return _look;
            }
            set
            {
                _look = value;
            }
        }

        public Vector3 up
        {
            get
            {
                return _up;
            }
            set
            {
                _up = value;
            }
        }

        public Vector3 localVelocities
        {
            get
            {
                return _localVelocities;
            }
            set
            {
                _localVelocities = value;
            }
        }
        
        public Vector3 axisVelocities
        {
            get
            {
                return _axisVelocities;
            }
            set
            {
                _axisVelocities = value;
            }
        }
    }
}
