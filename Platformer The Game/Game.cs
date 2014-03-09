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
        Font font = new Font("arial.ttf");
        public Settings settings  = Settings.Load();

        public Game()
        {
            w = new RenderWindow(new VideoMode(1280, 1024), "Platformer");

            // Setup the events
            w.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            w.JoystickButtonPressed += new EventHandler<JoystickButtonEventArgs>(OnJoyPressed);
            w.Closed += new EventHandler(OnClosed);
        }

        public void RunMainLoop()
        {
            MenuState menu = new MenuState(font, "suchbg.png", "Jouer", "Options", "Quitter");
            menu.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.selected_pos)
                {
                    case 2:
                        Close();
                        break;
                }
            };
            state = menu;
            Initialize();
            while (w.IsOpen())
            {
                // Trigger the Event Handling
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
            state.Initialize(this);
        }

        private void Update()
        {
            state.Update();
        }

        private void Draw()
        {
            state.Draw();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            state.OnEvent(settings.GetAction(e.Code));
        }

        private void OnJoyPressed(object sender, JoystickButtonEventArgs btn)
        {
            state.OnEvent(settings.GetAction(btn.Button));
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
