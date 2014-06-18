using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

namespace Platformer_The_Game
{
    class DamagerPlatform : IEntity
    {
        private readonly Game game;
        public Sprite sprite;
        string textureName = "metal_vault";
        int damageRate;
        public Sprite CollisionSprite
        {
            get { return sprite; }
        }
        public int DamageRate
        {
            get { return damageRate; }
        }
        public string[] ArgsType
        {
            get { return new string[] { "sizeX", "sizeY", "textureName", "damageRate" }; }
        }

        public string[] Args
        {
            get
            {
                return new string[] { sprite.TextureRect.Width.ToString(), 
                sprite.TextureRect.Height.ToString(),
                textureName,
                damageRate.ToString() };
            }
        }

        public DamagerPlatform(Game game, Vector2f pos)
        {
            this.game = game;
            sprite = new Sprite(game.ResMan.GetTexture("plateformes.metal_vault"));
            sprite.Texture.Repeated = true;
            sprite.TextureRect = new IntRect(0, 0, (int)sprite.Texture.Size.X, (int)sprite.Texture.Size.Y);
            Pos = pos;
            damageRate = 5;
            Debug.WriteLine(sprite.GetGlobalBounds());
        }

        public DamagerPlatform(Game game, Vector2f pos, params string[] args)
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
            damageRate = Convert.ToInt32(args.GetOrDefault(3, "5"));
        }

        public Vector2f Pos
        {
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