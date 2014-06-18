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

        public EventHandler<MouseButtonEventArgs> MouseClickHandler;
        public EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        //Media ressources
        private readonly Sprite _backgroundSprite;
        private bool _initialized;
        private Text _carretLeft, _carretRight;
        private Game _game;
        private List<Text> MenuBtns = new List<Text>();
        private int _nextmillis;
        private View _view;
        private int _selectedPos;

        //Scrolling text
        public bool ScrollingTextActivated;

        public string BgMusicName
        {
            get { return "eddsworldCreditsTheme.ogg"; }
        }


        // ReSharper disable PossibleLossOfFraction
        public MenuState(Font font, string img, bool hasScrollingText, params string[] menuItems)
        {
            ScrollingTextActivated = hasScrollingText;
            MouseClickHandler = OnMousePressed;
            MouseMoveHandler = OnMouseMoved;
            _menuFont = font;
            List<string> menuList = new List<string>(menuItems);
            for (int i = 0; i < menuList.Count; i++)
            {
                var menuItem = new Text(menuList[i], _menuFont, 0);
                MenuBtns.Add(menuItem);
            }
            Image backgroundImage = new Image(@"res\images\" + img);
            Texture backgroundTexture = new Texture(backgroundImage);
            _backgroundSprite = new Sprite(backgroundTexture);
        }

        public void Initialize(Game game)
        {
            _view = game.W.DefaultView;
            _nextmillis = Environment.TickCount + 150;
            game.W.MouseButtonPressed += MouseClickHandler;
            game.W.MouseMoved += MouseMoveHandler;
            if (_initialized) return;
            _game = game;
            _carretLeft = new Text("- ", _menuFont, game.MenuTextSize);
            _carretRight = new Text(" -", _menuFont, game.MenuTextSize);
            foreach (Text btn in MenuBtns)
            {
                btn.CharacterSize = game.MenuTextSize;
            }
            _initialized = true;
        }

        public void Update()
        {

            _carretLeft.CharacterSize = _game.MenuTextSize;
            _carretRight.CharacterSize = _game.MenuTextSize;
            for (int i = 0; i < MenuBtns.Count; i++)
            {
                Text text = MenuBtns[i];
                text.CharacterSize = _game.MenuTextSize;
                uint iWidth = (uint) text.GetLocalBounds().Width;
                uint iHeight = text.CharacterSize;
                text.Position = new Vector2f(_view.Size.X/2 - iWidth/2,
                    _view.Size.Y / 2 - MenuBtns.Count * iHeight / 2 + i * iHeight);
            }

            _backgroundSprite.Scale = new Vector2f(_view.Size.X/_backgroundSprite.GetLocalBounds().Width,
                _view.Size.Y/_backgroundSprite.GetLocalBounds().Height);

            Text item = MenuBtns[_selectedPos];
            var itemWidth = (uint) item.GetLocalBounds().Width;
            uint itemHeight = item.CharacterSize;

            _carretLeft.Position = new Vector2f(_view.Size.X/2 - itemWidth/2 - CarretLeftPos,
                _view.Size.Y / 2 - MenuBtns.Count * itemHeight / 2 + _selectedPos * itemHeight);

            _carretRight.Position = new Vector2f(_view.Size.X/2 + itemWidth/2 + CarretRightPos,
                _view.Size.Y / 2 - MenuBtns.Count * itemHeight / 2 + _selectedPos * itemHeight);
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
        }

        public void ModifyElement(int pos,string text)
        {
            MenuBtns[pos].DisplayedString = text;
        }

        public void Uninitialize()
        {
            _game.W.MouseButtonPressed -= MouseClickHandler;
            _game.W.MouseMoved -= MouseMoveHandler;
            _game.StopInput(100);
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
                        ItemSelected(this, new ItemSelectedEventArgs(_selectedPos, MenuBtns));
                    }
                    break;
            }

            if (_selectedPos < 0)
            {
                _selectedPos = MenuBtns.Count - 1;
            }
            if (_selectedPos >= MenuBtns.Count)
            {
                _selectedPos = 0;
            }

            _nextmillis = Environment.TickCount + 150;
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
            public List<Text> MenuList;
            public int SelectedPos;

            public ItemSelectedEventArgs(int selectedPos, List<Text> menuList)
            {
                MenuList = menuList;
                SelectedPos = selectedPos;
            }
        }
    }
}
// ReSharper restore PossibleLossOfFraction