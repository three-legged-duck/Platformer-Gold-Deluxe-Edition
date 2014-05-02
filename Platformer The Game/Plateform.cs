using System.Collections.Generic;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Platform : IEntity
    {
        private readonly Game game;
        public readonly Sprite sprite;
        private Texture texture;

        public Platform(Vector2f pos, Vector2i size, Texture texture, Game game)
        {
            Pos = pos;
            this.game = game;
            texture.Repeated = true;
            sprite = new Sprite(texture);
            sprite.TextureRect = new IntRect(0, 0, size.X, size.Y);
            sprite.Position = Pos;
            Debug.WriteLine(sprite.GetGlobalBounds());
        }

        public Vector2f Pos { get; set; }

        public void Update()
        {
        }

        public void Draw()
        {
            if (game.Settings.DrawTextures)
                game.W.Draw(sprite);
        }
    }
}