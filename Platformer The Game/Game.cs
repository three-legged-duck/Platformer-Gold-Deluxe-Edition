using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Game
    {
        private Dictionary<Settings.Action, int> _inputState = new Dictionary<Settings.Action, int>();
        public IDictionary<Settings.Action, int> InputState
        {
            get { return new ReadOnlyDictionary<Settings.Action, int>(_inputState); }
        }
        public ResMan ResMan = new ResMan();
        private Music _bgMusic;
        private string _bgMusicName;
        public int AcceptInput;
        public Font MenuFont = new Font(@"res\fonts\Square.ttf");
        public Settings Settings = Settings.Load();
        private IState _state;
        private IState _nextState;
        public RenderWindow W;
        public uint MenuTextSize;

        //Scrolling text for Menu states
        private Text _scrollingText;
        private string[] _textLines;
        private Random _rng = new Random();

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
            W = new RenderWindow(new VideoMode(Settings.VideoModeWidth, Settings.VideoModeHeight), "Platformer", Settings.WindowType);
            WindowInit();

            MenuTextSize = Convert.ToUInt32(W.DefaultView.Size.Y / 12);
            //Load text for scrolling text
            _textLines = File.ReadAllLines(@"res\strings\" + Settings.Language + "Menu.txt");
            ReloadScrollingText();
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
                W.MouseButtonPressed += menuState.MouseClickHandler;
                W.MouseMoved += menuState.MouseMoveHandler;
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
            W = new RenderWindow(new VideoMode(Settings.VideoModeWidth, Settings.VideoModeHeight), "Platformer", Settings.WindowType);
            WindowInit();
        }

        private void CleanInput()
        {
            _breakOutOfInputStateLoopHack = true;
            _inputState.Clear();
        }

        public void RunMainLoop()
        {
            MenuState menu = Utils.CreateMainMenu(this);
            ScrollingTextState scrollingText = new ScrollingTextState("Intro", menu);

            SwitchState(new SplashState("splash.png", true, scrollingText));
            
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

        bool _breakOutOfInputStateLoopHack;
        private void Update()
        {
            if (_nextState != null)
            {
                SwitchState(_nextState);
                _nextState = null;
            }
            if (AcceptInput < Environment.TickCount)
            {
                _breakOutOfInputStateLoopHack = false;
                foreach (var action in _inputState)
                {
                    if (action.Value == 0) continue;
                    _state.OnEvent(action.Key);
                    if (_breakOutOfInputStateLoopHack) break;
                }
            }
            _state.Update();
            if (_bgMusic != null && _bgMusicName != _state.BgMusicName)
            {
                _bgMusic.Stop();
            }
            if (_bgMusicName != _state.BgMusicName && _state.BgMusicName != null)
            {
                _bgMusic = new Music(@"res\music\" + _state.BgMusicName) {Volume = Settings.MusicVolume, Loop = true};
                _bgMusic.Play();
            }

            _bgMusicName = _state.BgMusicName;

            MenuTextSize = Convert.ToUInt32(W.DefaultView.Size.Y/12);
            if (_state is MenuState && ((MenuState) _state).ScrollingTextActivated)
            {
                if (_scrollingText.GetGlobalBounds().Left + _scrollingText.GetGlobalBounds().Width < 0)
                {
                    ReloadScrollingText();
                }
                else
                {
                    _scrollingText.Position = new Vector2f(_scrollingText.Position.X - W.DefaultView.Size.X/200,
                        W.DefaultView.Size.Y - (MenuTextSize*2));
                    _scrollingText.CharacterSize = MenuTextSize;
                }
            }
            else
            {
                ReloadScrollingText();
            }
        }

        private void Draw()
        {
            _state.Draw();
            W.Draw(_scrollingText);
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var action = Settings.GetAction(_state.GetType(), e.Code);
            int currentValue;
            if (!_inputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            _inputState[action] = currentValue + 1;
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
            if (!_inputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            _inputState[action] = Math.Max(currentValue - 1, 0);
        }

        private void OnJoyPressed(object sender, JoystickButtonEventArgs e)
        {
            var action = Settings.GetAction(_state.GetType(), e.Button);
            int currentValue;
            if (!_inputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            _inputState[action] = currentValue + 1;
        }

        private void OnJoyReleased(object sender, JoystickButtonEventArgs e)
        {
            var action = Settings.GetAction(_state.GetType(), e.Button);
            int currentValue;
            if (!_inputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            _inputState[action] = Math.Max(currentValue - 1, 0);
        }

        private void OnJoyAxisMoved(object sender, JoystickMoveEventArgs args)
        {
            var action = Settings.GetAction(_state.GetType(), args.Axis, args.Position);
            int currentValue;
            if (!_inputState.TryGetValue(action, out currentValue))
            {
                currentValue = 0;
            }
            if (args.Position < -50 || args.Position > 50)
            {
                _inputState[action] = currentValue + 1;
            }
            else
            {
                _inputState[action] = Math.Max(currentValue - 1, 0);
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
            AcceptInput = Environment.TickCount + ms;
        }

        public void ReloadMusicVolume()
        {
            _bgMusic.Volume = Settings.MusicVolume;
        }

        private void ReloadScrollingText()
        {
            _scrollingText = new Text(RandomTextLine(), MenuFont, MenuTextSize)
            {
                Position = new Vector2f(W.DefaultView.Size.X,
                    W.DefaultView.Size.Y - (MenuTextSize * 2))
            };
        }

        private string RandomTextLine()
        {
            return _textLines[_rng.Next(0, _textLines.Length)];
        }
    }
}