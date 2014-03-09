using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class SplashState : IState
    {
        Game game;
        Image SplashImg;
        Texture SplashTexture;
        Sprite SplashSprite;
        byte alpha = 0;
        const byte max = 255; //max for rgb drawing
        bool scale;
        IState nextState;

        public void Initialize(Game g)
        {
            this.game = g;
            if (scale)
            {
                SplashSprite.Scale = new Vector2f(game.w.Size.X / SplashSprite.GetGlobalBounds().Width, game.w.Size.Y / SplashSprite.GetGlobalBounds().Height);
            }
        }

        public SplashState(string img, bool doScale, IState nextState)
        {
            SplashImg = new Image(img);
            SplashTexture = new Texture(SplashImg);
            SplashSprite = new Sprite(SplashTexture);
            this.scale = doScale;
            this.nextState = nextState;
        }

        public void Update()
        {
            if (alpha < 255)
            {
                SplashSprite.Color = new Color(max, max, max, alpha);
                alpha++;
                System.Threading.Thread.Sleep(15);
            }
            else
            {
                Uninitialize();
            }
        }

        public void Draw()
        {
            game.w.Draw(SplashSprite);
        }

        public void Uninitialize()
        {
            game.state = nextState;
            nextState.Initialize(game);
        }

        public void OnEvent(Settings.Action a)
        {  
        }
    }
}
