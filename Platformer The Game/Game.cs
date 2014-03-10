using SFML.Graphics;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Font font = new Font("arial.ttf");
        public Settings settings  = Settings.Load();

        public Game()
        {
            //Setup the wimdow and disable resizing
            w = new RenderWindow(new VideoMode(800, 600), "Platformer", SFML.Window.Styles.Close);
            w.SetFramerateLimit(60);
            w.SetKeyRepeatEnabled(false);
            // Setup the events
            w.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            w.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased); 
            w.JoystickButtonPressed += new EventHandler<JoystickButtonEventArgs>(OnJoyPressed);
            w.Closed += new EventHandler(OnClosed);
        }

        public void RunMainLoop()
        {

            Initialize();

            MenuState menu = new MenuState(font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", "Jouer", "Options", "Quitter");
            menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selectedPos)
                {
                    case 0:
                        State = new GameState();
                        break;
                    case 2:
                        Close();
                        break;
                }
            };
            

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
            state.Update();
        }

        private void Draw()
        {
            state.Draw();
        }

        HashSet<Keyboard.Key> keyPressed = new HashSet<Keyboard.Key>();
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            keyPressed.Add(e.Code);
        }

        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            keyPressed.Remove(e.Code);
        }

        private void OnJoyPressed(object sender, JoystickButtonEventArgs btn)
        {
            state.OnEvent(settings.GetAction(this.GetType(), btn.Button));
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void Close()
        {
            w.Close();
            System.Environment.Exit(0);
        }
    }
}
