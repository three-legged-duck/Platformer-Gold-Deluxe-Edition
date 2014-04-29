using System;
using System.Collections.Generic;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Game
    {
        private Dictionary<Joystick.Axis, float> axisPressed = new Dictionary<Joystick.Axis, float>();
        private HashSet<uint> joyPressed = new HashSet<uint>();
        private HashSet<Keyboard.Key> keyPressed = new HashSet<Keyboard.Key>();
        public ResMan ResMan = new ResMan();
        private Music _bgMusic;
        private string _bgMusicName;
        private int _acceptInput;
        public Font MenuFont = new Font(@"res\fonts\Square.ttf");
        public Settings Settings = Settings.Load();
        private IState _state;
        public RenderWindow W;

        public IState State
        {
            get { return _state; }
            set
            {
                if (_state != null)
                {
                    _state.Uninitialize();
                }
                _state = value;
                _state.Initialize(this);
            }
        }

        public Game()
        {
            //Load ressources
            Utils.LoadTranslations();

            //Load main window
            W = new RenderWindow(new VideoMode(Settings.WindowWidth, Settings.WindowHeight), "Platformer", Settings.WindowType);
            WindowInit();
        }

        private void WindowInit()
        {
            W.SetFramerateLimit(60);
            W.SetVerticalSyncEnabled(true);
            W.SetKeyRepeatEnabled(false);
            W.SetIcon(32, 32, (new Image(@"res\images\icon.png")).Pixels);
            // Setup the events
            W.KeyPressed += OnKeyPressed;
            W.KeyReleased += OnKeyReleased;
            W.JoystickButtonPressed += OnJoyPressed;
            W.JoystickButtonReleased += OnJoyReleased;
            W.JoystickMoved += OnJoyAxisMoved;
            W.Closed += OnClosed;
        }

        public void RecreateWindow()
        {
            CleanInput();
            W.KeyPressed -= OnKeyPressed;
            W.KeyReleased -= OnKeyReleased;
            W.JoystickButtonPressed -= OnJoyPressed;
            W.JoystickButtonReleased -= OnJoyReleased;
            W.JoystickMoved -= OnJoyAxisMoved;
            W.Closed -= OnClosed;
            W.Close();
            W = new RenderWindow(new VideoMode(Settings.WindowWidth, Settings.WindowHeight), "Platformer", Settings.WindowType);
            WindowInit();
        }

        private void CleanInput()
        {
            axisPressed.Clear();
            joyPressed.Clear();
            keyPressed.Clear();
        }

        public void RunMainLoop()
        {
            MenuState menu = Utils.CreateMainMenu(this);
            ScrollingTextState scrollingText = new ScrollingTextState("Intro", menu);

            State = new SplashState("splash.bmp", true, scrollingText);

            while (W.IsOpen())
            {
                W.DispatchEvents();

                // Tick update
                Update();

                // Drawing the window
                W.Clear(Color.Black);
                Draw();
                W.Display();
            }
        }

        private void Update()
        {
            if (_acceptInput < Environment.TickCount)
            {
                HashSet<Keyboard.Key> keyPressedCopy = new HashSet<Keyboard.Key>(keyPressed);
                foreach (Keyboard.Key key in keyPressedCopy)
                {
                    _state.OnEvent(Settings.GetAction(_state.GetType(), key));
                }
                foreach (var axis in axisPressed)
                {
                    _state.OnEvent(Settings.GetAction(_state.GetType(), axis.Key, axis.Value));
                }
                foreach (uint key in joyPressed)
                {
                    _state.OnEvent(Settings.GetAction(_state.GetType(), key));
                }
            }
            _state.Update();
            if (_bgMusic != null && _bgMusicName != _state.BgMusicName)
            {
                _bgMusic.Stop();
            }
            else if (_bgMusicName != _state.BgMusicName && _state.BgMusicName != null)
            {
                _bgMusic = new Music(@"res\music\" + _state.BgMusicName.ToString());
                _bgMusic.Volume = 50f;
                _bgMusic.Loop = true;
                _bgMusic.Play();
            }

            _bgMusicName = State.BgMusicName;
        }

        private void Draw()
        {
            _state.Draw();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            keyPressed.Add(e.Code);
        }

        private void OnKeyReleased(object sender, KeyEventArgs e)
        {

            if (e.Code == Keyboard.Key.F1)
            {
                if (!Directory.Exists("screenshots"))
                {
                    Directory.CreateDirectory("screenshots");
                }
                string filename = String.Format("screenshots\\screen_{0}.png", DateTime.Now.ToFileTimeUtc());
                W.Capture().SaveToFile(filename);
            }

            keyPressed.Remove(e.Code);

        }

        private void OnJoyPressed(object sender, JoystickButtonEventArgs e)
        {
            joyPressed.Add(e.Button);

        }

        private void OnJoyReleased(object sender, JoystickButtonEventArgs e)
        {

            joyPressed.Remove(e.Button);

        }

        private void OnJoyAxisMoved(object sender, JoystickMoveEventArgs args)
        {

            if (args.Position < -50 || args.Position > 50)
                axisPressed[args.Axis] = args.Position;
            else
                axisPressed.Remove(args.Axis);

        }

        private void OnClosed(object sender, EventArgs e)
        {
            Close();
        }

        public void Close()
        {
            Settings.Save();
            W.Close();
            Environment.Exit(0);
        }

        public void StopInput(int ms)
        {
            _acceptInput = Environment.TickCount + ms;
        }
    }
}