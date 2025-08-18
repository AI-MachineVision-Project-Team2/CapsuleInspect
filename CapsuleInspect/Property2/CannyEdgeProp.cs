using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Property;
using CapsuleInspect.Util;
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
    public partial class CannyEdgeProp : UserControl
    {
        public CannyEdgeProp()
        {
            InitializeComponent();
            rangeSlider.ValueChanged += rangeSlider_ValueChanged;
            // 추가: 슬라이더 초기값 설정 (binary 이미지에 적합)
            rangeSlider.SliderMin = 10; // 낮은 min 값
            rangeSlider.SliderMax = 50; // 낮은 max 값
            SLogger.Write($"CannyEdgeProp: 초기화 (Min={rangeSlider.SliderMin}, Max={rangeSlider.SliderMax})", SLogger.LogType.Info);
        }
        public int Min => rangeSlider.SliderMin;
        public int Max => rangeSlider.SliderMax;

        private void rangeSlider_ValueChanged(object sender, EventArgs e)
        {
            int min = Min;
            int max = Max;

            var preview = Global.Inst.InspStage.PreView;
            if (preview != null)
            {
                preview.SetCannyPreview(min, max);
            }
            else
            {
                SLogger.Write("CannyEdgeProp: Preview 객체 null", SLogger.LogType.Error);
            }

            var curWindow = Global.Inst.InspStage.PreView?.CurrentInspWindow;
            if (curWindow != null)
            {
                var blob = curWindow.FindInspAlgorithm(InspectType.InspBinary) as BlobAlgorithm;
                if (blob != null)
                {
                    blob.Filter = FilterType.CannyEdge;
                    blob.FilterOptions = new { Min = min, Max = max };
                    SLogger.Write("CannyEdgeProp: BlobAlgorithm FilterOptions 업데이트", SLogger.LogType.Info);
                }
            }
        }
    }
}
