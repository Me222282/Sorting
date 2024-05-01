using System;
using System.Collections.Generic;
using Zene.Graphics;
using Zene.GUI;
using Zene.Structs;
using Zene.Windowing;

namespace Sorting
{
    public class BarContainer : Element
    {
        public BarContainer()
        {
            _animator = new BarAnimator();
            
            Graphics = new LocalGraphics(this, OnRender);
            Children = new ChildManager(this);
            ChildLayout = new BarLayout(100d, 5d);
            LayoutManager = Zene.GUI.LayoutManager.Empty;
        }
        
        public double MaxValue { get; set; } = 100d;
        
        public BarLayout ChildLayout { get; }
        public override ChildManager Children { get; }
        public int ChildCount => Children.Length;
        
        private BarAnimator _animator;
        
        public ColourF Colour { get; set; }
        public override GraphicsManager Graphics { get; }
        
        public void AddValue(double value) => Children.Add(new Bar(value));
        public void RemoveAt(int index) => Children.RemoveAt(index);
        
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);
            
            if (_animator != null &&
                _animator.Animating)
            {
                // Index b needs to be stored locally, as the first Trig() function
                // could cause the animator to reset due to the animation finishing.
                int b = _animator.IndexB;
                
                Children[_animator.IndexA].Trig();
                Children[b].Trig();
            }
        }
        
        private void OnRender(object sender, RenderArgs e)
        {
            double radius = 10d / Math.Min(Bounds.Width, Bounds.Height);
            e.Context.DrawRoundedBox(new Box(Vector2.Zero, Bounds.Size), Colour, radius);
        }
        
        public class ChildManager : Children<Bar>, IDisposable
        {
            public ChildManager(BarContainer source)
                : base(source)
            {
                _bc = source;
                _animator = _bc._animator;
                _animator.Finish += NextSwap;
            }
            
            private BarContainer _bc;
            private BarAnimator _animator;
            
            public bool AnimationsFinished => _swaps.Count <= 0 && !_animator.Animating;
        
            private bool SwapStack => _swaps.Count > 0;
            private List<Vector2I> _swaps = new List<Vector2I>();
            private object _sRef = new object();
            
            public bool Overflow => _swaps.Count > 100;
            
            public override void Add(Bar item)
            {
                item.Layout = _bc.ChildLayout;
                item.Animator = _animator;
                item.Click += BarClick;
                
                base.Add(item);
            }
            
            public override void RemoveAt(int index)
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }
                if (_swaps.Count > 0 || _animator.Animating) { return; }
                
                if (index == _swapIndex)
                {
                    _swapIndex = -1;
                }
                
                Element e = this[index];
                e.Click -= BarClick;
                
                base.RemoveAt(index);
            }
            
            public new void Swap(int indexA, int indexB)
            {
                if (!(SwapStack || _animator.Animating))
                {
                    _animator.SwapBars(indexA, indexB);
                    return;
                }
                
                lock (_sRef)
                {
                    _swaps.Add((indexA, indexB));
                }
            }
            private void NextSwap(object s, Vector2I swap)
            {
                base.Swap(swap.X, swap.Y);
                
                if (!SwapStack) { return; }
                
                swap = _swaps[0];
                lock (_sRef)
                {
                    _swaps.RemoveAt(0);
                }
                
                _animator.SwapBars(swap.X, swap.Y);
            }
            
            private int _swapIndex = -1;
            private void BarClick(object sender, MouseEventArgs e)
            {
                if (sender is not Element el) { return; }
                
                if (e.Button == MouseButton.Middle)
                {
                    if (!AnimationsFinished) { return; }
                    RemoveAt(el.CurrentIndex);
                    return;
                }
                
                if (_swapIndex < 0)
                {
                    _swapIndex = el.CurrentIndex;
                    return;
                }
                
                Swap(_swapIndex, el.CurrentIndex);
                _swapIndex = -1;
            }
            
            private bool _disposed = false;
            public void Dispose()
            {
                if (_disposed) { return; }
                
                _disposed = false;
                _bc._animator.Finish -= NextSwap;
                
                GC.SuppressFinalize(this);
            }
        }
    }
}
