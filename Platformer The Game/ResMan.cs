using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using SFML.Window;
using Newtonsoft.Json.Serialization;

namespace Platformer_The_Game
{
    class AnimationDescription
    {
        public int Top { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public int Interval { set; get; }
        public int Frames { set; get; }
    }

    class SpriteDescription
    {
        [JsonIgnore]
        public string File { set; get; }
        public Color? AlphaColor { get; set; }
        public AnimationDescription this[string index]
        {
            get { return Animations[index]; }
        }
        public Dictionary<string, AnimationDescription> Animations = new Dictionary<string, AnimationDescription>();
    }

    class ResMan
    {
        Dictionary<string, SpriteDescription> spriteDesc = new Dictionary<string, SpriteDescription>();
        Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        // ResMan-accessible constructor
        private static Func<SpriteDescription, Texture, string, AnimatedSprite> newAnimatedSprite;
        static ResMan()
        {
            AnimatedSprite.SetupAnimatedSprite();
        }

        public ResMan()
        {
            foreach (string jsonFile in Directory.GetFiles(@"res\sprites", "*.json"))
            {
                var quickSprite = JsonConvert.DeserializeObject<SpriteDescription>(File.ReadAllText(jsonFile));
                quickSprite.File = Path.GetFileNameWithoutExtension(jsonFile) + ".png";
                spriteDesc[Path.GetFileNameWithoutExtension(jsonFile)] = quickSprite;
            }
        }

        public void LoadLevel()
        {
            // TODO : Load textures in textures dict
        }

        public void UnloadLevel()
        {
            // TODO : Free memory
        }

        public AnimatedSprite NewSprite(string id, string animation)
        {
            SpriteDescription desc = spriteDesc[id];
            Texture tex;
            if (!textures.TryGetValue(id, out tex)) {
                Image img = new Image(@"res\sprites\" + desc.File);
                if (desc.AlphaColor.HasValue)
                {
                    img.CreateMaskFromColor(desc.AlphaColor.Value);
                }
                tex = new Texture(img);
            }

            AnimatedSprite sprite = newAnimatedSprite(desc, tex, animation);
            return sprite;
        }

        public class AnimatedSprite : Drawable
        {
            public static void SetupAnimatedSprite()
            {
                newAnimatedSprite = (SpriteDescription desc, Texture tex, string anim) => new AnimatedSprite(desc, tex, anim);
            }
            Sprite sprite;
            SpriteDescription desc;
            string currentAnimation = "";
            AnimationDescription anim
            {
                get { return desc[currentAnimation]; }
            }
            int nextUpdate = 0;
            bool reversed = false;
            int currentFrame = 0;
            int Frame
            {
                get { return currentFrame; }
                set { currentFrame = value % desc[currentAnimation].Frames; }
            }
            public AnimatedSprite(SpriteDescription desc, Texture tex, string animation)
            {
                this.desc = desc;
                this.currentAnimation = animation;
                this.sprite = new Sprite(tex);
                this.TextureRect = new IntRect(anim.Width * Frame, anim.Top, anim.Width, anim.Height);
            }

            public void ResetFrame()
            {
                currentFrame = 0;
            }

            public string Animation
            {
                get
                {
                    return currentAnimation;
                }
                set
                {
                    if (!desc.Animations.ContainsKey(value))
                    {
                        throw new InvalidOperationException("Animation not present in this spritesheet");
                    }
                    currentAnimation = value;
                    currentFrame = 0;
                    nextUpdate = 0;
                }
            }

            public void Reverse()
            {
                reversed = !reversed;
                nextUpdate = 0;
            }

            ////////////////////////////////////////////////////////////
            /// <summmary>
            /// Draw the object to a render target
            ///
            /// This is a pure virtual function that has to be implemented
            /// by the derived class to define how the drawable should be
            /// drawn.
            /// </summmary>
            /// <param name="target">Render target to draw to</param>
            /// <param name="states">Current render states</param>
            ////////////////////////////////////////////////////////////
            public void Draw(RenderTarget target, RenderStates states)
            {
                nextUpdate--;
                if (nextUpdate <= 0)
                {
                    nextUpdate = anim.Interval;
                    if (reversed)
                    {
                        this.TextureRect = new IntRect(anim.Width * (Frame + 1), anim.Top, -anim.Width, anim.Height);
                    }
                    else
                    {
                        this.TextureRect = new IntRect(anim.Width * Frame, anim.Top, anim.Width, anim.Height);
                    }
                    Frame += 1;
                }
                sprite.Draw(target, states);
            }

            #region Sprite

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Global color of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Color Color
            {
                get { return sprite.Color; }
                set { sprite.Color = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Source texture displayed by the sprite
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Texture Texture
            {
                get { return sprite.Texture; }
                set { sprite.Texture = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Sub-rectangle of the source image displayed by the sprite
            /// </summary>
            ////////////////////////////////////////////////////////////
            public IntRect TextureRect
            {
                get { return sprite.TextureRect; }
                set { sprite.TextureRect = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get the local bounding rectangle of the entity.
            ///
            /// The returned rectangle is in local coordinates, which means
            /// that it ignores the transformations (translation, rotation,
            /// scale, ...) that are applied to the entity.
            /// In other words, this function returns the bounds of the
            /// entity in the entity's coordinate system.
            /// </summary>
            /// <returns>Local bounding rectangle of the entity</returns>
            ////////////////////////////////////////////////////////////
            public FloatRect GetLocalBounds()
            {
                return sprite.GetLocalBounds();
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get the global bounding rectangle of the entity.
            ///
            /// The returned rectangle is in global coordinates, which means
            /// that it takes in account the transformations (translation,
            /// rotation, scale, ...) that are applied to the entity.
            /// In other words, this function returns the bounds of the
            /// sprite in the global 2D world's coordinate system.
            /// </summary>
            /// <returns>Global bounding rectangle of the entity</returns>
            ////////////////////////////////////////////////////////////
            public FloatRect GetGlobalBounds()
            {
                // we don't use the native getGlobalBounds function,
                // because we override the object's transform
                return sprite.GetGlobalBounds();
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Provide a string describing the object
            /// </summary>
            /// <returns>String description of the object</returns>
            ////////////////////////////////////////////////////////////
            public override string ToString()
            {
                return sprite.ToString();
            }

            #endregion

            #region Transformable
            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Position of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Vector2f Position
            {
                get
                {
                    return sprite.Position;
                }
                set
                {
                    sprite.Position = value;
                }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Rotation of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public float Rotation
            {
                get
                {
                    return sprite.Rotation;
                }
                set
                {
                    sprite.Rotation = value;
                }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Scale of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Vector2f Scale
            {
                get
                {
                    return sprite.Scale;
                }
                set
                {
                    sprite.Scale = value;
                }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// The origin of an object defines the center point for
            /// all transformations (position, scale, rotation).
            /// The coordinates of this point must be relative to the
            /// top-left corner of the object, and ignore all
            /// transformations (position, scale, rotation).
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Vector2f Origin
            {
                get
                {
                    return sprite.Origin;
                }
                set
                {
                    sprite.Origin = value;
                }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// The combined transform of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Transform Transform
            {
                get
                {
                    return sprite.Transform;
                }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// The combined transform of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Transform InverseTransform
            {
                get
                {
                    return sprite.InverseTransform;
                }
            }
            #endregion
        }
    }
}
