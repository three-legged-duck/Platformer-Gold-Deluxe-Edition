using System;
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
            this.Hitbox = new Hitbox(new List<RectangleShape> {
                Utils.NewRectangle(pos.X, pos.Y, sprite.GetGlobalBounds().Width, sprite.GetGlobalBounds().Height)
            });
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
            if (moving) speed.X += (direction == Facing.Left ? -1 : 1);
            else if (speed.X < -1 || speed.X > 1) speed.X += direction == Facing.Left ? 1 : -1;
            else speed.X = 0;
            if (!OnGround && speed.Y < 100) speed.Y++;
            _pos.X += speed.X;
            _pos.Y += speed.Y;
            moving = false;
        }

        public void Draw()
        {
            sprite.Position = _pos;
            game.w.Draw(sprite);
            Vector2f size = new Vector2f(sprite.GetGlobalBounds().Width, sprite.GetGlobalBounds().Height);
            RectangleShape boundingBox = new RectangleShape(size);
            boundingBox.Position = _pos;
            boundingBox.OutlineColor = Color.Red;
            boundingBox.FillColor = Color.Transparent;
            boundingBox.OutlineThickness = 3;
            game.w.Draw(boundingBox);
        }

        public void Uninitialize()
        {

        }
        
    }
}
