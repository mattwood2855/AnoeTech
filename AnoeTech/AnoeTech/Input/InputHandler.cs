using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace AnoeTech
{
    public static class InputHandler
    {
        public enum InputModes { RTS, UNIT_INFANTRY, UNIT_VEHICLE, UNIT_BOAT, UNIT_AERO, FREE_ROAM };

        #region Variables
        
        public static KeyboardState keyboardState, oldKeyboardState;
        public static MouseState mouseState, oldMouseState;
        public static InputModes InputMode = InputModes.FREE_ROAM;
        private static Vector3 moveVector = new Vector3(0, 0, 0);
        private static Vector2 mouseDelta = new Vector2(0, 0);
        static int selectionBoxID;

        #endregion

        public static void Initiate()
        {
            Mouse.SetPosition(GraphicsEngine.viewport.Width / 2, GraphicsEngine.viewport.Height / 2);
            oldMouseState = Mouse.GetState();
        }

        #region Input Controllers

        private static void SplashScreenController()
        {
            if (keyboardState.IsKeyDown(Keys.Escape))                           // If the User presses Esc
                GameState.EngineState = GameState.EngineStates.MAIN_MENU;       // Go straight to the main menu   
        }
        private static void FreeRoamController()
        {
            if (mouseState.MiddleButton == ButtonState.Pressed)                 // If the user presses the middle mouse button (clicks wheel)
                if (oldMouseState.MiddleButton != ButtonState.Pressed)          // And it wasnt pressed last iteration
                {
                    GameState.anoetech.IsMouseVisible = true;                   // Make the mouse visible
                    InputMode = InputModes.RTS;                                 // Set the input mode to RTS
                    GraphicsEngine.camera.LockToNode(GameState.anoetech.sceneGraph.commanderNode, false); // Lock the camera to the RTS node
                    GraphicsEngine.hud.Activate();                          // Turn on the HUD
                }

            if (GraphicsEngine.camera.Target != null)                       // Protect against intermitent null states
                if (mouseState != oldMouseState)                                    // If the mouse moved
                {
                    GraphicsEngine.camera.Target.leftrightRot -= GraphicsEngine.camera.Target.rotationSpeed * mouseDelta.X * GameState.timeDifference;
                    GraphicsEngine.camera.Target.updownRot -= GraphicsEngine.camera.Target.rotationSpeed * mouseDelta.Y * GameState.timeDifference;
                    Mouse.SetPosition(GraphicsEngine.viewport.Width / 2, GraphicsEngine.viewport.Height / 2);
                    oldMouseState = mouseState;
                    oldKeyboardState = keyboardState;
                    mouseState = Mouse.GetState();                                 // Set the current Mouse State
                    keyboardState = Keyboard.GetState();                           // Set the current Keyboard State
                }

            if (keyboardState.IsKeyDown(InputMap.FORWARD))                      // If the user presses the forward button
                moveVector += new Vector3(0, 0, -1.0f);                         // Adjust the move vector to move forward (negative Z axis)
            if (keyboardState.IsKeyDown(InputMap.BACKWARD))                     // If the user presses the backward button
                moveVector += new Vector3(0, 0, 1.0f);                          // Adjust the move vector to move backward (positive Z axis)
            if (keyboardState.IsKeyDown(InputMap.STRAFE_RIGHT))                 // If the user presses the right button
                moveVector += new Vector3(1.0f, 0, 0);                          // Adjust the move vector to move right (positive X axis)
            if (keyboardState.IsKeyDown(InputMap.STRAFE_LEFT))                  // If the user presses the left button
                moveVector += new Vector3(-1.0f, 0, 0);                         // Adjust the move vector to move left (negative X axis)

            if (GraphicsEngine.camera.Target != null)                       // Protect against intermitent null states
                GraphicsEngine.camera.Target.AddToVelocity(moveVector * GraphicsEngine.camera.Target.moveSpeed * GameState.timeDifference); // Add the move vector to the free roam node

            if (keyboardState.IsKeyDown(Keys.OemPlus))
                GraphicsEngine.FieldOfView((float)(GraphicsEngine.fov + 0.1), GraphicsEngine.viewport.AspectRatio);
            if (keyboardState.IsKeyDown(Keys.OemMinus))
                GraphicsEngine.FieldOfView((float)(GraphicsEngine.fov - 0.1), GraphicsEngine.viewport.AspectRatio);
            
        }
        private static void RTSController()
        {
            float moveSpeed = (float)(GraphicsEngine.camera.Position.Y * .01);  // Calculate the moveSpeed based on the elevation to create desirable move distance

            if (mouseState.X < 25)                                                  // If the mouse is in the left 25 pixels of the viewport
                moveVector += new Vector3(-moveSpeed, 0, 0);                        // Scroll left
            if (mouseState.X > GraphicsEngine.viewport.Width - 25)                  // If the mouse is in the right 25 pixels of the viewport
                moveVector += new Vector3(moveSpeed, 0, 0);                         // Scroll right
            if (mouseState.Y < 25)                                                  // If the mouse is in the top 25 pixels of the viewport
                moveVector += new Vector3(0, moveSpeed, 0);                         // Scroll up
            if (mouseState.Y > GraphicsEngine.viewport.Height - 25)                 // If the mouse is in the bottom 25 pixels of the viewport
                moveVector += new Vector3(0, -moveSpeed, 0);                        // Scroll down

            if (mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)       // If the user scrolls the mouse wheel down
                moveVector += new Vector3(0, 0, 40);                                // Raise the elevation of the user
            if (mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)       // If the user scrolls the mouse wheel up
                moveVector += new Vector3(0, 0, -40);                               // Lower the elevation of the user

            if (mouseState.MiddleButton == ButtonState.Pressed)                     // If the user presses middle mouse button (clicks wheel)
                if (oldMouseState.MiddleButton != ButtonState.Pressed)              // And it wasnt pressed last iteration
                {
                    if (GameState.anoetech.sceneGraph.GetSelectedUnits().Count > 0) // If at least one unit is selected
                    {
                        GameState.anoetech.IsMouseVisible = false;                  // Hide the mouse cursor
                        InputMode = InputModes.UNIT_VEHICLE;                        // Change the input mode to Vehicle ***TO-DO: change to mode unit requires
                        GraphicsEngine.camera.LockToNode(GameState.anoetech.sceneGraph.GetSelectedUnits().ElementAt(0), true); // Lock the camera to the first selected node
                        GraphicsEngine.hud.Deactivate();                        // Deactivate the HUD
                    }
                }            

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                GameState.anoetech.IsMouseVisible = false;
                Mouse.SetPosition(GraphicsEngine.graphicsDeviceManager.GraphicsDevice.Viewport.Width / 2, GraphicsEngine.graphicsDeviceManager.GraphicsDevice.Viewport.Height / 2);
                GraphicsEngine.camera.LockToNode(GameState.anoetech.sceneGraph.freeRoamNode, false);
                InputMode = InputModes.FREE_ROAM;
                GraphicsEngine.hud.Deactivate();
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (oldMouseState.LeftButton != ButtonState.Pressed)
                {
                    GameState.anoetech.sceneGraph.DeselectAll();
                    HUDSelectionBox selectionBox = new HUDSelectionBox();
                    selectionBox.StartSelectionBox(mouseState.X, mouseState.Y);
                    selectionBoxID = GraphicsEngine.hud.Add(selectionBox);
                }
                else
                {
                    object[] obj = new object[2];
                    obj[0] = mouseState.X; obj[1] = mouseState.Y;
                    GraphicsEngine.hud.GetObject(selectionBoxID).Update(obj);
                }
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                if (oldMouseState.LeftButton != ButtonState.Released)
                {
                    GameState.anoetech.sceneGraph.RectangleSelect(GraphicsEngine.viewport, GraphicsEngine.projectionMatrix, GraphicsEngine.camera.ViewMatrix,
                                                        new Rectangle((int)GraphicsEngine.hud.GetObject(selectionBoxID).Position.X, (int)GraphicsEngine.hud.GetObject(selectionBoxID).Position.Y,
                                                                      (int)GraphicsEngine.hud.GetObject(selectionBoxID).Size.X, (int)GraphicsEngine.hud.GetObject(selectionBoxID).Size.Y));
                    GraphicsEngine.hud.Remove(selectionBoxID);
                }
            }

            GraphicsEngine.camera.Target.AddToVelocity(moveVector * GraphicsEngine.camera.Target.moveSpeed * GameState.timeDifference);

        }
        private static void Unit_VehicleController()
        {
            if (mouseState.MiddleButton == ButtonState.Pressed)
                if (oldMouseState.MiddleButton != ButtonState.Pressed)
                {
                    GameState.anoetech.IsMouseVisible = true;
                    InputMode = InputModes.RTS;
                    GraphicsEngine.camera.LockToNode(GameState.anoetech.sceneGraph.commanderNode, true);
                    GraphicsEngine.hud.Activate();
                }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                GameState.anoetech.IsMouseVisible = false;
                Mouse.SetPosition(GraphicsEngine.viewport.Width / 2, GraphicsEngine.viewport.Height / 2);
                GraphicsEngine.camera.LockToNode(GameState.anoetech.sceneGraph.freeRoamNode, false);
                InputMode = InputModes.FREE_ROAM;
                GraphicsEngine.hud.Deactivate();
            }

            if (mouseState != oldMouseState)
            {
                GraphicsEngine.camera.Target.leftrightRot -= GraphicsEngine.camera.Target.rotationSpeed * mouseDelta.X * GameState.timeDifference;
                //camera.Target.updownRot -= camera.Target.rotationSpeed * yDifference * timeDifference;
                Mouse.SetPosition(GraphicsEngine.viewport.Width / 2, GraphicsEngine.viewport.Height / 2);
                oldMouseState = mouseState;
                oldKeyboardState = keyboardState;
                mouseState = Mouse.GetState();                                 // Set the current Mouse State
                keyboardState = Keyboard.GetState();                           // Set the current Keyboard State
            }

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                moveVector += new Vector3(0, 0, -0.5f);
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 0.5f);
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                moveVector += new Vector3(0.5f, 0, 0);
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-0.5f, 0, 0);

            

            GraphicsEngine.camera.Target.AddToVelocity(moveVector * GraphicsEngine.camera.Target.moveSpeed * GameState.timeDifference);
        }

        #endregion

        public static void ProcessInput()
        {
            oldMouseState = mouseState;
            oldKeyboardState = keyboardState;
            mouseState = Mouse.GetState();                                 // Set the current Mouse State
            keyboardState = Keyboard.GetState();                           // Set the current Keyboard State
            mouseDelta.X = mouseState.X - oldMouseState.X;                              // Calculate the mouse X delta
            mouseDelta.Y = mouseState.Y - oldMouseState.Y;                              // Calculate the mouse Y delta
            moveVector.X = moveVector.Y = moveVector.Z = 0;                             // Zero the move Vector

            switch (GameState.EngineState)                                              // Determine our Engine State
            {                
                case GameState.EngineStates.ENGINE_START:                               // If the engine is starting (Showing Splash Screens)
                    SplashScreenController();
                break;
/////////////////////////////////////////////////////////////////////////////////////////////////////////
                case GameState.EngineStates.MAIN_MENU:
                    GameState.anoetech.mainMenu.Update();
                break;
/////////////////////////////////////////////////////////////////////////////////////////////////////////
                case GameState.EngineStates.RUNNING:
                
                    if (keyboardState.IsKeyDown(InputMap.CONSOLE))                                                              // If the user presses the console key
                        if (!oldKeyboardState.IsKeyDown(InputMap.CONSOLE))                                                      // And it wasnt down the last iteration
                            if (GameState.virtualMachine.Console.IsVisible) GameState.virtualMachine.Console.Hide();            // If the console is already open, Hide it
                            else GameState.virtualMachine.Console.Show();                                                       // If it was hidden, show it
                    if (GameState.virtualMachine.Console.IsVisible)                                                             // If the console is open
                        GameState.virtualMachine.Console.Update();                                                              // Update input only for the console
                    else                                                                                                        // If the console is not opened.
                    {
                        if (keyboardState.IsKeyDown(Keys.Escape))                                                               // If the user presses the Esc key
                        {
                            MediaPlayer.Stop();                                                                                 // Stop the background music
                            GameState.EngineState = GameState.EngineStates.MAIN_MENU;                                           // Set the engine state to main menu mode
                        }

                        switch (InputMode)                                                                                      // Determine what input mode we are in
                        {
                            case InputModes.FREE_ROAM:                                
                                FreeRoamController();                                
                            break;

                            case InputModes.RTS:
                                RTSController();                                
                            break;

                            case InputModes.UNIT_VEHICLE:
                                Unit_VehicleController();
                                break;
                        }
                        
                    }
                break;
/////////////////////////////////////////////////////////////////////////////////////////////////////////
                case GameState.EngineStates.PAUSED:
                    if (GameState.virtualMachine.Console.IsVisible)             // If the virtual machine paused the game
                        GameState.virtualMachine.Console.Update();              // Update the Virtual Machine
                    else                                                        // Otherwise the main menu paused the game
                        GameState.anoetech.mainMenu.Update();                   // Update the main menu
                break;
/////////////////////////////////////////////////////////////////////////////////////////////////////////                                                                 
            }                        
        }
    }
}
