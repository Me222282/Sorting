using System;
using Zene.GUI;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Sorting
{
    public class Slider : Element, ILayout
    {
        public Slider()
        {
            Graphics = new LocalGraphics(this, OnRender);
            Layout = this;
        }
        
        private float _sliderPos;
        public float SliderPosition
        {
            get => _sliderPos;
            set
            {
                _sliderPos = Math.Clamp(value, 0f, 1f);
                
                SliderPos?.Invoke(this, Value);
            }
        }
        public ColourF SliderColour { get; set; } = new ColourF(0.6f, 0.6f, 0.6f);
        public ColourF SelectColour { get; set; } = new Colour(157, 159, 196);
        public ColourF BarColour { get; set; } = new Colour(157, 192, 196);
        
        private bool _relative = true;
        public bool Relative
        {
            get => _relative;
            set
            {
                if (_relative == value) { return; }
                
                _relative = value;
                
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        
        private Vector2 _pos;
        public Vector2 Position
        {
            get => _pos;
            set
            {
                if (_pos == value) { return; }
                
                _pos = value;
                
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        
        private Vector2 _padding;
        public Vector2 Padding
        {
            get => _padding;
            set
            {
                if (_padding == value) { return; }
                
                _padding = value;
                
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        
        private float _sliderWidth = 100f;
        public float SilderWidth
        {
            get => _sliderWidth;
            set
            {
                if (_sliderWidth == value) { return; }
                
                _sliderWidth = value;
                
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public float MaxValue { get; set; }= 1000f;
        public float MinValue { get; set; }= 50f;
        public float Value
        {
            get => MinValue.Lerp(MaxValue, _sliderPos);
            set => SliderPosition = (value - MinValue) / (MaxValue - MinValue);
        }
        
        private readonly Font _font = Shapes.SampleFont;
        public override GraphicsManager Graphics { get; }

        private const float _barHeight = 20f;
        private const float _barWidth = 10f;
        private const float _sliderHeight = 10f;
        
        public event EventHandler<float> SliderPos;
        
        public event EventHandler Change;
        public Box GetBounds(LayoutArgs args)
        {
            Vector2 pos = _pos;
            
            if (_relative)
            {
                pos *= args.Size;
            }
            
            return new Box(pos, (_sliderWidth + _padding.X, _barHeight + _padding.Y));
        }
        
        private Box GetBar()
        {
            return new Box(((_sliderPos - 0.5) * _sliderWidth, 0), (_barWidth, _barHeight));
        }
        
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);
            
            if (Focused && _fromMouse)
            {
                SliderPosition = (MouseLocation.X / _sliderWidth) + 0.5f;
            }
        }
        private void OnRender(object sender, RenderArgs e)
        {
            //e.Context.Framebuffer.Clear(new Colour(240, 120, 90));
            
            // SLIDER
            e.Context.DrawRoundedBox(
                new Box(Vector2.Zero, (_sliderWidth, _sliderHeight)),
                SliderColour,
                0.5f);
            
            // BAR
            e.Context.DrawRoundedBox(
                GetBar(),
                Focused ? SelectColour : BarColour,
                0.2f);
            
            e.TextRenderer.Colour = ColourF.White;
            e.Context.Model = Matrix4.CreateScale(15f) * Matrix4.CreateTranslation(0f, (_barHeight + 15f) * -0.5f);
            e.TextRenderer.DrawCentred(e.Context, $"{Value:N1}", _font, 0, 0);
        }
        
        private bool _fromMouse;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _fromMouse = true;
            
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _fromMouse = false;
            
            base.OnMouseDown(e);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            float offset = 0.05f;
            if (e[Mods.Shift])
            {
                offset = 0.01f;
            }
            
            if (!_fromMouse && e[Keys.Left])
            {
                SliderPosition -= offset;
                return;
            }
            if (!_fromMouse && e[Keys.Right])
            {
                SliderPosition += offset;
                return;
            }
        }
    }
}
