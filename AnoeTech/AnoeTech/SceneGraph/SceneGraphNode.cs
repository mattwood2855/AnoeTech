using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.DataStructures;
using Jitter.Dynamics.Constraints.SingleBody;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics;
using Jitter.LinearMath;


namespace AnoeTech
{
    public class InitPacket
    {
        public Script                 ai = null;
        public Vector3[] cameraPositions = new Vector3[] { new Vector3(0, 0, 0) };
        public int                health = 0;
        public string[]       modelFiles = new string[] { "null" };
        public string               name = "null";
        public Vector3          position = new Vector3(0, 0, 0);
        public RigidBody[]   rigidBodies = null;
        public string[]      textures = null;
        public int mass = 100;
    }

    public abstract class SceneGraphNode : Movable
    {

        private int id;
        public Vector3[] cameraPositions;
        public List<SceneGraphNode> children;

        public virtual void Initiate(InitPacket initPacket) { }
        public virtual void Draw(Object obj) { }
        public virtual void Destroy() { }

        public JVector ToJVector(Vector3 vector3)
        {
            return new JVector(vector3.X, vector3.Y, vector3.Z);
        }

        public Vector3 ToVector3(JVector jvector)
        {
            return new Vector3(jvector.X, jvector.Y, jvector.Z);
        }
    }

}
