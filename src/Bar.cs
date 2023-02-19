using System;
using Zene.GUI;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Sorting
{
    public class Bar : Element<BarLayout>
    {
        public Bar()
        {
            Graphics = new LocalGraphics(this, OnRender);
        }
        public Bar(double value)
            : this()
        {
            Value = value;
        }
        
        public BarAnimator Animator { get; set; }
        
        public double Index
        {
            get
            {
                if (Animator == null)
                {
                    return CurrentIndex;
                }
                
                return Animator.GetIndex(CurrentIndex);
            }
        }
        
        public double Value { get; set; }
        public ColourF Colour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF SelectColour { get; set; } = new ColourF(0.7f, 0.5f, 0.1f);
        public ColourF MoveColour { get; set; } = new ColourF(0.2f, 0.8f, 0.25f);
        
        public override GraphicsManager Graphics { get; }

        public void Trig() => Hande.LayoutElement(this);
        
        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            
            double offset = 5d;
            if (this[Mods.Shift])
            {
                offset = 1d;
            }
            else if (this[Mods.Control])
            {
                offset = 10d;
            }
            
            Value = Math.Clamp(Value + (e.DeltaY * offset), 1d, Layout.MaxValue);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            double offset = 5d;
            
            if (e[Mods.Shift])
            {
                offset = 1d;
            }
            
            if (e[Keys.Down])
            {
                Value = Math.Clamp(Value - offset, 1d, Layout.MaxValue);
                return;
            }
            if (e[Keys.Up])
            {
                Value = Math.Clamp(Value + offset, 1d, Layout.MaxValue);
                return;
            }
        }
        
        private const double _textSize = 20d;
        private readonly Font _font = SampleFont.GetInstance();
        private void OnRender(object sender, RenderArgs e)
        {
            ColourF c = Colour;
            
            if (Focused)
            {
                c = SelectColour;
            }
            if (Animator != null &&
                (Animator.IndexA == CurrentIndex ||
                Animator.IndexB == CurrentIndex))
            {
                c = MoveColour;
            }
            
            //e.Context.Framebuffer.Clear(c);
            
            Vector2 size = Bounds.Size;
            size.Y *= Math.Clamp(Value, 1d, Layout.MaxValue) / Layout.MaxValue;
            if (size.Y < 1d)
            {
                size.Y = 1d;
            }
            
            // ROUNDED CORNERS!
            double radius = (0.1 * size.X) / Math.Min(size.X, size.Y);
            double y = (size.Y - Bounds.Height) * 0.5;
            e.Context.DrawRoundedBox(new Box((0d, y), size), c, radius);
            
            ColourF textColour = new ColourF(0f, 0f, 0f);
            if (size.Y <= _textSize)
            {
                y += ((size.Y + _textSize) * 0.5) + 5d;
                textColour = new ColourF(1f, 1f, 1f);
            }
            
            e.TextRenderer.Colour = textColour;
            e.Context.Model = Matrix4.CreateBox(new Box((0d, y), _textSize));
            e.TextRenderer.DrawCentred(e.Context, $"{Value:N0}", _font, 0, 0);
            //e.TextRenderer.DrawCentred(e.Context, $"{CurrentIndex}", _font, 0, 0);
        }
    }
}
