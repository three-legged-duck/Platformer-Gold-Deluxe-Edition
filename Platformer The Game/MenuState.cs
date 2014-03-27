using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;

namespace Platformer_The_Game
{
    class MenuState : IState
    {
        Game game;
        View view;

        private static Font menuFont;
        List<string> menuList;
        public List<Text> menuBtns = new List<Text>(); // FIXME : Ugly options menu hack
        Text carretLeft, carretRight;
        Music backgroundMusic;

        const int carretLeftPos = 50;
        const int carretRightPos = 25;

        int selectedPos;
        EventHandler<MouseButtonEventArgs> MouseClickHandler;
        EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        Image backgroundImage;
        Sprite backgroundSprite;
        Texture backgroundTexture;

        public MenuState (Font font,string img, string music, params string[] menuItems)
        {
            MouseClickHandler = new EventHandler<MouseButtonEventArgs>(onMousePressed);
            MouseMoveHandler = new EventHandler<MouseMoveEventArgs>(onMouseMoved);
            menuList = new List<string>(menuItems);
            menuFont = font;
            backgroundImage = new Image(img);
            backgroundTexture = new Texture(backgroundImage);
            backgroundSprite = new Sprite(backgroundTexture);
            backgroundMusic = new Music(music);
            backgroundMusic.Loop = true;
        }



        bool Initialized = false;
        public void Initialize(Game game)
        {
            view = game.w.DefaultView;
            nextmillis = System.Environment.TickCount + 150;
            backgroundMusic.Play();
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
        }

        public void Uninitialize()
        {
            game.w.MouseButtonPressed -= MouseClickHandler;
            game.w.MouseMoved -= MouseMoveHandler;
            backgroundMusic.Stop();
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
