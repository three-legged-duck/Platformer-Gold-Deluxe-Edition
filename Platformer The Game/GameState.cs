using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace Platformer_The_Game
{
    class GameState : IState
    {
        Game game;
        Player player;

        Music backgroundMusic; 
        public List<Plateform> plateforms;
        

        public GameState()
        {
            
        }

        public void Initialize(Game game)
        {
            this.game = game;
            backgroundMusic = new Music("gameLoop.ogg");
            backgroundMusic.Play();
            backgroundMusic.Loop = true;
            plateforms = new List<Plateform>();

            player = new Player(game, this, new Vector2f(50, 200));
            plateforms.Add(new Plateform(new Vector2f(50, 240), "BlocksMisc.png", game));
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
            backgroundMusic.Stop();
        }

        public void OnEvent(Settings.Action action)
        {
            player.Event(action);
        }
    }
}
