using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    class Hitbox : Drawable
    {
        public List<FloatRect> shapes;
        public List<FloatRect> effectiveShapes;
        public Hitbox(List<FloatRect> shape)
        {
            this.shapes = shape;
            this.effectiveShapes = new List<FloatRect>(shapes.Count);
        }
        public void MoveTo(Vector2f newPos) // Called in Unit.Update()
        {
            effectiveShapes.Clear();
            foreach(FloatRect currShape in shapes)
            {
                FloatRect copy = currShape;
                copy.Left += newPos.X;
                copy.Top += newPos.Y;
                effectiveShapes.Add(copy);
            }
        }

        public bool Collides(Hitbox other)
        {
            foreach (FloatRect shapeOther in other.effectiveShapes)
            {
                foreach (FloatRect shapeHere in effectiveShapes)
                {
                    if (shapeOther.Intersects(shapeHere))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Utility method
        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (FloatRect rect in this.effectiveShapes)
            {
                RectangleShape boundingBox = new RectangleShape(new Vector2f(rect.Width, rect.Height));
                boundingBox.Position = new Vector2f(rect.Left, rect.Top);
                boundingBox.OutlineColor = Color.Red;
                boundingBox.FillColor = Color.Transparent;
                boundingBox.OutlineThickness = 3;
                target.Draw(boundingBox);
            }
        }
    }
}
