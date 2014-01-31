using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer_The_Game
{
    interface IState
    {
        public void Initialize(Game game);
        public void Update();
        public void Draw();
        public void Unintialize();
    }
}