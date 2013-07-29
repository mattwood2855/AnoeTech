using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AnoeTech
{
    public class HUDMiniMap : HUDObject
    {

        Texture2D _miniMap;

        public override void Initiate(Vector2 position, Vector2 size)
        {
            _position        = position;
            _size            = size;
            _isVisible       = false;
            _transitionStyle = TransitionStyle.SLIDE_IN_UP;
            _miniMap         = GameState.anoetech.sceneGraph.miniMap;
            SetMiniMap(GameState.anoetech.sceneGraph.miniMap);
        }

        private void SetMiniMap( Texture2D miniMap)
        {
            _miniMap = miniMap;
        }

        public override void Update(object[] obj) 
        {
            if (_transitioningIn)                                                                     // If sliding in
            {
                TransitionIn();
            }
            else if (_transitioningOut)                                                               // If sliding out
            {
                TransitionOut();
            }
        }

        public override void Draw() 
        {
            GraphicsEngine.spriteBatch.Draw(_miniMap, new Rectangle((int)_position.X, (int)_position.Y, (int)_position.X + (int)_size.X, (int)_position.Y + (int)_size.Y), Color.White);
        }
        public override void Destroy() { }              
    }
}
