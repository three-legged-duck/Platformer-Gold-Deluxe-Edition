using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    public class AnimationDescription
    {
        public int Top { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public int Interval { set; get; }
        public int Frames { set; get; }
    }

    public class SpriteDescription
    {
        public Dictionary<string, AnimationDescription> Animations = new Dictionary<string, AnimationDescription>();

        [JsonIgnore]
        public string File { set; get; }

        public Color? AlphaColor { get; set; }
        public bool TexturePerAnim { get; set; }

        public AnimationDescription this[string index]
        {
            get { return Animations[index]; }
        }
    }

    internal class ResMan
    {
        private static Func<SpriteDescription, Texture, string, AnimatedSprite> newAnimatedSprite;
        private readonly Dictionary<string, SpriteDescription> spriteDesc = new Dictionary<string, SpriteDescription>();
        private readonly Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        // ResMan-accessible constructor
        static ResMan()
        {
        //    AnimatedSprite.SetupAnimatedSprite();
            newAnimatedSprite = (desc, tex, str) => new AnimatedSprite(desc, tex, str);
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

        public void LoadLevel(string[] texs)
        {
            foreach (var id in texs)
            {
                GetTexture(id);
            }
        }

        public void UnloadLevel()
        {
            textures.Clear();
        }

        public Texture GetTexture(string fullid)
        {
            string id = fullid, animId = "";
            if (fullid.Contains("."))
            {
                animId = fullid.Split('.')[1];
                id = fullid.Split('.')[0];
            }
            if (!spriteDesc.ContainsKey(id))
            {
                // GO look in images
                if (File.Exists(@"res\images\" + fullid + ".png")) {
                    return new Texture(@"res\images\" + fullid + ".png");
                } else {
                    return new Texture(@"res\images\placeholder.png");
                }
            }
            SpriteDescription desc = spriteDesc[id];
            Texture tex;
            if (!textures.TryGetValue(fullid, out tex))
            {
                var img = new Image(@"res\sprites\" + desc.File);
                if (desc.AlphaColor.HasValue)
                {
                    img.CreateMaskFromColor(desc.AlphaColor.Value);
                }
                if (desc.TexturePerAnim)
                {
                    foreach (var anim in desc.Animations)
                    {
                        textures[id + "." + anim.Key] = new Texture(img, new IntRect(0, anim.Value.Top, anim.Value.Width * anim.Value.Frames, anim.Value.Height));
                        if (anim.Key == animId)
                        {
                            tex = textures[id + "." + anim.Key];
                        }
                    }
                }
                else
                {
                    tex = textures[id] = new Texture(img);
                }
            }
            return tex;
        }

        public AnimatedSprite NewSprite(string id, string animation)
        {
            SpriteDescription desc = spriteDesc[id];
            Texture tex = GetTexture(id);
            AnimatedSprite sprite = newAnimatedSprite(desc, tex, animation);
            return sprite;
        }

        public class AnimatedSprite : Drawable
        {
            private readonly SpriteDescription desc;
            private readonly Sprite sprite;
            private string currentAnimation = "";
            private int currentFrame;

            private int nextUpdate;
            private bool reversed;

            public AnimatedSprite(SpriteDescription desc, Texture tex, string animation)
            {
                this.desc = desc;
                currentAnimation = animation;
                sprite = new Sprite(tex);
                TextureRect = new IntRect(anim.Width * Frame, anim.Top, anim.Width, anim.Height);
            }

            public static implicit operator Sprite(AnimatedSprite s)
            {
                return s.sprite;
            }

            private AnimationDescription anim
            {
                get { return desc[currentAnimation]; }
            }

            private int Frame
            {
                get { return currentFrame; }
                set { currentFrame = value % desc[currentAnimation].Frames; }
            }

            public string Animation
            {
                get { return currentAnimation; }
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

            ////////////////////////////////////////////////////////////
            /// <summmary>
            ///     Draw the object to a render target
            ///     This is a pure virtual function that has to be implemented
            ///     by the derived class to define how the drawable should be
            ///     drawn.
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
                        TextureRect = new IntRect(anim.Width * (Frame + 1), anim.Top, -anim.Width, anim.Height);
                    }
                    else
                    {
                        TextureRect = new IntRect(anim.Width * Frame, anim.Top, anim.Width, anim.Height);
                    }
                    Frame += 1;
                }
                sprite.Draw(target, states);
            }

            #region Sprite

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Global color of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Color Color
            {
                get { return sprite.Color; }
                set { sprite.Color = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Source texture displayed by the sprite
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Texture Texture
            {
                get { return sprite.Texture; }
                set { sprite.Texture = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Sub-rectangle of the source image displayed by the sprite
            /// </summary>
            ////////////////////////////////////////////////////////////
            public IntRect TextureRect
            {
                get { return sprite.TextureRect; }
                set { sprite.TextureRect = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Get the local bounding rectangle of the entity.
            ///     The returned rectangle is in local coordinates, which means
            ///     that it ignores the transformations (translation, rotation,
            ///     scale, ...) that are applied to the entity.
            ///     In other words, this function returns the bounds of the
            ///     entity in the entity's coordinate system.
            /// </summary>
            /// <returns>Local bounding rectangle of the entity</returns>
            ////////////////////////////////////////////////////////////
            public FloatRect GetLocalBounds()
            {
                return sprite.GetLocalBounds();
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Get the global bounding rectangle of the entity.
            ///     The returned rectangle is in global coordinates, which means
            ///     that it takes in account the transformations (translation,
            ///     rotation, scale, ...) that are applied to the entity.
            ///     In other words, this function returns the bounds of the
            ///     sprite in the global 2D world's coordinate system.
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
            ///     Provide a string describing the object
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
            ///     Position of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Vector2f Position
            {
                get { return sprite.Position; }
                set { sprite.Position = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Rotation of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public float Rotation
            {
                get { return sprite.Rotation; }
                set { sprite.Rotation = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     Scale of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Vector2f Scale
            {
                get { return sprite.Scale; }
                set { sprite.Scale = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     The origin of an object defines the center point for
            ///     all transformations (position, scale, rotation).
            ///     The coordinates of this point must be relative to the
            ///     top-left corner of the object, and ignore all
            ///     transformations (position, scale, rotation).
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Vector2f Origin
            {
                get { return sprite.Origin; }
                set { sprite.Origin = value; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     The combined transform of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Transform Transform
            {
                get { return sprite.Transform; }
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            ///     The combined transform of the object
            /// </summary>
            ////////////////////////////////////////////////////////////
            public Transform InverseTransform
            {
                get { return sprite.InverseTransform; }
            }

            #endregion

            public static void SetupAnimatedSprite()
            {
                newAnimatedSprite =
                    (SpriteDescription desc, Texture tex, string anim) => new AnimatedSprite(desc, tex, anim);
            }

            public void ResetFrame()
            {
                currentFrame = 0;
            }

            public void Reverse()
            {
                reversed = !reversed;
                nextUpdate = 0;
            }
        }
    }
}