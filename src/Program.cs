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
            
            Window w = new Program(800, 500, "Sorting");
            w.RunMultithread();
            //w.Run();
            
            Core.Terminate();
        }
        
        public Program(int width, int height, string title)
            : base(width, height, title, 4.3f)
        {
            LoadGUI();
        }
        
        private BarContainer _barDisplay;
        
        private void LoadGUI()
        {
            Actions.Push(() =>
            {
                RootElement.ClearChildren();
#if DEBUG
                LoadXml(File.ReadAllText("Layouts/test.xml"));
#else
                test.LoadGUI(RootElement.Elements, this);
#endif
                
                _barDisplay = RootElement.Find<BarContainer>("MainContainer");
                
                Slider slider = RootElement.Find<Slider>("MaxValue");
                slider.SliderPos += (_, v) =>
                {
                    _barDisplay.MaxValue = v;
                };
            });
        }
        
        internal void OnTextInput(object sender, string e)
        {
            if (!float.TryParse(e, out float v) ||
                v <= 0f || _barDisplay.Children.Overflow)
            {
                return;
            }
            
            Actions.Push(() =>
            {
                _barDisplay.AddValue(v);
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
                if (!_barDisplay.Children.AnimationsFinished) { return; }
                
                IElement f = _barDisplay.Handle.Focus;
                if (f is not Bar) { return; }
                
                _barDisplay.RemoveAt(f.Properties.ElementIndex);
                return;
            }
        }
        
        internal void BubbleSortButton(object sender, EventArgs e)
        {
            if (_barDisplay.Children.Overflow) { return; }
            
            _barDisplay.Children.BubbleSort(false);
        }
        internal void InsertionSortButton(object sender, EventArgs e)
        {
            if (_barDisplay.Children.Overflow) { return; }
            
            _barDisplay.Children.InsertionSort(false);
        }
        internal void Randomise(object sender, EventArgs e)
        {
            if (_barDisplay.Children.Overflow) { return; }
            
            Random r = new Random();
            
            for (int i = 0; i < 20; i++)
            {
                int a = r.Next(_barDisplay.ChildCount);
                int b = r.Next(_barDisplay.ChildCount);
                
                if (a == b)
                {
                    b = r.Next(_barDisplay.ChildCount);
                }
                
                _barDisplay.Children.Swap(a, b);
            }
        }
    }
}
