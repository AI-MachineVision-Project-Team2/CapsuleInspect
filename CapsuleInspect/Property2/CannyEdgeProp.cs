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
    public class RangeChangedEventArgs : EventArgs
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public partial class CannyEdgeProp : UserControl
    {
        public event EventHandler<RangeChangedEventArgs> RangeChanged;
        public CannyEdgeProp()
        {
            InitializeComponent();
            rangeSlider.ValueChanged += rangeSlider_ValueChanged;
            // 추가: 슬라이더 초기값 설정 (binary 이미지에 적합)
            rangeSlider.SliderMin = 10; // 낮은 min 값
            rangeSlider.SliderMax = 50; // 낮은 max 값
           
        }
        public int Min => rangeSlider.SliderMin;
        public int Max => rangeSlider.SliderMax;

        private void rangeSlider_ValueChanged(object sender, EventArgs e)
        {
            UpdateCanny();
        }
        //  실시간으로 Canny 엣지 갱신
        private void UpdateCanny()
        {
            int min = Min;
            int max = Max;

            // Min > Max 방지 (BinaryProp처럼)
            if (min > max)
            {
                min = Max;
                max = Min;
            }

            // PreviewImage에 Canny 프리뷰 요청
            var preview = Global.Inst.InspStage.PreView;
            if (preview != null)
            {
                preview.SetCannyPreview(min, max);
               
            }
            else
            {
                SLogger.Write("UpdateCanny: Preview 객체 null", SLogger.LogType.Error);
            }

            // BlobAlgorithm 업데이트
            var curWindow = Global.Inst.InspStage.PreView?.CurrentInspWindow;
            if (curWindow != null)
            {
                var blob = curWindow.FindInspAlgorithm(InspectType.InspBinary) as BlobAlgorithm;
                if (blob != null)
                {
                    blob.Filter = FilterType.CannyEdge;
                    blob.FilterOptions = new { Min = min, Max = max };
                  
                }
            }

            // RangeChanged 이벤트 발생
            RangeChanged?.Invoke(this, new RangeChangedEventArgs { Min = min, Max = max });
           
        }
    }
}
