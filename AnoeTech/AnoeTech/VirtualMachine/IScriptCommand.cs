using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace AnoeTech
{
    public interface IScriptCommand
    {
        void Do( Script script );
    }

    struct ScriptCommand_SpawnUnit : IScriptCommand
    {
           //holds the position, velocity, and color
        SceneGraph.UnitTypes _unitType;
        Vector2 _position;
        Script _ai;
        public ScriptCommand_SpawnUnit(SceneGraph.UnitTypes unitType, Vector2 position, Script ai)
        {
            this._unitType = unitType;
            this._position = position;
            this._ai = ai;
        }
        
        public void Do(Script script)
        {
            //tells the Wave to spawn this enemy.
            script.SpawnUnit(_unitType, _position, _ai);
        }
    }


}
