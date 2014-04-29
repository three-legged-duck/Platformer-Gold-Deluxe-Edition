using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using View = SFML.Graphics.View;

namespace Platformer_The_Game
{
    class KeyBindingState : IState
    {
        private View _view;
        private Game _game;
        private IState nextState;

        private readonly Sprite _backgroundSprite;
        private Text _currentText;
        private uint _textSize;

        private SettingsToModify[] settingsArray;
        private int _currentModified;
        private int _currentModifiedAction;

        private class SettingsToModify
        {
            public Settings.Action[] Actions;
            public Type State;

            public SettingsToModify(Settings.Action[] a, Type s)
            {
                Actions = a;
                State = s;
            }
        }

        public string BgMusicName
        {
            get { return "eddsworldCreditsTheme.ogg"; }
        }

        public KeyBindingState(IState nextState)
        {
            this.nextState = nextState;
            Image backgroundImage = new Image(@"res\images\menuBg.bmp");
            Texture backgroundTexture = new Texture(backgroundImage);
            _backgroundSprite = new Sprite(backgroundTexture);

            SettingsToModify menuModify = new SettingsToModify(new[]{Settings.Action.Use},typeof(MenuState));
            SettingsToModify gameModify = new SettingsToModify(
                new[]
                {
                    Settings.Action.Use,
                    Settings.Action.Jump,
                    Settings.Action.Run,
                    Settings.Action.Pause
                },typeof(GameState));
            SettingsToModify defaultModify = new SettingsToModify(
                new[]
                {
                    Settings.Action.Up,
                    Settings.Action.Down,
                    Settings.Action.Left,
                    Settings.Action.Right,
                },typeof(Settings.Default));

            settingsArray = new SettingsToModify[3] {menuModify,gameModify,defaultModify};
        }

        public void Initialize(Game game)
        {
            _game = game;
            _game.W.KeyReleased += OnKeyReleased;
            _view = game.W.DefaultView;
            _backgroundSprite.Scale = new Vector2f(_view.Size.X / _backgroundSprite.GetLocalBounds().Width,_view.Size.Y / _backgroundSprite.GetLocalBounds().Height);
            _game.Settings.ResetKeys();

            _textSize = _game.W.Size.Y / 18;
        }

        public void Update()
        {
            Type currentType = settingsArray[_currentModified].State;
            string currentModifiedText =
                Utils.GetString(settingsArray[_currentModified].Actions[_currentModifiedAction].ToString(), _game) +
                " (" + Utils.GetString(currentType.Name, _game) + ")";
            _currentText = new Text(currentModifiedText, _game.MenuFont, _textSize);

            CenterText();
        }

        public void Draw()
        {
            _game.W.SetView(_view);
            _game.W.Draw(_backgroundSprite);
            _game.W.Draw(_currentText);
        }

        public void Uninitialize()
        {
            _game.W.KeyReleased -= OnKeyReleased;
        }

        public void OnEvent(Settings.Action a)
        {
        }

        private void CenterText()
        {
            var itemWidth = (uint)_currentText.GetLocalBounds().Width;
            var itemHeight = (uint)_currentText.GetLocalBounds().Height;
            _currentText.Position = new Vector2f((_view.Size.X / 2) - (itemWidth / 2), (_view.Size.Y / 2) - itemHeight);
        }

        void OnKeyReleased(object sender, KeyEventArgs e)
        {
            try
            {
                _game.Settings.SetButton(settingsArray[_currentModified].State, e.Code,
                    settingsArray[_currentModified].Actions[_currentModifiedAction]);
                if (_currentModifiedAction < settingsArray[_currentModified].Actions.Length -1)
                {
                    _currentModifiedAction++;
                }
                else if (_currentModified < settingsArray.Length -1)
                {
                    //Go to the next set of modifications
                    _currentModified++;
                    _currentModifiedAction = 0;
                }
                else
                {
                    _game.State = nextState;
                }
                _game.StopInput(10);
            }
            catch
            {
            }

        }
    }
}
