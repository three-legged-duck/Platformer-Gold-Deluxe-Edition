using System;
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
            try
            {
                if (x > tex.Size.X || y > tex.Size.Y)
                    return false;

                return mask[x, y];
            }
            catch (Exception)
            {

                return false;
            }

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

        public bool PixelPerfectTest(Sprite object1, Sprite object2)
        {
            FloatRect intersection;
            if (object1.GetGlobalBounds().Intersects(object2.GetGlobalBounds(), out intersection))
            {
                IntRect o1SubRect = object1.TextureRect;
                IntRect o2SubRect = object2.TextureRect;

                bool[,] mask1 = Bitmasks.GetMask(object1.Texture);
                bool[,] mask2 = Bitmasks.GetMask(object2.Texture);

                // Loop through our pixels
                for (var i = (int) intersection.Left; i < intersection.Left + intersection.Width; i++)
                {
                    for (var j = (int) intersection.Top; j < intersection.Top + intersection.Height; j++)
                    {
                        Vector2f o1V = object1.InverseTransform.TransformPoint(i, j);
                        Vector2f o2V = object2.InverseTransform.TransformPoint(i, j);

                        // Make sure pixels fall within the sprite's subrect
                        if (o1V.X > 0 && o1V.Y > 0 && o2V.X > 0 && o2V.Y > 0 &&
                            o1V.X < o1SubRect.Width && o1V.Y < o1SubRect.Height &&
                            o2V.X < o2SubRect.Width && o2V.Y < o2SubRect.Height)
                        {
                            if (
                                Bitmasks.GetPixel(mask1, object1.Texture, (uint) ((o1V.X) + o1SubRect.Left),
                                    (uint) ((o1V.Y) + o1SubRect.Top)) &&
                                Bitmasks.GetPixel(mask2, object2.Texture, (uint) ((o2V.X) + o2SubRect.Left),
                                    (uint) ((o2V.Y) + o2SubRect.Top)))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public Texture CreateTextureAndBitmask(string filename)
        {
            var img = new Image(filename);
            var tex = new Texture(img);
            Bitmasks.CreateMask(tex, img);
            return tex;
        }
    }
}