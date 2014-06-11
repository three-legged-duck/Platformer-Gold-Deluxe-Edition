using System;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class SplashState : IState
    {
        private const byte Max = 255; //max for rgb drawing
        private readonly Sprite _splashSprite;
        private readonly IState _nextState;
        private readonly bool _scale;
        private bool _isFadeOut;

        private EventHandler<MouseButtonEventArgs> _mouseBtnHandler;

        private byte _alpha;
        private Game _game;

        private View _view;

        public SplashState(string img, bool doScale, IState nextState)
        {
            _splashSprite = new Sprite(new Texture(@"res\images\" + img));
            _scale = doScale;
            _nextState = nextState;
        }

        public string BgMusicName
        {
            get { return null; }
        }

        public void Initialize(Game g)
        {
            _game = g;
            _view = new View();

            _mouseBtnHandler = delegate(object sender, MouseButtonEventArgs btn)
            {
                if (btn.Button == Mouse.Button.Left || btn.Button == Mouse.Button.Right)
                {
                    EndSplash();
                }
            };
            g.W.MouseButtonPressed += _mouseBtnHandler;

            if (_scale)
            {
                _splashSprite.Scale = new Vector2f(_view.Size.X/_splashSprite.GetGlobalBounds().Width,
                    _view.Size.Y/_splashSprite.GetGlobalBounds().Height);
            }
        }

        public void Update()
        {
            if (!_isFadeOut)
            {
                _splashSprite.Color = new Color(Max, Max, Max, _alpha);
                _alpha = (byte) Math.Min(_alpha + 4, 255);
                if (_alpha >= 255)
                {
                    _isFadeOut = true;
                }
            }
            else
            {
                _splashSprite.Color = new Color(Max, Max, Max, _alpha);
                _alpha = (byte) Math.Max(_alpha - 4, 0);
                if (_alpha <= 0)
                {
                    EndSplash();
                }
            }
        }

        public void Draw()
        {
            _game.W.SetView(_view);
            _game.W.Draw(_splashSprite);
        }

        public void Uninitialize()
        {
            _game.W.MouseButtonPressed -= _mouseBtnHandler;
        }

        public void OnEvent(Settings.Action a)
        {
            EndSplash();
        }

        public void EndSplash()
        {
            _game.State = _nextState;
            _game.StopInput(600);
        }
    }
}