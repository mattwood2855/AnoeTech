using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AnoeTech
{
    public class VMConsole
    {

        #region Variables

        private      Texture2D _consolePanel;
        private       string[] _consoleData;
        private         string _command;
        private        Vector2 _consoleTextPosition;
        private         char[] _delimiterChars = { ' ', '(', ',', ')' };
        private            int _maxLines = 8;
        private        Vector2 _position, _targetPosition;
        private           bool _slidingIn = false;
        private           bool _slidingOut = false;
        private           bool _visible = false;
        private VirtualMachine _vm;
        private            int _width, _height;


        #endregion

        #region Accessors

        public bool IsVisible { get { return _visible; } private set { _visible = value; } }
        public string Command { get { return _command; } private set { _command = value; } }

        #endregion

        #region Public Functions

        public void Draw()
        {
            if (IsVisible)
            {
                GraphicsEngine.spriteBatch.Begin();
                // Draw the console panel graphic
                GraphicsEngine.spriteBatch.Draw(_consolePanel, new Rectangle((int)_position.X, (int)_position.Y, _width, _height), Color.White);

                // Draw the current command
                GraphicsEngine.spriteBatch.DrawString(GraphicsEngine.font, _command, _consoleTextPosition, Color.White, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0.5f);
                
                // Draw the command/debug history
                if (_consoleData != null)                                               // If there is prior data
                {
                    Vector2 originalPosition = _consoleTextPosition;                    // Save the original start position 
                    for( int counter = 0; counter < _consoleData.Length; counter++ )    // Go through all the lines of debug info
                    {
                        _consoleTextPosition.Y -= 14;                                   // Move up to print the next line of debug info
                        GraphicsEngine.spriteBatch.DrawString(GraphicsEngine.font,              // Write the info onto the console
                            _consoleData[counter], _consoleTextPosition, Color.White, 
                            0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0.5f);
                        if (counter == _maxLines) break;                                // If we are at the max # of visible lines then break
                    }
                    _consoleTextPosition = originalPosition;                            // Reset the console text position back to the original
                }
                GraphicsEngine.spriteBatch.End();
            }
        }

        public void Hide()
        {
            _slidingOut = true;
        }

        public void Initiate(VirtualMachine vm) 
        { 
            _vm                  = vm;
            _consolePanel        = GameState.contentManager.Load<Texture2D>("HUD/consolePanel");
            _position            = new Vector2(0, GraphicsEngine.viewport.Height);
            _targetPosition = new Vector2(0, (int)Math.Ceiling(GraphicsEngine.viewport.Height - GraphicsEngine.viewport.Height * 0.2f));
            _consoleTextPosition = new Vector2(10, _position.Y + GraphicsEngine.viewport.Height * 0.2f - 26);
            _width = GraphicsEngine.viewport.Width;
            _height = (int)(GraphicsEngine.viewport.Height * 0.2);

            // Register console based functions with the VM
            Type[] y = new Type[1]; y[0] = typeof(string);
            vm.RegisterFunction("CONSOLE2FILE", 1, y, arg => SaveToFile((string)arg[0]));   // Saves the console to a txt file
        }

        public void Output(string line) 
        {
            _command = line;
            UpdateText();
        }

        public void Show()
        {
            IsVisible  = true;
            _slidingIn = true;
            GameState.EngineState = GameState.EngineStates.PAUSED;
        }

        public void Update() 
        {
            if (_slidingIn)                                                                     // If sliding in
            {
                _position.Y -= 0.1f * (_position.Y - _targetPosition.Y);                        // Move 10% of the total distance to target
                _consoleTextPosition.Y -= 0.1f * (_position.Y - _targetPosition.Y);             // Move the text 10% of the totale distance too
                if (Math.Abs(_position.Y - _targetPosition.Y) < 0.1f)                           // If we are within .1 of the target
                {
                    _position.Y = _targetPosition.Y;                                            // Snap to the target
                    _consoleTextPosition.Y = _position.Y + GraphicsEngine.viewport.Height * 0.2f - 14; // Snap the text to the target
                    _targetPosition = new Vector2(0, GraphicsEngine.viewport.Height);                  // Set the target to the original starting pos
                    _slidingIn = false;                                                         // Set liding in to false
                }
            }
            else if (_slidingOut)                                                               // If sliding out
            {
                _position.Y -= 0.1f * (_position.Y - _targetPosition.Y);                        // Opposite code of sliding in
                _consoleTextPosition.Y -= 0.1f * (_position.Y - GraphicsEngine.viewport.Height);
                if (Math.Abs(_position.Y - _targetPosition.Y) < 0.1f)
                {
                    _position.Y = _targetPosition.Y;
                    _consoleTextPosition.Y = _position.Y + GraphicsEngine.viewport.Height * 0.2f - 26;
                    _targetPosition = new Vector2(0, (int)(GraphicsEngine.viewport.Height - GraphicsEngine.viewport.Height * 0.2f));
                    _slidingOut = false;
                    IsVisible = false;
                    GameState.EngineState = GameState.EngineStates.RUNNING;                                                  // Unpause the engine
                }
            }
            else                                                                                // If not sliding in or out
            {
                Keys[] keys = InputHandler.keyboardState.GetPressedKeys();                                        // Make an array of all the pressed keys
                foreach (Keys key in keys)                                                      // For each key
                {
                    if (key == Keys.LeftShift || key == Keys.RightShift || key == Keys.Escape)
                        continue;
                    if (InputHandler.oldKeyboardState.IsKeyUp(key))                                               // If not pressed last iteration ( done to avoid registering a key press twice or more )
                        if (key == Keys.Back)                                                   // If user pressed BackSpace
                        {
                            if (_command.Length > 0)                                            // If there is text
                                _command = _command.Remove(_command.Length - 1, 1);             // Remove the last character in the string
                        }
                        else if (key == Keys.Space)                                             // If user presses SPACE
                            _command = _command.Insert(_command.Length, " ");                   // Add a space
                        else if (key == Keys.Enter)                                             // If user presses ENTER
                            BuildCommand();                                                     // Build and attempt to execute the command
                        else if ((int)key >= 48 && (int)key <= 57)                              // If the users presses 0-9
                            _command += ((int)key - 48);                                        // Add "0" - "9" (as a char)
                        else if ((int)key >= 96 && (int)key <= 105)                             //..
                            _command += ((int)key - 96);
                        else if (key == Keys.OemPeriod)                                         // If user presses period
                            _command += ".";                                                    // Add "."

                        else                                                                    // If any other entry
                                _command += key.ToString();                                     // Add the entry as a string
                }
            }
        }

        #endregion

        #region Private Functions

        private void BuildCommand()
        {
            string[] words = _command.Split(_delimiterChars);           // Create an array of all the words entered

            string command = words[0];                                  // Assign the first word as the command
            object[] args = new object[words.Length-1];                 // Create an array to hold all the arguements in the command

            bool tmpBool;
            float tmpFloat;
            int tmpInt;
            for (int counter = 1; counter < words.Length; counter++ )   // Build the list of arguements from the command string
            {
                if(int.TryParse(words[counter], out tmpInt))            // If the string is an Integer convert it
                    args[counter - 1] = tmpInt;
                else if (float.TryParse(words[counter], out tmpFloat))  // If the string is a Float convert it
                    args[counter - 1] = tmpFloat;
                else if (bool.TryParse(words[counter], out tmpBool))    // If the string is a Bool convert it
                    args[counter - 1] = tmpBool;
                else                                                    // Otherwise its a string
                    args[counter - 1] = words[counter];
            }
            _vm.ExecuteFromConsole(command, args);                      // Execute the command
        }

        public void SaveToFile( string filename )
        {
            System.IO.File.WriteAllLines(System.IO.Directory.GetCurrentDirectory() + "'\'" + filename, _consoleData);
        }

        private void UpdateText()
        {
            int length = 0;                                     // Assume there are no lines of text in the console cache
            if (_consoleData != null)                           // But if there is lines of data in the console cache
                length = _consoleData.Length;                   // Set length to the number of lines there are
            string[] consoleData = new string[length + 1];      // Make a new string array with space for the new line
            for (int x = length; x > 0; x--)                    // Go through each line in the console starting from the back
            {
                consoleData[x] = _consoleData[x - 1];           // Place them in the new array, filling from the end of the array towards the front
            }
            consoleData[0] = _command.ToString();               // Put the newest command in the the first slot of the array
            _consoleData = consoleData;                         // Assign the new array to _consoleData
            _command = "";                                      // Clear the command line for the next command
        }

        #endregion
    }
}
