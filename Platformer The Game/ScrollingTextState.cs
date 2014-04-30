using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class ScrollingTextState : IState
    {
        private readonly IState nextState;
        private Text[] textLines;
        private string textName;
        private uint textSize;
        private Game game;

        private EventHandler<MouseButtonEventArgs> MouseBtnHandler;

        public string BgMusicName
        {
            get { return null; }
        }

        public ScrollingTextState(string textName, IState nextState)
        {
            this.nextState = nextState;
            this.textName = textName;
        }


        public void Initialize(Game game)
        {
            textSize = game.W.Size.Y/18;
            this.game = game;

            MouseBtnHandler = delegate(object sender, MouseButtonEventArgs btn)
            {
                if (btn.Button == Mouse.Button.Left || btn.Button == Mouse.Button.Right)
                {
                    EndScrollingText();
                }
            };
            game.W.MouseButtonPressed += MouseBtnHandler;

            string[] textFile = File.ReadAllLines(@"res\strings\" + game.Settings.Language + textName + ".txt");
            textLines = new Text[textFile.Length];
            for (int i = 0; i < textLines.Length; i++)
            {
                textLines[i] = new Text(textFile[i],game.MenuFont,textSize);
                textLines[i].Position = new Vector2f(0, game.W.Size.Y + ( textSize *i));
            }
        }

        public void Update()
        {
            if (textLines[textLines.Length - 1].Position.Y + textSize < 0)
            {
                EndScrollingText();
            }
            foreach (Text line in textLines)
            {
                line.Position = new Vector2f(0, line.Position.Y - (game.W.Size.Y / 400));
            }
        }

        public void Draw()
        {
            foreach (Text line in textLines)
            {
                game.W.Draw(line);
            }
        }

        public void Uninitialize()
        {
            game.W.MouseButtonPressed -= MouseBtnHandler;
        }

        public void OnEvent(Settings.Action a)
        {
            EndScrollingText();
        }

        private void EndScrollingText()
        {
            game.State = nextState;
            game.StopInput(600);
        }
    }
}