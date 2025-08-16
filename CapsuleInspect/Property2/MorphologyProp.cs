using CapsuleInspect.Util;
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
    public partial class MorphologyProp : UserControl
    {
        private int _kernelSize = 3; // 단일 변수로 커널 크기 관리

        public MorphologyProp()
        {
            InitializeComponent();
            // txtpixelsize 초기값 3으로 설정
            if (txtpixelsize != null)
            {
                txtpixelsize.Text = "3";
                _kernelSize = 3;
               
            }
        }

        public MorphTypes SelectedMorphType
        {
            get
            {
                if (btnOpening.Checked) return MorphTypes.Open;
                if (btnClosing.Checked) return MorphTypes.Close;
                return MorphTypes.Open; // 기본값
            }
        }

        // txtpixelsize 값이 변경될 때 커널 크기 업데이트


        private void txtpixelsize_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtpixelsize.Text, out int newSize))
            {
                // 커널 크기는 홀수여야 하며, 최소 3 이상이어야 함
                newSize = Math.Max(3, newSize);
                if (newSize % 2 == 0) newSize++; // 짝수면 다음 홀수로 조정
                _kernelSize = newSize;
                txtpixelsize.Text = _kernelSize.ToString(); // 유효한 값으로 업데이트

            }
            else
            {
                // 유효한 숫자가 아닌 경우 초기값으로 복원
                txtpixelsize.Text = _kernelSize.ToString();
            }
        }

        

        // 커널 크기를 반환하는 속성
        public int KernelSize
        {
            get { return _kernelSize; }
        }

      
    }

}
