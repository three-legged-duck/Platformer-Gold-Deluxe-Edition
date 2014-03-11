using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

namespace Platformer_The_Game
{
    class Platform : IEntity
    {
        Game game;
        public Vector2f Pos { get; set; }
        Texture texture;
        Sprite sprite;
        public readonly Hitbox hitbox;
        
        public Platform(Vector2f pos, Vector2i size, string imgPath, Game game)
        {
            this.Pos = pos;
            this.game = game;
            texture = new Texture(imgPath);
            texture.Repeated = true;
            sprite = new Sprite(texture);
            sprite.TextureRect = new IntRect(0, 0, size.X, size.Y);
            sprite.Position = Pos;
            Debug.WriteLine(sprite.GetLocalBounds());
            hitbox = new Hitbox(new List<FloatRect>() { sprite.GetLocalBounds() });
            hitbox.MoveTo(Pos);
        }
        public void Update()
        {
        }
        public void Draw()
        {
            game.w.Draw(sprite);
            //game.w.Draw(hitbox);
        }
    }
}
