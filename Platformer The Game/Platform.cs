using System.Collections.Generic;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;
using System;

namespace Platformer_The_Game
{
    internal class Platform : IEntity
    {
        private readonly Game game;
        public Sprite sprite;
        public Platform(Game game, Vector2f pos)
        {
            this.game = game;
            Pos = pos;
            sprite = new Sprite(game.ResMan.GetTexture("plateformes.stone"));
            sprite.Texture.Repeated = true;
            sprite.TextureRect = new IntRect(0, 0, (int)sprite.Texture.Size.X, (int)sprite.Texture.Size.Y);
            sprite.Position = Pos;
            Debug.WriteLine(sprite.GetGlobalBounds());
        }

        public Platform(Game game, Vector2f pos, params string[] args)
        {
            this.game = game;
            Pos = pos;
            Initialize(args);
        }

        public void Initialize(params string[] args)
        {
            sprite = new Sprite(game.ResMan.GetTexture("plateformes." + args.GetOrDefault(2, "stones")));
            sprite.TextureRect = new IntRect(0, 0,
                                Convert.ToInt32(args.GetOrDefault(0, sprite.Texture.Size.X.ToString())), 
                                Convert.ToInt32(args.GetOrDefault(1, sprite.Texture.Size.Y.ToString())));
        }

        public Vector2f Pos { get; set; }

        public void Update()
        {
        }

        public void Draw()
        {
            if (game.Settings.DrawTextures)
            {
                game.W.Draw(sprite);
            }
        }
    }
}