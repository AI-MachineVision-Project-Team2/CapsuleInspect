using OpenCvSharp;
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
    public partial class FlipProp : UserControl
    {
        public FlipProp()
        {
            InitializeComponent();
        }
        public FlipMode? SelectedFlipMode
        {
            get
            {
                if (xFlip.Checked) return FlipMode.Y;
                if (yFlip.Checked) return FlipMode.X;
                if (xyFlip.Checked) return FlipMode.XY;
                return null;
            }
        }
    }
}
