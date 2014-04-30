using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class BitmaskManager
    {
        private Dictionary<Texture, bool[,]> Bitmasks = new Dictionary<Texture, bool[,]>();

        public bool GetPixel(bool[,] mask, Texture tex, uint x, uint y)
        {
            if (x > tex.Size.X || y > tex.Size.Y)
                return false;

            return mask[x,y];
        }

        public bool[,] GetMask(Texture tex)
        {
            bool[,] mask;
            if (!Bitmasks.TryGetValue(tex, out mask))
            {
                Image img = tex.CopyToImage();
                mask = CreateMask(tex, img);
            }

            return mask;
        }

        public bool[,] CreateMask(Texture tex, Image img)
        {
            var mask = new bool[tex.Size.X,tex.Size.Y];

            for (uint y = 0; y < tex.Size.Y; y++)
            {
                for (uint x = 0; x < tex.Size.X; x++)
                    mask[x,y] = img.GetPixel(x, y).A > 0;
            }

            Bitmasks[tex] = mask;

            return mask;
        }
    }

    internal class Collision
    {
        public BitmaskManager Bitmasks = new BitmaskManager();

        public bool PixelPerfectTest(Sprite Object1, Sprite Object2)
        {
            FloatRect Intersection;
            if (Object1.GetGlobalBounds().Intersects(Object2.GetGlobalBounds(), out Intersection))
            {
                IntRect O1SubRect = Object1.TextureRect;
                IntRect O2SubRect = Object2.TextureRect;

                bool[,] mask1 = Bitmasks.GetMask(Object1.Texture);
                bool[,] mask2 = Bitmasks.GetMask(Object2.Texture);

                // Loop through our pixels
                for (var i = (int) Intersection.Left; i < Intersection.Left + Intersection.Width; i++)
                {
                    for (var j = (int) Intersection.Top; j < Intersection.Top + Intersection.Height; j++)
                    {
                        Vector2f o1v = Object1.InverseTransform.TransformPoint(i, j);
                        Vector2f o2v = Object2.InverseTransform.TransformPoint(i, j);

                        // Make sure pixels fall within the sprite's subrect
                        if (o1v.X > 0 && o1v.Y > 0 && o2v.X > 0 && o2v.Y > 0 &&
                            o1v.X < O1SubRect.Width && o1v.Y < O1SubRect.Height &&
                            o2v.X < O2SubRect.Width && o2v.Y < O2SubRect.Height)
                        {
                            if (
                                Bitmasks.GetPixel(mask1, Object1.Texture, (uint) ((o1v.X) + O1SubRect.Left),
                                    (uint) ((o1v.Y) + O1SubRect.Top)) &&
                                Bitmasks.GetPixel(mask2, Object2.Texture, (uint) ((o2v.X) + O2SubRect.Left),
                                    (uint) ((o2v.Y) + O2SubRect.Top)))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public Texture CreateTextureAndBitmask(string Filename)
        {
            var img = new Image(Filename);
            var tex = new Texture(img);
            Bitmasks.CreateMask(tex, img);
            return tex;
        }
    }
}