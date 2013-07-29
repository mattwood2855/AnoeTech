using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnoeTech
{
    public class HUDHealthBar : HUDObject
    {

        Texture2D lifeBar;
        SGNUnit _target;

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            GraphicsEngine.spriteBatch.Draw(lifeBar, new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y), Color.Green);
        }

        public override void Initiate(Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 size)
        {
            _isVisible = true;
            _position  = position;
            _size      = size;
            
            lifeBar = GameState.contentManager.Load<Texture2D>("HUD/hudWhitePixel");
        }

        public void SetTarget( SGNUnit target )
        {
            _target = target;
        }

        public override void  Update(object[] obj)
        {
            if (obj != null)
            {
                
            }

            if (_target != null)
                _position = new Vector2(GraphicsEngine.viewport.Project(_target.Position, GraphicsEngine.projectionMatrix, GraphicsEngine.camera.ViewMatrix, Matrix.Identity).X, GraphicsEngine.viewport.Project(_target.Position, GraphicsEngine.projectionMatrix, GraphicsEngine.camera.ViewMatrix, Matrix.Identity).Y);
        }

    }
}
