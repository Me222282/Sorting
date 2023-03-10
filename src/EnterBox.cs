using System;
using Zene.GUI;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Sorting
{
    public class EnterBox : TextInput
    {
        public event EventHandler<string> TextEntered;
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e[Keys.Enter] || e[Keys.NumPadEnter])
            {
                TextEntered?.Invoke(this, Text);
                
                Text = "";
                return;
            }
            
            base.OnKeyDown(e);
        }
    }
}
