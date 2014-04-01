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

        View view;

        Music backgroundMusic;
        Image spriteSheet;
        public List<Platform> platforms;

        Sprite BackgroundSprite;

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
            view = new View();
            BackgroundSprite = new Sprite(new Texture("backgroundStars.bmp"));
            BackgroundSprite.Scale = new Vector2f(game.w.DefaultView.Size.X / BackgroundSprite.GetGlobalBounds().Width, game.w.DefaultView.Size.Y / BackgroundSprite.GetGlobalBounds().Height);
            backgroundMusic = new Music("gameLoop.ogg");
            backgroundMusic.Play();
            backgroundMusic.Volume = 50f;
            backgroundMusic.Loop = true;
            platforms = new List<Platform>();

            player = new Player(game, this, new Vector2f(50, 180));
            
            spriteSheet = new Image("plateformes.png");
            spriteSheet.CreateMaskFromColor(new Color(0,255,0));
            Texture blockTexture = new Texture(spriteSheet, new IntRect(12 * 32, 0, 32, 32));
            
            platforms.Add(new Platform(new Vector2f(180, 256), new Vector2i((int)game.w.Size.X - 48, 32), blockTexture, game));
            platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 30), new Vector2i((int)game.w.Size.X , 32), blockTexture, game));
            platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 180), new Vector2i((int)game.w.Size.X - 192, 32), blockTexture, game));
            Initialized = true;
            view.Center = player.Pos;
        }
        
        public void Draw()
        {
            game.w.SetView(game.w.DefaultView);
            game.w.Draw(BackgroundSprite);
            game.w.SetView(view);
            foreach (Platform plateform in platforms)
            {
                plateform.Draw();
            }
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
                    player.Collided(platform, rect);
                }
            }
            view.Center = player.Pos;
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
                    MenuState pause = new MenuState(game.font, "menuBg.bmp", "eddsworldCreditsTheme.ogg", Utils.GetString("resume", game), Utils.GetString("settings", game), Utils.GetString("backMain", game));
                    pause.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                    {
                        switch (args.selectedPos)
                        {
                            case 0:
                                game.State = this;
                                break;
                            case 1:
                                game.State = Utils.CreateOptionsMenu(this.game, pause);
                                break;
                            case 2:
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
