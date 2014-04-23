using System;
using System.IO;
using System.Text;
using System.Xml;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal static class Utils
    {
        public enum Language
        {
            French,
            English
        };

        //Translation files
        public static XmlDocument frenchStrings = new XmlDocument();
        public static XmlDocument englishStrings = new XmlDocument();

        public static long ReadVarInt(this Stream stream)
        {
            long result = 0;
            int shift = 0;
            int b;
            while ((b = stream.ReadByte()) != -1)
            {
                result |= (b & 0x7F) << shift++*7;

                if (shift > 5) throw new Exception("VarInt too big");

                if ((b & 0x80) != 128) break;
            }

            return result;
        }

        public static void WriteVarInt(this Stream stream, long value)
        {
            while (true)
            {
                if ((value & 0xFFFFFF80) == 0)
                {
                    stream.WriteByte((byte) value);
                    return;
                }

                stream.WriteByte((byte) (value & 0x7F | 0x80));
                value >>= 7;
            }
        }

        public static string ReadString(this Stream stream)
        {
            long len = stream.ReadVarInt();
            var strBytes = new byte[len];
            int offset = 0;
            do
            {
                int bytesRead = stream.Read(strBytes, offset, (int) len - offset);
                offset += bytesRead;
            } while (offset < len);
            return Encoding.UTF8.GetString(strBytes);
        }

        public static void WriteString(this Stream stream, string text)
        {
            stream.WriteVarInt(text.Length);
            byte[] strBytes = Encoding.UTF8.GetBytes(text);
            stream.Write(strBytes, 0, strBytes.Length);
        }

        public static void LoadTranslations()
        {
            frenchStrings.Load(@"res\strings\French.xml");
            englishStrings.Load(@"res\strings\English.xml");
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
            try
            {
                return translatedString.Item(0).InnerText;
            }
            catch
            {
                try
                {
                    return (englishStrings.SelectNodes(nodeSearch)).Item(0).InnerText;
                }
                catch
                {
                    return key;
                }
            }
        }

        public static RectangleShape NewRectangle(float coordX, float coordY, float sizeX, float sizeY)
        {
            var shape = new RectangleShape(new Vector2f(coordX, coordY));
            shape.Position = new Vector2f(sizeX, sizeY);
            return shape;
        }

        public static MenuState CreateMainMenu(Game game)
        {
            var menu = new MenuState(game.menuFont, "menuBg.bmp", "eddsworldCreditsTheme.ogg", GetString("play", game),
                GetString("settings", game), GetString("quit", game));
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
            string[] menuItems =
            {
                GetOptionText("drawTextures", game.settings.drawTextures, game),
                GetOptionText("drawHitboxes", game.settings.drawHitbox, game),
                GetString("language", game) + " : " +
                (game.settings.language == Language.English ? Language.English.ToString() : Language.French.ToString()),
                GetOptionText("fullscreen", game.settings.fullscreen, game),
                GetString("back", game)
            };
            var options = new MenuState(game.menuFont, "menuBg.bmp", "eddsworldCreditsTheme.ogg", menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selectedPos)
                {
                    case 0:
                        game.settings.drawTextures = !game.settings.drawTextures;
                        options.menuBtns[0].DisplayedString = GetOptionText("drawTextures", game.settings.drawTextures,
                            game);
                        break;
                    case 1:
                        game.settings.drawHitbox = !game.settings.drawHitbox;
                        options.menuBtns[1].DisplayedString = GetOptionText("drawHitboxes", game.settings.drawHitbox,
                            game);
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
                        options.menuBtns[2].DisplayedString = GetString("language", game) + " : " +
                                                              (game.settings.language == Language.English
                                                                  ? Language.English.ToString()
                                                                  : Language.French.ToString());
                        break;
                    case 3:
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