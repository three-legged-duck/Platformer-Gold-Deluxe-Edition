using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using System.Diagnostics;

namespace Platformer_The_Game
{
    class GameState : IState
    {
        Game game;
        Player player;

        Music backgroundMusic; 
        public List<Platform> platforms;
        

        public GameState()
        {
            
        }

        public void Initialize(Game game)
        {
            this.game = game;
            backgroundMusic = new Music("gameLoop.ogg");
            backgroundMusic.Play();
            backgroundMusic.Loop = true;
            platforms = new List<Platform>();

            player = new Player(game, this, new Vector2f(50, 200));
            platforms.Add(new Platform(new Vector2f(50, 240), "BlocksMisc.png", game));
        }
        
        public void Draw()
        {
            foreach (Platform plateform in platforms)
                plateform.Draw();
            player.Draw();
        }
        
        public void Update()
        {
            foreach (Platform plateform in platforms)
                plateform.Update();
            Vector2f oldPos = player.Pos;
            player.Update();
            
            foreach (Platform platform in platforms)
            {
                if (player.Hitbox.Collides(platform.hitbox))
                {
                    Debug.WriteLine("Collided");
                    player.Collided(platform);
                }
            }
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
