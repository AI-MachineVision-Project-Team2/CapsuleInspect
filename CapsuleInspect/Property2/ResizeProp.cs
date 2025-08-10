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
    public partial class ResizeProp : UserControl
    {
        public ResizeProp()
        {
            InitializeComponent();
        }
        public NumericUpDown NumericUpDownX => numericUpDown1;
        public NumericUpDown NumericUpDownY => numericUpDown2;

    }
}
