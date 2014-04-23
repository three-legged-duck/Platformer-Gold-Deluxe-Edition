using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class GameState : IState
    {
        private Sprite BackgroundSprite;
        private Game game;
        public List<Platform> platforms;
        private Player player;
        private Image spriteSheet;

        private View view;

        public string BgMusicName
        {
            get { return "gameLoop.ogg"; }
        }

        public void Initialize(Game game)
        {
            this.game = game;
            view = new View();
            BackgroundSprite = new Sprite(new Texture(@"res\images\backgroundStars.bmp"));
            BackgroundSprite.Scale = new Vector2f(game.w.DefaultView.Size.X/BackgroundSprite.GetGlobalBounds().Width,
                game.w.DefaultView.Size.Y/BackgroundSprite.GetGlobalBounds().Height);
            platforms = new List<Platform>();

            player = new Player(game, this, new Vector2f(50, 180));

            spriteSheet = new Image(@"res\images\plateformes.png");
            spriteSheet.CreateMaskFromColor(new Color(0, 255, 0));
            var blockTexture = new Texture(spriteSheet, new IntRect(12*32, 0, 32, 32));

            platforms.Add(new Platform(new Vector2f(180, 256), new Vector2i((int) game.w.Size.X - 48, 32), blockTexture,
                game));
            platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 30), new Vector2i((int) game.w.Size.X, 32),
                blockTexture, game));
            platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 180), new Vector2i((int) game.w.Size.X - 192, 32),
                blockTexture, game));
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
        }

        public void OnEvent(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Pause:
                    var pause = new MenuState(game.menuFont, "menuBg.bmp", "eddsworldCreditsTheme.ogg",
                        Utils.GetString("resume", game), Utils.GetString("settings", game),
                        Utils.GetString("backMain", game));
                    pause.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                    {
                        switch (args.selectedPos)
                        {
                            case 0:
                                game.State = this;
                                break;
                            case 1:
                                game.State = Utils.CreateOptionsMenu(game, pause);
                                break;
                            case 2:
                                game.State = Utils.CreateMainMenu(game);
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