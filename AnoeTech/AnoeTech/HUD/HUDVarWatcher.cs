using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnoeTech
{
    class HUDVarWatcher : HUDObject
    {
        private    float _fontSize;
        private string[] _VMCommands;
        private object[][] _args;

        public override void Destroy()
        {
            throw new NotImplementedException();
        }
        public override void Draw()
        {
            GraphicsEngine.spriteBatch.Draw(_bgTexture, new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y), new Color(255,255,255, 100));
            for (int x = 0; x < _VMCommands.Length; x++)
            {
                if(_args != null)
                    GraphicsEngine.spriteBatch.DrawString(GraphicsEngine.font, GameState.virtualMachine.Execute(_VMCommands[x], _args[x]).ToString(), _position, Color.White, 0, new Vector2(0, 0), _fontSize, SpriteEffects.None, 0.5f);
                else
                    GraphicsEngine.spriteBatch.DrawString(GraphicsEngine.font, GameState.virtualMachine.Execute(_VMCommands[x], null).ToString(), _position, Color.White, 0, new Vector2(0, 0), _fontSize, SpriteEffects.None, 0.5f);
            }
        }

        public override void Initiate(Vector2 position, Vector2 size)
        {
            _position  = position;
            _size      = size;
            _bgTexture = GameState.contentManager.Load<Texture2D>("HUD/hudPanelGeneric");
            _transitionStyle = TransitionStyle.SLIDE_IN_UP;
        }

        public void LockToVariable(string[] VMCommands, ref object[][] args, float fontSize)
        {
            _args            = args;
            _VMCommands      = VMCommands;
            _fontSize        = fontSize;
        }

        public override void Update( object[] obj )
        {
            if (_transitioningIn) TransitionIn();
            if (_transitioningOut) TransitionOut();
        }
    }
}
