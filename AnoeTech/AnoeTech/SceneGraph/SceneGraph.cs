using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints.SingleBody;
using Jitter.Dynamics.Joints;
using Jitter.LinearMath;


/* The SceneGraph keeps account of all objects used in a scene and provides an interface to the objects for other classes.
 * All elements in the game are attached to SceneGraphNodes. There are many types of nodes depending on their function in the world.
 * The SceneGraph also handles the physics engine.
 */
namespace AnoeTech
{

    public class SceneGraph
    {

        #region Variables
        public enum NodeTypes { LIGHT, TERRAIN, UNIT };
        public enum UnitTypes { INFANTRY, VEHICLE, AIRCRAFT, WATERCRAFT };
        public SGNFreeRoam freeRoamNode;
        public SGNCommander commanderNode;
        public SceneGraphNode[] nodes;

        public SceneGraphNode rootNode;

        private List<SGNHeightmap> _terrainNodes = new List<SGNHeightmap>();
        public int terrainStep, terrainScale, terrainWidthTotal, terrainHeightTotal, smallestX, biggestX, smallestZ, biggestZ;
        public RenderTarget2D miniMap;
        
        private SGNSkyDome _skyDome;
        public SGNSkyDome SkyDome{ get{ return _skyDome;}}

        public double _timeOfDay = 900.0f;
        InitPacket packet;
        Model[] _preLoadedModels;

        CollisionSystem collisionSystem;
        public World world;

        #endregion

        #region Public Functions

        private void CollisionDetected ( RigidBody body1 , RigidBody body2 ,
                          JVector point1 , JVector point2 , JVector normal , 
                          float penetration )
        {
            //_engine.VM.Console.Output(" Collision detected !");
        }

        public void RectangleSelect(Viewport viewport, Matrix projection, Matrix view, Rectangle selectionRect)
        {
            foreach (SGNUnit unit in nodes)
            {
                // Getting the 2D position of the object
                Vector3 screenPos = GraphicsEngine.viewport.Project(unit.Position, projection, view, Matrix.Identity);

                // screenPos is window relative, we change it to be viewport relative
                screenPos.X -= viewport.X;
                screenPos.Y -= viewport.Y;

                if (selectionRect.Contains((int)screenPos.X, (int)screenPos.Y))
                {
                    // Add object to selected objects list
                    if (unit.IsSelectable) unit.Select();
                }
            }
        }

        public void DeselectAll()
        {
            foreach (SGNUnit unit in nodes)
            {
                if (unit.IsSelected) unit.Deselect();
            }
            
        }

        public List<SGNUnit> GetSelectedUnits()
        {
            List<SGNUnit> selectedUnits = new List<SGNUnit>();
            foreach( SGNUnit unit in nodes )
                if(unit.IsSelected)
                    selectedUnits.Add(unit);
            return selectedUnits;
        }

        private void CreateCommanderNode()
        {
            // Create a Commander Node
            commanderNode = new SGNCommander();
            packet.position = new Vector3(300, 450, -1000);
            freeRoamNode.cameraPositions = new Vector3[1] { new Vector3(0, 0, 0) };
            commanderNode.Initiate(packet);
        }

        private void CreateFreeRoamNode()
        {
            packet = new InitPacket();
            // Create a Free Roam Node
            freeRoamNode = new SGNFreeRoam();
            freeRoamNode.Position = new Vector3(10, 200, -10);
            freeRoamNode.updownRot = 0;
            freeRoamNode.leftrightRot = 0;
            freeRoamNode.cameraPositions = new Vector3[1] { new Vector3(0, 0, 0) };
            freeRoamNode.moveSpeed = 20.0f;
        }

        private void RegisterVmFunctions()
        {
            GameState.virtualMachine.RegisterFunction("GET_ANGLE_BETWEEN", 2, new Type[2] { typeof(Vector3), typeof(Vector3) }, arg => this.GetAngleBetween((Vector3)arg[0], (Vector3)arg[1]));
        }

        public void Initiate()
        {
            collisionSystem = new CollisionSystemSAP();     // Create a physics system instance
            world = new World(collisionSystem);             // Create a world in the physics engine

            LoadLevel();

            CreateFreeRoamNode();
            CreateCommanderNode();            
            
            BuildMiniMap();

            RegisterVmFunctions();

            //Load a tank
            packet = new InitPacket();
            packet.position = new Vector3(800, 200, -1000);
            packet.modelFiles[0] = "Models/tank";
            Shape shape = new BoxShape(4, 4, 4);
            RigidBody body = new RigidBody(shape);
            body.Position = new JVector(packet.position.X, packet.position.Y, packet.position.Z);
            packet.rigidBodies = new RigidBody[1] { body };
            packet.textures = new string[] { "Textures/digcamo", "Textures/steel" };
            packet.cameraPositions = new Vector3[] { new Vector3(0, 5.5f, 0.0f) };
            packet.mass = 5000;
            

            _preLoadedModels = new Model[2];
            _preLoadedModels[0] = GameState.contentManager.Load<Model>(packet.modelFiles[0]);

            world.Gravity = new JVector(0, -100, 0);
            collisionSystem.Detect(true);     
            
        }

