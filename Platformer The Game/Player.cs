using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Player
    {
        Game Game;
        private readonly Sound _sound;
        public Hitbox Hitbox;
        public bool OnGround;

        private Vector2f _pos;
        private float _lastverticalpos;
        private Facing _direction;
        public int Life;
        private bool _isjumping;


        private bool _moving;
        public Vector2f Speed; // TODO : Why no int ?
        protected ResMan.AnimatedSprite Sprite;

        public Player(Game game, Vector2f pos)
        {
            Game = game;

            Life = 100;
            _direction = Facing.Right;
            OnGround = true;

            // Visual Appearance
            Sprite = game.ResMan.NewSprite("character", "idle");

            // Hitbox
            Hitbox = new Hitbox(new List<FloatRect> {Sprite.GetLocalBounds()});
            _direction = Facing.Right;
            Pos = pos;

            _sound = new Sound();
        }

        public Vector2f Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public void Initialize()
        {
        }

        public void Event(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Left:
                    _moving = true;
                    if (_direction == Facing.Right)
                    {
                        Sprite.Reverse();
                        _direction = Facing.Left;
                    }
                    break;
                case Settings.Action.Right:
                    _moving = true;
                    if (_direction == Facing.Left)
                    {
                        Sprite.Reverse();
                        _direction = Facing.Right;
                    }
                    break;
                case Settings.Action.Jump:
                    if (!_isjumping)
                    {
                        OnGround = false;
                        _isjumping = true;
                        Speed.Y = -15;
                    }
                    else if(_isjumping && _pos.Y >= _lastverticalpos)
                    {
                        Speed.Y -= (float)0.5;
                    }
                    break;
            }
        }

        public void Update()
        {
            // TODO : Movement handling is pretty ugly. We could prettify it a lot.
            if (_moving)
            {
                if (Sprite.Animation == "idle") Sprite.Animation = "movement";
                if (Math.Abs(Speed.X) < 75) Speed.X += (_direction == Facing.Left ? -1 : 1);
            } // below : not moving
            else
            {
                if (Sprite.Animation == "movement") Sprite.Animation = "idle";
                if (-1 < Speed.X && Speed.X < 1) Speed.X = 0;
                else if (Speed.X > 0) Speed.X--;
                else Speed.X++;
            }
            if (!OnGround && Speed.Y < 15)
            {
                Speed.Y++;
            }
            else if (OnGround && Math.Abs(Speed.Y) > 0.1) Speed.Y = 0;
            _pos.X = Math.Min(Math.Max(_pos.X + Speed.X, 0), Game.Settings.WindowWidth - Sprite.GetLocalBounds().Width);
            _pos.Y = Math.Min(Math.Max(_pos.Y + Speed.Y, 0), Game.Settings.WindowHeight - Sprite.GetLocalBounds().Height);
            if (_pos.X >= Game.Settings.WindowWidth - Sprite.GetLocalBounds().Width || _pos.X <= 1) Speed.X = 0;
            Hitbox.MoveTo(_pos);

            _moving = false;
            OnGround = false;
            Sprite.Position = _pos;
            _lastverticalpos = _pos.Y;
        }

        public void Draw()
        {
            if (Game.Settings.DrawTextures)
                Game.W.Draw(Sprite);
            if (Game.Settings.DrawHitbox)
                Game.W.Draw(Hitbox);
            if (Sprite.Color.A != 255)
            {
                Sprite.Color = new Color(Sprite.Color.R, Sprite.Color.G, Sprite.Color.B, 255);
            }
        }

        public void Uninitialize()
        {
        }

        public void GetDamage(int dmg)
        {
            _sound.SoundBuffer = new SoundBuffer(@"res\music\hurt.aiff");
            _sound.Play();
            Sprite.Color = new Color(Sprite.Color.R,Sprite.Color.G,Sprite.Color.B,128);
            Life -= dmg;
            if (Life <= 0)
            {
                MenuState gameoverMenuState = new MenuState(Game.MenuFont, "gameoverBg.bmp", false, Utils.GetString("backMain", Game), Utils.GetString("quit", Game));
                gameoverMenuState.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
                {
                    switch (args.SelectedPos)
                    {
                        case 0:
                            Game.State = Utils.CreateMainMenu(Game);
                            break;
                        case 1:
                            Game.Close();
                            break;
                    }
                };
                Game.State = gameoverMenuState;
            }
        }

        public void Collided(Platform platform, FloatRect collision)
        {
            if (platform.Pos.Y + platform.hitbox.effectiveShapes[0].Height > Pos.Y + Sprite.GetLocalBounds().Height)
            {
                _pos.Y = collision.Top - Sprite.TextureRect.Height;
                Hitbox.MoveTo(_pos);
                OnGround = true;
            }
            else
            {
                _sound.SoundBuffer = new SoundBuffer(@"res\music\bump.aiff");
                _sound.Play();
                _pos.Y = collision.Top + collision.Height;
            }
            Hitbox.MoveTo(_pos);
            Sprite.Position = _pos;
            Speed.Y = 0;
            _isjumping = false;
            _lastverticalpos = _pos.Y + 1;
        }

        private enum Facing
        {
            Left,
            Right
        };
    }
}