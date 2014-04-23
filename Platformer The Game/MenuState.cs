using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class MenuState : IState
    {
        public delegate void onItemSelected(object sender, ItemSelectedEventArgs args);

        private const int carretLeftPos = 50;
        private const int carretRightPos = 25;
        private static Font menuFont;

        private readonly EventHandler<MouseButtonEventArgs> MouseClickHandler;
        private readonly EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        //Media ressources
        private readonly Sprite backgroundSprite;
        private readonly List<string> menuList;
        private readonly Random rng = new Random();
        private bool Initialized;
        private Image backgroundImage;
        private Texture backgroundTexture;
        private Text carretLeft, carretRight;
        private Game game;
        public List<Text> menuBtns = new List<Text>(); // FIXME : Ugly options menu hack
        private int nextmillis;

        //Scrolling text
        private Text scrollingText;
        private int selectedPos;
        private string[] textLines;
        private View view;

        public MenuState(Font font, string img, string music, params string[] menuItems)
        {
            MouseClickHandler = onMousePressed;
            MouseMoveHandler = onMouseMoved;
            menuList = new List<string>(menuItems);
            menuFont = font;
            backgroundImage = new Image(@"res\images\" + img);
            backgroundTexture = new Texture(backgroundImage);
            backgroundSprite = new Sprite(backgroundTexture);
        }

        public string BgMusicName
        {
            get { return "eddsworldCreditsTheme.ogg"; }
        }

        public void Initialize(Game game)
        {
            view = game.w.DefaultView;
            nextmillis = Environment.TickCount + 150;
            game.w.MouseButtonPressed += MouseClickHandler;
            game.w.MouseMoved += MouseMoveHandler;
            if (Initialized) return;
            this.game = game;
            backgroundSprite.Scale = new Vector2f(view.Size.X/backgroundSprite.GetLocalBounds().Width,
                view.Size.Y/backgroundSprite.GetLocalBounds().Height);
            carretLeft = new Text("- ", menuFont);
            carretRight = new Text(" -", menuFont);
            for (int i = 0; i < menuList.Count; i++)
            {
                var menuItem = new Text(menuList[i], menuFont);
                var itemWidth = (uint) menuItem.GetLocalBounds().Width;
                uint itemHeight = menuItem.CharacterSize;

                menuItem.Position = new Vector2f(view.Size.X/2 - itemWidth/2,
                    view.Size.Y/2 - menuList.Count*itemHeight/2 + i*itemHeight);

                menuBtns.Add(menuItem);
            }
            textLines = File.ReadAllLines(@"res\strings\" + game.settings.language + "Menu.txt");
            scrollingText = new Text(RandomTextLine(), menuFont);
            scrollingText.Position = new Vector2f(view.Size.X, view.Size.Y - (scrollingText.GetLocalBounds().Height*2));
            Initialized = true;
        }

        public void Update()
        {
            Text item = menuBtns[selectedPos];
            var itemWidth = (uint) item.GetLocalBounds().Width;
            uint itemHeight = item.CharacterSize;

            carretLeft.Position = new Vector2f(view.Size.X/2 - itemWidth/2 - carretLeftPos,
                view.Size.Y/2 - menuList.Count*itemHeight/2 + selectedPos*itemHeight);

            carretRight.Position = new Vector2f(view.Size.X/2 + itemWidth/2 + carretRightPos,
                view.Size.Y/2 - menuList.Count*itemHeight/2 + selectedPos*itemHeight);
        }

        public void Draw()
        {
            game.w.SetView(view);
            game.w.Draw(backgroundSprite);
            for (int i = 0; i < menuBtns.Count; i++)
            {
                Text t = menuBtns[i];
                if (i == selectedPos)
                {
                    t.Color = new Color(t.Color.R, t.Color.G, t.Color.B, 255);
                }
                else
                {
                    t.Color = new Color(t.Color.R, t.Color.G, t.Color.B, 150);
                }
                game.w.Draw(t);
            }
            game.w.Draw(carretLeft);
            game.w.Draw(carretRight);

            if (scrollingText.GetGlobalBounds().Left + scrollingText.GetGlobalBounds().Width < 0)
            {
                scrollingText = new Text(RandomTextLine(), menuFont);
                scrollingText.Position = new Vector2f(view.Size.X,
                    view.Size.Y - (scrollingText.GetLocalBounds().Height*2));
            }
            else
            {
                scrollingText.Position = new Vector2f(scrollingText.Position.X - 5, scrollingText.Position.Y);
            }
            game.w.Draw(scrollingText);
        }

        public void Uninitialize()
        {
            game.w.MouseButtonPressed -= MouseClickHandler;
            game.w.MouseMoved -= MouseMoveHandler;
        }

        public void OnEvent(Settings.Action action)
        {
            if (nextmillis > Environment.TickCount)
            {
                return;
            }
            switch (action)
            {
                case Settings.Action.Up:
                    selectedPos = selectedPos - 1;
                    break;
                case Settings.Action.Down:
                    selectedPos = selectedPos + 1;
                    break;
                case Settings.Action.Use:
                    if (ItemSelected != null)
                    {
                        ItemSelected(this, new ItemSelectedEventArgs(selectedPos, menuList));
                    }
                    break;
            }

            if (selectedPos < 0)
            {
                selectedPos = menuList.Count - 1;
            }
            if (selectedPos >= menuList.Count)
            {
                selectedPos = 0;
            }

            nextmillis = Environment.TickCount + 150;
        }

        private string RandomTextLine()
        {
            return textLines[rng.Next(0, textLines.Length)];
        }

        public void onMousePressed(object sender, MouseButtonEventArgs args)
        {
            if (args.Button != Mouse.Button.Left)
            {
                return;
            }
            for (int i = 0; i < menuBtns.Count; i++)
            {
                Vector2f mousePos = game.w.MapPixelToCoords(new Vector2i(args.X, args.Y));
                FloatRect realRect = getRealRect(menuBtns[i].GetGlobalBounds());
                if (realRect.Contains(mousePos.X, mousePos.Y))
                {
                    OnEvent(Settings.Action.Use);
                }
            }
        }

        public void onMouseMoved(object sender, MouseMoveEventArgs args)
        {
            for (int i = 0; i < menuBtns.Count; i++)
            {
                Vector2f mousePos = game.w.MapPixelToCoords(new Vector2i(args.X, args.Y));
                FloatRect realRect = getRealRect(menuBtns[i].GetGlobalBounds());
                if (realRect.Contains(mousePos.X, mousePos.Y))
                {
                    selectedPos = i;
                }
            }
        }

        private FloatRect getRealRect(FloatRect fr)
        {
            return new FloatRect(fr.Left - carretLeftPos, fr.Top,
                fr.Width + carretLeftPos + carretRightPos, fr.Height);
        }

        public event onItemSelected ItemSelected;

        public class ItemSelectedEventArgs : EventArgs
        {
            public List<string> menuList;
            public int selectedPos;

            public ItemSelectedEventArgs(int selectedPos, List<string> menuList)
            {
                this.menuList = menuList;
                this.selectedPos = selectedPos;
            }
        }
    }
}