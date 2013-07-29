using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AnoeTech
{
    public static class GameState
    {
        public enum EngineStates { ENGINE_START, MAIN_MENU, LOADING, LEVEL_LOAD, RUNNING, PAUSED, TERMINATING };

        public static       Anoetech anoetech;
        public static ContentManager contentManager;        
        public static   EngineStates EngineState;
        public static          float timeDifference;
        public static       GameTime gameTime;
        public static VirtualMachine virtualMachine;
        

        public static void ChangeEngineState(EngineStates newState)
        {
            EngineState = newState;
        }
    }
}
