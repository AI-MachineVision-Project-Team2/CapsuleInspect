using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapsuleInspect.Property;
namespace CapsuleInspect.Property2
{
    public partial class CannyEdgeProp : UserControl
    {
        public CannyEdgeProp()
        {
            InitializeComponent();
            rangeSlider.ValueChanged += rangeSlider_ValueChanged;
        }
        public int Min => rangeSlider.SliderMin;
        public int Max => rangeSlider.SliderMax;

        private void rangeSlider_ValueChanged(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.PreviewFilter(FilterType.CannyEdge, new { Min = Min, Max = Max });
            }
        }
    }
}
