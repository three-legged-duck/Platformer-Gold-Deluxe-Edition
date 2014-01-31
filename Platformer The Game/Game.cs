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

        public Game()
        {
            w = new RenderWindow(new VideoMode(800, 600), "Platformer");

            // Setup the events
            w.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            w.Closed += new EventHandler(OnClosed);
        }

        public void RunMainLoop()
        {
            state = new MenuState();
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
            if (e.Code == Keyboard.Key.Escape)
            {
                ((Window)sender).Close();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            w.Close();
        }
    }
}
