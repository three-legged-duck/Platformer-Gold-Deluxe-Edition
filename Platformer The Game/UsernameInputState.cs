using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SFML.Graphics;
using SFML.Window;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using View = SFML.Graphics.View;

namespace Platformer_The_Game
{
    internal class UsernameInputState : IState
    {
        private Sprite _backgroundSprite;
        private Game _game;
        private string username;
        private Text helpText;
        private Text usernameText;

        public string BgMusicName
        {
            get { return null; }
        }

        public void Initialize(Game game)
        {
            _game = game;
            _game.W.TextEntered += OnTextEntered;
            helpText = new Text("Please type your username : ", _game.MenuFont, _game.MenuTextSize/2);
            uint tWidth = (uint) helpText.GetLocalBounds().Width;
            uint tHeight = (uint) helpText.GetLocalBounds().Height;
            helpText.Position = new Vector2f((_game.W.Size.X/2) - (tWidth/2),
                (_game.W.Size.Y/2) - (tHeight/2));
            usernameText = new Text("", _game.MenuFont, _game.MenuTextSize/2);
            Image backgroundImage = new Image(@"res\images\menuBg.bmp");
            Texture backgroundTexture = new Texture(backgroundImage);
            _backgroundSprite = new Sprite(backgroundTexture);
            _backgroundSprite.Scale = new Vector2f(_game.W.Size.X/_backgroundSprite.GetLocalBounds().Width,
                _game.W.Size.Y/_backgroundSprite.GetLocalBounds().Height);
        }

        public void Update()
        {
            uint tWidth = (uint) helpText.GetLocalBounds().Width;
            usernameText.DisplayedString = username;
            uint tHeight = (uint) helpText.GetLocalBounds().Height;
            usernameText.Position = new Vector2f(_game.W.Size.X/2 - (tWidth/2), (_game.W.Size.Y/3) - (tHeight/2));
        }

        public void Draw()
        {
            _game.W.Draw(_backgroundSprite);
            _game.W.Draw(helpText);
            _game.W.Draw(usernameText);
        }

        public void Uninitialize()
        {
            _game.W.TextEntered -= OnTextEntered;
        }

        public void OnEvent(Settings.Action a)
        {
            if (a == Settings.Action.Use)
            {
                _game.State = OptionsMenu.CreateOptionsMenu(_game, Utils.CreateMainMenu(_game));
            }
        }

        private void OnTextEntered(object sender, TextEventArgs e)
        {
            if (e.Unicode == "\b" && username.Length != 0)
            {
                username = username.Remove(username.Length - 1);
            }
            else if (Char.IsLetterOrDigit((e.Unicode[0])))
            {
                username += e.Unicode.ToUpper();
            }
        }
    }
}