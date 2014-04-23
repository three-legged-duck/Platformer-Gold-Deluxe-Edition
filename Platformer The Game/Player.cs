using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;
using SFML.Audio;

namespace Platformer_The_Game
{


    class Player
    {
        enum Facing { Left, Right };
        Facing direction;
        bool moving;
        public bool OnGround;

        protected Vector2f speed; // TODO : Why no int ?
        Vector2f _pos;
        public Vector2f Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Hitbox Hitbox;


        Image image;
        protected Sprite sprite;
        
        GameState gameState;
        protected readonly Game game;

        SoundBuffer SoundBuffer;
        Sound sound;

        public Player(Game game, GameState state, Vector2f pos)
        {
            this.game = game;
            this.gameState = state;
            
            direction = Facing.Right;
            OnGround = true;
            
            // Visual Appearance
            image = new Image("character.png");
            image.CreateMaskFromColor(new Color(0, 255, 0));
            this.sprite = new Sprite(new Texture(image));
            sprite.TextureRect = new IntRect(0, 0, 90, 94);

            // Hitbox
            this.Hitbox = new Hitbox(new List<FloatRect> { sprite.GetLocalBounds() });
            direction = Facing.Right;
            //state = State.Stopped;
            this.Pos = pos;

            SoundBuffer = new SoundBuffer("bump.aiff");
            sound = new Sound();
        }

        public void Initialize()
        { }
        
        int updateSprite = 0;
        int currFrame = 0;
        public void Event(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Left:
                    moving = true;
                    if (direction == Facing.Right || updateSprite-- == 0)
                    {
                        sprite.TextureRect = new IntRect(90 * ((currFrame++ % 6) + 1), 0, -90, 94); 
                        updateSprite = 5;
                    }
                    direction = Facing.Left;
                    break;
                case Settings.Action.Right:
                    moving = true;
                    if (direction == Facing.Left || updateSprite-- == 0)
                    {
                        sprite.TextureRect = new IntRect(90 * (currFrame++ % 6), 0, 90, 94);
                        updateSprite = 5;
                    }
                    direction = Facing.Right;
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
            if (moving)
            {
                if (Math.Abs(speed.X) < 75) speed.X += (direction == Facing.Left ? -1 : 1);
            } // below : not moving
            else if (-1 < speed.X && speed.X < 1) speed.X = 0;
            else
            {
                if (speed.X > 0) speed.X--;
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

            if (!moving && direction == Facing.Right)
            {
                sprite.TextureRect = new IntRect(0, 94, 90, 94);
                currFrame = 0;
            }
            else if (!moving && direction == Facing.Left)
            {
                sprite.TextureRect = new IntRect(90, 94, -90, 94);
                currFrame = 0;
            }

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
    }
}
