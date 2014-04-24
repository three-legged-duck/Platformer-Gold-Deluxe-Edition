using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class GameState : IState
    {
        private Sprite _backgroundSprite;
        private Game _game;
        public List<Platform> Platforms;
        private Player _player;
        private Image _spriteSheet;

        private View _view;

        public string BgMusicName
        {
            get { return "gameLoop.ogg"; }
        }

        public void Initialize(Game game)
        {
            _game = game;
            _view = new View();
            _backgroundSprite = new Sprite(new Texture(@"res\images\backgroundStars.bmp"));
            _backgroundSprite.Scale = new Vector2f(game.w.DefaultView.Size.X/_backgroundSprite.GetGlobalBounds().Width,
                game.w.DefaultView.Size.Y/_backgroundSprite.GetGlobalBounds().Height);
            Platforms = new List<Platform>();

            _player = new Player(game, new Vector2f(50, 180));

            _spriteSheet = new Image(@"res\images\plateformes.png");
            _spriteSheet.CreateMaskFromColor(new Color(0, 255, 0));
            var blockTexture = new Texture(_spriteSheet, new IntRect(12*32, 0, 32, 32));

            Platforms.Add(new Platform(new Vector2f(180, 256), new Vector2i((int) game.w.Size.X - 48, 32), blockTexture,
                game));
            Platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 30), new Vector2i((int) game.w.Size.X, 32),
                blockTexture, game));
            Platforms.Add(new Platform(new Vector2f(0, game.w.Size.Y - 180), new Vector2i((int) game.w.Size.X - 192, 32),
                blockTexture, game));
            _view.Center = _player.Pos;
        }

        public void Draw()
        {
            _game.w.SetView(_game.w.DefaultView);
            _game.w.Draw(_backgroundSprite);
            _game.w.SetView(_view);
            foreach (Platform plateform in Platforms)
            {
                plateform.Draw();
            }
            _player.Draw();
        }

        public void Update()
        {
            foreach (Platform plateform in Platforms)
                plateform.Update();
            //Vector2f oldPos = _player.Pos;
            _player.Update();

            foreach (Platform platform in Platforms)
            {
                FloatRect rect;
                if (_player.Hitbox.Collides(platform.hitbox, out rect))
                {
                    _player.Collided(platform, rect);
                }
            }
            _view.Center = _player.Pos;
        }

        public void Uninitialize()
        {
        }

        public void OnEvent(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Pause:
                    var pause = new MenuState(_game.menuFont, "menuBg.bmp",
                        Utils.GetString("resume", _game), Utils.GetString("settings", _game),
                        Utils.GetString("backMain", _game));
                    pause.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                    {
                        switch (args.SelectedPos)
                        {
                            case 0:
                                _game.State = this;
                                break;
                            case 1:
                                _game.State = Utils.CreateOptionsMenu(_game, pause);
                                break;
                            case 2:
                                _game.State = Utils.CreateMainMenu(_game);
                                break;
                        }
                    };
                    _game.State = pause;
                    break;
                default:
                    _player.Event(action);
                    break;
            }
        }
    }
}