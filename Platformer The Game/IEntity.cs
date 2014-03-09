using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    interface IEntity
    {
        Vector2f Pos { get; set; }
       // Vector2i Size { get; set; }
        void Update();
        void Draw();

    }
}
