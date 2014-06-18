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

        public Sprite CollisionSprite
        {
            get { return Sprite; }
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
            Pos = pos;
        }

        // WILL NEED TO FILL IT UP
        public virtual void Initialize(params string[] args)
        {

        }

        protected virtual void UpdatePreCollision()
        {

        }

        protected virtual void UpdatePostCollision(ISet<IEntity> collidedWith, Vector2f oldpos, Vector2f newpos)
        {
            foreach (var ent in collidedWith)
            {
                if (typeof(Platform).IsAssignableFrom(ent.GetType())
                  || typeof(DamagerPlatform).IsAssignableFrom(ent.GetType()))
                {
                    // First, compare if it is any different from the old newpos.
                    if (oldpos.Y < newpos.Y // If we are moving down
                        && Math.Abs((newpos.Y + Size.Y) - ent.Pos.Y) < Math.Abs((newpos.X + Size.X) - ent.Pos.X) // and top border is closer than left border
                        && Math.Abs((newpos.Y + Size.Y) - ent.Pos.Y) < Math.Abs(newpos.X - (ent.Pos.X + ent.Size.X))) // and closer to right border
                    {
                        // Now we need to resolve the player's pos
                        newpos = new Vector2f(newpos.X, Math.Min(ent.Pos.Y - Size.Y, newpos.Y));
                        Speed = new Vector2f(Speed.X, 0);
                        // We hit a platform with our feets, we're onground
                        OnGround = true;
                    }
                    if (oldpos.Y > newpos.Y // If we are moving up 
                        && Math.Abs(newpos.Y - (ent.Pos.Y + ent.Size.Y)) < Math.Abs((newpos.X + Size.X) - ent.Pos.X) // and top border is closer than left border
                        && Math.Abs(newpos.Y - (ent.Pos.Y + ent.Size.Y)) < Math.Abs(newpos.X - (ent.Pos.X + ent.Size.X))) // and closer to right border
                    {
                        newpos = new Vector2f(newpos.X, Math.Max(ent.Pos.Y + ent.Size.Y, newpos.Y));
                        Speed = new Vector2f(Speed.X, 0);
                    }
                    if (oldpos.X < newpos.X // If we are moving right 
                        && Math.Abs((newpos.X + Size.X) - ent.Pos.X) < Math.Abs((newpos.Y + Size.Y) - ent.Pos.Y) // and top border is closer than left border
                        && Math.Abs((newpos.X + Size.X) - ent.Pos.X) < Math.Abs(newpos.Y - (ent.Pos.Y + ent.Size.Y))) // and closer to right border
                    {
                        newpos = new Vector2f(Math.Min(ent.Pos.X - Size.X, newpos.X), newpos.Y);
                        Speed = new Vector2f(0, Speed.Y);
                    }
                    if (oldpos.X > newpos.X
                        && Math.Abs(newpos.X - (ent.Pos.X + ent.Size.X)) < Math.Abs((newpos.Y + Size.Y) - ent.Pos.Y) // and top border is closer than left border
                        && Math.Abs(newpos.X - (ent.Pos.X + ent.Size.X)) < Math.Abs(newpos.Y - (ent.Pos.Y + ent.Size.Y))) // and closer to right border
                    {
                        newpos = new Vector2f(Math.Max(ent.Pos.X + ent.Size.X, newpos.X), newpos.Y);
                        Speed = new Vector2f(0, Speed.Y);
                    }
                }
            }
            Pos = newpos;
        }

        public virtual void Update()
        {
            FloatRect under = UnderUnit;
            ISet<IEntity> collidedWith = new HashSet<IEntity>();
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
                //if (GameState.Collision.PixelPerfectTest(this.Sprite, entity.CollisionSprite))
                if (this.CollisionSprite.GetGlobalBounds().Intersects(entity.CollisionSprite.GetGlobalBounds()))
                {
                    collidedWith.Add(entity);
                }
            }
/*            foreach (IEntity entity in GameState.Level.entities) // TODO: IEntity instead of Platform
            {
                //if (entity.GetType() != typeof(Platform)) continue;
                // Btw, is it colliding with bottom ?
                if (UnderUnit.Intersects(entity.CollisionSprite.GetGlobalBounds()))
                {
                    OnGround = true;
                    break;
                }
            }*/
            UpdatePostCollision(collidedWith, oldPos, Pos);
        }

        public void Draw()
        {
            if (Game.Settings.DrawTextures)
                Game.W.Draw(Sprite);
        }
    }

    class CollisionEvent
    {
        public IEntity Entity { get; set; }
        public Vector2f OldPos { get; set; }
        public Vector2f NewPos { get; set; }
    }
}