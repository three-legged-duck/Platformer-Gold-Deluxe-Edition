using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace Platformer_The_Game
{
    [Serializable]
    class Settings
    {
        public enum Action { Use, Up, Down, Left, Right, Jump, Run, None };
        Dictionary<Keyboard.Key, Action> keyboard_keys = new Dictionary<Keyboard.Key,Action>();
        Dictionary<uint, Action> joystick_keys = new Dictionary<uint, Action>();
        [NonSerialized] const string file_name = "game.settings";

        private Settings()
        {
            SetButton(Keyboard.Key.E, Action.Use);
            SetButton(Keyboard.Key.Return, Action.Use);
            SetButton(Keyboard.Key.W, Action.Up);
            SetButton(Keyboard.Key.S, Action.Down);
            SetButton(Keyboard.Key.A, Action.Left);
            SetButton(Keyboard.Key.D, Action.Right);
            SetButton(Keyboard.Key.Up, Action.Up);
            SetButton(Keyboard.Key.Down, Action.Down);
            SetButton(Keyboard.Key.Left, Action.Left);
            SetButton(Keyboard.Key.Right, Action.Right);
            SetButton(Keyboard.Key.Space, Action.Jump);
            SetButton(Keyboard.Key.LShift, Action.Run);
            SetButton(1, Action.Use);
            SetButton(2, Action.Up);
            SetButton(3, Action.Down);
            SetButton(4, Action.Left);
            SetButton(5, Action.Right);
            SetButton(6, Action.Jump);
            SetButton(7, Action.Run);
        }

        public Action GetAction(Keyboard.Key k)
        {
            Action a;
            if (keyboard_keys.TryGetValue(k, out a))
            {
                return a;
            }
            else
            {
                return Action.None;
            }
        }

        public Action GetAction(uint j_k)
        {
            Action a;
            if (joystick_keys.TryGetValue(j_k, out a))
            {
                return a;
            }
            else
            {
                return Action.None;
            }
        }

        public void SetButton(uint k, Action a)
        {
            joystick_keys.Add(k, a);
        }

        public void SetButton(Keyboard.Key k, Action a)
        {
            keyboard_keys.Add(k, a);
        }

        public void Save()
        {

            BinaryFormatter Formatter = new BinaryFormatter();
            try
            {
                FileStream writeFileStream = new FileStream(file_name, FileMode.Create, FileAccess.Write);
                Formatter.Serialize(writeFileStream, this);
                writeFileStream.Close();
            }
            catch
            {
                Debug.WriteLine("Error while saving");
            }
        }

        public static Settings Load()
        {
            BinaryFormatter Formatter = new BinaryFormatter();

            try
            {
                FileStream readerFileStream = new FileStream(file_name, FileMode.Open, FileAccess.Read);
                return Formatter.Deserialize(readerFileStream) as Settings;
            }
            catch
            {
                Debug.WriteLine("Error while loading");
                return new Settings();
            }

        }
    }
}