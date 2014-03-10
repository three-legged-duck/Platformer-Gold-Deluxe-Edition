using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

namespace Platformer_The_Game
{
    class Plateform : IEntity
    {
        Game game;
        public Vector2f Pos { get; set; }
        Texture texture;
        Sprite sprite;
        public readonly Hitbox hitbox;
        RectangleShape box; // FIXME

        public Plateform(Vector2f pos, string imgPath, Game game)
        {
            this.Pos = pos;
            this.game = game;
            texture = new Texture(imgPath);
            texture.Repeated = true;
            sprite = new Sprite(texture);
            box = new RectangleShape(new Vector2f(sprite.GetLocalBounds().Width, sprite.GetLocalBounds().Height)); // FIXME
            box.Position = this.Pos;
            hitbox = new Hitbox(new List<RectangleShape> { box }); // FIXME use the real RectShape list
            sprite.TextureRect = new IntRect(0, 0, (int)(game.w.Size.X), sprite.TextureRect.Height);
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
