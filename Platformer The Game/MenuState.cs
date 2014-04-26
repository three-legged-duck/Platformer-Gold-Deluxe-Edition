using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class MenuState : IState
    {
        public delegate void OnItemSelected(object sender, ItemSelectedEventArgs args);

        private const int CarretLeftPos = 50;
        private const int CarretRightPos = 25;
        private static Font _menuFont;
        private uint textSize;

        private readonly EventHandler<MouseButtonEventArgs> _mouseClickHandler;
        private readonly EventHandler<MouseMoveEventArgs> _mouseMoveHandler;

        //Media ressources
        private readonly Sprite _backgroundSprite;
        private readonly List<string> _menuList;
        private readonly Random _rng = new Random();
        private bool _initialized;
        private Text _carretLeft, _carretRight;
        private Game _game;
        public List<Text> MenuBtns = new List<Text>(); // FIXME : Ugly options menu hack
        private int _nextmillis;

        //Scrolling text
        private bool _scrollingTextActivated;
        private Text _scrollingText;
        private int _selectedPos;
        private string[] _textLines;
        private View _view;

        // ReSharper disable PossibleLossOfFraction
        public MenuState(Font font, string img, bool hasScrollingText, params string[] menuItems)
        {
            _scrollingTextActivated = hasScrollingText;
            _mouseClickHandler = OnMousePressed;
            _mouseMoveHandler = OnMouseMoved;
            _menuList = new List<string>(menuItems);
            _menuFont = font;
            Image backgroundImage = new Image(@"res\images\" + img);
            Texture backgroundTexture = new Texture(backgroundImage);
            _backgroundSprite = new Sprite(backgroundTexture);
        }

        public string BgMusicName
        {
            get { return "eddsworldCreditsTheme.ogg"; }
        }

        public void Initialize(Game game)
        {
            _view = game.W.DefaultView;
            _nextmillis = Environment.TickCount + 150;
            game.W.MouseButtonPressed += _mouseClickHandler;
            game.W.MouseMoved += _mouseMoveHandler;
            if (_initialized) return;
            _game = game;
            textSize = _game.W.Size.Y / 12;
            _backgroundSprite.Scale = new Vector2f(_view.Size.X/_backgroundSprite.GetLocalBounds().Width,
                _view.Size.Y/_backgroundSprite.GetLocalBounds().Height);
            _carretLeft = new Text("- ", _menuFont,textSize);
            _carretRight = new Text(" -", _menuFont, textSize);
            for (int i = 0; i < _menuList.Count; i++)
            {
                var menuItem = new Text(_menuList[i], _menuFont, textSize);
                var itemWidth = (uint) menuItem.GetLocalBounds().Width;
                uint itemHeight = menuItem.CharacterSize;

                menuItem.Position = new Vector2f(_view.Size.X/2 - itemWidth/2,
                    _view.Size.Y/2 - _menuList.Count*itemHeight/2 + i*itemHeight);

                MenuBtns.Add(menuItem);
            }
            if (_scrollingTextActivated)
            {
                _textLines = File.ReadAllLines(@"res\strings\" + game.Settings.Language + "Menu.txt");
                _scrollingText = new Text(RandomTextLine(), _menuFont,textSize);
                _scrollingText.Position = new Vector2f(_view.Size.X,
                    _view.Size.Y - (_scrollingText.GetLocalBounds().Height*2));
            }
            _initialized = true;
        }

        public void Update()
        {
            Text item = MenuBtns[_selectedPos];
            var itemWidth = (uint) item.GetLocalBounds().Width;
            uint itemHeight = item.CharacterSize;

            _carretLeft.Position = new Vector2f(_view.Size.X/2 - itemWidth/2 - CarretLeftPos,
                _view.Size.Y/2 - _menuList.Count*itemHeight/2 + _selectedPos*itemHeight);

            _carretRight.Position = new Vector2f(_view.Size.X/2 + itemWidth/2 + CarretRightPos,
                _view.Size.Y/2 - _menuList.Count*itemHeight/2 + _selectedPos*itemHeight);
        }

        public void Draw()
        {
            _game.W.SetView(_view);
            _game.W.Draw(_backgroundSprite);
            for (int i = 0; i < MenuBtns.Count; i++)
            {
                Text t = MenuBtns[i];
                if (i == _selectedPos)
                {
                    t.Color = new Color(t.Color.R, t.Color.G, t.Color.B, 255);
                }
                else
                {
                    t.Color = new Color(t.Color.R, t.Color.G, t.Color.B, 150);
                }
                _game.W.Draw(t);
            }
            _game.W.Draw(_carretLeft);
            _game.W.Draw(_carretRight);

            if (_scrollingTextActivated)
            {
                if (_scrollingText.GetGlobalBounds().Left + _scrollingText.GetGlobalBounds().Width < 0)
                {
                    _scrollingText = new Text(RandomTextLine(), _menuFont,textSize);
                    _scrollingText.Position = new Vector2f(_view.Size.X,
                        _view.Size.Y - (_scrollingText.GetLocalBounds().Height*2));
                }
                else
                {
                    _scrollingText.Position = new Vector2f(_scrollingText.Position.X - _game.W.Size.X / 160, _scrollingText.Position.Y);
                }
                _game.W.Draw(_scrollingText);
            }
        }

        public void Uninitialize()
        {
            _game.W.MouseButtonPressed -= _mouseClickHandler;
            _game.W.MouseMoved -= _mouseMoveHandler;
        }

        public void OnEvent(Settings.Action action)
        {
            if (_nextmillis > Environment.TickCount)
            {
                return;
            }
            switch (action)
            {
                case Settings.Action.Up:
                    _selectedPos = _selectedPos - 1;
                    break;
                case Settings.Action.Down:
                    _selectedPos = _selectedPos + 1;
                    break;
                case Settings.Action.Use:
                    if (ItemSelected != null)
                    {
                        ItemSelected(this, new ItemSelectedEventArgs(_selectedPos, _menuList));
                    }
                    break;
            }

            if (_selectedPos < 0)
            {
                _selectedPos = _menuList.Count - 1;
            }
            if (_selectedPos >= _menuList.Count)
            {
                _selectedPos = 0;
            }

            _nextmillis = Environment.TickCount + 150;
        }

        private string RandomTextLine()
        {
            return _textLines[_rng.Next(0, _textLines.Length)];
        }

        public void OnMousePressed(object sender, MouseButtonEventArgs args)
        {
            if (args.Button != Mouse.Button.Left)
            {
                return;
            }
            for (int i = 0; i < MenuBtns.Count; i++)
            {
                Vector2f mousePos = _game.W.MapPixelToCoords(new Vector2i(args.X, args.Y));
                FloatRect realRect = getRealRect(MenuBtns[i].GetGlobalBounds());
                if (realRect.Contains(mousePos.X, mousePos.Y))
                {
                    OnEvent(Settings.Action.Use);
                }
            }
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs args)
        {
            for (int i = 0; i < MenuBtns.Count; i++)
            {
                Vector2f mousePos = _game.W.MapPixelToCoords(new Vector2i(args.X, args.Y));
                FloatRect realRect = getRealRect(MenuBtns[i].GetGlobalBounds());
                if (realRect.Contains(mousePos.X, mousePos.Y))
                {
                    _selectedPos = i;
                }
            }
        }

        private FloatRect getRealRect(FloatRect fr)
        {
            return new FloatRect(fr.Left - CarretLeftPos, fr.Top,
                fr.Width + CarretLeftPos + CarretRightPos, fr.Height);
        }

        public event OnItemSelected ItemSelected;

        public class ItemSelectedEventArgs : EventArgs
        {
            public List<string> MenuList;
            public int SelectedPos;

            public ItemSelectedEventArgs(int selectedPos, List<string> menuList)
            {
                MenuList = menuList;
                SelectedPos = selectedPos;
            }
        }
    }
}
// ReSharper restore PossibleLossOfFraction