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
        string textureName = "stone";

        public string[] ArgsType
        {
            get { return new string[] { "sizeX", "sizeY", "textureName" }; }
        }

        public string[] Args
        {
            get { return new string[] { sprite.TextureRect.Width.ToString(), 
                sprite.TextureRect.Height.ToString(),
                textureName }; }
        }

        public Platform(Game game, Vector2f pos)
        {
            this.game = game;
            sprite = new Sprite(game.ResMan.GetTexture("plateformes.stone"));
            sprite.Texture.Repeated = true;
            sprite.TextureRect = new IntRect(0, 0, (int)sprite.Texture.Size.X, (int)sprite.Texture.Size.Y);
            Pos = pos;
            Debug.WriteLine(sprite.GetGlobalBounds());
        }

        public Platform(Game game, Vector2f pos, params string[] args)
        {
            this.game = game;
            sprite = new Sprite();
            Initialize(args);
            Pos = pos;
        }

        public void Initialize(params string[] args)
        {
            textureName = args.GetOrDefault(2, "stone");
            sprite.Texture = game.ResMan.GetTexture("plateformes." + textureName);
            sprite.Texture.Repeated = true;
            sprite.TextureRect = new IntRect(0, 0,
                                Convert.ToInt32(args.GetOrDefault(0, sprite.Texture.Size.X.ToString())), 
                                Convert.ToInt32(args.GetOrDefault(1, sprite.Texture.Size.Y.ToString())));
        }

        public Vector2f Pos {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public Vector2f Size
        {
            get { return new Vector2f(sprite.GetGlobalBounds().Width, sprite.GetGlobalBounds().Height); }
        }

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