/*using System;
using Zene.GUI;
using Zene.Structs;

namespace Sorting
{
    public class BarLayoutManager : LayoutManager
    {
        public BarLayoutManager()
        {
            
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
        
        protected override void SetupManager(LayoutArgs args)
        {
            _x = args.Size.X - _border;
            _x /= args.Neighbours.Length;
            
            _h = args.Size.Y - (2 * _border);
            
            _offset = (args.Size * 0.5);
        }
        
        private double _h;
        private double _x;
        private Vector2 _offset;
        protected override Box GetBounds(LayoutArgs args, Box layout)
        {
            if (args.Element is not Bar)
            {
                throw new ArgumentException(nameof(args.Element));
            }
            
            double scale = ((Bar)args.Element).Value / MaxValue;
            
            Vector2 pos = (_border + (_x * args.Index), _border);
            Vector2 size = (_x - _border, _h * scale);
            
            return new Box((pos - _offset) + (size * 0.5), size);
        }
        
        public BarLayout GetLayout() => new BarLayout(_maxValue, _border);
    }
}
*/