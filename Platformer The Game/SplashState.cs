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
        Sprite SplashSprite;
        byte alpha = 0;
        const byte max = 255; //max for rgb drawing
        bool scale;
        IState nextState;

        EventHandler<KeyEventArgs> KeyReleasedHandler;
        EventHandler<MouseButtonEventArgs> MouseBtnHandler;
        public void Initialize(Game g)
        {
            this.game = g;
            KeyReleasedHandler = delegate(object sender, KeyEventArgs args)
            {
                EndSplash();
            };

            MouseBtnHandler = delegate(object sender, MouseButtonEventArgs btn)
            {
                if (btn.Button == Mouse.Button.Left)
                {
                    EndSplash();
                }
            };

            g.w.KeyReleased += KeyReleasedHandler;
            g.w.MouseButtonPressed += MouseBtnHandler;


            if (scale)
            {
                SplashSprite.Scale = new Vector2f(game.w.Size.X / SplashSprite.GetGlobalBounds().Width, game.w.Size.Y / SplashSprite.GetGlobalBounds().Height);
            }
        }

        public SplashState(string img, bool doScale, IState nextState)
        {
            SplashSprite = new Sprite(new Texture(img));
            this.scale = doScale;
            this.nextState = nextState;
        }

        bool IsFadeOut = false;
        public void Update()
        {
            if (!IsFadeOut)
            {
                SplashSprite.Color = new Color(max, max, max, alpha);
                alpha = (byte)Math.Min(alpha + 4 , 255);
                if (alpha >= 255)
                {
                    IsFadeOut = true;
                }
            }
            else
            {
                SplashSprite.Color = new Color(max, max, max, alpha);
                alpha = (byte)Math.Max(alpha - 4, 0);
                if (alpha <= 0)
                {
                    EndSplash();
                }
            }
        }

        public void Draw()
        {
            game.w.Draw(SplashSprite);
        }

        public void EndSplash()
        {
            game.State = nextState;
        }

        public void Uninitialize()
        {
            game.w.KeyReleased -= KeyReleasedHandler;
            game.w.MouseButtonPressed -= MouseBtnHandler;
        }

        public void OnEvent(Settings.Action a)
        {
            //EndSplash();
        }
    }
}
