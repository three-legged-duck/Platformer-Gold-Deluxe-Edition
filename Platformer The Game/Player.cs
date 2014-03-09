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
        int weight;
        Vector2f acceleration = new Vector2f();

        public Player(Vector2f spawnPos, int weight, Game game)
            : base(game, "character1.bmp")
        {
            this.weight = weight;
            Pos = spawnPos;
            unitState = UnitState.Stopped;
            acceleration.X = 0;
            celerity.X = .0001f;
        }

        public void Initialize()
        { }
        public void SetState(UnitState state)
        {
            this.unitState = state;
        }

        public void Event(Settings.Action action)
        {
            switch (action)
            {
                case Settings.Action.Right:
                    acceleration.X = .1f;
                    break;
                case Settings.Action.Left:
                    acceleration.X = -.1f;
                    break;
                case Settings.Action.Jump:
                    break;

            }
        }
        public override void Update()
        {
            celerity.X += acceleration.X;
            acceleration.X = 0;
            base.Update();
        }

        public override void Uninitialize()
        {

        }
        
    }
}
