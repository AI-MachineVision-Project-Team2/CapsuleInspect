using CapsuleInspect.Algorithm;
using CapsuleInspect.Grab;
using CapsuleInspect.Inspect;
using CapsuleInspect.Setting;
using CapsuleInspect.Teach;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CapsuleInspect.Core
{

    //검사와 관련된 클래스를 관리하는 클래스
    public class InspStage : IDisposable
    {
        public static readonly int MAX_GRAB_BUF = 5;
        
        private ImageSpace _imageSpace = null;
        //Dispose도 GrabModel에서 상속받아 사용
        private GrabModel _grabManager = null;
        private CameraType _camType = CameraType.WebCam;

        SaigeAI _saigeAI; // SaigeAI 인스턴스

        //이진화 프리뷰에 필요한 변수 선언
       
        private PreviewImage _previewImage = null;
        //모델과 선택된 ROI 윈도우 변수 선언
        private Model _model = null;

        private InspWindow _selectedInspWindow = null;
        // 필터링된 이미지를 저장하는 필드 추가
        private Mat _filteredImage = null;
        public InspStage() { }
        public ImageSpace ImageSpace
        {
            get => _imageSpace;
        }

        public SaigeAI AIModule
        {
            get
            {
                if (_saigeAI is null)
                    _saigeAI = new SaigeAI();
                return _saigeAI;
            }
        }

       
        public PreviewImage PreView
        {
            get => _previewImage;
        }
        //현재 모델 프로퍼티 생성
        public Model CurModel
        {
            get => _model;
        }
        public bool LiveMode { get; private set; } = false;
        public int SelBufferIndex { get; set; } = 0;
        public eImageChannel SelImageChannel { get; set; } = eImageChannel.Gray;



        public void ToggleLiveMode()
        {
            LiveMode = !LiveMode;
        }
        // 필터링된 이미지를 가져오거나 설정하는 메서드
        public Mat GetFilteredImage() => _filteredImage?.Clone();
        public void SetFilteredImage(Mat filtered)
        {
            _filteredImage?.Dispose();
            _filteredImage = filtered?.Clone();
            
            // UI 업데이트
            if (_filteredImage != null)
            {
                UpdateDisplay(_filteredImage.ToBitmap());
            }
        }
        public CameraType GetCurrentCameraType()
        {
            return _camType;
        }
        public void SetCameraType(CameraType camType)
        {
            if (_camType == camType)
                return;

            _camType = camType;

            _grabManager?.Dispose();
            _grabManager = null;

            switch (_camType)
            {
                case CameraType.WebCam:
                    _grabManager = new WebCam();
                    break;
                case CameraType.HikRobotCam:
                    _grabManager = new HikRobotCam();
                    break;
                case CameraType.None:
                    return;
            }

            if (_grabManager.InitGrab())
            {
                _grabManager.TransferCompleted += _multiGrab_TransferCompleted;
                InitModelGrab(MAX_GRAB_BUF);
            }
        }

        public bool Initialize()
        {
            _imageSpace = new ImageSpace();
            //이진화 알고리즘과 프리뷰 변수 인스턴스 생성
            
            _previewImage = new PreviewImage();

            // 모델 인스턴스 생성
            _model = new Model();

            //환경설정에서 설정값 가져오기
            LoadSetting();
            switch (_camType)
            {
                //타입에 따른 카메라 인스턴스 생성
                case CameraType.WebCam:
                    {
                        _grabManager = new WebCam();
                        break;
                    }
                case CameraType.HikRobotCam:
                    {
                        _grabManager = new HikRobotCam();
                        break;
                    }
            }


            if (_grabManager != null && _grabManager.InitGrab() == true)
            {
                _grabManager.TransferCompleted += _multiGrab_TransferCompleted;

                InitModelGrab(MAX_GRAB_BUF);
            }

            return true;
        }

        private void LoadSetting()
        {
            //카메라 설정 타입 얻기
            _camType = SettingXml.Inst.CamType;
        }

        public void InitModelGrab(int bufferCount)
        {
            if (_grabManager == null)
                return;

            int pixelBpp = 8;
            _grabManager.GetPixelBpp(out pixelBpp);

            int inspectionWidth;
            int inspectionHeight;
            int inspectionStride;
            _grabManager.GetResolution(out inspectionWidth, out inspectionHeight, out inspectionStride);

            if (_imageSpace != null)
            {
                _imageSpace.SetImageInfo(pixelBpp, inspectionWidth, inspectionHeight, inspectionStride);
            }

            SetBuffer(bufferCount);

        }

        //속성창 업데이트 기준을 알고리즘에서 InspWindow로 변경
        private void UpdateProperty(InspWindow inspWindow)

        {
            if (inspWindow is null)
                return;

            PropertiesForm propertiesForm = MainForm.GetDockForm<PropertiesForm>();
            if (propertiesForm is null)
                return;

            propertiesForm.UpdateProperty(inspWindow);
        }

        //패턴매칭 속성창과 연동된 패턴 이미지 관리 함수
        public void UpdateTeachingImage(int index)
        {
            if (_selectedInspWindow is null)
                return;

            SetTeachingImage(_selectedInspWindow, index);
        }

        public void DelTeachingImage(int index)
        {
            if (_selectedInspWindow == null) return;

            var matchAlgo = (MatchAlgorithm)_selectedInspWindow.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo == null) return;

            matchAlgo.DelTemplateImage(index); // MatchAlgorithm 템플릿 삭제
            _selectedInspWindow.DelWindowImage(index); // InspWindow 동기화

            // UI 갱신
            var propForm = MainForm.GetDockForm<PropertiesForm>();
            if (propForm != null)
            {
                propForm.ShowProperty(_selectedInspWindow);
            }
        }

        public void SetTeachingImage(InspWindow inspWindow, int index = -1)
        {
            if (inspWindow is null)
                return;

            // 필터링된 이미지 우선, 없으면 원본
            Mat curImage = GetFilteredImage() ?? GetMat();
            if (curImage == null) return;

            if (inspWindow.WindowArea.Right >= curImage.Width ||
                inspWindow.WindowArea.Bottom >= curImage.Height)
            {
                Console.Write("ROI 영역이 잘못되었습니다!");
                return;
            }

            Mat windowImage = curImage[inspWindow.WindowArea].Clone();
            // MatchAlgorithm에 템플릿 설정
            var matchAlgo = (MatchAlgorithm)inspWindow.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo != null)
            {
                if (index < 0)
                {
                    matchAlgo.AddTemplateImage(windowImage);
                    inspWindow.AddWindowImage(windowImage); // 동기화 유지
                }
                else
                {
                    matchAlgo.SetTemplateImage(windowImage, index); // Set 메서드 필요
                    inspWindow.SetWindowImage(windowImage, index); // 동기화 유지
                }
            }

            inspWindow.IsPatternLearn = false;

            // UI 갱신
            var propForm = MainForm.GetDockForm<PropertiesForm>();
            if (propForm != null)
            {
                propForm.ShowProperty(inspWindow); // 탭 갱신
            }
        }

        public void SetBuffer(int bufferCount)
        {
            if (_grabManager == null)
                return;

            if (_imageSpace.BufferCount == bufferCount)
                return;

            _imageSpace.InitImageSpace(bufferCount);
            _grabManager.InitBuffer(bufferCount);

            for (int i = 0; i < bufferCount; i++)
            {
                _grabManager.SetBuffer(
                    _imageSpace.GetInspectionBuffer(i),
                    _imageSpace.GetnspectionBufferPtr(i),
                    _imageSpace.GetInspectionBufferHandle(i),
                    i);
            }
        }
        //inspWindow에 대한 검사구현
        public void TryInspection(InspWindow inspWindow = null)
        {
            if (inspWindow is null)
            {
                if (_selectedInspWindow is null)
                    return;

                inspWindow = _selectedInspWindow;
            }

            UpdateDiagramEntity();

            List<DrawInspectInfo> totalArea = new List<DrawInspectInfo>();

            Rect windowArea = inspWindow.WindowArea;

            foreach (var inspAlgo in inspWindow.AlgorithmList)
            {
                if (!inspAlgo.IsUse)
                    continue;

                //검사 영역 초기화
                inspAlgo.TeachRect = windowArea;
                inspAlgo.InspRect = windowArea;

                InspectType inspType = inspAlgo.InspectType;

                switch (inspType)
                {
                    case InspectType.InspBinary:
                        {
                            BlobAlgorithm blobAlgo = (BlobAlgorithm)inspAlgo;

                            Mat srcImage = Global.Inst.InspStage.GetMat();
                            blobAlgo.SetInspData(srcImage);

                            if (blobAlgo.DoInspect())
                            {
                                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                                int resultCnt = blobAlgo.GetResultRect(out resultArea);
                                if (resultCnt > 0)
                                {
                                    totalArea.AddRange(resultArea);
                                }
                            }

                            break;
                        }
                }

                if (inspAlgo.DoInspect())
                {
                    List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                    int resultCnt = inspAlgo.GetResultRect(out resultArea);
                    if (resultCnt > 0)
                    {
                        totalArea.AddRange(resultArea);
                    }
                }
            }

            if (totalArea.Count > 0)
            {
                //찾은 위치를 이미지상에서 표시
                var cameraForm = MainForm.GetDockForm<CameraForm>();
                if (cameraForm != null)
                {
                    cameraForm.AddRect(totalArea);
                }
            }
        }

        //#10_INSPWINDOW#13 ImageViewCtrl에서 ROI 생성,수정,이동,선택 등에 대한 함수
        public void SelectInspWindow(InspWindow inspWindow)
        {
            _selectedInspWindow = inspWindow;

            var propForm = MainForm.GetDockForm<PropertiesForm>();
            if (propForm != null)
            {
                if (inspWindow is null)
                {
                    propForm.ResetProperty();
                    return;
                }

                //속성창을 현재 선택된 ROI에 대한 것으로 변경
                propForm.ShowProperty(inspWindow);
            }

            UpdateProperty(inspWindow);

            Global.Inst.InspStage.PreView.SetInspWindow(inspWindow);
        }

        //ImageViwer에서 ROI를 추가하여, InspWindow생성하는 함수
        public void AddInspWindow(InspWindowType windowType, Rect rect)
        {
            InspWindow inspWindow = _model.AddInspWindow(windowType);
            if (inspWindow is null)
                return;

            inspWindow.WindowArea = rect;
            inspWindow.IsTeach = false;
            // 새로운 ROI가 추가되면, 티칭 이미지 추가
            SetTeachingImage(inspWindow);

            UpdateProperty(inspWindow);
            UpdateDiagramEntity();

            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SelectDiagramEntity(inspWindow);
                SelectInspWindow(inspWindow);
            }
        }

        public bool AddInspWindow(InspWindow sourceWindow, OpenCvSharp.Point offset)
        {
            InspWindow cloneWindow = sourceWindow.Clone(offset);
            if (cloneWindow is null)
                return false;

            if (!_model.AddInspWindow(cloneWindow))
                return false;

            UpdateProperty(cloneWindow);
            UpdateDiagramEntity();

            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SelectDiagramEntity(cloneWindow);
                SelectInspWindow(cloneWindow);
            }

            return true;
        }


        //입력된 윈도우 이동
        public void MoveInspWindow(InspWindow inspWindow, OpenCvSharp.Point offset)
        {
            if (inspWindow == null)
                return;

            inspWindow.OffsetMove(offset);
            UpdateProperty(inspWindow);
        }

        //기존 ROI 수정되었을때, 그 정보를 InspWindow에 반영
        public void ModifyInspWindow(InspWindow inspWindow, Rect rect)
        {
            if (inspWindow == null)
                return;

            inspWindow.WindowArea = rect;
            inspWindow.IsTeach = false;

            UpdateProperty(inspWindow);
        }

        // InspWindow 삭제하기
        public void DelInspWindow(InspWindow inspWindow)
        {
            _model.DelInspWindow(inspWindow);
            UpdateDiagramEntity();
        }


        public void DelInspWindow(List<InspWindow> inspWindowList)
        {
            _model.DelInspWindowList(inspWindowList);
            UpdateDiagramEntity();
        }

        public void Grab(int bufferIndex)
        {
            if (_grabManager == null)
                return;

            _grabManager.Grab(bufferIndex, true);
            // Grab 후 필터링된 이미지 초기화 (필요 시)
            SetFilteredImage(null);
        }

        //영상 취득 완료 이벤트 발생시 후처리
        private async void _multiGrab_TransferCompleted(object sender, object e)
        {
            int bufferIndex = (int)e;
            Console.WriteLine($"_multiGrab_TransferCompleted {bufferIndex}");

            _imageSpace.Split(bufferIndex);

            DisplayGrabImage(bufferIndex);

            if (_previewImage != null)
            {
                Bitmap bitmap = ImageSpace.GetBitmap(0);
                _previewImage.SetImage(BitmapConverter.ToMat(bitmap));
            }
            if (LiveMode)
            {
                //SLogger.Write("Grab");
                await Task.Delay(100); // 너무 빠른 촬영 방지
                _grabManager.Grab(bufferIndex, true); // 반복 촬영
            }
        }

        private void DisplayGrabImage(int bufferIndex)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.LoadGrabbedImage(ImageSpace.GetBitmap(bufferIndex));
            }
        }

        public void UpdateDisplay(Bitmap bitmap)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDisplay(bitmap);
            }
        }
        public Bitmap GetBitmap(int bufferIndex = -1)
        {
            if (Global.Inst.InspStage.ImageSpace is null)
                return null;

            return Global.Inst.InspStage.ImageSpace.GetBitmap();
        }
        //이진화 프리뷰를 위해, ImageSpace에서 이미지 가져오기

           public Mat GetMat(int bufferIndex = 0, eImageChannel imageChannel = eImageChannel.None)
        {
            if (_filteredImage != null)
                return _filteredImage.Clone();

            if (imageChannel != eImageChannel.None)
                SelImageChannel = imageChannel;
            int index = bufferIndex >= 0 ? bufferIndex : SelBufferIndex;
            return ImageSpace.GetMat(index, SelImageChannel);
        }



        //변경된 모델 정보 갱신하여, ImageViewer와 모델트리에 반영
        public void UpdateDiagramEntity()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDiagramEntity();
            }

            ModelTreeForm modelTreeForm = MainForm.GetDockForm<ModelTreeForm>();
            if (modelTreeForm != null)
            {
                modelTreeForm.UpdateDiagramEntity();
            }
        }

        //이진화 임계값 변경시, 프리뷰 갱신
        public void RedrawMainView()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateImageViewer();
            }
        }

        #region Disposable

        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_saigeAI != null)
                    {
                        _saigeAI.Dispose();
                        _saigeAI = null;
                    }
                    if (_grabManager != null)
                    {
                        _grabManager.Dispose();
                        _grabManager = null;
                    }
                }

                // Dispose unmanaged managed resources.

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion //Disposable
    }
}
