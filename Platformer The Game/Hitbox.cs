using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class Hitbox
    {
        List<RectangleShape> shape;

        public Hitbox(List<RectangleShape> shape)
        {
            this.shape = shape;
        }
        public void Update(Vector2f newPos) // Called in Unit.Update()
        {
            foreach (RectangleShape rect in shape)
            {
                rect.Position = new Vector2f(newPos.X, newPos.Y);
            }
        }
        

    }
}
