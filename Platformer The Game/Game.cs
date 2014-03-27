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
        public Font font = new Font("arial.ttf");
        public Settings settings  = Settings.Load();

        public Game()
        {
            //Setup the wimdow and disable resizing
            w = new RenderWindow(new VideoMode(800, 600), "Platformer", SFML.Window.Styles.Close);
            w.SetFramerateLimit(60);
            w.SetKeyRepeatEnabled(false);
            w.SetIcon(128, 128, (new Image("icon.png")).Pixels);
            // Setup the events
            w.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            w.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased); 
            w.JoystickButtonPressed += new EventHandler<JoystickButtonEventArgs>(OnJoyPressed);
            w.JoystickButtonReleased += new EventHandler<JoystickButtonEventArgs>(OnJoyReleased);
            w.JoystickMoved += new EventHandler<JoystickMoveEventArgs>(OnJoyAxisMoved);
            w.Closed += new EventHandler(OnClosed);
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
                DirectoryInfo info = new DirectoryInfo("capture");
                if (!info.Exists) info.Create();
                long uniqueKey = info.LastWriteTime.Ticks + 1L;
                string filename = String.Format("capture\\file{0}.png", uniqueKey);
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
