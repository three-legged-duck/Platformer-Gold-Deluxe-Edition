using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class Hitbox : Drawable
    {
        public FloatRect box;

        public Hitbox(FloatRect box)
        {
            this.box = box;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
                var boundingBox = new RectangleShape(new Vector2f(box.Width, box.Height));
                boundingBox.Position = new Vector2f(box.Left, box.Top);
                boundingBox.OutlineColor = Color.Red;
                boundingBox.FillColor = Color.Transparent;
                boundingBox.OutlineThickness = 3;
                boundingBox.Draw(target, states); // Pass-through to SFML's real function.
        }

        public void MoveTo(Vector2f newPos) // set when unit.Pos is mset
        {
            this.box = new FloatRect(box.Left, box.Top, box.Width, box.Height);
        }

        public bool Collides(Hitbox other)
        {
            FloatRect overlap;
            return Collides(other, out overlap);
        }

        public bool Collides(Hitbox other, out FloatRect overlap)
        {
            if (other.box.Intersects(box, out overlap))
                return true;
            return false;
        }
    }
}