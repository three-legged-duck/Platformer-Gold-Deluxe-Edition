using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using System.Xml;
using System.IO;

namespace Platformer_The_Game
{
    class Utils
    {
        public enum Language { French, English };


        //Translation files
        public static XmlDocument frenchStrings = new XmlDocument();
        public static XmlDocument englishStrings = new XmlDocument();

        public static void LoadTranslations()
        {
            frenchStrings.Load("French.xml");
            englishStrings.Load("English.xml");
        }

        public static string GetString(string key, Game game)
        {
            string nodeSearch = "//string[@name='" + key + "']";
            XmlNodeList translatedString;
            switch (game.settings.language)
            {
                case Language.French:
                    translatedString = frenchStrings.SelectNodes(nodeSearch);
                    break;
                default:
                    translatedString = englishStrings.SelectNodes(nodeSearch);
                    break;
            }
            try { return translatedString.Item(0).InnerText; }
            catch
            {
                try { return (englishStrings.SelectNodes(nodeSearch)).Item(0).InnerText; }
                catch { return key; }
            }
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
