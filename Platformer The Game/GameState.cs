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
        bool Initialized = false;
        public void Initialize(Game game)
        {
            if (Initialized)
            {
                // after main menu
                backgroundMusic.Play();
                return;
            }
            this.game = game;
            backgroundMusic = new Music("gameLoop.ogg");
            backgroundMusic.Play();
            backgroundMusic.Loop = true;
            platforms = new List<Platform>();

            player = new Player(game, this, new Vector2f(50, 180));
            platforms.Add(new Platform(new Vector2f(50, 240), new Vector2i((int)game.w.Size.X, 40), "BlocksMisc.png", game));
            platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 30), new Vector2i((int)game.w.Size.X, 40), "BlocksMisc.png", game));
            Initialized = true;
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
                FloatRect rect;
                if (player.Hitbox.Collides(platform.hitbox, out rect))
                {
                    Debug.WriteLine("Collided");
                    player.Collided(platform, rect);
                }
            }
        }
        
        public void Uninitialize()
        {
            backgroundMusic.Stop();
        }

        public void OnEvent(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Pause:
                    MenuState pause = new MenuState(game.font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", "Reprendre", "Retour au Menu Principal");
                    pause.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                    {
                        switch (args.selectedPos)
                        {
                            case 0:
                                game.State = this;
                                break;
                            case 1:
                                game.State = Utils.CreateMainMenu(this.game);
                                break;
                        }
                    };
                    game.State = pause;
                    break;
                default:
                    player.Event(action);
                    break;
            }
        }
    }
}
