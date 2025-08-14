using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapsuleInspect.UIControl
{
    public partial class ToolboxCtrl : UserControl
    {
        public ToolboxCtrl()
        {
            InitializeComponent();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var helpForm = MainForm.GetDockForm<HelpForm>();
            helpForm.Show();
        }
    }
}
