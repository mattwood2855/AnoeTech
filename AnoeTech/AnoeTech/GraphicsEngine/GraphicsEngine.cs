using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnoeTech
{
    public struct VertexMultitextured
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 TextureCoordinate;
        public Vector4 TexWeights;

        public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);
        public static VertexElement[] VertexElements = new VertexElement[]
     {
         new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
         new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
         new VertexElement( sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0 ),
         new VertexElement( sizeof(float) * 10, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1 ),
     };
    }

    static class GraphicsEngine
    {
        //GraphicsEngine
        public static                Camera camera;
        public static GraphicsDeviceManager graphicsDeviceManager;
        public static        GraphicsDevice graphicsDevice;
        public static                   HUD hud;        
        public static                Matrix projectionMatrix; 
        public static           SpriteBatch spriteBatch;
        public static            SpriteFont font;
        public static              Viewport viewport;

        public static void Initialize()
        {
            graphicsDevice = graphicsDeviceManager.GraphicsDevice;      // Create a global reference to the Graphics Device
            viewport = graphicsDevice.Viewport;                         // Create a global reference to the Viewport
            spriteBatch = new SpriteBatch(graphicsDevice);
            font = GameState.contentManager.Load<SpriteFont>("Fonts/Verdana");
            ChangeResolution(800, 450, false);
            GraphicsEngine.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsEngine.viewport.AspectRatio, 0.5f, 5000.0f);
            camera = new Camera(); camera.Initiate();
            GraphicsEngine.hud = new HUD(); GraphicsEngine.hud.Initiate();
        }

        /// <summary>
        /// Change the resolution and set whether or not the application runs in FullScreen mode
        /// </summary>
        /// <param name="x">New X resolution</param>
        /// <param name="y">New Y resolution</param>
        /// <param name="fullscreen">Fullscreen flag (Default = true)</param>
        public static void ChangeResolution(int x, int y, bool fullscreen = true)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = x;       // Set the Resolution
            graphicsDeviceManager.PreferredBackBufferHeight = y;
            graphicsDeviceManager.IsFullScreen = fullscreen;                               // Set fullscreen flag
            graphicsDeviceManager.ApplyChanges();                                     // Apply the changes
        }
    }
}
