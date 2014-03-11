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
        public class Default { }
        public enum Action { Use, Up, Down, Left, Right, Jump, Run, Pause, None };
        
        Dictionary<Type,Dictionary<Keyboard.Key,Action>> keyboardKeys =
            new Dictionary<Type, Dictionary<Keyboard.Key, Action>>();
        Dictionary<Type, Dictionary<uint, Action>> joystickKeys =
            new Dictionary<Type, Dictionary<uint, Action>>();
        [NonSerialized] const string fileName = "game.settings";
        [NonSerialized]
        public bool DrawTextures = true;
        [NonSerialized]
        public bool DrawHitbox = false;

        private Settings()
        {
            SetButton(typeof(GameState), Keyboard.Key.E, Action.Use);


            SetButton(typeof(MenuState), Keyboard.Key.Return, Action.Use);
            SetButton(typeof(Default), Keyboard.Key.W, Action.Up);
            SetButton(typeof(Default), Keyboard.Key.S, Action.Down);
            SetButton(typeof(Default), Keyboard.Key.A, Action.Left);
            SetButton(typeof(Default), Keyboard.Key.D, Action.Right);
            SetButton(typeof(Default), Keyboard.Key.Up, Action.Up);
            SetButton(typeof(Default), Keyboard.Key.Down, Action.Down);
            SetButton(typeof(Default), Keyboard.Key.Left, Action.Left);
            SetButton(typeof(Default), Keyboard.Key.Right, Action.Right);
            SetButton(typeof(GameState), Keyboard.Key.Space, Action.Jump);
            SetButton(typeof(GameState), Keyboard.Key.LShift, Action.Run);
            SetButton(typeof(GameState), Keyboard.Key.Escape, Action.Pause);
            SetButton(typeof(Default), 1, Action.Use);
            SetButton(typeof(Default), 2, Action.Up);
            SetButton(typeof(Default), 3, Action.Down);
            SetButton(typeof(Default), 4, Action.Left);
            SetButton(typeof(Default), 5, Action.Right);
            SetButton(typeof(GameState), 6, Action.Jump);
            SetButton(typeof(GameState), 7, Action.Run);
        }

        public Action GetAction(Type state, Keyboard.Key k)
        {
            Dictionary<Keyboard.Key, Action> tmp;
            Action a;
            if (keyboardKeys.TryGetValue(state, out tmp)
                && tmp.TryGetValue(k, out a))
            {
                return a;
            }
            if (keyboardKeys.TryGetValue(typeof(Default), out tmp)
                && tmp.TryGetValue(k, out a))
            {
                return a;
            }
            return Action.None;
        }

        public Action GetAction(Type state, uint jK)
        {
            Dictionary<uint, Action> tmp;
            Action a;
            if (joystickKeys.TryGetValue(state, out tmp)
                && tmp.TryGetValue(jK, out a))
            {
                return a;
            }
            if (joystickKeys.TryGetValue(typeof(Default), out tmp)
                && tmp.TryGetValue(jK, out a))
            {
                return a;
            }
            return Action.None;
        }

        public void SetButton(Type state, uint k, Action a)
        {
            Dictionary<uint, Action> tmp;
            if (!joystickKeys.TryGetValue(state, out tmp))
            {
                tmp = new Dictionary<uint, Action>();
                joystickKeys.Add(state, tmp);
            }
            tmp.Add(k, a);
        }

        public void SetButton(Type state, Keyboard.Key k, Action a)
        {
            Dictionary<Keyboard.Key, Action> tmp;
            if (!keyboardKeys.TryGetValue(state, out tmp))
            {
                tmp = new Dictionary<Keyboard.Key, Action>();
                keyboardKeys.Add(state, tmp);
            }
            tmp.Add(k, a);
        }

        public void Save()
        {

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                FileStream writeFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(writeFileStream, this);
                writeFileStream.Close();
            }
            catch
            {
                Debug.WriteLine("Error while saving");
            }
        }

        public static Settings Load()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                FileStream readerFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return formatter.Deserialize(readerFileStream) as Settings;
            }
            catch
            {
                Debug.WriteLine("Error while loading");
                return new Settings();
            }

        }
    }
}