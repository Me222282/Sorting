using System;
using Zene.GUI;
using Zene.Structs;
using Zene.Windowing;

namespace Sorting
{
    public class BarAnimator
    {
        public void SwapBars(int a, int b)
        {
            if (Animating)
            {
                throw new Exception("BIG PROBLEM");
            }
            
            // No animation
            if (a == b) { return; }
            
            _animationTime = Core.Time;
            _indexA = a;
            _indexB = b;
            _diff = a - b;
            Animating = true;
        }
        
        private double _animationTime;
        private int _indexA = -1;
        private int _indexB = -1;
        private int _diff;
        
        public int IndexA => _indexA;
        public int IndexB => _indexB;
        
        public bool Animating { get; private set; } = false;
        public double AnimationTime { get; set; } = 0.2;
        
        public event EventHandler<Vector2I> Finish;
        
        public double GetIndex(int elementIndex)
        {
            if (!Animating)
            {
                return elementIndex;
            }
            
            if ((Core.Time - _animationTime) >= AnimationTime)
            {
                int a = _indexA;
                int b = _indexB;
                Animating = false;
                _indexA = -1;
                _indexB = -1;
                Finish?.Invoke(this, (a, b));
            }
            
            if (elementIndex == _indexA)
            {
                double t = Core.Time - _animationTime;
                t *= _diff;
                
                return elementIndex - (t / AnimationTime);
            }
            
            if (elementIndex == _indexB)
            {
                double t = Core.Time - _animationTime;
                t *= _diff;
                
                return elementIndex + (t / AnimationTime);
            }
            
            return elementIndex;
        }
    }
}
