using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace Platformer_The_Game
{
    abstract class Unit : IEntity
    {
        public enum UnitState { Stopped, Walking, Running, Jumping}

        protected Vector2f celerity;
        public int Weight;

        Vector2f _pos;
        public Vector2f Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        Image image;
        Texture texture;
        protected Sprite sprite;
        protected readonly Game game;
        public Unit(Game game, string imgPath)
        {
            this.game = game;
            this.image = new Image(imgPath);
            image.CreateMaskFromColor(new Color(0, 255, 0));
            this.texture = new Texture(image);
            this.sprite = new Sprite(texture);
        }

        protected UnitState unitState;

        public virtual void Update()
        {
            Pos = new Vector2f(Pos.X + celerity.X, Pos.Y);
            sprite.Position = new Vector2f(Pos.X, Pos.Y);
        }

        public virtual void Draw()
        {
            game.w.Draw(sprite);
        }
        
        public abstract void Uninitialize();

    }
}
