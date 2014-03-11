using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace Platformer_The_Game
{
    class Utils
    {
        /*
        public static Dictionary<K,V> CreateDict<K,V>()
        {
            return new Dictionary<K, V>();
        }
        */

        public static RectangleShape NewRectangle(float coordX, float coordY, float sizeX, float sizeY)
        {
            RectangleShape shape = new RectangleShape(new Vector2f(coordX, coordY));
            shape.Position = new Vector2f(sizeX, sizeY);
            return shape;
        }

        public static MenuState CreateMainMenu(Game game)
        {
            MenuState menu = new MenuState(game.font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", "Jouer", "Options", "Quitter");
            menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selectedPos)
                {
                    case 0:
                        game.State = new GameState();
                        break;
                    case 1:
                        game.State = CreateOptionsMenu(game, menu);
                        break;
                    case 2:
                        game.Close();
                        break;
                }
            };
            return menu;
        }

        public static MenuState CreateOptionsMenu(Game game, IState returnState)
        {
            string[] menuItems = new string[] 
            {
                "Afficher textures : " + (game.settings.DrawTextures ? "oui" : "non"),
                "Afficher masques de collision : " + (game.settings.DrawHitbox ? "oui" : "non"),
                "Retour"
            };
            MenuState options = new MenuState(game.font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", menuItems);
            
            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selectedPos)
                {
                    case 0:
                        game.settings.DrawTextures = !game.settings.DrawTextures;
                        options.menuBtns[0].DisplayedString = "Afficher textures : " + (game.settings.DrawTextures ? "oui" : "non");
                        break;
                    case 1:
                        game.settings.DrawHitbox = !game.settings.DrawHitbox;
                        options.menuBtns[1].DisplayedString = "Afficher masques de collision : " + (game.settings.DrawHitbox ? "oui" : "non");
                        break;
                    case 2:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }
    }
}
