using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class Unit : IEntity
    {
        protected enum Facing { Left, Right };

        protected Game Game;
        protected GameState GameState;

        public Hitbox Hitbox;
        public bool OnGround;
        protected FloatRect underUnit; // 1 x width rectangle under the feet of the unit

        protected Vector2f _pos;
        protected Facing _direction;
        public int Life;

        public Vector2f Speed;
        protected ResMan.AnimatedSprite Sprite;

        public Vector2f Pos
        {
            get { return _pos; }
            set { _pos = value;
                Hitbox.MoveTo(value); }
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

            Hitbox = new Hitbox(Sprite.GetGlobalBounds());

            Pos = pos;
        }

        public virtual void Update()
        {
            OnGround = false;
            foreach(Platform p in GameState.Platforms)
                OnGround = underUnit.Intersects(p.hitbox.box) && OnGround;
            bool colliding = false;
            Vector2f destination = new Vector2f(Pos.X + Speed.X, Pos.Y + Speed.Y);
            FloatRect overlap;
            foreach (Platform entity in GameState.Platforms) // TODO: IEntity instead of Platform
                if (this.Hitbox.Collides(entity.hitbox, out overlap))
                {
                    if (GameState.Collision.PixelPerfectTest(this.Sprite, entity.sprite))
                    {
                        colliding = true;
                        break;
                    }
                }
            if (colliding)
            {
                Speed = new Vector2f(0, 0);
            }
            Pos = new Vector2f(Pos.X+Speed.X, Pos.Y+Speed.Y);
            Hitbox.MoveTo(Pos);
        }
        public void Draw()
        {
            if (Game.Settings.DrawTextures)
                Game.W.Draw(Sprite);
            if (Game.Settings.DrawHitbox)
                Game.W.Draw(Hitbox);
        }
    }
}