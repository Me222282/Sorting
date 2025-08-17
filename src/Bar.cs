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
        public Bar(float value)
            : this()
        {
            Value = value;
        }
        
        public BarAnimator Animator { get; set; }
        
        public float Index
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
        
        private float _value;
        public float Value
        {
            get => _value;
            set => _value = Math.Clamp(value, 1f, MaxValue);
        }
        public ColourF Colour { get; set; } = ColourF.White;
        public ColourF SelectColour { get; set; } = new ColourF(0.7f, 0.5f, 0.1f);
        public ColourF MoveColour { get; set; } = new ColourF(0.2f, 0.8f, 0.25f);
        
        public override GraphicsManager Graphics { get; }
        
        private float MaxValue
        {
            get
            {
                if (Parent is null)
                {
                    return float.MaxValue;
                }
                
                return ((BarContainer)Parent).MaxValue;
            }
        }
        
        public void Trig() => Handle.LayoutElement(this);
        
        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            
            float offset = 5f;
            if (this[Mods.Shift])
            {
                offset = 1f;
            }
            else if (this[Mods.Control])
            {
                offset = 10f;
            }
            
            Value += e.DeltaY * offset;
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            float offset = 5f;
            
            if (e[Mods.Shift])
            {
                offset = 1f;
            }
            
            if (e[Keys.Down])
            {
                Value -= offset;
                return;
            }
            if (e[Keys.Up])
            {
                Value += offset;
                return;
            }
        }
        
        private const float _textSize = 20f;
        private readonly Font _font = Shapes.SampleFont;
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
            size.Y *= Math.Clamp(_value, 1f, MaxValue) / MaxValue;
            if (size.Y < 1f)
            {
                size.Y = 1f;
            }
            
            // ROUNDED CORNERS!
            float radius = (0.1f * size.X) / Math.Min(size.X, size.Y);
            float y = (size.Y - Bounds.Height) * 0.5f;
            e.Context.DrawRoundedBox(new Box((0f, y), size), c, radius);
            
            ColourF textColour = new ColourF(0f, 0f, 0f);
            if (size.Y <= _textSize)
            {
                y += ((size.Y + _textSize) * 0.5f) + 5f;
                textColour = new ColourF(1f, 1f, 1f);
            }
            
            e.TextRenderer.Colour = textColour;
            e.Context.Model = Matrix4.CreateBox(new Box((0f, y), _textSize));
            e.TextRenderer.DrawCentred(e.Context, $"{_value:N0}", _font, 0, 0);
            //e.TextRenderer.DrawCentred(e.Context, $"{CurrentIndex}", _font, 0, 0);
        }
    }
}
