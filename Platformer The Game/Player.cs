using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Player
    {
        private readonly SoundBuffer SoundBuffer;
        protected readonly Game game;
        private readonly Sound sound;
        public Hitbox Hitbox;
        public bool OnGround;

        private Vector2f _pos;
        private Facing direction;
        private GameState gameState;


        private Image image;
        private bool moving;
        protected Vector2f speed; // TODO : Why no int ?
        protected ResMan.AnimatedSprite sprite;

        public Player(Game game, GameState state, Vector2f pos)
        {
            this.game = game;
            gameState = state;

            direction = Facing.Right;
            OnGround = true;

            // Visual Appearance
            //image = new Image(@"res\sprites\character.png");
            //image.CreateMaskFromColor(new Color(0, 255, 0));
            sprite = game.ResMan.NewSprite("character", "idle");

            // Hitbox
            Hitbox = new Hitbox(new List<FloatRect> {sprite.GetLocalBounds()});
            direction = Facing.Right;
            //state = State.Stopped;
            Pos = pos;

            SoundBuffer = new SoundBuffer(@"res\music\bump.aiff");
            sound = new Sound();
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
                    moving = true;
                    if (direction == Facing.Right)
                    {
                        sprite.Reverse();
                        direction = Facing.Left;
                    }
                    break;
                case Settings.Action.Right:
                    moving = true;
                    if (direction == Facing.Left)
                    {
                        sprite.Reverse();
                        direction = Facing.Right;
                    }
                    break;
                case Settings.Action.Jump:
                    if (OnGround)
                    {
                        OnGround = false;
                        speed.Y = -15;
                    }
                    else if (!OnGround && speed.Y < -10)
                    {
                        speed.Y--;
                    }
                    break;
            }
        }

        public void Update()
        {
            // TODO : Movement handling is pretty ugly. We could prettify it a lot.
            if (moving)
            {
                if (sprite.Animation == "idle") sprite.Animation = "movement";
                if (Math.Abs(speed.X) < 75) speed.X += (direction == Facing.Left ? -1 : 1);
            } // below : not moving
            else
            {
                if (sprite.Animation == "movement") sprite.Animation = "idle";
                if (-1 < speed.X && speed.X < 1) speed.X = 0;
                else if (speed.X > 0) speed.X--;
                else speed.X++;
            }
            if (!OnGround && speed.Y < 100)
            {
                speed.Y++;
            }
            else if (OnGround && speed.Y != 0) speed.Y = 0;
            _pos.X = Math.Min(Math.Max(_pos.X + speed.X, 0), game.settings.windowWidth - sprite.GetLocalBounds().Width);
            _pos.Y = Math.Min(Math.Max(_pos.Y + speed.Y, 0), game.settings.windowHeight - sprite.GetLocalBounds().Height);
            if (_pos.X >= game.settings.windowWidth - sprite.GetLocalBounds().Width || _pos.X <= 1) speed.X = 0;
            Hitbox.MoveTo(_pos);

            moving = false;
            OnGround = false;
            sprite.Position = _pos;
        }

        public void Draw()
        {
            if (game.settings.drawTextures)
                game.w.Draw(sprite);
            if (game.settings.drawHitbox)
                game.w.Draw(Hitbox);
        }

        public void Uninitialize()
        {
        }


        public void Collided(Platform platform, FloatRect collision)
        {
            if (platform.Pos.Y + platform.hitbox.effectiveShapes[0].Height > Pos.Y + sprite.GetLocalBounds().Height)
            {
                _pos.Y = collision.Top - sprite.TextureRect.Height;
                Hitbox.MoveTo(_pos);
                sprite.Position = _pos;
                speed.Y = 0;
                OnGround = true;
            }
            else
            {
                sound.SoundBuffer = SoundBuffer;
                sound.Play();
                _pos.Y = collision.Top + collision.Height;
                Hitbox.MoveTo(_pos);
                sprite.Position = _pos;
                speed.Y = 0;
            }
        }

        private enum Facing
        {
            Left,
            Right
        };
    }
}