using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

namespace Platformer_The_Game
{
    abstract class Unit : IEntity
    {
        protected enum Facing { Left, Right };

        protected Game Game;
        protected GameState GameState;

        public bool OnGround;
        protected FloatRect UnderUnit {
            get {
                FloatRect bounds = Sprite.GetGlobalBounds();
                bounds.Top += bounds.Height;
                bounds.Height = 1;
                return bounds;
            }
        }

        protected Facing _direction;
        public int Life;

        public Vector2f Speed;
        protected ResMan.AnimatedSprite Sprite;

        public Vector2f Pos
        {
            get { return Sprite.Position; }
            set { Sprite.Position = value; }
        }

        public Vector2f Size
        {
            get { return new Vector2f(Sprite.GetGlobalBounds().Width, Sprite.GetGlobalBounds().Height); }
        }

        public string[] Args
        {
            get { return new string[0]; }
        }

        public string[] ArgsType
        {
            get { return new string[0]; }
        }

        public Unit(Game game, GameState gameState, Vector2f pos, string spriteSheet, string animation)
        {
            Game = game;
            GameState = gameState;
            Life = 100;
            _direction = Facing.Right;
            OnGround = false;

            // Visual appearance
            Sprite = game.ResMan.NewSprite(spriteSheet, animation);
        }

        // WILL NEED TO FILL IT UP
        public virtual void Initialize(params string[] args)
        {

        }

        protected virtual void UpdatePreCollision()
        {

        }

        protected virtual void UpdatePostCollision()
        {

        }

        public virtual void Update()
        {
            FloatRect under = UnderUnit;

            Vector2f oldPos = Sprite.Position;
            UpdatePreCollision();
            Debug.WriteLine("===");
            Debug.WriteLine(Speed);
            Debug.WriteLine(OnGround);
            Pos = new Vector2f(Pos.X + Speed.X, Pos.Y + Speed.Y);
            OnGround = false;
            var colliding = false;
            foreach (IEntity entity in GameState.Level.entities) // TODO: IEntity instead of Platform
            {
                if (entity.GetType() != typeof(Platform)) continue;
                if (GameState.Collision.PixelPerfectTest(this.Sprite, (entity as Platform).sprite))
                {
                    colliding = true;
                }
            }
            foreach (IEntity entity in GameState.Level.entities) // TODO: IEntity instead of Platform
            {
                if (entity.GetType() != typeof(Platform)) continue;
                // Btw, is it colliding with bottom ?
                if (UnderUnit.Intersects((entity as Platform).sprite.GetGlobalBounds()))
                {
                    OnGround = true;
                    break;
                }
            }
            if (colliding)
            {
                Speed = new Vector2f(0, 0);
                Pos = oldPos;
            }
            UpdatePostCollision();
        }

        public void Draw()
        {
            if (Game.Settings.DrawTextures)
                Game.W.Draw(Sprite);
        }
    }
}