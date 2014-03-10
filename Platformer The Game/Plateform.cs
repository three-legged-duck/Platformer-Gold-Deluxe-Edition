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
        public readonly Hitbox hitbox;
        RectangleShape box; // FIXME

        public Plateform(Vector2f pos, string imgPath, Game game)
        {
            this.Pos = pos;
            this.game = game;
            image = new Image(imgPath);
            texture = new Texture(image);
            sprite = new Sprite(texture);
            box = new RectangleShape(new Vector2f(sprite.GetLocalBounds().Width, sprite.GetLocalBounds().Height)); // FIXME
            box.Position = this.Pos;
            hitbox = new Hitbox(new List<RectangleShape> { box }); // FIXME use the real RectShape list
            sprite.Scale = new Vector2f(game.w.Size.X / sprite.GetLocalBounds().Width, 1);
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
