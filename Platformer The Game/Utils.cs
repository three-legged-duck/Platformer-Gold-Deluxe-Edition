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
            MenuState menu = new MenuState(game.menuFont, "menuBg.bmp", "eddsworldCreditsTheme.ogg", GetString("play", game), GetString("settings", game), GetString("quit", game));
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
                GetOptionText("drawTextures", game.settings.drawTextures, game),
                GetOptionText("drawHitboxes", game.settings.drawHitbox, game),
                GetString("language", game) + " : "+ (game.settings.language == Language.English ? Language.English.ToString() : Language.French.ToString()),
                GetOptionText("fullscreen", game.settings.fullscreen, game),
                GetString("back",game)
            };
            MenuState options = new MenuState(game.menuFont, "menuBg.bmp", "eddsworldCreditsTheme.ogg", menuItems);
            
            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selectedPos)
                {
                    case 0:
                        game.settings.drawTextures = !game.settings.drawTextures;
                        options.menuBtns[0].DisplayedString = GetOptionText("drawTextures", game.settings.drawTextures, game);
                        break;
                    case 1:
                        game.settings.drawHitbox = !game.settings.drawHitbox;
                        options.menuBtns[1].DisplayedString = GetOptionText("drawHitboxes", game.settings.drawHitbox, game);
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
                    case 3 :
                        game.settings.fullscreen = !game.settings.fullscreen;
                        game.RecreateWindow();
                        options.menuBtns[3].DisplayedString = GetOptionText("fullscreen", game.settings.fullscreen, game);
                        break;
                    case 4:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        private static string GetOptionText(string key, bool value, Game g)
        {
            return GetString(key, g) + " : " + (value ? GetString("yes", g) : GetString("no", g));
        }
    }
}
