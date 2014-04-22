using SFML.Graphics;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Platformer_The_Game
{
    class Game
    {
        public RenderWindow w;
        IState state;
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
        public Font menuFont = new Font("Square.ttf");
        public Settings settings  = Settings.Load();

        public Game()
        {
            //Load ressources
            Utils.LoadTranslations();

            //Load main window
            w = new RenderWindow(new VideoMode(settings.windowWidth, settings.windowHeight), "Platformer", (settings.fullscreen) ? SFML.Window.Styles.Fullscreen : SFML.Window.Styles.Close);
            WindowInit();
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
            w = new RenderWindow(new VideoMode(settings.windowWidth, settings.windowHeight), "Platformer", (settings.fullscreen) ? SFML.Window.Styles.Fullscreen : SFML.Window.Styles.Close);
            WindowInit();

        }

        private void WindowInit()
        {
            w.SetFramerateLimit(60);
            w.SetKeyRepeatEnabled(false);
            w.SetIcon(128, 128, (new Image("icon.png")).Pixels);
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
            foreach (KeyValuePair<Joystick.Axis, float> axis in axisPressed)
            {
                state.OnEvent(settings.GetAction(state.GetType(), axis.Key, axis.Value));
            }
            foreach (uint key in joyPressed)
            {
                state.OnEvent(settings.GetAction(state.GetType(), key));
            }
            state.Update();
        }

        private void Draw()
        {
            state.Draw();
        }

        HashSet<Keyboard.Key> keyPressed = new HashSet<Keyboard.Key>();
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("Pressed : " + e.Code.ToString());
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

            Debug.WriteLine("Released : " + e.Code);
            keyPressed.Remove(e.Code);
        }

        HashSet<uint> joyPressed = new HashSet<uint>();
        private void OnJoyPressed(object sender, JoystickButtonEventArgs e)
        {
            joyPressed.Add(e.Button);
        }

        private void OnJoyReleased(object sender, JoystickButtonEventArgs e)
        {
            joyPressed.Remove(e.Button);
        }

        Dictionary<Joystick.Axis, float> axisPressed = new Dictionary<Joystick.Axis, float>();
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
            System.Environment.Exit(0);
        }
    }
}
