﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

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
        }

        public void Initialize()
        { }
        int currFrame = 0;
        public void Event(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Left:
                    moving = true;
                    sprite.TextureRect = new IntRect(90 * ((currFrame++ % 6) + 1), 0, -90, 94); 
/*                    if (direction == Facing.Right)
                    {
                        Debug.WriteLine("Changed : Left");
                        sprite.TextureRect = new IntRect((int)sprite.GetLocalBounds().Width, 0, (int)-sprite.GetLocalBounds().Width, (int)sprite.GetLocalBounds().Height);
                    }*/
                    direction = Facing.Left;
                    break;
                case Settings.Action.Right:
                    moving = true;
                    sprite.TextureRect = new IntRect(90 * (currFrame++ % 6), 0, 90, 94); 
                    /*if (direction == Facing.Left)
                    {
                        Debug.WriteLine("Changed : Right");
                        sprite.TextureRect = new IntRect(0, 0, (int)sprite.GetLocalBounds().Width, (int)sprite.GetLocalBounds().Height);
                    } */ 
                    direction = Facing.Right;
                    break;
                case Settings.Action.Jump:
                    if (OnGround)
                    {
                        OnGround = false;
                        speed.Y = -18;
                    }
                    break;

            }
        }

        public void Update()
        {
            if (moving && Math.Abs(speed.X) < 100) speed.X += (direction == Facing.Left ? -1 : 1);
            else if (!moving && (speed.X < -1 || speed.X > 1)) speed.X += direction == Facing.Left ? 1 : -1;
            else if (!moving) speed.X = 0;
            if (!OnGround && speed.Y < 100) speed.Y++;
            else if (OnGround && speed.Y != 0) speed.Y = 0;
            _pos.X = Math.Min(Math.Max(_pos.X + speed.X, 0), 800 - sprite.GetLocalBounds().Width);
            _pos.Y = Math.Min(Math.Max(_pos.Y + speed.Y, 0), 600 - sprite.GetLocalBounds().Height);
            if (_pos.X >= 800 - sprite.GetLocalBounds().Width || _pos.X <= 1) speed.X = 0;
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
            game.w.Draw(sprite);
            //game.w.Draw(Hitbox);
        }

        public void Uninitialize()
        {

        }


        public void Collided(Platform platform, FloatRect collision)
        {
            if (platform.Pos.Y + platform.hitbox.effectiveShapes[0].Height > Pos.Y + sprite.GetLocalBounds().Height)
            {
                //if (collision.Top - Pos.Y
                _pos.Y = collision.Top - sprite.TextureRect.Height;
                Hitbox.MoveTo(_pos);
                sprite.Position = _pos;
                speed.Y = 0;
                OnGround = true;
            }
            else
            {
                _pos.Y = collision.Top + collision.Height;
                Hitbox.MoveTo(_pos);
                sprite.Position = _pos;
                speed.Y = 0;
            }
        }
    }
}
