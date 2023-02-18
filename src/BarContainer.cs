using System;
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
            Graphics = new Renderer(this);
            Children = new ChildManager(this);
            ChildLayout = new BarLayout(100d, 5d);
            LayoutManager = Zene.GUI.LayoutManager.Empty;
        }
        
        public BarLayout ChildLayout { get; }
        
        public override ChildManager Children { get; }
        
        private BarAnimator _animator;
        public BarAnimator Animator
        {
            get => _animator;
            set
            {
                if (_animator != null)
                {
                    _animator.Finish -= AniFinish;
                }
                
                _animator = value;
                _animator.Finish += AniFinish;
            }
        }
        
        public ColourF Colour { get; set; }

        public override GraphicsManager Graphics { get; }

        private void AniFinish(object sender, Vector2I i)
        {
            Children.Swap(i.X, i.Y);
            UpdateLayouts(i.X, i.Y);
        }
        private void UpdateLayouts(int a, int b)
        {
            Children[a].Trig();
            Children[b].Trig();
        }
        internal void FullRecal()
        {
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].Trig();
            }
        }
        
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);
            
            if (_animator != null &&
                _animator.Animating)
            {
                UpdateLayouts(_animator.IndexA, _animator.IndexB);
            }
        }

        public class ChildManager : Children<Bar>
        {
            public ChildManager(BarContainer source)
                : base(source)
            {
                _bc = source;
            }
            
            private BarContainer _bc;
            
            public override void Add(Bar item)
            {
                item.Layout = _bc.ChildLayout;
                
                base.Add(item);
            }
        }

        private class Renderer : GraphicsManager<BarContainer>
        {
            public Renderer(BarContainer source)
                : base(source)
            {
            }
            
            private BorderShader _shader = BorderShader.GetInstance(); 
            
            public override void OnRender(DrawManager context)
            {
                context.Shader = _shader;
                
                _shader.BorderWidth = 0d;
                _shader.Size = Bounds.Size;
                _shader.Radius = 10d / Math.Min(Bounds.Width, Bounds.Height);
                
                _shader.ColourSource = ColourSource.UniformColour;
                _shader.Colour = Source.Colour;
                
                context.Model = Matrix4.CreateScale(2d);
                context.View = Matrix4.Identity;
                context.Projection = Matrix4.Identity;
                context.Draw(Shapes.Square);
            }
        }
    }
}
