using CapsuleInspect.Core;
using CapsuleInspect.Property;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using CapsuleInspect.Algorithm;
namespace CapsuleInspect
{
    public partial class CameraForm : DockContent
    {
        private Stack<Mat> _imgHistory = new Stack<Mat>();
        private Stack<Mat> _redoImg = new Stack<Mat>();
        private bool _useAccumulativeFilter = false;
        private Mat _originalImage;
        private bool _isFirstApply = true;

        public CameraForm()
        {
            InitializeComponent();
        }
        public Bitmap GetCurrentBitmap()
        {
            return imageViewer.GetCurBitmap();
        }
        public void LoadImage(string filePath)
        {
            if (File.Exists(filePath) == false)
                return;

            //이미지 뷰어 컨트롤을 사용하여 이미지를 로드
            Image bitmap = Image.FromFile(filePath);

            var mat = BitmapConverter.ToMat((Bitmap)bitmap);

            _originalImage = mat.Clone();
            _imgHistory.Clear();
            _redoImg.Clear();
            _isFirstApply = true;

            imageViewer.LoadBitmap((Bitmap)bitmap);
        }

        private void CameraForm_Resize(object sender, EventArgs e)
        {
            int margin = 0;
            imageViewer.Width = this.Width - margin * 2;
            imageViewer.Height = this.Height - margin * 2;

            imageViewer.Location = new System.Drawing.Point(margin, margin);
        }
        public void SetFilterMode(bool enableAccumulative)
        {
            _useAccumulativeFilter = enableAccumulative;
            _isFirstApply = true; // 새 필터 시작
        }
        public void RunFilterAlgorithm(FilterType filterType, dynamic options = null)
        {
            if (_originalImage == null)
            {
                MessageBox.Show("원본 이미지가 없습니다.");
                return;
            }

            var filterAlgo = new FilterAlgorithm();
            filterAlgo.SetSourceImage(_useAccumulativeFilter
                ? BitmapConverter.ToMat(GetCurrentBitmap())
                : _originalImage.Clone());
            filterAlgo.Filter = filterType;
            filterAlgo.Options = options;

            if (filterAlgo.DoInspect())
            {
                // DoInspect 결과로 업데이트된 _srcImage를 표시
                imageViewer.LoadBitmap(filterAlgo.ResultImage.ToBitmap());
                _imgHistory.Push(filterAlgo.ResultImage.Clone());
                _redoImg.Clear();
            }
            else
            {
                MessageBox.Show("필터 적용에 실패했습니다.");
            }
        }
       
        public void Undo()
        {
            if (_imgHistory.Count > 0)
            {
                var current = BitmapConverter.ToMat(GetCurrentBitmap());
                _redoImg.Push(current.Clone());

                var prev = _imgHistory.Pop();
                imageViewer.LoadBitmap(prev.ToBitmap());
            }
            else
            {
                MessageBox.Show("되돌릴 수 있는 이전 이미지가 없습니다.");
            }
        }

        public void Redo()
        {
            if (_redoImg.Count > 0)
            {
                var current = BitmapConverter.ToMat(GetCurrentBitmap());
                _imgHistory.Push(current.Clone());

                var redo = _redoImg.Pop();
                imageViewer.LoadBitmap(redo.ToBitmap());
            }
            else
            {
                MessageBox.Show("다시 실행할 수 있는 단계가 없습니다.");
            }
        }

        //원본 이미지 복원
        public void RestoreOriginal()
        {
            if (_originalImage != null)
            {
                imageViewer.LoadBitmap(_originalImage.ToBitmap());
                _isFirstApply = true;
                _imgHistory.Clear();
                _redoImg.Clear();
            }
            else
            {
                MessageBox.Show("원본 이미지가 없습니다.");
            }
        }
        public void PreviewFilter(FilterType filterType, dynamic options = null)
        {
            if (_originalImage == null) return;

            // 원본 기준에서만 미리보기
            Mat previewBase = _useAccumulativeFilter
                ? BitmapConverter.ToMat(GetCurrentBitmap())
                : _originalImage.Clone();

            Mat previewResult = FilterAlgorithm.Apply(previewBase, filterType, options);

            imageViewer.LoadBitmap(previewResult.ToBitmap());
        }
    
        public void UpdateDisplay(Bitmap bitmap = null)
        {
            if (bitmap == null)
            {
                //업데이트시 bitmap이 없다면 InspSpace에서 가져온다
                bitmap = Global.Inst.InspStage.GetBitmap(0);
                if (bitmap == null)
                    return;
            }

            if (imageViewer != null)
                imageViewer.LoadBitmap(bitmap);
            //현재 선택된 이미지로 Preview 이미지 갱신
            //이진화 프리뷰에서 각 채널별로 설정이 적용되도록, 현재 이미지를 프리뷰 클래스 설정            
            Mat curImage = Global.Inst.InspStage.GetMat();
            Global.Inst.InspStage.PreView.SetImage(curImage);
        }

        public Bitmap GetDisplayImage()
        {
            Bitmap curImage = null;

            if (imageViewer != null)
                curImage = imageViewer.GetCurBitmap();

            return curImage;
        }
        public void UpdateImageViewer()
        {
            imageViewer.Invalidate();
        }
    }
}
