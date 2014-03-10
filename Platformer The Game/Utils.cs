using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace Platformer_The_Game
{
    class Utils
    {
        /*
        public static Dictionary<K,V> CreateDict<K,V>()
        {
            return new Dictionary<K, V>();
        }
        */

        public static RectangleShape NewRectangle(float coordX, float coordY, float sizeX, float sizeY)
        {
            RectangleShape shape = new RectangleShape(new Vector2f(coordX, coordY));
            shape.Position = new Vector2f(sizeX, sizeY);
            return shape;
        }
    }
}
