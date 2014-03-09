using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class Plateform : IEntity
    {
        Game game;
        public Vector2f Pos { get; set; }
        Image image;
        Texture texture;
        Sprite sprite;
        public readonly RectangleShape Hitbox;

        public Plateform(Vector2f pos, string imgPath, Game game)
        {
            this.Pos = pos;
            this.game = game;
            image = new Image(imgPath);
            texture = new Texture(image);
            sprite = new Sprite(texture);
            Hitbox = new RectangleShape();
            Hitbox.Position = new Vector2f();
            sprite.Scale = new Vector2f(game.w.Size.X / sprite.GetGlobalBounds().Width, 1);
            sprite.Position = Pos;

        } 
        public void Update()
        {
        }
        public void Draw()
        {
            game.w.Draw(sprite);
        }
    }
}
