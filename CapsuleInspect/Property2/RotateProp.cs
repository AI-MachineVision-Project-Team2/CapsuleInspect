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
    public partial class RotateProp : UserControl
    {
        private double _angle = 90.0;
        public RotateProp()
        {
            InitializeComponent();
            vScrollBarRotate.Maximum = 360; // 최대 각도 360도
            vScrollBarRotate.Minimum = 0;   // 최소 각도 0도
            vScrollBarRotate.Scroll += vScrollBarRotate_Scroll;
            vScrollBarRotate.Value = (int)(360 - _angle); // 기본값 90에 맞춰 Value 설정
            // txtRotate 기본값 설정 (MorphologyProp처럼 텍스트 박스 초기화)
            if (txtRotate != null)
            {
                txtRotate.Text = _angle.ToString();
            }

            vScrollBarRotate.Scroll += vScrollBarRotate_Scroll;
            txtRotate.TextChanged += txtRotate_TextChanged; // 사용자 입력 처리를 위한 이벤트 추가
            txtRotate.LostFocus += txtRotate_LostFocus; // LostFocus 이벤트 추가
        }
        public double Angle => _angle;

        public event EventHandler Preview;

        private void vScrollBarRotate_Scroll(object sender, ScrollEventArgs e)
        {
            _angle = vScrollBarRotate.Maximum - vScrollBarRotate.Value;
            txtRotate.Text = _angle.ToString();
            Preview?.Invoke(this, EventArgs.Empty); // 미리보기 반영
        }

        private void txtRotate_TextChanged(object sender, EventArgs e)
        {
            // MorphologyProp처럼 사용자 입력을 파싱하여 _angle 업데이트
            string input = txtRotate.Text.Replace("도 회전", "").Trim(); // "도 회전" 제거하고 숫자만 추출

            if (double.TryParse(input, out double newAngle))
            {
                // 범위 제한 (0 ~ 360도)
                newAngle = Math.Max(0, Math.Min(360, newAngle));
                _angle = newAngle;

                // 스크롤바 동기화
                vScrollBarRotate.Value = (int)(vScrollBarRotate.Maximum - _angle);

                // 텍스트 박스 업데이트
                txtRotate.Text = _angle.ToString();

                Preview?.Invoke(this, EventArgs.Empty); // 미리보기 반영
            }
            else
            {
                // 유효하지 않은 입력 시 이전 값으로 복원
                txtRotate.Text = _angle.ToString();
            }
        }
        private void txtRotate_LostFocus(object sender, EventArgs e)
        {
            string input = txtRotate.Text.Trim();
            if (string.IsNullOrEmpty(input) || !double.TryParse(input, out double newAngle))
            {
                // 유효하지 않은 입력 시 이전 값으로 복원 (숫자만)
                txtRotate.Text = _angle.ToString();
            }
            else
            {
                // 범위 제한 (0 ~ 360도)
                newAngle = Math.Max(0, Math.Min(360, newAngle));
                if (newAngle != _angle)
                {
                    _angle = newAngle;
                    vScrollBarRotate.Value = (int)(vScrollBarRotate.Maximum - _angle);
                    txtRotate.Text = _angle.ToString();
                    Preview?.Invoke(this, EventArgs.Empty); // 미리보기 반영
                    SLogger.Write($"RotateProp: txtRotate_LostFocus - 새 _angle = {_angle}", SLogger.LogType.Info);
                }
            }
        }
    }
}
