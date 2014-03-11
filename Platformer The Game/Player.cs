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
            image = new Image("character1.bmp");
            image.CreateMaskFromColor(new Color(0, 255, 0));
            this.sprite = new Sprite(new Texture(image));

            // Hitbox
            this.Hitbox = new Hitbox(new List<FloatRect> { sprite.GetLocalBounds() });
            direction = Facing.Right;
            //state = State.Stopped;
            this.Pos = pos;
        }

        public void Initialize()
        { }

        public void Event(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Left:
                    moving = true;
                    if (direction == Facing.Right)
                    {
                        Debug.WriteLine("Changed : Left");
                        sprite.TextureRect = new IntRect((int)image.Size.X, 0, (int)-image.Size.X, (int)image.Size.Y);
                    }
                    direction = Facing.Left;
                    break;
                case Settings.Action.Right:
                    moving = true;
                    if (direction == Facing.Left)
                    {
                        Debug.WriteLine("Changed : Right");
                        sprite.TextureRect = new IntRect(0, 0, (int)image.Size.X, (int)image.Size.Y);
                    }
                    direction = Facing.Right;
                    break;
                case Settings.Action.Jump:
                    if (OnGround)
                    {
                        OnGround = false;
                        speed.Y = -15;
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
            Hitbox.MoveTo(_pos);
            moving = false;
            OnGround = false;
            sprite.Position = _pos;
        }

        public void Draw()
        {
            game.w.Draw(sprite);
        }

        public void Uninitialize()
        {

        }


        public void Collided(Platform platform, FloatRect collision)
        {
            _pos.Y = collision.Top - sprite.TextureRect.Height;
            Hitbox.MoveTo(_pos);
            sprite.Position = _pos;
            OnGround = true;

        }
    }
}
