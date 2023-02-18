using System;
using System.IO;
using Zene.Graphics;
using Zene.GUI;
using Zene.Structs;
using Zene.Windowing;

namespace Sorting
{
    class Program : GUIWindow
    {
        static void Main(string[] args)
        {
            Core.Init();
            
            Window w = new Program(800, 500, "BEANS!");
            w.RunMultithread();
            //w.Run();
            
            Core.Terminate();
        }
        
        public Program(int width, int height, string title)
            : base(width, height, title, 4.3)
        {
            LoadGUI();
        }
        
        private BarContainer _barDisplay;
        private readonly BarAnimator _animator = new BarAnimator();
        private BarCollection _collection;
        
        private void LoadGUI()
        {
            Actions.Push(() =>
            {
                RootElement.ClearChildren();
                LoadXml(File.ReadAllText("Layouts/test.xml"));
                
                _barDisplay = RootElement.Find<BarContainer>();
                _barDisplay.Animator = _animator;
                _collection = new BarCollection(_barDisplay, _animator);
                
                Slider slider = RootElement.Find<Slider>();
                slider.SliderPos += (_, v) =>
                {
                    _barDisplay.ChildLayout.MaxValue = v;
                };
            });
        }
        
        private void OnTextInput(object sender, string e)
        {
            if (!double.TryParse(e, out double v) ||
                v <= 0d || _collection.Overflow)
            {
                return;
            }
            
            Actions.Push(() =>
            {
                _collection.AddChild(v);
            });
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (e[Keys.R])
            {
                LoadGUI();
                return;
            }
            if (e[Keys.Delete])
            {
                if (!_collection.AnimationsFinished) { return; }
                
                IElement f = _barDisplay.Hande.Focus;
                if (f is not Bar) { return; }
                
                _collection.RemoveChild(f.Properties.ElementIndex);
                return;
            }
        }
        
        private void BubbleSortButton(object sender, EventArgs e)
        {
            if (_collection == null || _collection.Overflow) { return; }
            
            _collection.BubbleSort(false);
        }
        private void InsertionSortButton(object sender, EventArgs e)
        {
            if (_collection == null || _collection.Overflow) { return; }
            
            _collection.InsertionSort(false);
        }
        private void Randomise(object sender, EventArgs e)
        {
            if (_collection == null || _collection.Overflow) { return; }
            
            Random r = new Random();
            
            for (int i = 0; i < 20; i++)
            {
                int a = r.Next(_collection.Length);
                int b = r.Next(_collection.Length);
                
                if (a == b)
                {
                    b = r.Next(_collection.Length);
                }
                
                _collection.Swap(a, b);
            }
        }
    }
}
