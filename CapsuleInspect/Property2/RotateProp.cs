using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapsuleInspect.Property2
{
    public partial class RotateProp : UserControl
    {
        public RotateProp()
        {
            InitializeComponent();
            vScrollBarRotate.Scroll += vScrollBarRotate_Scroll;
        }
        public int Angle => vScrollBarRotate.Maximum - vScrollBarRotate.Value;

        public event EventHandler Preview;

        private void vScrollBarRotate_Scroll(object sender, ScrollEventArgs e)
        {
            txtRotate.Text = Angle + "도 회전";
            Preview?.Invoke(this, EventArgs.Empty); // 미리보기 반영
        }
    }
}
