using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen.Control;
using Gwen;
using System.Drawing;
using System.Diagnostics;
namespace Platformer_The_Game.GwenExtensions
{
    class ClippedImage : Base
    {
        private readonly Texture m_Texture;
        private readonly float[] m_uv = new float[] { 0, 0, 1, 1 };
        public Color Color {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePanel"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ClippedImage(Base parent, Texture tex)
            : base(parent)
        {
            m_uv = new float[4];
            m_Texture = tex;
            MouseInputEnabled = true;
            Color = Color.White;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            m_Texture.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Sets the texture coordinates of the image.
        /// </summary>
        public virtual void SetTextureRect(float left, float top, float width, float height)
        {
            m_uv[0] = left / m_Texture.Width;
            m_uv[1] = top / m_Texture.Height;
            m_uv[2] = (left + width + 1) / (m_Texture.Width); // TODO : not to sure about the + 1 there
            m_uv[3] = (top + height + 1) / (m_Texture.Height);
            Debug.WriteLine("[{0}, {1}, {2}, {3}]", m_uv[0] * m_Texture.Width, m_uv[1] * m_Texture.Height, m_uv[2] * m_Texture.Width, m_uv[3] * m_Texture.Height);
        }

        /// <summary>
        /// Texture name.
        /// </summary>
        public string ImageName
        {
            get { return m_Texture.Name; }
            set { m_Texture.Load(value); }
        }

        /// <summary>
        /// Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Gwen.Skin.Base skin)
        {
            base.Render(skin);
            skin.Renderer.DrawColor = Color;
            skin.Renderer.DrawTexturedRect(m_Texture, RenderBounds, m_uv[0], m_uv[1], m_uv[2], m_uv[3]);
        }

        /// <summary>
        /// Sizes the control to its contents.
        /// </summary>
        public virtual void SizeToContents()
        {
            SetSize(m_Texture.Width, m_Texture.Height);
        }

        /// <summary>
        /// Control has been clicked - invoked by input system. Windows use it to propagate activation.
        /// </summary>
        public override void Touch()
        {
            base.Touch();
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
