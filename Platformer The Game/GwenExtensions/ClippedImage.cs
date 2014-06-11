using Gwen.Control;
using Gwen;
using System.Drawing;
using System.Diagnostics;
namespace Platformer_The_Game.GwenExtensions
{
    class ClippedImage : Base
    {
        private readonly Texture _mTexture;
        private readonly float[] _mUv = { 0, 0, 1, 1 };
        public Color Color {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePanel"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="tex">Texture.</param>
        public ClippedImage(Base parent, Texture tex)
            : base(parent)
        {
            _mUv = new float[4];
            _mTexture = tex;
            MouseInputEnabled = true;
            Color = Color.White;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _mTexture.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Sets the texture coordinates of the image.
        /// </summary>
        public virtual void SetTextureRect(float left, float top, float width, float height)
        {
            _mUv[0] = left / _mTexture.Width;
            _mUv[1] = top / _mTexture.Height;
            _mUv[2] = (left + width + 1) / (_mTexture.Width); // TODO : not to sure about the + 1 there
            _mUv[3] = (top + height + 1) / (_mTexture.Height);
            Debug.WriteLine("[{0}, {1}, {2}, {3}]", _mUv[0] * _mTexture.Width, _mUv[1] * _mTexture.Height, _mUv[2] * _mTexture.Width, _mUv[3] * _mTexture.Height);
        }

        /// <summary>
        /// Texture name.
        /// </summary>
        public string ImageName
        {
            get { return _mTexture.Name; }
            set { _mTexture.Load(value); }
        }

        /// <summary>
        /// Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Gwen.Skin.Base skin)
        {
            base.Render(skin);
            skin.Renderer.DrawColor = Color;
            skin.Renderer.DrawTexturedRect(_mTexture, RenderBounds, _mUv[0], _mUv[1], _mUv[2], _mUv[3]);
        }

        /// <summary>
        /// Sizes the control to its contents.
        /// </summary>
        public virtual void SizeToContents()
        {
            SetSize(_mTexture.Width, _mTexture.Height);
        }

        /// <summary>
        /// Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        /// True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            if (down)
                base.OnMouseClickedLeft(0, 0, true);
            return true;
        }
    }
}
