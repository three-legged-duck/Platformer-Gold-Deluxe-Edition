﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SFML.Window;

namespace Platformer_The_Game
{
    [Serializable]
    class Settings
    {
        public enum Action
        {
            Use,
            Up,
            Down,
            Left,
            Right,
            Jump,
            Pause,
            None
        };

        [NonSerialized] private const string FileName = "game.settings";

        private readonly Dictionary<Type, Dictionary<Joystick.Axis, Tuple<Action, Action>>> _joystickAxis =
            new Dictionary<Type, Dictionary<Joystick.Axis, Tuple<Action, Action>>>();

        private readonly Dictionary<Type, Dictionary<uint, Action>> _joystickKeys =
            new Dictionary<Type, Dictionary<uint, Action>>();

        private Dictionary<Type, Dictionary<Keyboard.Key, Action>> _keyboardKeys =
            new Dictionary<Type, Dictionary<Keyboard.Key, Action>>();

        public bool DrawTextures = true;
        public Styles WindowType = Styles.Fullscreen;
        public Utils.Language Language = Utils.Language.English;
        public uint VideoModeWidth = VideoMode.FullscreenModes[0].Width;
        public uint VideoModeHeight = VideoMode.FullscreenModes[0].Height;
        public bool IsCorrupted = false;
        public float MusicVolume = 50f;
        public float FxVolume = 50f;
        public bool LocalLeaderboards = false;
        public string Username = "player";

        public Settings()
        {
            // Keyboard
            SetButton(typeof (Default), Keyboard.Key.W, Action.Up);
            SetButton(typeof (Default), Keyboard.Key.S, Action.Down);
            SetButton(typeof (Default), Keyboard.Key.A, Action.Left);
            SetButton(typeof (Default), Keyboard.Key.D, Action.Right);

            SetButton(typeof (GameState), Keyboard.Key.Space, Action.Jump);
            SetButton(typeof (GameState), Keyboard.Key.Escape, Action.Pause);

            SetButton(typeof (MenuState), Keyboard.Key.Return, Action.Use);
            // Axis
            SetButton(typeof (Default), Joystick.Axis.X, Action.Left, Action.Right);
            SetButton(typeof (Default), Joystick.Axis.Y, Action.Up, Action.Down);
            SetButton(typeof (GameState), (uint) 0, Action.Jump);
            SetButton(typeof (GameState), 2, Action.Use);
            SetButton(typeof (GameState), 7, Action.Pause);
            SetButton(typeof (MenuState), (uint) 0, Action.Use);
        }

        public void ResetKeys()
        {
            _keyboardKeys = new Dictionary<Type, Dictionary<Keyboard.Key, Action>>();
        }

        public Action GetAction(Type state, Keyboard.Key k)
        {
            Dictionary<Keyboard.Key, Action> tmp;
            Action a;
            if (_keyboardKeys.TryGetValue(state, out tmp)
                && tmp.TryGetValue(k, out a))
            {
                return a;
            }
            if (_keyboardKeys.TryGetValue(typeof (Default), out tmp)
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
            if (_joystickKeys.TryGetValue(state, out tmp)
                && tmp.TryGetValue(jK, out a))
            {
                return a;
            }
            if (_joystickKeys.TryGetValue(typeof (Default), out tmp)
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
            if (_joystickAxis.TryGetValue(state, out tmp)
                && tmp.TryGetValue(axis, out a))
            {
                if (position < -50)
                    return a.Item1;
                return a.Item2;
            }
            if (_joystickAxis.TryGetValue(typeof (Default), out tmp)
                && tmp.TryGetValue(axis, out a))
            {
                if (position < -50)
                    return a.Item1;
                return a.Item2;
            }
            return Action.None;
        }

        public void SetButton(Type state, uint k, Action a)
        {
            Dictionary<uint, Action> tmp;
            if (!_joystickKeys.TryGetValue(state, out tmp))
            {
                tmp = new Dictionary<uint, Action>();
                _joystickKeys.Add(state, tmp);
            }
            tmp.Add(k, a);
        }

        public void SetButton(Type state, Keyboard.Key k, Action a)
        {
            Dictionary<Keyboard.Key, Action> tmp;
            if (!_keyboardKeys.TryGetValue(state, out tmp))
            {
                tmp = new Dictionary<Keyboard.Key, Action>();
                _keyboardKeys.Add(state, tmp);
            }
            tmp.Add(k, a);
        }

        public void SetButton(Type state, Joystick.Axis axis, Action low, Action high)
        {
            Dictionary<Joystick.Axis, Tuple<Action, Action>> tmp;
            if (!_joystickAxis.TryGetValue(state, out tmp))
            {
                tmp = new Dictionary<Joystick.Axis, Tuple<Action, Action>>();
                _joystickAxis.Add(state, tmp);
            }
            tmp.Add(axis, new Tuple<Action, Action>(low, high));
        }

        public void Save()
        {
            var formatter = new BinaryFormatter();
            try
            {
                var writeFileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
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
            var formatter = new BinaryFormatter();
            FileStream readerFileStream;
            if (File.Exists(FileName))
            {
                readerFileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            }
            else
            {
                 return new Settings();
            }

            Settings s;
            try
            {
                s = formatter.Deserialize(readerFileStream) as Settings;

                if (s.IsCorrupted)
                {
                    s = new Settings();
                }
            }
            catch
            {
                Debug.WriteLine("Error while loading");
                s = new Settings();
            }
            readerFileStream.Close();
            return s;
        }

        public class Default
        {
        }
    }
}