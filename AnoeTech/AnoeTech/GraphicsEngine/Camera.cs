using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnoeTech
{
    public class Camera : Movable
    {
        private         Matrix _viewMatrix;
        public BoundingFrustum _frustum;
        private SceneGraphNode _target;
        private Plane _frustumFar;
        private bool _transitioning;
        float _originalTargetDistance;

        public SceneGraphNode Target { get { return _target; } }
        public     Matrix ViewMatrix { get { return _viewMatrix; } }
        public Plane FarFustrum { get { return _frustumFar; } }

        public void Initiate()
        {
            _position = new Vector3(10, 10, -10);
            _transitioning = false;
        }

        private void GenerateQuadraticToTarget()
        {
            // To create a parabola track we must first get the lengths of a right triangle where a is the height, 
            // b is the base and c is the hypotenuse(distance between the camera and target). We will only generate b.
            // We can treate this length as the X value in a quadratic function and the height as our Y. This gives
            // us a point on the line and the target is the vertex.
            float b = (float)Math.Sqrt(((_target.Position.X + _target.cameraPositions[0].X) - _position.X) * ((_target.Position.X + _target.cameraPositions[0].X) - _position.X) + ((_target.Position.Z + _target.cameraPositions[0].Z) - _position.Z) * ((_target.Position.Z + _target.cameraPositions[0].Z) - _position.Z));
            float m = ((_target.Position.Z + _target.cameraPositions[0].Z) - _position.Z) / ((_target.Position.X + _target.cameraPositions[0].X) - _position.X);
            float theta = (float)Math.Sin(Math.Abs((_target.Position.X + _target.cameraPositions[0].X) - _position.X) / b);
            // In vertex form y = a(x-h)^2 + k
            // H is the vertex X value and k is the Vertex Y
            float a, h, k, x, y;
            k = (_target.Position.Y + _target.cameraPositions[0].Y);
            h = 0; //Always 0 since this is the vertex of the parabola and 0 is easiest.
            x = -b; // This puts us on the "x" of the point that the camera is at. We use -b to be on the left side of the vertex on an imaginary plane
            y = _position.Y;
            // Now solve for a
            a = (y - k) / ((x - h) * (x - h));
            // Solve for 1 increment closer to get your next desired height
            _position.X = (_target.Position.X + _target.cameraPositions[0].X) - ((x * 0.97f) * (float)Math.Asin(theta));
            _position.Z = (_target.Position.Z + _target.cameraPositions[0].Z) + (((x * 0.97f) * (float)Math.Asin(theta)) * (float)Math.Atan(theta));
            _position.Y = a * ((x * 0.97f) * (x* 0.97f)) + k;
            //if (Math.Abs((_target.Position.Y + _target.cameraPositions[0].Y) - _position.Y) < 4)
            //    _transitioning = false;

            float distance = DistanceTo(Target.Position + Target.cameraPositions[0]);

            


            
            if( Math.Abs(updownRot - _target.updownRot) > .01)
                if (updownRot - _target.updownRot > Math.PI)
                    updownRot = ((Target.updownRot + 2 * (float)Math.PI) - updownRot) * (1 - (distance / _originalTargetDistance));
                else
                    updownRot += (Target.updownRot - updownRot) * (1-(distance / _originalTargetDistance));
            if (updownRot > MathHelper.Pi * 2)
                updownRot -= MathHelper.Pi * 2;
            if (updownRot < 0)
                updownRot += MathHelper.Pi * 2;

            if (Math.Abs(leftrightRot - _target.leftrightRot) > .01)
                if (leftrightRot - _target.leftrightRot > Math.PI)
                    leftrightRot += ((Target.leftrightRot + 2 * (float)Math.PI) - leftrightRot) * (1 - (distance / _originalTargetDistance));
                else
                    leftrightRot += (Target.leftrightRot - leftrightRot) * (1 - (distance / _originalTargetDistance));
            if (leftrightRot > MathHelper.Pi * 2)
                leftrightRot -= MathHelper.Pi * 2;
            if (leftrightRot < 0)
                leftrightRot += MathHelper.Pi * 2;

            if (distance < 2.0f) _transitioning = false;
        }

        public override void Update()
        {
            if (Target != null)
            {
                if (_transitioning)
                {
                    GenerateQuadraticToTarget();
                    Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

                    Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
                    Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
                    Vector3 cameraFinalTarget = _position + cameraRotatedTarget;

                    Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
                    Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);


                    _viewMatrix = Matrix.CreateLookAt(_position, cameraFinalTarget, cameraRotatedUpVector);
                }
                else
                {
                    if (_target.GetType() == typeof(SGNUnitTank))
                    {
                        _position = _target.Position - 10 * _target.localVelocities + _target.cameraPositions[0];
                        GraphicsEngine.FieldOfView(MathHelper.PiOver4 + (Target.currentSpeed * MathHelper.Pi/16), GraphicsEngine.viewport.AspectRatio);
                    }
                    else
                        _position = _target.Position + _target.cameraPositions[0];

                    updownRot = _target.updownRot;
                    //leftrightRot = _target.leftrightRot;
                    Matrix cameraRotation = Matrix.CreateRotationX(_target.updownRot) * Matrix.CreateRotationY(_target.leftrightRot);

                    Vector3 cameraOriginalTarget = new Vector3(0, 0.1f, -1);
                    Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
                    Vector3 cameraFinalTarget = _position + cameraRotatedTarget;

                    Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
                    Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

                    
                    _viewMatrix = Matrix.CreateLookAt(_position, cameraFinalTarget, cameraRotatedUpVector);
                    
                }
                CalculateFustrum();
            }
        }

        public void CalculateFustrum()
        {         
            _frustum = new BoundingFrustum(GraphicsEngine.projectionMatrix * _viewMatrix);
            _frustumFar = _frustum.Far;
        }

        public void LockToNode(SceneGraphNode target, bool smoothTransition)
        {
            _target = target;
            _transitioning = smoothTransition;
            _originalTargetDistance = DistanceTo(target.Position + target.cameraPositions[0]);
            //updownRot = 3 * MathHelper.PiOver2;
        }

        private float DistanceTo(Vector3 position)
        {
            return (float)Math.Sqrt((position.X - _position.X) * (position.X - _position.X) +
                                    (position.Y - _position.Y) * (position.Y - _position.Y) +
                                    (position.Z - _position.Z) * (position.Z - _position.Z));
        }

    }
}
