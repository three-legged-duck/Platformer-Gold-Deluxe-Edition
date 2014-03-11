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
        
        public Platform(Vector2f pos, Vector2i size, Texture texture, Game game)
        {
            this.Pos = pos;
            this.game = game;
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
            if (game.settings.DrawTextures)
                game.w.Draw(sprite);
            if (game.settings.DrawHitbox)
                game.w.Draw(hitbox);
        }
    }
}
