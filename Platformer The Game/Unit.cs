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
        public enum State { Stopped, Walking, Running, Jumping}
        bool direction; // true is right, false is left
        protected readonly float g; // gravity
        protected Vector2f celerity;
        public int Weight;
        public Hitbox Hitbox;
        Vector2f _pos;
        public Vector2f Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        GameState gameState;
        Image image;
        Texture texture;
        protected Sprite sprite;
        protected Sprite baseSprite;
        protected readonly Game game;
        protected State state;


        public Unit(Game game, string imgPath, GameState gameState)
        {
            this.game = game;
            this.gameState = gameState;
            direction = false;
            this.image = new Image(imgPath);
            image.CreateMaskFromColor(new Color(0, 255, 0));
            this.texture = new Texture(image);
            this.sprite = new Sprite(texture);
            this.Hitbox = new Hitbox(new List<RectangleShape> { new RectangleShape(new Vector2f(sprite.GetLocalBounds().Left, sprite.GetLocalBounds().Top)) } ); // FIXME
            g = 0; // Fixme: gravity
        }

        public Unit(Game game, string imgPath, List<RectangleShape> boxes, GameState gameState)
            : this(game, imgPath, gameState)
        {
            Hitbox = new Hitbox(boxes);
        }
        public virtual void Update()
        {

            Vector2f newPos = new Vector2f(Pos.X + celerity.X,  Pos.Y);
           // MoveTo(newPos, gameState.plateforms);
            Hitbox.Update(newPos);
            Pos = newPos;
            sprite.Position = new Vector2f(Pos.X, Pos.Y);
        }
        // Movement and collision function
        protected void MoveTo(Vector2f newPos, List<IEntity> entities)
        {
            //foreach(Plateform plateform in entities)
            //    if(plateform.hitbox
            Hitbox.Update(newPos);
            if (celerity.X > 0)
                sprite = baseSprite;
            else if (celerity.X < 0)
                sprite.Transform.Scale(new Vector2f(-1,1));
        }

        public virtual void Draw()
        {

            game.w.Draw(sprite);
        }

        public virtual void Uninitialize()
        {

        }
    }
}
