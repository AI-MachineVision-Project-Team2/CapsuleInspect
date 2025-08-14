using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapsuleInspect.Core
{
    public static class DesignModeDetector
    {
        public static bool IsDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                       || Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
    }
}
