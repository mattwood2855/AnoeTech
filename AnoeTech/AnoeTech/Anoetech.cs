// May 2012

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace AnoeTech
{    
    public class Anoetech : Microsoft.Xna.Framework.Game
    {

        #region Variables
                        
        public   MainMenu mainMenu;        
        public SceneGraph sceneGraph;    

        public Vector2 resolution = new Vector2(800, 600);
        Matrix SpriteScale;

        // Variables for determining FPS
        float Fps = 0f;
        private const int NumberSamples = 50; //Update fps timer based on this number of samples
        int[] Samples = new int[NumberSamples];
        int CurrentSample = 0;
        int TicksAggregate = 0;
        int SecondSinceStart = 0;

        Texture2D splashLogo;

        Song bgMusic;


        #endregion

        #region Functions

        /// <summary>
        /// Constructor of the Anoetech Game Engine
        /// </summary>
        public Anoetech()
        {
            GameState.anoetech                     = this;                              // Create a global reference to the engine 
            GameState.contentManager               = this.Content;                      // Create a global reference to the Content Manager
            GameState.contentManager.RootDirectory = "Content";                         // Set the root directory
            GraphicsEngine.graphicsDeviceManager   = new GraphicsDeviceManager(this);   // Initiate and create a global reference to a Graphics Device Manager
        }

        /// <summary>
        /// Initialize all variables and parts of the Anoetech Engine
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "AnoeTech Engine";       // Set the Window Title  
            GraphicsEngine.Initialize();            // Set the resolution                                  
            base.Initialize();                      // Call class base function
        }

        protected override void LoadContent()
        {            
            GameState.EngineState = GameState.EngineStates.ENGINE_START;    // Put the engine into start mode

            GraphicsEngine.Initialize();                                    // Initialize the Graphics Engine (get a gfx device and set resolution etc) 

            GameState.virtualMachine = new VirtualMachine();
            GameState.virtualMachine.Initiate();
            
            InputHandler.Initiate();     
            
            mainMenu       = new MainMenu();        mainMenu.Initiate();
            GameState.virtualMachine = new VirtualMachine(); GameState.virtualMachine.Initiate();
            
            RegisterCommandsToVM();

            splashLogo = Content.Load<Texture2D>("Menus/WoodwerksSplash");

            bgMusic = Content.Load<Song>("Music/panteraCowboysFromHell");
            float screenscale = GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height;
            // Create the scale transform for Draw. 
            // Do not scale the sprite depth (Z=1).
            SpriteScale = Matrix.CreateScale(screenscale, screenscale, 1);
            
        }        

        private void RegisterCommandsToVM()
        {
            Type[] x = new Type[0];
            GameState.virtualMachine.RegisterFunction("GET_FPS", 0, x, arg => this.Fps);
            Type[] y = new Type[1]; y[0] = typeof(bool);
            GameState.virtualMachine.RegisterFunction("PAUSE", 0, x, arg => GameState.ChangeEngineState(GameState.EngineState == GameState.EngineStates.PAUSED ? GameState.EngineStates.RUNNING : GameState.EngineStates.PAUSED));
        }

        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Main engine loop for the application
        /// </summary>
        /// <param name="gameTime">Amount of time passed</param>
        protected override void Update(GameTime gameTime)
        {
            // Set all Game State Variables for this iteration
            GameState.gameTime = gameTime;                                                              // Set the total Game Time
            GameState.timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;     // Set the Time Difference
  
            // Process All Input
            InputHandler.ProcessInput();

            // Determine what state the engine is in
            switch( GameState.EngineState )
            {
                case GameState.EngineStates.ENGINE_START:///////////////////////////// This state represents when the engine is first starting (pre-main menu)
                    if ((gameTime.TotalGameTime.Ticks / 10000) > 5000.0f)           // If the logo screen has been on for more than 5 seconds
                        GameState.EngineState = GameState.EngineStates.MAIN_MENU;   // Go to the Main Menu       
                    break;

                case GameState.EngineStates.LEVEL_LOAD://///////////////////////////// This state represents when the engine is loading a level
                    sceneGraph.AddNode(0, null);                                    //

                    GameState.EngineState = GameState.EngineStates.RUNNING;

                    IsMouseVisible = true;
                    GraphicsEngine.camera.LockToNode(sceneGraph.commanderNode, false);
                    InputHandler.InputMode = InputHandler.InputModes.RTS;
                    GraphicsEngine.hud.Activate();
                    break;

                case GameState.EngineStates.LOADING:////////////////////////////////////// This state represents when the engine is preparing itself to load levels
                    sceneGraph = new SceneGraph();                                      // Create a scene graph instance
                    sceneGraph.Initiate();                                              // Initiate the scene graph
                    GraphicsEngine.camera.LockToNode(sceneGraph.freeRoamNode, false);   //
                    InputHandler.InputMode = InputHandler.InputModes.FREE_ROAM;         //
            
                    /// FPS tracker widget
                    HUDVarWatcher tester = new HUDVarWatcher();
                    tester.Initiate( new Vector2( 100, 100 ), new Vector2( 75, 25 ));
                    String[] str = new String[1];
                    str[0] = "GET_FPS";
                    Object[][] args = new object[1][] { new object[1] { null } };
                    tester.LockToVariable(str, ref args, 0.7f);

                    /// Angle tracker widget
                    HUDVarWatcher tester2 = new HUDVarWatcher();
                    tester2.Initiate( new Vector2(100, 200), new Vector2(75, 25));
                    String[] str2 = new String[1];
                    str2[0] = "GET_ANGLE_BETWEEN";
                    Object[][] args2 = new object[1][] { new object[2] { GraphicsEngine.camera.Position, GraphicsEngine.camera.Target.Position } };
                    tester2.LockToVariable(str2, ref args2, 0.7f);

                    /// MiniMap widget
                    HUDMiniMap miniMap = new HUDMiniMap();
                    float startX = (float)(GraphicsEngine.viewport.Width * 0.8);
                    float startY = (float)(GraphicsEngine.viewport.Height * 0.8);
                    float mmWidth = GraphicsEngine.viewport.Width - startX;
                    float mmHeight = GraphicsEngine.viewport.Height - startY;
                    miniMap.Initiate(new Vector2(startX, startY), new Vector2(mmWidth, mmHeight));

                    GraphicsEngine.hud.Add(tester);
                    //hud.Add(tester2);
                    GraphicsEngine.hud.miniMapID = GraphicsEngine.hud.Add(miniMap);
                    GameState.EngineState = GameState.EngineStates.LEVEL_LOAD;
                    break;

                case GameState.EngineStates.MAIN_MENU:///////////////////////////////////////////////////////////////////////////////////////////
                    break;

                case GameState.EngineStates.PAUSED://////////////////////////////////////////////////////////////////////////////////////////////
                    if (MediaPlayer.State == MediaState.Playing)    // If music is playing
                        MediaPlayer.Pause();                        // Pause the music                    
                    break;

                case GameState.EngineStates.RUNNING://///////////////////////////////////////////////////////////////////////////////////////////
                    if (MediaPlayer.State == MediaState.Paused)
                        MediaPlayer.Resume();
                    if (MediaPlayer.State == MediaState.Stopped)
                        MediaPlayer.Play(bgMusic);

                    sceneGraph.world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, true, 1.0f / 60.0f, 2);
                    GraphicsEngine.camera.Update();
                    sceneGraph.Update();
                    if (GraphicsEngine.hud.IsActive) GraphicsEngine.hud.Update();                    
                    break;

                case GameState.EngineStates.TERMINATING://////////////////////////////////////////////////////////////////////////////////////////
                    break;
            
            }

            
            base.Update(gameTime);
        }
        
        private float Sum(int[] Samples)
        {
            float RetVal = 0f;
            for (int i = 0; i < Samples.Length; i++)
            {
                RetVal += (float)Samples[i];
            }
            return RetVal;
        }

        protected override void Draw(GameTime gameTime)
        {
            GameState.gameTime = gameTime;                                                              // Set the total Game Time
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsEngine.graphicsDevice.RasterizerState = rs;

            GraphicsEngine.graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            Samples[CurrentSample++] = (int)gameTime.ElapsedGameTime.Ticks;
            TicksAggregate += (int)gameTime.ElapsedGameTime.Ticks;
            if (TicksAggregate > TimeSpan.TicksPerSecond)
            {
                TicksAggregate -= (int)TimeSpan.TicksPerSecond;
                SecondSinceStart += 1;
            }
            if (CurrentSample == NumberSamples) //We are past the end of the array since the array is 0-based and NumberSamples is 1-based
            {
                float AverageFrameTime = Sum(Samples) / NumberSamples;
                Fps = TimeSpan.TicksPerSecond / AverageFrameTime;
                CurrentSample = 0;
            }

            switch (GameState.EngineState)
            {
                case GameState.EngineStates.ENGINE_START:

                    GraphicsEngine.spriteBatch.Begin();
                        GraphicsEngine.spriteBatch.Draw(splashLogo, new Rectangle(0, 0, GraphicsEngine.viewport.Width, GraphicsEngine.viewport.Height), Color.White);
                    GraphicsEngine.spriteBatch.End();

                    break;

                case GameState.EngineStates.MAIN_MENU:

                    mainMenu.Draw();

                    break;

                case GameState.EngineStates.LEVEL_LOAD:                    
                    break;

                case GameState.EngineStates.PAUSED:

                    if (GameState.virtualMachine.Console.IsVisible)
                    {
                        sceneGraph.Draw();
                        GameState.virtualMachine.Console.Draw();
                        GraphicsEngine.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                        if (GraphicsEngine.hud.IsVisible) GraphicsEngine.hud.Draw();
                        GraphicsEngine.spriteBatch.End();
                    }
                    else
                        mainMenu.Draw();

                    break;

                case GameState.EngineStates.RUNNING:

                    sceneGraph.Draw();

                    GraphicsEngine.spriteBatch.Begin();//SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

                    string output = "Time of day is: " + (int)sceneGraph._timeOfDay / 60 + " : " + (int)sceneGraph._timeOfDay % 60;
                    Vector2 FontOrigin = GraphicsEngine.font.MeasureString(output) / 2;
                    Vector2 FontPos = new Vector2(250, 10);
                    GraphicsEngine.spriteBatch.DrawString(GraphicsEngine.font, output, FontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                    if (GraphicsEngine.hud.IsVisible) GraphicsEngine.hud.Draw();                   
                    GraphicsEngine.spriteBatch.End();
                    if (GameState.virtualMachine.Console.IsVisible) GameState.virtualMachine.Console.Draw();
                break;
        }
            
            base.Draw(gameTime);
            
        }

        public void DestroyCurrentGame()
        {

        }
        #endregion

    }
}

