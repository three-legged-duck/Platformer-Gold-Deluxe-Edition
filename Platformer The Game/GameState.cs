﻿using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class GameState : IState
    {
        private Sprite _backgroundSprite;
        private Game _game;
        private Level level;
        private Player _player;
        public Collision Collision;

        public Level Level
        {
            get { return level; }
        }

        private string levelname;

        private View _view;

        public GameState(string levelname)
        {
            this.levelname = levelname;
        }

        public GameState(string levelname, Level level)
        {
            this.levelname = levelname;
            this.level = level;
        }

        public string BgMusicName
        {
            get { return "gameLoop.ogg"; }
        }

        public void Initialize(Game game)
        {
            _game = game;
            _view = new View(new Vector2f(0, 0), new Vector2f(game.W.Size.X, game.W.Size.Y));
            if (level == null) level = Level.LoadLevel(_game, levelname);
            if (level.background != null)
            {
                _backgroundSprite = new Sprite(_game.ResMan.GetTexture(level.background));
            } else {
                _backgroundSprite = new Sprite();
            }
            _backgroundSprite.Scale = new Vector2f(game.W.DefaultView.Size.X / _backgroundSprite.GetGlobalBounds().Width,
                game.W.DefaultView.Size.Y/_backgroundSprite.GetGlobalBounds().Height);

            Collision = new Collision();

            _player = new Player(game, this, new Vector2f(50, 180));

            _view.Center = _player.Pos;
        }

        public void Draw()
        {
            _game.W.SetView(_game.W.DefaultView);
            _game.W.Draw(_backgroundSprite);
            _game.W.SetView(_view);
            foreach (IEntity ent in level.entities)
            {
                ent.Draw();
            }
            _player.Draw();
            _game.W.SetView(_game.W.DefaultView);
            Text lifeText = new Text(Utils.GetString("life", _game) + " : " + _player.Life.ToString("D3") + "  " +
                Utils.GetString("speed", _game) + " : " + Math.Abs(Convert.ToInt32(_player.Speed.X)).ToString("D3"), _game.MenuFont, _game.W.Size.Y / 40) { Position = new Vector2f(0, 0) };
            _game.W.Draw(lifeText);
        }

        public void Update()
        {
            foreach (var ent in level.entities)
                ent.Update();
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
                    var pause = new MenuState(_game.MenuFont, "menuBg.png", true,
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
