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
        private readonly Dictionary<Joystick.Axis, float> axisPressed = new Dictionary<Joystick.Axis, float>();
        private readonly HashSet<uint> joyPressed = new HashSet<uint>();
        private readonly HashSet<Keyboard.Key> keyPressed = new HashSet<Keyboard.Key>();
        public ResMan ResMan = new ResMan();
        private Music bgMusic;
        private string bgMusicName;
        public Font menuFont = new Font(@"res\fonts\Square.ttf");
        public Settings settings = Settings.Load();
        private IState state;
        public RenderWindow w;

        public Game()
        {
            //Load ressources
            Utils.LoadTranslations();

            //Load main window
            w = new RenderWindow(new VideoMode(settings.windowWidth, settings.windowHeight), "Platformer",
                (settings.fullscreen) ? Styles.Fullscreen : Styles.Close);
            WindowInit();
        }

        public IState State
        {
            get { return state; }
            set
            {
                if (state != null)
                {
                    state.Uninitialize();
                }
                state = value;
                state.Initialize(this);
            }
        }

        public void RecreateWindow()
        {
            w.KeyPressed -= OnKeyPressed;
            w.KeyReleased -= OnKeyReleased;
            w.JoystickButtonPressed -= OnJoyPressed;
            w.JoystickButtonReleased -= OnJoyReleased;
            w.JoystickMoved -= OnJoyAxisMoved;
            w.Closed -= OnClosed;
            w.Close();
            w = new RenderWindow(new VideoMode(settings.windowWidth, settings.windowHeight), "Platformer",
                (settings.fullscreen) ? Styles.Fullscreen : Styles.Close);
            WindowInit();
        }

        private void WindowInit()
        {
            w.SetFramerateLimit(60);
            w.SetKeyRepeatEnabled(false);
            w.SetIcon(128, 128, (new Image(@"res\images\icon.png")).Pixels);
            // Setup the events
            w.KeyPressed += OnKeyPressed;
            w.KeyReleased += OnKeyReleased;
            w.JoystickButtonPressed += OnJoyPressed;
            w.JoystickButtonReleased += OnJoyReleased;
            w.JoystickMoved += OnJoyAxisMoved;
            w.Closed += OnClosed;
        }

        public void RunMainLoop()
        {
            Initialize();

            MenuState menu = Utils.CreateMainMenu(this);

            State = new SplashState("splash.bmp", true, menu);

            while (w.IsOpen())
            {
                w.DispatchEvents();

                // Tick update
                Update();

                // Drawing the window
                w.Clear(Color.Black);
                Draw();
                w.Display();
            }
        }

        private void Initialize()
        {
        }

        private void Update()
        {
            foreach (Keyboard.Key key in keyPressed)
            {
                state.OnEvent(settings.GetAction(state.GetType(), key));
            }
            foreach (var axis in axisPressed)
            {
                state.OnEvent(settings.GetAction(state.GetType(), axis.Key, axis.Value));
            }
            foreach (uint key in joyPressed)
            {
                state.OnEvent(settings.GetAction(state.GetType(), key));
            }
            state.Update();
            if (bgMusic != null && bgMusicName != state.BgMusicName)
            {
                bgMusic.Stop();
            }
            else if (bgMusicName != state.BgMusicName && bgMusicName != null)
            {
                bgMusic = new Music(@"res\music\" + state.BgMusicName);
                bgMusic.Volume = 50f;
                bgMusic.Loop = true;
                bgMusic.Play();
            }
            bgMusicName = state.BgMusicName;
        }

        private void Draw()
        {
            state.Draw();
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
                w.Capture().SaveToFile(filename);
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
            w.Close();
            Environment.Exit(0);
        }
    }
}