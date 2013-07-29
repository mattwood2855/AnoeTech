using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnoeTech
{
    class HUDSelectionBox : HUDObject
    {
        private Vector2 _selectionBoxStart, _selectionBoxEnd;
        private Texture2D _selectionBox;
        private bool _started = false;

        public override void Initiate(Vector2 position, Vector2 size)
        {
            throw new NotImplementedException();
        }

        public void StartSelectionBox(int x, int y)
        {
            _selectionBox = GameState.contentManager.Load<Texture2D>("HUD/hudSelectionBox");
            _selectionBoxStart.X = x; _selectionBoxStart.Y = y;
            _selectionBoxEnd.X = x+1; _selectionBoxEnd.Y = y+1;
            _started = true; _isVisible = true;
        }

        public override void Update(object[] obj)
        {
            if (_started)
            {
                if (obj != null)
                    _selectionBoxEnd.X = (int)obj[0];
                if (obj != null)
                    _selectionBoxEnd.Y = (int)obj[1];
            }
            if (_selectionBoxStart.X < _selectionBoxEnd.X)
            {
                _position.X = _selectionBoxStart.X; _size.X = _selectionBoxEnd.X - _selectionBoxStart.X;
            }
            else
            {
                _position.X = _selectionBoxEnd.X; _size.X = _selectionBoxStart.X - _selectionBoxEnd.X;
            }
            if (_selectionBoxStart.Y < _selectionBoxEnd.Y)
            {
                _position.Y = _selectionBoxStart.Y; _size.Y = _selectionBoxEnd.Y - _selectionBoxStart.Y;
            }
            else
            {
                _position.Y = _selectionBoxEnd.Y; _size.Y = _selectionBoxStart.Y - _selectionBoxEnd.Y;
            }
        }

        public override void Draw()
        {
            if (_selectionBoxStart.X < _selectionBoxEnd.X)
            {
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxStart.X, (int)_selectionBoxStart.Y, (int)_selectionBoxEnd.X - (int)_selectionBoxStart.X, 2), Color.White);
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxStart.X, (int)_selectionBoxEnd.Y, (int)_selectionBoxEnd.X - (int)_selectionBoxStart.X, 2), Color.White);
            }
            else
            {
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxEnd.X, (int)_selectionBoxEnd.Y, (int)_selectionBoxStart.X - (int)_selectionBoxEnd.X, 2), Color.White);
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxEnd.X, (int)_selectionBoxStart.Y, (int)_selectionBoxStart.X - (int)_selectionBoxEnd.X, 2), Color.White);
            }
            if (_selectionBoxStart.Y < _selectionBoxEnd.Y)
            {
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxEnd.X, (int)_selectionBoxStart.Y, 2, (int)_selectionBoxEnd.Y - (int)_selectionBoxStart.Y), Color.White);
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxStart.X, (int)_selectionBoxStart.Y, 2, (int)_selectionBoxEnd.Y - (int)_selectionBoxStart.Y), Color.White);
            }
            else
            {
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxStart.X, (int)_selectionBoxEnd.Y, 2, (int)_selectionBoxStart.Y - (int)_selectionBoxEnd.Y), Color.White);
                GraphicsEngine.spriteBatch.Draw(_selectionBox, new Rectangle((int)_selectionBoxEnd.X, (int)_selectionBoxEnd.Y, 2, (int)_selectionBoxStart.Y - (int)_selectionBoxEnd.Y), Color.White);
            }
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
