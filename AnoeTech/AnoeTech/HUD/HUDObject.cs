using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnoeTech
{
    public abstract class HUDObject
    {
        protected enum TransitionStyle { SLIDE_IN_RIGHT, SLIDE_IN_LEFT, SLIDE_IN_UP, SLIDE_IN_DOWN, FADE_IN, POP_OUT }

        protected             int _alpha;
        protected       Texture2D _bgTexture;        
        protected            bool _isVisible;
        protected         Vector2 _position, _targetPosition;
        protected         Vector2 _size;
        protected            bool _transitioningIn, _transitioningOut; 
        protected TransitionStyle _transitionStyle;

        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public Vector2 Position { get { return _position; } }
        public Vector2 Size { get { return _size; } }


        public abstract void Initiate(Vector2 position, Vector2 size);
        public abstract void Update(object[] obj);
        public abstract void Draw();
        public abstract void Destroy();

        public virtual void Activate()
        {
            _transitioningOut = false;
            _transitioningIn = true;
            _isVisible = true;
            _targetPosition = new Vector2(0, (int)Math.Ceiling(GraphicsEngine.viewport.Height - _size.Y));
        }

        public virtual void Deactivate()
        {
            _transitioningIn = false;
            _transitioningOut = true;
            _targetPosition = new Vector2(0, GraphicsEngine.viewport.Height);
        }

        protected void TransitionIn()
        {
            switch (_transitionStyle)
            {
                case TransitionStyle.SLIDE_IN_UP:

                    _position.Y -= 0.1f * (_position.Y - _targetPosition.Y);

                    if (Math.Abs(_position.Y - _targetPosition.Y) < 0.1f)
                    {
                        _position.Y = _targetPosition.Y;
                        _targetPosition = new Vector2(0, GraphicsEngine.viewport.Height);
                        _transitioningIn = false;
                    }
                    break;

                case TransitionStyle.SLIDE_IN_DOWN:
                    break;
            }
        }

        protected void TransitionOut()
        {
            switch (_transitionStyle)
            {
                case TransitionStyle.SLIDE_IN_UP:

                    _position.Y -= 0.1f * (_position.Y - _targetPosition.Y);
                    if (Math.Abs(_position.Y - _targetPosition.Y) < 0.1f)
                    {
                        _position.Y = _targetPosition.Y;
                        _targetPosition = new Vector2(0, (int)(GraphicsEngine.viewport.Height - GraphicsEngine.viewport.Height - _size.Y));
                        _transitioningOut = false;
                        _isVisible = false;
                    }
                    break;

                case TransitionStyle.SLIDE_IN_DOWN:
                    break;
            }
        }
    }
}
