using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using System.IO;

namespace Platformer_The_Game
{
    class MenuState : IState
    {
        const int carretLeftPos = 50;
        const int carretRightPos = 25;

        Game game;
        View view;

        List<string> menuList;
        public List<Text> menuBtns = new List<Text>(); // FIXME : Ugly options menu hack
        Text carretLeft, carretRight;
        int selectedPos;

        EventHandler<MouseButtonEventArgs> MouseClickHandler;
        EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        //Media ressources
        private static Font menuFont;
        public string BgMusicName { get { return "eddsworldCreditsTheme.ogg"; } }
        Image backgroundImage;
        Sprite backgroundSprite;
        Texture backgroundTexture;

        //Scrolling text
        Text scrollingText;
        string[] textLines;
        Random rng = new Random();

        public MenuState (Font font,string img, string music, params string[] menuItems)
        {
            MouseClickHandler = onMousePressed;
            MouseMoveHandler = onMouseMoved;
            menuList = new List<string>(menuItems);
            menuFont = font;
            backgroundImage = new Image(img);
            backgroundTexture = new Texture(backgroundImage);
            backgroundSprite = new Sprite(backgroundTexture);
        }

        bool Initialized = false;
        public void Initialize(Game game)
        {
            view = game.w.DefaultView;
            nextmillis = System.Environment.TickCount + 150;
            game.w.MouseButtonPressed += MouseClickHandler;
            game.w.MouseMoved += MouseMoveHandler; if (Initialized) return;
            this.game = game;
            backgroundSprite.Scale = new Vector2f(view.Size.X / backgroundSprite.GetLocalBounds().Width, view.Size.Y / backgroundSprite.GetLocalBounds().Height);
            carretLeft = new Text("- ", menuFont);
            carretRight = new Text(" -", menuFont);
            for (int i = 0; i < menuList.Count; i++)
            {
                Text menuItem = new Text(menuList[i], menuFont);
                uint itemWidth = (uint)menuItem.GetLocalBounds().Width;
                uint itemHeight = menuItem.CharacterSize;

                menuItem.Position = new Vector2f(view.Size.X / 2 - itemWidth / 2,
                    view.Size.Y / 2 - menuList.Count * itemHeight / 2 + i * itemHeight);

                menuBtns.Add(menuItem);
            }
            textLines = File.ReadAllLines(game.settings.language.ToString() + "Menu.txt");
            scrollingText = new Text(RandomTextLine(),menuFont);
            scrollingText.Position = new Vector2f(view.Size.X, view.Size.Y - (scrollingText.GetLocalBounds().Height * 2));
            Initialized = true;
        }

        public void Update()
        {
            Text item = menuBtns[selectedPos];
            uint itemWidth = (uint)item.GetLocalBounds().Width;
            uint itemHeight = item.CharacterSize;

            carretLeft.Position = new Vector2f(view.Size.X / 2 - itemWidth / 2 - carretLeftPos,
                view.Size.Y / 2 - menuList.Count * itemHeight / 2 + selectedPos * itemHeight);

            carretRight.Position = new Vector2f(view.Size.X / 2 + itemWidth / 2 + carretRightPos,
                view.Size.Y / 2 - menuList.Count * itemHeight / 2 + selectedPos * itemHeight);
        }

        public void Draw()
        {
            game.w.SetView(view);
            game.w.Draw(backgroundSprite);
            for (int i = 0;i < menuBtns.Count;i++)
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
                scrollingText.Position = new Vector2f(view.Size.X, view.Size.Y - (scrollingText.GetLocalBounds().Height * 2));
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

        int nextmillis;
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

        public class ItemSelectedEventArgs : EventArgs
        {
            public ItemSelectedEventArgs(int selectedPos, List<string> menuList)
            {
                this.menuList = menuList;
                this.selectedPos = selectedPos;
            }
            public List<string> menuList;
            public int selectedPos;
        }
        public delegate void onItemSelected(object sender, ItemSelectedEventArgs args);
        public event onItemSelected ItemSelected;
    }
}
