using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Player : Unit
    {
        private readonly Sound _sound;
        private float _lastverticalpos;
        private bool _isjumping;

        private bool _moving;

        public Player(Game game, GameState gameState, Vector2f pos) : base (game, gameState, pos, "character", "idle")
        {
            _direction = Facing.Right;
            _sound = new Sound();
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
                        _isjumping = true;
                        Speed.Y = -15;
                    }
                    else if(_isjumping && Pos.Y >= _lastverticalpos)
                    {
                        Speed.Y -= (float)0.5;
                    }
                    break;
            }
        }

        protected override void UpdatePreCollision()
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
            if (!OnGround && Speed.Y < 15) // If we're not on ground, accelerate
            {
                Speed.Y++;
            }
            else if (OnGround && Speed.Y > 0)
            {
                Speed.Y = 0;
                _isjumping = false;
            }

            _moving = false;
            _lastverticalpos = Pos.Y;
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
    }
}