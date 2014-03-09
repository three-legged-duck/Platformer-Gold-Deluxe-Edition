using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class MenuState : IState
    {
        Game game;
        private static Font menuFont;
        List<string> menuList;
        List<Text> menuBtns = new List<Text>();
        Text carretLeft, carretRight;

        const int carretLeftPos = 50;
        const int carretRightPos = 25;

        int selectedPos;
        EventHandler<KeyEventArgs> KeyPressHandler;
        EventHandler<JoystickButtonEventArgs> JoyPressHandler;
        EventHandler<MouseButtonEventArgs> MouseClickHandler;
        EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        Image backgroundImage;
        Sprite backgroundSprite;
        Texture backgroundTexture;

        public void OnEvent(Settings.Action a)
        {
        }

        public MenuState (Font font,string img, params string[] menuItems)
        {
            KeyPressHandler = new EventHandler<KeyEventArgs>(onKeyPressed);
            JoyPressHandler = new EventHandler<JoystickButtonEventArgs>(onKeyPressed);
            MouseClickHandler = new EventHandler<MouseButtonEventArgs>(onMousePressed);
            MouseMoveHandler = new EventHandler<MouseMoveEventArgs>(onMouseMoved);
            menuList = new List<string>(menuItems);
            menuFont = font;
            backgroundImage = new Image(img);
            backgroundTexture = new Texture(backgroundImage);
            backgroundSprite = new Sprite(backgroundTexture);
        }

        public void Initialize(Game game)
        {
            this.game = game;
            backgroundSprite.Scale = new Vector2f(game.w.Size.X / backgroundSprite.GetGlobalBounds().Width, game.w.Size.Y / backgroundSprite.GetGlobalBounds().Height);
            game.w.KeyPressed += KeyPressHandler;
            game.w.JoystickButtonPressed += JoyPressHandler;
            game.w.MouseButtonPressed += MouseClickHandler;
            game.w.MouseMoved += MouseMoveHandler;
            carretLeft = new Text("- ", menuFont);
            carretRight = new Text(" -", menuFont);
            for (int i = 0; i < menuList.Count; i++)
            {
                Text menuItem = new Text(menuList[i], menuFont);
                uint itemWidth = (uint)menuItem.GetGlobalBounds().Width;
                uint itemHeight = menuItem.CharacterSize;

                menuItem.Position = new Vector2f(game.w.Size.X / 2 - itemWidth / 2,
                    game.w.Size.Y / 2 - menuList.Count * itemHeight / 2 + i * itemHeight);

                menuBtns.Add(menuItem);
            }
        }

        public void Update()
        {
            Text item = menuBtns[selectedPos];
            uint itemWidth = (uint)item.GetGlobalBounds().Width;
            uint itemHeight = item.CharacterSize;

            carretLeft.Position = new Vector2f(game.w.Size.X / 2 - itemWidth / 2 - carretLeftPos,
                game.w.Size.Y / 2 - menuList.Count * itemHeight / 2 + selectedPos * itemHeight);

            carretRight.Position = new Vector2f(game.w.Size.X / 2 + itemWidth / 2 + carretRightPos,
                game.w.Size.Y / 2 - menuList.Count * itemHeight / 2 + selectedPos * itemHeight);
        }

        public void Draw()
        {
            game.w.Draw(backgroundSprite);
            foreach (Text t in menuBtns)
            {
                game.w.Draw(t);
            }
            game.w.Draw(carretLeft);
            game.w.Draw(carretRight);
        }

        public void Uninitialize()
        {
            game.w.KeyPressed -= KeyPressHandler;
        }

        public void onKeyPressed(object sender, KeyEventArgs args)
        {
            onAction(game.settings.GetAction(typeof(MenuState), args.Code));
        }

        public void onKeyPressed(object sender, JoystickButtonEventArgs args)
        {
            onAction(game.settings.GetAction(typeof(MenuState), args.Button));
        }

        public void onMousePressed(object sender, MouseButtonEventArgs args)
        {
            if (args.Button != Mouse.Button.Left)
            {
                return;
            }
            for (int i = 0; i < menuBtns.Count; i++)
            {
                FloatRect realRect = getRealRect(menuBtns[i].GetGlobalBounds());
                if (realRect.Contains(args.X, args.Y))
                {
                    onAction(Settings.Action.Use);
                }
            }
        }

        public void onMouseMoved(object sender, MouseMoveEventArgs args)
        {
            for (int i = 0; i < menuBtns.Count; i++)
            {
                FloatRect realRect = getRealRect(menuBtns[i].GetGlobalBounds());
                if (realRect.Contains(args.X, args.Y))
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

        public void onAction(Settings.Action action)
        {
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

            Console.WriteLine(selectedPos);
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
