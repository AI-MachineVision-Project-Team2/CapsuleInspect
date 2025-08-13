using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Property;
using CapsuleInspect.Teach;
using CapsuleInspect.UIControl;
using CapsuleInspect.Util;
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

namespace CapsuleInspect
{
    public partial class CameraForm : DockContent
    {
        private Stack<Mat> _imgHistory = new Stack<Mat>();
        private Stack<Mat> _redoImg = new Stack<Mat>();
        private bool _useAccumulativeFilter = false;
        private Mat _originalImage;
        private bool _isFirstApply = true;

        // 현재 선택된 이미지 채널을 저장하는 변수

        eImageChannel _currentImageChannel = eImageChannel.Color;

        public CameraForm()
        {
            InitializeComponent();
            this.FormClosed += CameraForm_FormClosed;
            // ImageViewCtrl에서 발생하는 이벤트 처리
            imageViewer.DiagramEntityEvent += ImageViewer_DiagramEntityEvent;
            mainViewToolbar.ButtonChanged += Toolbar_ButtonChanged;
        }
        private void ImageViewer_DiagramEntityEvent(object sender, DiagramEntityEventArgs e)
        {
            SLogger.Write($"ImageViewer Action {e.ActionType.ToString()}");
            switch (e.ActionType)
            {
                case EntityActionType.Select:
                    Global.Inst.InspStage.SelectInspWindow(e.InspWindow);
                    imageViewer.Focus();
                    break;
                case EntityActionType.Inspect:
                    UpdateDiagramEntity();
                    Global.Inst.InspStage.TryInspection(e.InspWindow);
                    break;
                case EntityActionType.Add:
                    Global.Inst.InspStage.AddInspWindow(e.WindowType, e.Rect);
                    break;
                case EntityActionType.Copy:
                    Global.Inst.InspStage.AddInspWindow(e.InspWindow, e.OffsetMove);
                    break;
                case EntityActionType.Move:
                    Global.Inst.InspStage.MoveInspWindow(e.InspWindow, e.OffsetMove);
                    break;
                case EntityActionType.Resize:
                    Global.Inst.InspStage.ModifyInspWindow(e.InspWindow, e.Rect);
                    break;
                case EntityActionType.Delete:
                    Global.Inst.InspStage.DelInspWindow(e.InspWindow);
                    break;
                case EntityActionType.DeleteList:
                    Global.Inst.InspStage.DelInspWindow(e.InspWindowList);
                    break;
            }
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

            // 필터링된 이미지 초기화
            Global.Inst.InspStage.SetFilteredImage(null);

            imageViewer.LoadBitmap((Bitmap)bitmap);
            _imgHistory.Push(mat.Clone());
        }
        public Mat GetDisplayImage()
        {
            return Global.Inst.InspStage.ImageSpace.GetMat(0, _currentImageChannel);
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

            // 필터링된 이미지 초기화
            Global.Inst.InspStage.SetFilteredImage(null);

            imageViewer.LoadBitmap(bitmap);
            _imgHistory.Push(mat.Clone());
        }

        private void CameraForm_Resize(object sender, EventArgs e)
        {
            int margin = 0;
            imageViewer.Width = this.Width - mainViewToolbar.Width - margin * 2;
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
                // 필터링된 이미지를 InspStage에 저장
                Global.Inst.InspStage.SetFilteredImage(filterAlgo.ResultImage);
                Global.Inst.InspStage.UpdateDisplay(filterAlgo.ResultImage.ToBitmap());
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
                // 필터링된 이미지 업데이트
                Global.Inst.InspStage.SetFilteredImage(prev);
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
                // 필터링된 이미지 업데이트
                Global.Inst.InspStage.SetFilteredImage(redo);
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
                // 필터링된 이미지 초기화
                Global.Inst.InspStage.SetFilteredImage(null);
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
                bitmap = Global.Inst.InspStage.GetBitmap(0, _currentImageChannel);

                if (bitmap == null)
                    return;
            }

            if (imageViewer != null)
                imageViewer.LoadBitmap(bitmap);

        }

        public void UpdateImageViewer()
        {
            imageViewer.UpdateInspParam();
            imageViewer.Invalidate();
        }
        //모델 정보를 이용해, ROI 갱신
        public void UpdateDiagramEntity()
        {
            imageViewer.ResetEntity();

            Model model = Global.Inst.InspStage.CurModel;
            List<DiagramEntity> diagramEntityList = new List<DiagramEntity>();

            foreach (InspWindow window in model.InspWindowList)
            {
                if (window is null)
                    continue;

                DiagramEntity entity = new DiagramEntity()
                {
                    LinkedWindow = window,
                    EntityROI = new Rectangle(
                        window.WindowArea.X, window.WindowArea.Y,
                            window.WindowArea.Width, window.WindowArea.Height),
                    EntityColor = imageViewer.GetWindowColor(window.InspWindowType),
                    IsHold = window.IsTeach
                };
                diagramEntityList.Add(entity);
            }

            imageViewer.SetDiagramEntityList(diagramEntityList);
        }

        public void SelectDiagramEntity(InspWindow window)
        {
            imageViewer.SelectDiagramEntity(window);
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
        //새로운 ROI를 추가하는 함수
        public void AddRoi(InspWindowType inspWindowType)
        {
            imageViewer.NewRoi(inspWindowType);
        }
        // 검사 양불판정 갯수 설정 함수
        public void SetInspResultCount(int totalArea, int okCnt, int ngCnt)
        {
            imageViewer.SetInspResultCount(new InspectResultCount(totalArea, okCnt, ngCnt));
        }

        // 작업 상태 화면 표시 설정
        public void SetWorkingState(WorkingState workingState)
        {
            string state = "";
            switch (workingState)
            {
                case WorkingState.INSPECT:
                    state = "INSPECT";
                    break;

                case WorkingState.LIVE:
                    state = "LIVE";
                    break;

                case WorkingState.ALARM:
                    state = "ALARM";
                    break;
            }

            imageViewer.WorkingState = state;
            imageViewer.Invalidate();
        }

        //#18_IMAGE_CHANNEL#2 메인툴바의 버튼 이벤트를 처리하는 함수
        private void Toolbar_ButtonChanged(object sender, ToolbarEventArgs e)
        {
            switch (e.Button)
            {
                case ToolbarButton.ShowROI:
                    if (e.IsChecked)
                        UpdateDiagramEntity();
                    else
                        imageViewer.ResetEntity();
                    break;
                case ToolbarButton.ChannelColor:
                    _currentImageChannel = eImageChannel.Color;
                    UpdateDisplay();
                    break;
                case ToolbarButton.ChannelGray:
                    _currentImageChannel = eImageChannel.Gray;
                    UpdateDisplay();
                    break;
                case ToolbarButton.ChannelRed:
                    _currentImageChannel = eImageChannel.Red;
                    UpdateDisplay();
                    break;
                case ToolbarButton.ChannelGreen:
                    _currentImageChannel = eImageChannel.Green;
                    UpdateDisplay();
                    break;
                case ToolbarButton.ChannelBlue:
                    _currentImageChannel = eImageChannel.Blue;
                    UpdateDisplay();
                    break;
            }
        }

        public void SetImageChannel(eImageChannel channel)
        {
            mainViewToolbar.SetSelectButton(channel);
            UpdateDisplay();
        }

        private void CameraForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainViewToolbar.ButtonChanged -= Toolbar_ButtonChanged;

            imageViewer.DiagramEntityEvent -= ImageViewer_DiagramEntityEvent;

            this.FormClosed -= CameraForm_FormClosed;
        }
    }
}
