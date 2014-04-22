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
        Dictionary<Type, Dictionary<Joystick.Axis, Tuple<Action, Action>>> joystickAxis =
            new Dictionary<Type, Dictionary<Joystick.Axis, Tuple<Action, Action>>>();

        public bool drawTextures = true;
        public bool drawHitbox = false;
        public Utils.Language language = Utils.Language.English;
        public bool fullscreen = false;
        public uint windowWidth = 800;
        public uint windowHeight = 600;
        [NonSerialized] const string fileName = "game.settings";

        private Settings()
        {
            // Keyboard
            SetButton(typeof(Default), Keyboard.Key.W, Action.Up);
            SetButton(typeof(Default), Keyboard.Key.S, Action.Down);
            SetButton(typeof(Default), Keyboard.Key.A, Action.Left);
            SetButton(typeof(Default), Keyboard.Key.D, Action.Right);
            SetButton(typeof(Default), Keyboard.Key.Up, Action.Up);
            SetButton(typeof(Default), Keyboard.Key.Down, Action.Down);
            SetButton(typeof(Default), Keyboard.Key.Left, Action.Left);
            SetButton(typeof(Default), Keyboard.Key.Right, Action.Right);

            SetButton(typeof(GameState), Keyboard.Key.E, Action.Use);
            SetButton(typeof(GameState), Keyboard.Key.Space, Action.Jump);
            SetButton(typeof(GameState), Keyboard.Key.LShift, Action.Run);
            SetButton(typeof(GameState), Keyboard.Key.Escape, Action.Pause);
            
            SetButton(typeof(MenuState), Keyboard.Key.Return, Action.Use);
            
            // Axis
            SetButton(typeof(Default), Joystick.Axis.X, Action.Left, Action.Right);
            SetButton(typeof(Default), Joystick.Axis.Y, Action.Up, Action.Down);
            SetButton(typeof(GameState), (uint)0, Action.Jump);
            SetButton(typeof(GameState), 2, Action.Use);
            SetButton(typeof(GameState), 7, Action.Pause);
            SetButton(typeof(MenuState), (uint)0, Action.Use);
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

        public Action GetAction(Type state, Joystick.Axis axis, float position)
        {
            Dictionary<Joystick.Axis, Tuple<Action, Action>> tmp;
            Tuple<Action, Action> a;
            if (joystickAxis.TryGetValue(state, out tmp)
                && tmp.TryGetValue(axis, out a))
            {
                if (position < -50)
                    return a.Item1;
                else
                    return a.Item2;
            }
            if (joystickAxis.TryGetValue(typeof(Default), out tmp)
                && tmp.TryGetValue(axis, out a))
            {
                if (position < -50)
                    return a.Item1;
                else
                    return a.Item2;
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

        public void SetButton(Type state, Joystick.Axis axis, Action low, Action high)
        {
            Dictionary<Joystick.Axis, Tuple<Action,Action>> tmp;
            if (!joystickAxis.TryGetValue(state, out tmp))
            {
                tmp = new Dictionary<Joystick.Axis, Tuple<Action,Action>>();
                joystickAxis.Add(state, tmp);
            }
            tmp.Add(axis, new Tuple<Action, Action>(low, high));
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