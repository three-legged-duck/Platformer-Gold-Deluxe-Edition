using System;
using System.IO;
using System.Text;
using System.Xml;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System.Collections;

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

        public static T GetOrDefault<T>(this T[] arr, int i, T def)
        {
            if (arr.Length <= i)
            {
                return def;
            }
            else
            {
                return arr[i];
            }
        }

        public static int ReadVarInt(this Stream stream)
        {
            uint result = 0;
            int length = 0;
            while (true)
            {
                int currentMAYBEFAIL = stream.ReadByte();
                if (currentMAYBEFAIL == -1) throw new InvalidDataException("EOF");
                byte current = (byte)currentMAYBEFAIL;
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt may not be longer than 28 bits.");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }

        public static void WriteVarInt(this Stream stream, int _value)
        {
            uint value = (uint)_value;
            while (true)
            {
                if ((value & 0xFFFFFF80u) == 0)
                {
                    stream.WriteByte((byte)value);
                    break;
                }
                stream.WriteByte((byte)(value & 0x7F | 0x80));
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
            if (text == null)
            {
                stream.WriteVarInt(0);
                return;
            }
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

        public static MenuState CreateSelectionMenu(Game game)
        {
            var selectedLevel = 0;
            string[] levels = null;
            if (Directory.Exists(@"res\levels"))
            {
                levels = Directory.GetFiles(@"res\levels");
                for (int i = 0; i < levels.Length; i++)
                {
                    levels[i] = Path.GetFileName(levels[i]);
                }
            }
            MenuState menu;
            if (levels == null || levels.Length == 0)
            {
                menu = new MenuState(game.MenuFont, "menuBg.png", true, GetString("noLevelsFound", game),
                    GetString("back", game));
                menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                {
                    game.State = CreateMainMenu(game);
                };
            }
            else
            {
                menu = new MenuState(game.MenuFont, "menuBg.png", true, GetString("level", game) + " : " + levels[0],
                GetString("play", game), GetString("back", game));
                menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                {
                    switch (args.SelectedPos)
                    {
                        case 0:
                            selectedLevel = (selectedLevel + 1) % levels.Length;
                            menu.ModifyElement(0, GetString("level", game) + " : " + levels[selectedLevel]);
                            break;
                        case 1:
                            game.State = new GameState(levels[selectedLevel]);
                            break;
                        case 2:
                            game.State = CreateMainMenu(game);
                            break;
                        case 3:
                            game.Close();
                            break;
                    }
                };
            }
            return menu;
        }

        public static MenuState CreateMainMenu(Game game)
        {
            var menu = new MenuState(game.MenuFont, "menuBg.png", true, GetString("play", game),
                GetString("levelEditor", game),
                GetString("settings", game), GetString("quit", game));
            menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
                        game.State = CreateSelectionMenu(game) ;
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

    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public ReadOnlyDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        #region IDictionary<TKey,TValue> Members

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw ReadOnlyException();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw ReadOnlyException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                throw ReadOnlyException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw ReadOnlyException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw ReadOnlyException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw ReadOnlyException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static Exception ReadOnlyException()
        {
            return new NotSupportedException("This dictionary is read-only");
        }
    }
}