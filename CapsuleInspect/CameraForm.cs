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
            _imgHistory.Push(mat.Clone());
        }
        public void LoadGrabbedImage(Bitmap bitmap)
        {
            if (bitmap == null)
                return;

            var mat = BitmapConverter.ToMat(bitmap);

            _originalImage = mat.Clone();
            _imgHistory.Clear();
            _redoImg.Clear();
            _isFirstApply = true;

            imageViewer.LoadBitmap(bitmap);
            _imgHistory.Push(mat.Clone());
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
            Mat src;
            if (filterType == FilterType.Pyramid || filterType == FilterType.Flip)
            {
                src = BitmapConverter.ToMat(GetCurrentBitmap());
            }
            else
            {
                src = _useAccumulativeFilter
                    ? BitmapConverter.ToMat(GetCurrentBitmap())
                    : _originalImage.Clone();
            }
            // 필터 적용 전 소스 이미지를 history에 저장
            _imgHistory.Push(src.Clone());
            _redoImg.Clear();

            filterAlgo.SetSourceImage(src);
            filterAlgo.Filter = filterType;
            filterAlgo.Options = options;

            if (filterAlgo.DoInspect())
            {
                bool autoFit = !(filterType == FilterType.Resize || filterType == FilterType.Pyramid);
                // DoInspect 결과로 업데이트된 _srcImage를 표시
                imageViewer.LoadBitmap(filterAlgo.ResultImage.ToBitmap(), autoFit);
                _imgHistory.Push(filterAlgo.ResultImage.Clone());
                _redoImg.Clear();
            }
            else
            {
                MessageBox.Show("필터 적용에 실패했습니다.");
                _imgHistory.Pop(); // 실패 시 소스 이미지 제거
            }
        }

        public void Undo()
        {
            if (_imgHistory.Count > 1)
            {
                var current = BitmapConverter.ToMat(GetCurrentBitmap());
                _redoImg.Push(current.Clone());

                _imgHistory.Pop(); // 현재 이미지 제거
                var prev = _imgHistory.Peek(); // 이전 이미지 가져오기
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

        //imageViewer에 검사 결과 정보를 연결해주기 위한 함수
        public void ResetDisplay()
        {
            imageViewer.ResetEntity();
        }

        //검사 결과를 그래픽으로 출력하기 위한 정보를 받는 함수
        public void AddRect(List<DrawInspectInfo> rectInfos)
        {
            imageViewer.AddRect(rectInfos);
        }
    }
}