        private void LoadLevel()
        {
            Texture2D heightmap = GameState.contentManager.Load<Texture2D>("HeightMaps/heightmapBig");
            terrainStep = 15;
            terrainScale = 1;
            _skyDome = new SGNSkyDome();
            _terrainNodes = TerrainNodeBuilder.BuildHeightmaps(heightmap, terrainStep, terrainScale);
            foreach (SGNHeightmap hm in _terrainNodes)
            {
                hm.Initiate(world);
            }

            terrainWidthTotal = heightmap.Width * terrainStep;
            terrainHeightTotal = heightmap.Height * terrainStep;
        }

        public void BuildMiniMap()
        {

            GraphicsEngine.graphicsDevice.SetRenderTarget(miniMap);
            GraphicsEngine.graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            GraphicsEngine.graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            Vector3 oldpos = freeRoamNode.Position;
            Matrix oldProj = GraphicsEngine.projectionMatrix;
            freeRoamNode.Position = new Vector3(260 * terrainScale, 470 * terrainScale, -360 * terrainScale);
            GraphicsEngine.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsEngine.viewport.AspectRatio, 1.0f, 10000.0f);
            GraphicsEngine.camera.LockToNode(freeRoamNode, false);
            GraphicsEngine.camera.Update();
            foreach (SGNHeightmap hm in _terrainNodes)
            {
                hm.Draw(null);
            }

            GraphicsEngine.graphicsDevice.SetRenderTarget(null);

            freeRoamNode.Position = oldpos;
            
            GraphicsEngine.projectionMatrix = oldProj;

            miniMap = new RenderTarget2D(
                GraphicsEngine.graphicsDevice,
                GraphicsEngine.graphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsEngine.graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsEngine.graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth16);
        }
    
        /// <summary>
        /// Add a node to the scenegraph
        /// </summary>
        /// <param name="nodeType">Node type. Use SceneGraph.NodeTypes enum</param>
        /// <param name="node">The node to be added to the collection</param>
        /// <returns>An interger ID of the object</returns>
        public int AddNode( int nodeType, SceneGraphNode node )
        {
            if (nodes == null)
                nodes = new SGNUnit[1];
            nodes[0] = new SGNUnitTank();
            nodes[0].Initiate(packet);

            return 0;
        }

        public int AddChildNode(int parentNode)
        {
            return 0;
        }

        public void RemoveNode(int ID)
        {
        }

        public SceneGraphNode GetUnitNode(int ID)
        {
            return nodes[ID];
        }


        public void Update()
        {
            _timeOfDay += 0.01f;
            if (_timeOfDay >= 1440)
                _timeOfDay = 0.01f;

            _skyDome.Update(_timeOfDay);

            if(InputHandler.InputMode == InputHandler.InputModes.FREE_ROAM)
                freeRoamNode.Update();
            if (InputHandler.InputMode == InputHandler.InputModes.RTS)
                commanderNode.Update();

            foreach (SGNHeightmap hm in _terrainNodes)
                hm.Update(_skyDome);

            if (nodes != null)
                foreach (SGNUnit unit in nodes)
                    unit.Update();

           
        }

        public void Draw()
        {
            BasicEffect lineEffect = new BasicEffect(GraphicsEngine.graphicsDevice);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;

            if(InputHandler.InputMode != InputHandler.InputModes.RTS)
                _skyDome.Draw(_timeOfDay);

            foreach (SGNHeightmap hm in _terrainNodes)
            {
                if (GraphicsEngine.camera._frustum.Intersects(hm.boundingBox))
                    hm.Draw(null);
                //Debug.DrawBoundingBox(Debug.CreateBoundingBoxBuffers(hm.boundingBox, GraphicsEngine.graphicsDevice), lineEffect, GraphicsEngine.graphicsDevice, GraphicsEngine.camera.ViewMatrix, GraphicsEngine.projectionMatrix);
            }
            if (nodes != null)
                foreach (SGNUnit unit in nodes)
                    unit.Draw(null);
        }

        #endregion

        public int AddUnit(UnitTypes unitType, Vector2 position, Script ai)
        {
            return 1;
        }

        public float GetAngleBetween(Vector3 position1, Vector3 position2)
        {
            if (position1.X == position2.X && position1.Z == position2.Z)
                return 0;
            else
                return (float)Math.Atan2(position2.Z - position1.Z, position2.X - position1.X);
        }

    }
}
