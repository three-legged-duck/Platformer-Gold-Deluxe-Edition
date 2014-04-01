using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using System.Xml;
using System.IO;
using System.Text;

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

        public enum Language { French, English };

        public static string GetString(string key, Game game)
        {
            
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(game.settings.language.ToString() + ".xml"); // load the file according to the current language

            XmlNodeList nodes = xDoc.SelectNodes("//string[@name='" + key + "']");
            return nodes.Item(0).InnerText;
        }

        public static RectangleShape NewRectangle(float coordX, float coordY, float sizeX, float sizeY)
        {
            RectangleShape shape = new RectangleShape(new Vector2f(coordX, coordY));
            shape.Position = new Vector2f(sizeX, sizeY);
            return shape;
        }

        public static MenuState CreateMainMenu(Game game)
        {
            MenuState menu = new MenuState(game.font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", GetString("play", game), GetString("settings", game), GetString("quit", game));
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
                GetString("drawTextures", game) + " : "+ (game.settings.DrawTextures ? GetString("yes", game) : GetString("no", game)),
                GetString("drawHitboxes", game) + " : "+ (game.settings.DrawHitbox ? GetString("yes", game) : GetString("no", game)),
                GetString("language", game) + " : "+ (game.settings.language == Language.English ? Language.English.ToString() : Language.French.ToString()),
                "Retour"
            };
            MenuState options = new MenuState(game.font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", menuItems);
            
            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selectedPos)
                {
                    case 0:
                        game.settings.DrawTextures = !game.settings.DrawTextures;
                        options.menuBtns[0].DisplayedString = GetString("drawTextures", game) + " : " + (game.settings.DrawTextures ? GetString("yes", game) : GetString("no", game));
                        break;
                    case 1:
                        game.settings.DrawHitbox = !game.settings.DrawHitbox;
                        options.menuBtns[1].DisplayedString = GetString("drawHitboxes", game) + " : " + (game.settings.DrawHitbox ? GetString("yes", game) : GetString("no", game));
                        break;
                    case 2:
                        if (game.settings.language == Language.English)
                        {
                            game.settings.language = Language.French;
                        }
                        else
                        {
                            game.settings.language = Language.English;
                        }
                        options.menuBtns[2].DisplayedString = GetString("language", game) + " : " + (game.settings.language == Language.English ? Language.English.ToString() : Language.French.ToString());
                        break;
                    case 3:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }
    }
}
