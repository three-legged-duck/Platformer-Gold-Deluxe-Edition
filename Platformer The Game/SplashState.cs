using System;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class SplashState : IState
    {
        private const byte max = 255; //max for rgb drawing
        private readonly Sprite SplashSprite;
        private readonly IState nextState;
        private readonly bool scale;
        private bool IsFadeOut;

        private EventHandler<MouseButtonEventArgs> MouseBtnHandler;

        private byte alpha;
        private Game game;

        private View view;

        public SplashState(string img, bool doScale, IState nextState)
        {
            SplashSprite = new Sprite(new Texture(@"res\images\" + img));
            scale = doScale;
            this.nextState = nextState;
        }

        public string BgMusicName
        {
            get { return null; }
        }

        public void Initialize(Game g)
        {
            game = g;
            view = new View();

            MouseBtnHandler = delegate(object sender, MouseButtonEventArgs btn)
            {
                if (btn.Button == Mouse.Button.Left || btn.Button == Mouse.Button.Right)
                {
                    EndSplash();
                }
            };
            g.W.MouseButtonPressed += MouseBtnHandler;

            if (scale)
            {
                SplashSprite.Scale = new Vector2f(view.Size.X/SplashSprite.GetGlobalBounds().Width,
                    view.Size.Y/SplashSprite.GetGlobalBounds().Height);
            }
        }

        public void Update()
        {
            if (!IsFadeOut)
            {
                SplashSprite.Color = new Color(max, max, max, alpha);
                alpha = (byte) Math.Min(alpha + 4, 255);
                if (alpha >= 255)
                {
                    IsFadeOut = true;
                }
            }
            else
            {
                SplashSprite.Color = new Color(max, max, max, alpha);
                alpha = (byte) Math.Max(alpha - 4, 0);
                if (alpha <= 0)
                {
                    EndSplash();
                }
            }
        }

        public void Draw()
        {
            game.W.SetView(view);
            game.W.Draw(SplashSprite);
        }

        public void Uninitialize()
        {
            game.W.MouseButtonPressed -= MouseBtnHandler;
        }

        public void OnEvent(Settings.Action a)
        {
            EndSplash();
        }

        public void EndSplash()
        {
            game.State = nextState;
            game.StopInput(600);
        }
    }
}