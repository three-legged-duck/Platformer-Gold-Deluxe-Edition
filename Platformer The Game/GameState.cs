using System;
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
        public Collision Collision;

        private int levelNumber;
        private int worldNumber;

        private View _view;

        public GameState(int selectedWorld, int selectedLevel)
        {
            levelNumber = selectedLevel;
            worldNumber = selectedWorld;
        }

        public string BgMusicName
        {
            get { return "gameLoop.ogg"; }
        }

        public void Initialize(Game game)
        {
            _game = game;
            _view = new View();
            _backgroundSprite = new Sprite(new Texture(@"res\images\backgroundStars.bmp"));
            _backgroundSprite.Scale = new Vector2f(game.W.DefaultView.Size.X/_backgroundSprite.GetGlobalBounds().Width,
                game.W.DefaultView.Size.Y/_backgroundSprite.GetGlobalBounds().Height);
            Platforms = new List<Platform>();

            Collision = new Collision();

            _player = new Player(game, this, new Vector2f(50, 180));

            _spriteSheet = new Image(@"res\images\plateformes.png");
            _spriteSheet.CreateMaskFromColor(new Color(0, 255, 0));
            var blockTexture = new Texture(_spriteSheet, new IntRect(12*32, 0, 32, 32));

            //TODO : true level loading
            switch (levelNumber)
            {
                case 1:
                    Platforms.Add(new Platform(game, new Vector2f(180, 256), "752", "32", "space_3"));
                    Platforms.Add(new Platform(game, new Vector2f(0, 570), "752", "32", "space_3"));
                    break;
                case 2:
                    Platforms.Add(new Platform(game, new Vector2f(0, 180), "752", "32", "space_3"));
                    Platforms.Add(new Platform(game, new Vector2f(0, 570), "800", "32", "space_3"));
                    break;
                default:
                    Platforms.Add(new Platform(game, new Vector2f(250, 256), "752", "32", "space_3"));
                    Platforms.Add(new Platform(game, new Vector2f(0, 570), "800", "32", "space_3"));
                    break;
            }

            _view.Center = _player.Pos;
        }

        public void Draw()
        {
            _game.W.SetView(_game.W.DefaultView);
            _game.W.Draw(_backgroundSprite);
            _game.W.SetView(_view);
            foreach (Platform plateform in Platforms)
            {
                plateform.Draw();
            }
            _player.Draw();
            _game.W.SetView(_game.W.DefaultView);
            Text lifeText = new Text(Utils.GetString("life", _game) + " : " + _player.Life.ToString("D3") + "  " +
                Utils.GetString("speed", _game) + " : " + Math.Abs(Convert.ToInt32(_player.Speed.X)).ToString("D3"), _game.MenuFont, _game.W.Size.Y / 40) { Position = new Vector2f(0, 0) };
            _game.W.Draw(lifeText);
        }

        public void Update()
        {
            foreach (Platform plateform in Platforms)
                plateform.Update();
            //Vector2f oldPos = _player.Pos;
            _player.Update();
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
                    var pause = new MenuState(_game.MenuFont, "menuBg.bmp", true,
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
                                _game.State = OptionsMenu.CreateOptionsMenu(_game, pause);
                                break;
                            case 2:
                                _game.State = Utils.CreateMainMenu(_game);
                                break;
                        }
                    };
                    _game.State = pause;
                    break;
                case Settings.Action.DebugDamage:
                    _player.GetDamage(10);
                    break;
                default:
                    _player.Event(action);
                    break;
            }
        }
    }
}