using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace Platformer_The_Game
{
    class GameState : IState
    {
        Game game;
        Player player;
         public List<Plateform> plateforms;
        

        public GameState()
        {
            
        }

        public void Initialize(Game game)
        {
            this.game = game;
            plateforms = new List<Plateform>();

            player = new Player(new Vector2f(50, 200), 100, game, this); // exemple value
            plateforms.Add(new Plateform(new Vector2f(50, 240), "floor.bmp", game)); // example value
        }
        
        public void Draw()
        {
            foreach (Plateform plateform in plateforms)
                plateform.Draw();
            player.Draw();
        }
        
        public void Update()
        {
            foreach (Plateform plateform in plateforms)
                plateform.Update();
            player.Update();
            

        }
        
        public void Uninitialize()
        {
        }

        public void OnEvent(Settings.Action action)
        {
            player.Event(action);
        }
    }
}
