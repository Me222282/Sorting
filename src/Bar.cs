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
            Graphics = new Renderer(this);
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
            
            Value = Math.Clamp(Value + (e.DeltaY * offset), 1d, Layout.MaxValue);
            Trig();
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
                Trig();
                return;
            }
            if (e[Keys.Up])
            {
                Value = Math.Clamp(Value + offset, 1d, Layout.MaxValue);
                Trig();
                return;
            }
        }

        private class Renderer : GraphicsManager<Bar>
        {
            public Renderer(Bar source)
                : base(source)
            {
                
            }
            
            private readonly Font _font = SampleFont.GetInstance();
            private readonly BorderShader _shader = BorderShader.GetInstance();
            
            public override void OnRender(DrawManager context)
            {
                ColourF c = Source.Colour;
                
                if (Source.Focused)
                {
                    c = Source.SelectColour;
                }
                if (Source.Animator != null &&
                    (Source.Animator.IndexA == Source.CurrentIndex ||
                    Source.Animator.IndexB == Source.CurrentIndex))
                {
                    c = Source.MoveColour;
                }
                
                //e.Context.Framebuffer.Clear(c);
                
                // ROUNDED CORNERS!
                _shader.ColourSource = ColourSource.UniformColour;
                _shader.BorderWidth = 0d;
                _shader.Size = Bounds.Size;
                _shader.Colour = c;
                _shader.BorderColour = new ColourF(0.5f, 0.5f, 0.5f);
                _shader.Radius = 0.1;
                
                context.Shader = _shader;
                context.Model = Matrix4.CreateScale(2d);
                context.View = Matrix4.Identity;
                context.Projection = Matrix4.Identity;
                context.Draw(Shapes.Square);
                
                TextRenderer.Colour = new ColourF(0f, 0f, 0f);
                TextRenderer.Model = Matrix4.CreateScale(20);
                TextRenderer.DrawCentred(context, $"{Source.Value:N0}", _font, 0, 0);
                //TextRenderer.DrawCentred(e.Context, $"{CurrentIndex}", _font, 0, 0);
            }
        }
    }
}
