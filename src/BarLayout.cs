using System;
using Zene.GUI;
using Zene.Structs;

namespace Sorting
{
    public class BarLayout : ILayout
    {
        public BarLayout(double mv, double b)
        {
            MaxValue = mv;
            Border = b;
        }
        
        private double _maxValue = 100d;
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue == value) { return; }
                
                _maxValue = value;
                
                InvokeChange();
            }
        }
        
        private double _border = 5d;
        public double Border
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
            
            double x = args.Size.X - _border;
            x /= args.Neighbours.Length;
            
            double h = args.Size.Y - (2 * _border);
            
            Vector2 offset = (args.Size * 0.5);
            
            Bar e = (Bar)args.Element;
            
            double scale = 1d;//Math.Clamp(e.Value, 1d, MaxValue) / MaxValue;
            
            Vector2 pos = (_border + (x * e.Index), _border);
            Vector2 size = (x - _border, h * scale);
            
            return new Box((pos - offset) + (size * 0.5), size);
        }
    }
}
