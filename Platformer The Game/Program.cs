using System;
using System.Collections.Generic;
using SFML;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    static class Program
    {
        static RenderWindow w;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            w = new RenderWindow(new VideoMode(800, 600), "Platformer");

            // Setup the events
            w.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            w.Closed += new EventHandler(OnClosed);

            // Event loop
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

        static void Update()
        {
            
        }

        static void Draw()
        {

        }
        
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                ((Window)sender).Close();
            }
        }

        static void OnClosed(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            w.Close();
        }
    }
}
