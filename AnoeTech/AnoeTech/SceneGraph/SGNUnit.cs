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
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace AnoeTech
{


    /* A SceneGraphUnit is a parent class for all player controllable units. It is to be inherited from as a template 
     * for all units that will require
     * 
     *      Physics Calculation
     *      Mountable Camera
     *      Animation
     *      Custom Control Set
     *      Typical Game Variables such as Life, Ammo, ect.
     */

    public class SGNUnit : SceneGraphNode
    {
        public       Model[] models;
        public RigidBody[] rigidBodies;
        public Texture2D[] textures;
        public HUDHealthBar healthBar;
        public bool IsSelectable = false, IsSelected = false;
        public int healthBarID;

        public virtual void Select()
        {
            IsSelected = true;
            healthBar = new HUDHealthBar();
            healthBar.Initiate(new Vector2(GraphicsEngine.viewport.Project(_position, GraphicsEngine.projectionMatrix, GraphicsEngine.camera.ViewMatrix, Matrix.Identity).X, GraphicsEngine.viewport.Project(_position, GraphicsEngine.projectionMatrix, GraphicsEngine.camera.ViewMatrix, Matrix.Identity).Y), new Vector2(20.0f, 5.0f));
            healthBar.SetTarget(this);
            healthBarID = GraphicsEngine.hud.Add(healthBar);
        }

        public virtual void Deselect()
        {
            IsSelected = false;
            GraphicsEngine.hud.Remove(healthBarID);
            healthBarID = -1;
        }

        public override void Initiate(InitPacket initPacket)
        {


        }

        public override void Draw(Object obj)
        {

        }
        public override void Destroy()
        {
        }
        public override void Update()
        {

        }
    }
}
