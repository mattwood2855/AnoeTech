using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AnoeTech
{
    public class Script
    {
        //list of commands in this wave
        List<IScriptCommand> commands = new List<IScriptCommand>();

        //adds a new command to the list
        public void AddCommand(IScriptCommand command)
        {
            commands.Add(command);
        }

        //index of current command we're at
        int currentCommandIndex = 0;
        public void Reset()
        {
            currentCommandIndex = 0;
        }

        //executes the next command. Not literally, mind you. That would be... messy.
        public bool Run(Anoetech engine)
        {

            //next command
            currentCommandIndex++;

            //if we're at the end of the line exit
            if (currentCommandIndex > commands.Count - 1)
                return true;
            else
            {
                //otherwise run this command
                commands[currentCommandIndex].Do(this);
                return false;
            }
        }

        //spawns an enemy. all commands access the Wave class, so it acts as
        //an intermediary to the rest of the game. This is useful if you add things
        //like spawn timer modifiers.
        public void SpawnUnit(SceneGraph.UnitTypes unitType, Vector2 position, Script ai)
        {
            GameState.anoetech.sceneGraph.AddUnit( unitType, position, ai);
        }
    }
}
