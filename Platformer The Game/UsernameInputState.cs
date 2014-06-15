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
        private string username = "";
        private Text[] textlines = new Text[2];

        public string BgMusicName
        {
            get { return null; }
        }

        public void Initialize(Game game)
        {
            _game = game;
            _game.W.TextEntered += OnTextEntered;
            _game.W.KeyReleased += OnKeyReleased;
            username = _game.Settings.Username;
            textlines[0] = new Text(Utils.GetString("usernameHelp",_game), _game.MenuFont, _game.MenuTextSize / 2);
            textlines[1] = new Text("", _game.MenuFont, _game.MenuTextSize / 2);
            Image backgroundImage = new Image(@"res\images\menuBg.bmp");
            Texture backgroundTexture = new Texture(backgroundImage);
            _backgroundSprite = new Sprite(backgroundTexture);
            _backgroundSprite.Scale = new Vector2f(_game.W.Size.X/_backgroundSprite.GetLocalBounds().Width,
                _game.W.Size.Y/_backgroundSprite.GetLocalBounds().Height);
        }

        public void Update()
        {
            textlines[1].DisplayedString = username;
            for (int i = 0; i < textlines.Length; i++)
            {
                Text text = textlines[i];
                uint iWidth = (uint)text.GetLocalBounds().Width;
                uint iHeight = text.CharacterSize;
                text.Position = new Vector2f(_game.W.Size.X / 2 - iWidth / 2,
                    _game.W.Size.Y / 2 - textlines.Length * iHeight / 2 + i * iHeight);
            }
        }

        public void Draw()
        {
            _game.W.Draw(_backgroundSprite);
            foreach (Text textline in textlines)
            {
                _game.W.Draw(textline);
            }
        }

        public void Uninitialize()
        {
            _game.W.TextEntered -= OnTextEntered;
            _game.W.KeyReleased -= OnKeyReleased;
        }

        public void OnEvent(Settings.Action a)
        {
        }

        public void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Return && !String.IsNullOrWhiteSpace(username))
            {
                _game.Settings.Username = username.ToUpper();
                _game.State = OptionsMenu.CreateOptionsMenu(_game, Utils.CreateMainMenu(_game));
            }
        }

        private void OnTextEntered(object sender, TextEventArgs e)
        {
            if (e.Unicode == "\b" && username.Length != 0)
            {
                username = username.Remove(username.Length - 1);
            }
            else if (Char.IsLetterOrDigit((e.Unicode[0])) && username.Length <= 17)
            {
                username += e.Unicode.ToUpper();
            }
        }
    }
}