using System;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class ScrollingTextState : IState
    {
        private readonly IState _nextState;
        private Text[] _textLines;
        private string textName;
        private uint _textSize;
        private Game _game;

        private EventHandler<MouseButtonEventArgs> _mouseBtnHandler;

        public string BgMusicName
        {
            get { return null; }
        }

        public ScrollingTextState(string textName, IState nextState)
        {
            _nextState = nextState;
            this.textName = textName;
        }


        public void Initialize(Game game)
        {
            _textSize = game.W.Size.Y/18;
            _game = game;
            _game.W.SetView(_game.W.DefaultView);
            _mouseBtnHandler = delegate(object sender, MouseButtonEventArgs btn)
            {
                if (btn.Button == Mouse.Button.Left || btn.Button == Mouse.Button.Right)
                {
                    EndScrollingText();
                }
            };
            game.W.MouseButtonPressed += _mouseBtnHandler;

            string[] textFile = File.ReadAllLines(@"res\strings\" + game.Settings.Language + textName + ".txt");
            _textLines = new Text[textFile.Length];
            for (int i = 0; i < _textLines.Length; i++)
            {
                _textLines[i] = new Text(textFile[i],game.MenuFont,_textSize);
                _textLines[i].Position = new Vector2f(0, game.W.Size.Y + ( _textSize *i));
            }
        }

        public void Update()
        {
            if (_textLines[_textLines.Length - 1].Position.Y + _textSize < 0)
            {
                EndScrollingText();
            }
            foreach (Text line in _textLines)
            {
                // ReSharper disable once PossibleLossOfFraction
                line.Position = new Vector2f(0, line.Position.Y - (_game.W.Size.Y / 400));
            }
        }

        public void Draw()
        {
            foreach (Text line in _textLines)
            {
                _game.W.Draw(line);
            }
        }

        public void Uninitialize()
        {
            _game.W.MouseButtonPressed -= _mouseBtnHandler;
        }

        public void OnEvent(Settings.Action a)
        {
            EndScrollingText();
        }

        private void EndScrollingText()
        {
            _game.State = _nextState;
            _game.StopInput(600);
        }
    }
}