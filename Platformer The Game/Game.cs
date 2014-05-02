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
        public Dictionary<Settings.Action, int> InputState = new Dictionary<Settings.Action, int>();

        public ResMan ResMan = new ResMan();
        private Music _bgMusic;
        private string _bgMusicName;
        private int _acceptInput;
        public Font MenuFont = new Font(@"res\fonts\Square.ttf");
        public Settings Settings = Settings.Load();
        private IState _state;
        private IState _nextState = null;
        public RenderWindow W;

        public IState State
        {
            get { return _state; }
            set { _nextState = value; }
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
            W.LostFocus += W_LostFocus;
            W.Closed += OnClosed;
            if (State is MenuState)
            {
                MenuState menuState = (MenuState) State;
                W.MouseButtonPressed += menuState._mouseClickHandler;
                W.MouseMoved += menuState._mouseMoveHandler;
                State = menuState;
            }
        }

        void W_LostFocus(object sender, EventArgs e)
        {
            CleanInput();
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
            InputState.Clear();
        }

        public void RunMainLoop()
        {
            MenuState menu = Utils.CreateMainMenu(this);
            ScrollingTextState scrollingText = new ScrollingTextState("Intro", menu);

            SwitchState(new SplashState("splash.bmp", true, scrollingText));
            
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

        private void SwitchState(IState state)
        {
            if (_state != null)
            {
                _state.Uninitialize();
            }
            CleanInput();
            _state = state;
            _state.Initialize(this);
        }

        private void Update()
        {
            if (_nextState != null)
            {
                SwitchState(_nextState);
                _nextState = null;
            }
            if (_acceptInput < Environment.TickCount)
            {
                foreach (var action in InputState)
                {
                    if (action.Value == 0) continue;
                    else _state.OnEvent(action.Key);
                }
            }
            _state.Update();
            if (_bgMusic != null && _bgMusicName != _state.BgMusicName)
            {
                _bgMusic.Stop();
            }
            if (_bgMusicName != _state.BgMusicName && _state.BgMusicName != null)
            {
                _bgMusic = new Music(@"res\music\" + _state.BgMusicName) {Volume = 50f, Loop = true};
                _bgMusic.Play();
            }

            _bgMusicName = _state.BgMusicName;
        }

        private void Draw()
        {
            _state.Draw();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var action = Settings.GetAction(_state.GetType(), e.Code);
            int currentValue;
            if (!InputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            InputState[action] = currentValue + 1;
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

            var action = Settings.GetAction(_state.GetType(), e.Code);
            int currentValue;
            if (!InputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            InputState[action] = Math.Max(currentValue - 1, 0);
        }

        private void OnJoyPressed(object sender, JoystickButtonEventArgs e)
        {
            var action = Settings.GetAction(_state.GetType(), e.Button);
            int currentValue;
            if (!InputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            InputState[action] = currentValue + 1;
        }

        private void OnJoyReleased(object sender, JoystickButtonEventArgs e)
        {
            var action = Settings.GetAction(_state.GetType(), e.Button);
            int currentValue;
            if (!InputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            InputState[action] = Math.Max(currentValue - 1, 0);
        }

        private void OnJoyAxisMoved(object sender, JoystickMoveEventArgs args)
        {
            var action = Settings.GetAction(_state.GetType(), args.Axis, args.Position);
            int currentValue;
            if (!InputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            if (args.Position < -50 || args.Position > 50)
            {
                InputState[action] = currentValue + 1;
            }
            else
            {
                InputState[action] = Math.Max(currentValue - 1, 0);
            }
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