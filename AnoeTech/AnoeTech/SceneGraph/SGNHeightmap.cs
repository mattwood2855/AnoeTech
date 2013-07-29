using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace AnoeTech
{
    class SGNHeightmap : SceneGraphNode
    {
        private float[,] _heightData;
        private int _drawDistance = 9;
        public int terrainWidth, terrainHeight;
        public float terrainStep, terrainScale;
        
        public VertexBuffer terrainVertexBuffer, DrawableTerrainVertexBuffer;
        public IndexBuffer terrainIndexBuffer, DrawableIndexBuffer;
        
        public VertexMultitextured[] vertices, drawableVertices;
        public int[] indices;

        private float lightIntensity = 1.0f;

        Texture2D grassTexture, snowTexture, rockTexture, sandTexture;

        Effect effect;

        const float waterHeight = 5.0f;
       
        public Vector3 lightDirection;
        public float ambientLighting = 0.0f;
        float[] color = new float[4] { 1, 1, 1, 1 };


        Vector3 windDirection = new Vector3(-1, 0, 0);

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SGNHeightmap()
        {
        }

        /// <summary>
        /// Creates a heightmap from a gray-scale image
        /// </summary>
        /// <param name="position">Starting position of the heightmap</param>
        /// <param name="heightMapData">The texture to use for height data</param>
        public SGNHeightmap(Vector3 position, Texture2D heightMapData, float step, float scale)
        {
            _position = position;                                       // Set the heightmap's starting position
            _heightData = TerrainNodeBuilder.CreateHeightMatrix(heightMapData, step, scale);
            terrainStep = step;
            terrainScale = scale;
            effect = GameState.contentManager.Load<Effect>("effects");  // Load an effect shader file for the heightmap
        }

        public SGNHeightmap(Vector3 position, float[,] heightMapData, float step, float scale)
        {
            _position = position;                                       // Set the heightmap's starting position
            _heightData = heightMapData;                             // Create a local reference to the height data
            terrainStep = step;
            terrainScale = scale;
            effect = GameState.contentManager.Load<Effect>("effects");  // Load an effect shader file for the heightmap
        }

        #endregion



        public void Initiate( World world )
        {
            // TODO - texture manager
            try
            {
                grassTexture = GameState.contentManager.Load<Texture2D>("Textures/grass");
                sandTexture = GameState.contentManager.Load<Texture2D>("Textures/sand");
                rockTexture = GameState.contentManager.Load<Texture2D>("Textures/rock");
                snowTexture = GameState.contentManager.Load<Texture2D>("Textures/snow");
            }
            catch(Exception e){}

            lightDirection = new Vector3(0.4f, -0.47f, -1.0f);
            ambientLighting = 1.0f;

            terrainWidth = 128;
            terrainHeight = 128;            
            
            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
            CopyToTerrainBuffers(vertices, indices, GraphicsEngine.graphicsDevice);

            List<JVector> verts = new List<JVector>();
            for (int tmp = 0; tmp < vertices.Length; tmp++)
            {
                JVector tmp2 = new JVector();                
                tmp2.X = vertices[tmp].Position.X;
                tmp2.Y = vertices[tmp].Position.Y;
                tmp2.Z = vertices[tmp].Position.Z;
                verts.Add(tmp2);
            }
            List<TriangleVertexIndices> inds = new List<TriangleVertexIndices>();
            for (int tmp = 0; tmp < indices.Length; tmp += 3)
            {
                TriangleVertexIndices tmp2 = new TriangleVertexIndices();
                tmp2.I0 = indices[tmp];
                tmp2.I1 = indices[tmp+1];
                tmp2.I2 = indices[tmp+2];
                inds.Add(tmp2);
            }
            Octree octree = new Octree(verts, inds);
            octree.BuildOctree();
            
            // Pass it to a new instance of the triangleMeshShape
            TriangleMeshShape triangleMeshShape = new TriangleMeshShape(octree);
            RigidBody triangleBody = new RigidBody(triangleMeshShape);
            triangleBody.Tag = Color.LightGray;
            triangleBody.Position = new JVector(0, 0, 0);
            // Add the mesh to the world .
            triangleBody.IsStatic = true;
            world.AddBody(triangleBody);
            BuildDrawableHeightMap();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        private void SetUpVertices()
        {
            
            vertices = new VertexMultitextured[terrainWidth * terrainHeight];
            for (int y = 0; y < terrainHeight; y++)
            for (int x = 0; x < terrainWidth; x++)
                
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x*terrainStep, _heightData[x, y], -y*terrainStep);
                    vertices[x + y * terrainWidth].TextureCoordinate.X = (float)x * terrainStep / 256.0f;
                    vertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y * terrainStep / 256.0f;
                    
                    float points = from x in Enumerable.Range(0, this.Size.Width - 1)
                                 from y in Enumerable.Range(0, this.Size.Width - 1)
                                 where this.Map[x, y]
                                 select new { X = x, Y = y };
                    float max = points.Max(p => p);
                    vertices[x + y * terrainWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(_heightData[x, y] - 0) / (300 / 5), 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(_heightData[x, y] - 300 * 0.2f) / (300 / 4), 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(_heightData[x, y] - 300 * 0.6f) / (300 / 5), 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(_heightData[x, y] - 300) / (300 / 4), 0, 1);

                    float total = vertices[x + y * terrainWidth].TexWeights.X;
                    total += vertices[x + y * terrainWidth].TexWeights.Y;
                    total += vertices[x + y * terrainWidth].TexWeights.Z;
                    total += vertices[x + y * terrainWidth].TexWeights.W;

                    vertices[x + y * terrainWidth].TexWeights.X /= total;
                    vertices[x + y * terrainWidth].TexWeights.Y /= total;
                    vertices[x + y * terrainWidth].TexWeights.Z /= total;
                    vertices[x + y * terrainWidth].TexWeights.W /= total;
                }
        }

        private void SetUpIndices()
        {
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
        }

        private void CalculateNormals()
         {
             for (int i = 0; i < vertices.Length; i++)
                 vertices[i].Normal = new Vector3(0, 0, 0);
             for (int i = 0; i < indices.Length / 3; i++)
             {
                 int index1 = indices[i * 3];
                 int index2 = indices[i * 3 + 1];
                 int index3 = indices[i * 3 + 2];

                 Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                 Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                 Vector3 normal = Vector3.Cross(side1, side2);

                 vertices[index1].Normal += normal;
                 vertices[index2].Normal += normal;
                 vertices[index3].Normal += normal;
             }
             for (int i = 0; i < vertices.Length; i++)
                 vertices[i].Normal.Normalize();
         }

        private void CopyToTerrainBuffers(VertexMultitextured[] vertices, int[] indices, GraphicsDevice device)
        {
            VertexDeclaration vertexDeclaration = new VertexDeclaration(VertexMultitextured.VertexElements);


            terrainVertexBuffer = new VertexBuffer(device, vertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices.ToArray());

            terrainIndexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }

        public float GetHeight(Vector3 position)
        {
            float[] temp = {0,0,0,0};
 
            float tX = (float)Math.Floor(position.X / terrainStep);
            float tZ = (float)Math.Floor(-position.Z / terrainStep);

            temp[0] = Height((int)tX, (int)tZ);
            temp[1] = Height((int)tX + 1, (int)tZ);
            temp[2] = Height((int)tX + 1, (int)tZ + 1);
            temp[3] = Height((int)tX, (int)tZ + 1);    
    //now for the interpolation    
            float tx = position.X / terrainStep - tX;
            float tz = -position.Z / terrainStep - tZ;    
            float txtz=tx*tz;   
    //bilinear interpolation to get the current height above the surface    
            return temp[0]*(1-tz-tx+txtz)+temp[1]*(tx-txtz)+temp[2]*txtz+temp[3]*(tz-txtz);

        }

        public float Height(int x, int z)			// This Returns The Height From A Height Map Index
        {
	        return vertices[x + z * terrainHeight].Position.Y;			// Index Into Our Height Array And Return The Height}
        }

        public void BuildDrawableHeightMap()
        {
            drawableVertices = new VertexMultitextured[_drawDistance * _drawDistance];
            Vector2 cameraPosition = new Vector2(GraphicsEngine.camera.Position.X / terrainStep,
                                                 -GraphicsEngine.camera.Position.Z / terrainStep);
            int cameraCell = (int)(Math.Floor(cameraPosition.X) * terrainHeight + Math.Floor(cameraPosition.Y));
            int leftright = (int)Math.Floor(_drawDistance / 2.0f);
            int counter = 0;

            while (cameraCell - leftrightRot < 0)
                cameraCell += terrainHeight;
            //while (cameraCell - leftrightRot < 0)
            //    cameraCell += terrainHeight;
            //while (cameraCell - leftrightRot < 0)
            //    cameraCell += terrainHeight;
            //while (cameraCell - leftrightRot < 0)
            //    cameraCell += terrainHeight;

            for (int x = -leftright; x < leftright; x++)
                for (int y = -leftright; y < leftright; y++)
                {
                    //drawableVertices[counter] = vertices[cameraCell + y + x * terrainHeight];
                    counter++;
                }
        }

        public void Update(SGNSkyDome sky)
        {
            ambientLighting = GameState.anoetech.sceneGraph.SkyDome.AmbientLight;
            lightDirection = GameState.anoetech.sceneGraph.SkyDome.LightDirection;
            color = GameState.anoetech.sceneGraph.SkyDome.lightColor;

            BuildDrawableHeightMap();
        }

        public override void Draw( Object obj )
        {

            effect.CurrentTechnique = effect.Techniques["MultiTextured"];
            effect.Parameters["xTexture0"].SetValue(sandTexture);
            effect.Parameters["xTexture1"].SetValue(grassTexture);
            effect.Parameters["xTexture2"].SetValue(rockTexture);
            effect.Parameters["xTexture3"].SetValue(snowTexture);

            effect.Parameters["xDiffuseIntensity"].SetValue(1.0f);
            effect.Parameters["xDiffuseColor"].SetValue(color);

            Matrix worldMatrix = Matrix.CreateTranslation( _position );
            effect.Parameters["xView"].SetValue(GraphicsEngine.camera.ViewMatrix);
            effect.Parameters["xProjection"].SetValue(GraphicsEngine.projectionMatrix);
            effect.Parameters["xWorld"].SetValue(worldMatrix);
             
            
            lightDirection.Normalize();
            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xLightIntensity"].SetValue(ambientLighting);
            effect.Parameters["xAmbient"].SetValue(ambientLighting);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsEngine.graphicsDevice.Indices = terrainIndexBuffer;
                GraphicsEngine.graphicsDevice.SetVertexBuffer(terrainVertexBuffer);

                GraphicsEngine.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            }
        }

    }
}
