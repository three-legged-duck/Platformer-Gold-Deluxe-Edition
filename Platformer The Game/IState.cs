using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer_The_Game
{
    interface IState
    {
        void Initialize(Game game);
        void Update();
        void Draw();
        void Unintialize();
    }
}