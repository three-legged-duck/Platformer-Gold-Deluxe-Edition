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

        protected Vector2f _pos;
        protected Facing _direction;
        public int Life;

        public Vector2f Speed;
        protected ResMan.AnimatedSprite Sprite;

        public Vector2f Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Unit(Game game, GameState gameState, Vector2f pos, string spriteSheet, string animation)
        {
            Game = game;
            GameState = gameState;
            Life = 100;
            _direction = Facing.Right;
            OnGround = true;

            // Visual appearance
            Sprite = game.ResMan.NewSprite(spriteSheet, animation);

            Hitbox = new Hitbox(new List<FloatRect> { Sprite.GetLocalBounds() });

            Pos = pos;
        }

        public void Update()
        {
            bool colliding = false;
            Vector2f destination = new Vector2f(Pos.X + Speed.X, Pos.Y + Speed.Y);
            FloatRect overlap;
            foreach(Platform entity in GameState.Platforms) // TODO: IEntity instead of Platform
                if(this.Hitbox.Collides(entity.hitbox, out overlap))
                    if (GameState.Collision.PixelPerfectTest(this.Sprite, entity.sprite))
                    {
                        colliding = true;
                        break;
                    }
                    if (!colliding)
                    {
                        Pos = new Vector2f();
                    }
        }
        public void Draw()
        {

        }
    }
}