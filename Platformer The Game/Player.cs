using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class Player : Unit
    {
        Vector2f acceleration = new Vector2f();

        public Player(Vector2f spawnPos, int weight, Game game, GameState state)
            : base(game, "character1.bmp", state)
        {
            this.Weight = weight;
            Pos = spawnPos;
            //state = State.Stopped;
            acceleration.X = 0;
            celerity.X = 0f;
        }

        public void Initialize()
        { }
        public void SetState(State state)
        {
            this.state = state;
        }

        public void Event(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Right:
                    if (celerity.X == 0)
                        celerity.X = 4;
                    acceleration.X = .2f;
                    break;
                case Settings.Action.Left:
                    if (celerity.X == 0)
                        celerity.X = -4;
                    acceleration.X = -.2f;
                    break;
                case Settings.Action.Jump:
                    if (celerity.Y == 0f)
                        celerity.Y = 3;
                    break;

            }
        }
        public override void Update()
        {
            celerity.X += acceleration.X;
            acceleration.X *= -.4f;
            if (Math.Abs(acceleration.X) < .00002f)
                celerity.X = 0;

            base.Update();
        }

        public override void Uninitialize()
        {

        }
        
    }
}
