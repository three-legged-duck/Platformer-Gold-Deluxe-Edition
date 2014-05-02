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
        public static XmlDocument FrenchStrings = new XmlDocument();
        public static XmlDocument EnglishStrings = new XmlDocument();

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
            FrenchStrings.Load(@"res\strings\French.xml");
            EnglishStrings.Load(@"res\strings\English.xml");
        }

        public static string GetString(string key, Game game)
        {
            string nodeSearch = "//string[@name='" + key + "']";
            XmlNodeList translatedString;
            switch (game.Settings.Language)
            {
                case Language.French:
                    translatedString = FrenchStrings.SelectNodes(nodeSearch);
                    break;
                default:
                    translatedString = EnglishStrings.SelectNodes(nodeSearch);
                    break;
            }
            try
            {
                // ReSharper disable PossibleNullReferenceException
                return translatedString != null ? translatedString.Item(0).InnerText : (EnglishStrings.SelectNodes(nodeSearch)).Item(0).InnerText;
                // ReSharper restore PossibleNullReferenceException
            }
            catch
            {
                return key;
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
            var menu = new MenuState(game.MenuFont, "menuBg.bmp", true, GetString("play", game),
                GetString("levelEditor", game),
                GetString("settings", game), GetString("quit", game));
            menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
                        game.State = new GameState();
                        break;
                    case 1:
                        game.State = new LevelEditor(game);
                        break;
                    case 2:
                        game.State = OptionsMenu.CreateOptionsMenu(game, menu);
                        break;
                    case 3:
                        game.Close();
                        break;
                }
            };
            return menu;
        }
    }
}