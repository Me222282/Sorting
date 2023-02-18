using System;
using Zene.Structs;
using System.Collections.Generic;
using Zene.Windowing;
using Zene.GUI;

namespace Sorting
{
    public class BarCollection : IDisposable
    {
        public BarCollection(BarContainer bc, BarAnimator ba)
        {
            _source = bc;
            _values = new List<Bar>(bc.Children.Length);
            
            // Fill values
            for (int i = 0; i < bc.Children.Length; i++)
            {
                Bar b = _source.Children[i];
                b.Animator = ba;
                b.Layout = _source.ChildLayout;
                b.Click += BarClick;
                
                _values.Add(b);
            }
            
            _animator = ba;
            _animator.Finish += NextSwap;
        }
        
        private BarContainer _source;
        private BarAnimator _animator;
        
        public bool AnimationsFinished => _swaps.Count <= 0 && !_animator.Animating;
        
        private bool _swapping = false;
        private List<Vector2I> _swaps = new List<Vector2I>();
        private object _sRef = new object();
        
        private List<Bar> _values;
        public double this[int index] => _values[index].Value;
        
        public int Length => _values.Count;
        
        public bool Overflow => _swaps.Count > 100;
        
        public void AddChild(double v)
        {
            Bar b = new Bar()
            {
                Layout = _source.ChildLayout,
                Animator = _animator,
                Value = v
            };
            b.Click += BarClick;
            
            _values.Add(b);
            _source.Children.Add(b);
            b.Trig();
        }
        
        public bool RemoveChild(int index)
        {
            if (index < 0 || index >= _values.Count)
            {
                throw new IndexOutOfRangeException();
            }
            
            if (_swaps.Count > 0 || _animator.Animating)
            {
                return false;
            }
            
            if (index == _swapIndex)
            {
                _swapIndex = -1;
            }
            
            _values.RemoveAt(index);
            Element e = _source.Children[index];
            e.Click -= BarClick;
            
            _source.Children.Remove(e);
            _source.FullRecal();
            return true;
        }
        
        public void Swap(int indexA, int indexB)
        {
            Bar value = _values[indexA];
            
            _values[indexA] = _values[indexB];
            _values[indexB] = value;
            
            if (!(_swapping || _animator.Animating))
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
            if (_swaps.Count <= 0)
            {
                _swapping = false;
                return;
            }
            
            _swapping = true;
            
            swap = _swaps[0];
            lock (_sRef)
            {
                _swaps.RemoveAt(0);
            }
            
            _animator.SwapBars(swap.X, swap.Y);
        }
        
        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed) { return; }
            
            _disposed = false;
            _animator.Finish -= NextSwap;
            
            GC.SuppressFinalize(this);
        }
        
        private int _swapIndex = -1;
        private void BarClick(object sender, MouseEventArgs e)
        {
            if (sender is not Element el) { return; }
            
            if (e.Button == MouseButton.Middle)
            {
                if (!AnimationsFinished) { return; }
                RemoveChild(el.CurrentIndex);
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
    }
}
