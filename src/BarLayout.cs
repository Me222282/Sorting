using System;
using Zene.GUI;
using Zene.Structs;

namespace Sorting
{
    public class BarLayout : ILayout
    {
        public BarLayout(float mv, float b)
        {
            //MaxValue = mv;
            Border = b;
        }
        
        /*
        private float _maxValue = 100f;
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue == value) { return; }
                
                _maxValue = value;
                
                InvokeChange();
            }
        }*/
        
        private float _border = 5f;
        public float Border
        {
            get => _border;
            set
            {
                if (_border == value) { return; }
                
                _border = value;
                
                InvokeChange();
            }
        }
        
        public event EventHandler Change;
        protected void InvokeChange() => Change?.Invoke(this, EventArgs.Empty);
        
        public Box GetBounds(LayoutArgs args)
        {
            if (args.Element is not Bar)
            {
                throw new ArgumentException(nameof(args.Element));
            }
            
            float x = args.Size.X - _border;
            x /= args.Neighbours.Length;
            
            float h = args.Size.Y - (2f * _border);
            
            Vector2 offset = (args.Size * 0.5f);
            
            Bar e = (Bar)args.Element;
            
            float scale = 1f;//Math.Clamp(e.Value, 1f, MaxValue) / MaxValue;
            
            Vector2 pos = (_border + (x * e.Index), _border);
            Vector2 size = (x - _border, h * scale);
            
            return new Box((pos - offset) + (size * 0.5f), size);
        }
    }
}
