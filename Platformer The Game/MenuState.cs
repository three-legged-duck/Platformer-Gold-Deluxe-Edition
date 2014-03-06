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
        private static Font menu_font = null;
        List<string> menu_list;
        int starting_pos;

        public static void SetFont(Font font)
        {
            menu_font = font;
        }

        public void Initialize(Game game, List<string> menu, Vector2f starting_pos, int character_size)
        {
            this.game = game;
            menu_list = menu;
            this.starting_pos = starting_pos;
        }

        public void Update()
        {

        }

        public void Draw()
        {
            int pos = starting_pos;
            foreach (string element in menu_list)
            {

            }

        }

        public void Unintialize()
        {

        }
    }
}
