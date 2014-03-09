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
        private static Font menu_font;
        List<string> menu_list;
        int selected_pos;
        EventHandler<KeyEventArgs> KeyPressHandler;
        EventHandler<JoystickButtonEventArgs> JoyPressHandler;

        Image background_image;
        Sprite background_sprite;
        Texture background_texture;

        public void OnEvent(Settings.Action a)
        {
        }

        public MenuState (Font font,string img, params string[] menuItems)
        {
            KeyPressHandler = new EventHandler<KeyEventArgs>(onKeyPressed);
            JoyPressHandler = new EventHandler<JoystickButtonEventArgs>(onKeyPressed);
            menu_list = new List<string>(menuItems);
            menu_font = font;
            background_image = new Image(img);
            background_texture = new Texture(background_image);
            //800 x 600
            //? x ?
            background_sprite = new Sprite(background_texture);
        }

        public void Initialize(Game game)
        {
            this.game = game;
            background_sprite.Scale = new Vector2f(game.w.Size.X / background_sprite.GetGlobalBounds().Width, game.w.Size.Y / background_sprite.GetGlobalBounds().Height);
            game.w.KeyPressed += KeyPressHandler;
            game.w.JoystickButtonPressed += JoyPressHandler;
        }

        public void Update()
        {

        }

        public void Draw()
        {
            game.w.Draw(background_sprite);
            for (int i = 0; i < menu_list.Count; i++)
            {
                Text menu_item;
                menu_item = new Text(menu_list.ElementAt(i), menu_font);
                uint item_width = (uint)menu_item.GetGlobalBounds().Width;
                uint item_height = menu_item.CharacterSize;

                menu_item.Position = new Vector2f(game.w.Size.X / 2 - item_width / 2, 
                    game.w.Size.Y / 2 - menu_list.Count * item_height / 2 + i * item_height);

                game.w.Draw(menu_item);

               if (i == selected_pos)
                {
                    Text selected = new Text("- ", menu_font);
                    selected.Position = new Vector2f(game.w.Size.X / 2 - item_width / 2 - 50,
                        game.w.Size.Y / 2 - menu_list.Count * item_height / 2 + i * item_height);
                    Text selected2 = new Text(" -", menu_font);
                    selected2.Position = new Vector2f(game.w.Size.X / 2 + item_width / 2 + 25,
                        game.w.Size.Y / 2 - menu_list.Count * item_height / 2 + i * item_height);
                    game.w.Draw(selected);
                    game.w.Draw(selected2);
                }
            }
        }

        public void Uninitialize()
        {
            game.w.KeyPressed -= KeyPressHandler;
        }

        public void onKeyPressed(object sender, KeyEventArgs args)
        {
            onAction(game.settings.GetAction(args.Code));
        }

        public void onKeyPressed(object sender, JoystickButtonEventArgs args)
        {
            onAction(game.settings.GetAction(args.Button));
        }

        public void onAction(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Up:
                    selected_pos = selected_pos - 1;
                    break;
                case Settings.Action.Down:
                    selected_pos = selected_pos + 1;
                    break;
                case Settings.Action.Use:
                    if (ItemSelected != null)
                    {
                        ItemSelected(this, new ItemSelectedEventArgs(selected_pos, menu_list));
                    }
                    break;
            }

            if (selected_pos < 0)
            {
                selected_pos = menu_list.Count - 1;
            }
            if (selected_pos >= menu_list.Count)
            {
                selected_pos = 0;
            }

            Console.WriteLine(selected_pos);
        }

        public class ItemSelectedEventArgs : EventArgs
        {
            public ItemSelectedEventArgs(int selected_pos, List<string> menu_list)
            {
                this.menu_list = menu_list;
                this.selected_pos = selected_pos;
            }
            public List<string> menu_list;
            public int selected_pos;
        }
        public delegate void onItemSelected(object sender, ItemSelectedEventArgs args);
        public event onItemSelected ItemSelected;
    }
}
