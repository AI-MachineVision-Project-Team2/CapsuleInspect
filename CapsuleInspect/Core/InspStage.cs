using BrightIdeasSoftware;
using CapsuleInspect.Algorithm;
using CapsuleInspect.Grab;
using CapsuleInspect.Inspect;
using CapsuleInspect.Sequence;
using CapsuleInspect.Setting;
using CapsuleInspect.Teach;
using CapsuleInspect.Util;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CapsuleInspect.Core
{
    //누적 카운트 증가 
    public class AccumCounter
    {
        public long Total { get; set; }
        public long OK { get; set; }
        public long NG { get; set; }
        public long NG_Scratch { get; set; }
        public long NG_Squeeze { get; set; }
        public long NG_PrintDefect { get; set; }
        public long NG_Crack { get; set; }

        public void Reset() { Total = OK = NG = NG_Scratch = NG_Squeeze = NG_PrintDefect = NG_Crack = 0; }
        public void Add(int total, int ok, int ng)
        { Total += total; OK += ok; NG += ng; }

    }


    //추가 
    //검사와 관련된 클래스를 관리하는 클래스
    public class InspStage : IDisposable
    {
        public static readonly int MAX_GRAB_BUF = 1;

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
        //#15_INSP_WORKER#5 InspWorker 클래스 선언
        private InspWorker _inspWorker = null;
        public ImageLoader _imageLoader = null;
        public ImageLoader ImageLoader
        {
            get => _imageLoader;
            private set => _imageLoader = value; // 필요 시 setter 추가
        }
        // 가장 최근 모델 파일 경로와 저장할 REGISTRY 키 변수 선언

        // 레지스트리 키 생성 또는 열기
        RegistryKey _regKey = null;

        //가장 최근 모델 파일 경로를 저장하는 변수
        private bool _lastestModelOpen = false;

        public bool UseCamera { get; set; } = false;

        private string _lotNumber;
        private string _serialID;
        [XmlIgnore]
        private Mat _filteredImage;

        // 검사 완료를 알리는 이벤트 (true=NG, false=OK)
        public event Action<string> InspectionCompleted;


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
                {
                    _saigeAI = new SaigeAI();
                    _saigeAI.SetEngineType(EngineType.SEG); // SEG로 고정
                }
                return _saigeAI;
            }
        }

        public PreviewImage PreView
        {
            get => _previewImage;
        }
        //#15_INSP_WORKER#6 InspWorker 프로퍼티
        public InspWorker InspWorker
        {
            get => _inspWorker;
        }
        //현재 모델 프로퍼티 생성
        public Model CurModel
        {
            get => _model;
        }
        public bool LiveMode { get; private set; } = false;
        public int SelBufferIndex { get; set; } = 0;
        public eImageChannel SelImageChannel { get; set; } = eImageChannel.Color;

        public AccumCounter Accum { get; } = new AccumCounter();
        public event Action<AccumCounter> AccumChanged;

        //지피티 추가
        public int LastDistinctNgCount { get; private set; }
        public event Action<int> DistinctNgCountUpdated;


        private void OnUi(Action a)
        {
            var main = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (main != null && main.InvokeRequired) main.BeginInvoke(a);
            else a();
        }

        public void ResetAccum()
        {
            Accum.Reset();
            AccumChanged?.Invoke(Accum);
        }

        public void AddAccumCount(int total, int ok, int ng)
        {
            Accum.Add(total, ok, ng);
            AccumChanged?.Invoke(Accum);
        }
        public void AddNgDetailCount(int crack, int scratch, int squeeze, int printDefect)
        {
            Accum.NG_Crack += crack;
            Accum.NG_Scratch += scratch;
            Accum.NG_Squeeze += squeeze;
            Accum.NG_PrintDefect += printDefect;

            AccumChanged?.Invoke(Accum);
        }
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
            SLogger.Write("InspStage 초기화!");
            _imageSpace = new ImageSpace();
            //이진화 알고리즘과 프리뷰 변수 인스턴스 생성

            _previewImage = new PreviewImage();
            //#15_INSP_WORKER#7 InspWorker 인스턴스 생성
            _inspWorker = new InspWorker();
            _imageLoader = new ImageLoader();

            //#REGISTRY 키 생성
            _regKey = Registry.CurrentUser.CreateSubKey("Software\\SomVision");

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
            // VisionSequence 초기화
            VisionSequence.Inst.InitSequence();
            VisionSequence.Inst.SeqCommand += SeqCommand;

            // 마지막 모델 열기 여부 확인
            if (!LastestModelOpen())
            {
                MessageBox.Show("모델 열기 실패!");
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
            //카메라 칼라 여부에 따라, 기본 채널 설정
            eImageChannel imageChannel = (pixelBpp == 24) ? eImageChannel.Color : eImageChannel.Gray;
            SetImageChannel(imageChannel);

        }
        public void SetImageBuffer(string filePath)
        {
            // SLogger.Write($"이미지 불러오기 : {filePath}");
            var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            mainForm?.UpdateFilePathTextBox(filePath);

            Mat matImage = Cv2.ImRead(filePath);

            int pixelBpp = 8;
            int imageWidth;
            int imageHeight;
            int imageStride;

            if (matImage.Type() == MatType.CV_8UC3)
                pixelBpp = 24;

            imageWidth = (matImage.Width + 3) / 4 * 4;
            imageHeight = matImage.Height;

            // 4바이트 정렬된 새로운 Mat 생성
            Mat alignedMat = new Mat();
            Cv2.CopyMakeBorder(matImage, alignedMat, 0, 0, 0, imageWidth - matImage.Width, BorderTypes.Constant, Scalar.Black);

            imageStride = imageWidth * matImage.ElemSize();

            if (_imageSpace != null)
            {
                if (_imageSpace.ImageSize.Width != imageWidth || _imageSpace.ImageSize.Height != imageHeight)
                {
                    _imageSpace.SetImageInfo(pixelBpp, imageWidth, imageHeight, imageStride);
                    SetBuffer(_imageSpace.BufferCount);
                }
            }

            int bufferIndex = 0;

            // Mat의 데이터를 byte 배열로 복사
            int bufSize = (int)(alignedMat.Total() * alignedMat.ElemSize());
            Marshal.Copy(alignedMat.Data, ImageSpace.GetInspectionBuffer(bufferIndex), 0, bufSize);

            _imageSpace.Split(bufferIndex);
            SetPreviewImage(SelImageChannel);
            DisplayGrabImage(bufferIndex);
        }

        public void CheckImageBuffer()
        {
            if (_grabManager != null && SettingXml.Inst.CamType != CameraType.None)
            {
                int imageWidth;
                int imageHeight;
                int imageStride;
                _grabManager.GetResolution(out imageWidth, out imageHeight, out imageStride);

                if (_imageSpace.ImageSize.Width != imageWidth || _imageSpace.ImageSize.Height != imageHeight)
                {
                    int pixelBpp = 8;
                    _grabManager.GetPixelBpp(out pixelBpp);

                    _imageSpace.SetImageInfo(pixelBpp, imageWidth, imageHeight, imageStride);
                    SetBuffer(_imageSpace.BufferCount);
                }
            }
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
            if (_selectedInspWindow is null)
                return;

            InspWindow inspWindow = _selectedInspWindow;

            inspWindow.DelWindowImage(index);

            MatchAlgorithm matchAlgo = (MatchAlgorithm)inspWindow.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo != null)
            {
                UpdateProperty(inspWindow);
            }
        }
        public void SetTeachingImage(InspWindow inspWindow, int index = -1)
        {
            if (inspWindow is null)
                return;

            // 새로운 코드: CameraForm에서 이미지 가져오기
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm is null)
                return;

            // 새로운 코드: GetDisplayImage()로 이미지 가져오기
            Mat curImage = cameraForm.GetDisplayImage() ?? GetFilteredImage() ?? GetMat();
            if (curImage == null)
                return;

            // ROI 영역 유효성 검사
            if (inspWindow.WindowArea.Right >= curImage.Width ||
                inspWindow.WindowArea.Bottom >= curImage.Height)
            {
                SLogger.Write("ROI 영역이 잘못되었습니다!");
                return;
            }

            // 기존 코드: Clone 사용, 새로운 코드에서는 Clone 제거
            // 두 가지 옵션을 유지하기 위해 Clone 유지 (필요 시 주석 처리 가능)
            Mat windowImage = curImage[inspWindow.WindowArea];

            if (index < 0)
                inspWindow.AddWindowImage(windowImage);
            else
                inspWindow.SetWindowImage(windowImage, index);

            inspWindow.IsPatternLearn = false;

            MatchAlgorithm matchAlgo = (MatchAlgorithm)inspWindow.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo != null)
            {
                //패턴매칭 이미지 채널 설정, 칼라인 경우 그레이로 변경
                matchAlgo.ImageChannel = SelImageChannel;
                if (matchAlgo.ImageChannel == eImageChannel.Color)
                    matchAlgo.ImageChannel = eImageChannel.Gray;

                UpdateProperty(inspWindow);
            }
        }

        //InitImageSpace를 먼저 실행하도록 수정
        public void SetBuffer(int bufferCount)
        {
            _imageSpace.InitImageSpace(bufferCount);

            if (_grabManager != null)
            {
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

            SLogger.Write("버퍼 초기화 성공!");
        }



        //inspWindow에 대한 검사구현
        //검사 결과를 출력하기 위해, 코드 수정
        public void TryInspection(InspWindow inspWindow)
        {
            UpdateDiagramEntity();
            InspWorker.TryInspect(inspWindow, InspectType.InspNone);
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

        public bool Grab(int bufferIndex)
        {
            if (_grabManager == null)
                return false;

            if (!_grabManager.Grab(bufferIndex, true))
                return false;

            return true;
        }

        //영상 취득 완료 이벤트 발생시 후처리
        private async void _multiGrab_TransferCompleted(object sender, object e)
        {
            int bufferIndex = (int)e;
            SLogger.Write($"_multiGrab_TransferCompleted {bufferIndex}");


            _imageSpace.Split(bufferIndex);

            DisplayGrabImage(bufferIndex);


            if (LiveMode)
            {
                SLogger.Write("Grab");
                await Task.Delay(100); // 너무 빠른 촬영 방지
                _grabManager.Grab(bufferIndex, true); // 반복 촬영
            }
        }

        private void DisplayGrabImage(int bufferIndex)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDisplay();
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
        //프리뷰 이미지 채널을 설정하는 함수
        public void SetPreviewImage(eImageChannel channel)
        {
            if (_previewImage is null)
                return;

            Bitmap bitmap = ImageSpace.GetBitmap(0, channel);
            _previewImage.SetImage(BitmapConverter.ToMat(bitmap));

            SetImageChannel(channel);
        }

        // 이미지 채널을 설정하는 함수
        public void SetImageChannel(eImageChannel channel)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SetImageChannel(channel);
            }
        }
        //비트맵 이미지 요청시, 이미지 채널이 있다면 SelImageChangel에 설정
        public Bitmap GetBitmap(int bufferIndex = -1, eImageChannel imageChannel = eImageChannel.None)
        {
            if (bufferIndex >= 0)
                SelBufferIndex = bufferIndex;

            //채널 정보가 유지되도록, eImageChannel.None 타입을 추가
            if (imageChannel != eImageChannel.None)
                SelImageChannel = imageChannel;

            if (Global.Inst.InspStage.ImageSpace is null)
                return null;

            return Global.Inst.InspStage.ImageSpace.GetBitmap(SelBufferIndex, SelImageChannel);
        }

        //이진화 프리뷰를 위해, ImageSpace에서 이미지 가져오기

        public Mat GetMat(int bufferIndex = 0, eImageChannel imageChannel = eImageChannel.None)
        {
            if (_filteredImage != null)
                return _filteredImage.Clone();


            int index = bufferIndex >= 0 ? bufferIndex : SelBufferIndex;
            return Global.Inst.InspStage.ImageSpace.GetMat(SelBufferIndex, imageChannel);
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
        public void ResetDisplay()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.ResetDisplay();
            }
        }
        // Mainform에서 호출되는 모델 열기와 저장 함수        
        public bool LoadModel(string filePath)
        {
            SLogger.Write($"모델 로딩:{filePath}");

            _model = _model.Load(filePath);

            if (_model is null)
            {
                SLogger.Write($"모델 로딩 실패:{filePath}");
                return false;
            }

            string inspImagePath = _model.InspectImagePath;
            if (File.Exists(inspImagePath))
            {
                Global.Inst.InspStage.SetImageBuffer(inspImagePath);
            }

            UpdateDiagramEntity();
            //마지막 저장 모델 경로를 레지스트리에 저장
            _regKey.SetValue("LastestModelPath", filePath);

            return true;
        }

        public void SaveModel(string filePath)
        {
            SLogger.Write($"모델 저장:{filePath}");

            //입력 경로가 없으면 현재 모델 저장
            if (string.IsNullOrEmpty(filePath))
                Global.Inst.InspStage.CurModel.Save();
            else
                Global.Inst.InspStage.CurModel.SaveAs(filePath);
        }

        private bool LastestModelOpen()
        {
            if (_lastestModelOpen)
                return true;

            _lastestModelOpen = true;

            string lastestModel = (string)_regKey.GetValue("LastestModelPath");
            if (File.Exists(lastestModel) == false)
                return true;

            DialogResult result = MessageBox.Show($"최근 모델을 로딩할까요?\r\n{lastestModel}", "Question", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
                return true;

            return LoadModel(lastestModel);
        }

        // 자동 연속 검사 함수
        public void CycleInspect(bool isCycle)
        {
            if (InspWorker.IsRunning)
                return;

            if (!UseCamera)
            {
                string inspImagePath = CurModel.InspectImagePath;
                if (string.IsNullOrEmpty(inspImagePath))
                {
                    SLogger.Write("[InspStage] 검사 이미지 경로가 비어있음", SLogger.LogType.Error);
                    return;
                }

                string inspImageDir = Path.GetDirectoryName(inspImagePath);
                if (!Directory.Exists(inspImageDir))
                {
                    SLogger.Write($"[InspStage] 이미지 폴더가 존재하지 않음: {inspImageDir}", SLogger.LogType.Error);
                    return;
                }

                if (!_imageLoader.IsLoadedImages())
                    _imageLoader.LoadImages(inspImageDir);
            }
            // 검사 모드 분기
            if (SettingXml.Inst.CycleMode) // 무한 자동 반복 검사
            {
                SLogger.Write("[InspStage] 무한 반복 검사 모드 시작");
                _imageLoader.CyclicMode = true;
                _inspWorker.StartCycleInspectImage();
            }
            else if (SettingXml.Inst.CycleMode2) // 단일 사이클 자동 반복 검사
            {
                SLogger.Write("[InspStage] 단일 사이클 검사 모드 시작");
                _imageLoader.CyclicMode = false;
                _imageLoader.Reset(); // 인덱스 초기화
                _inspWorker.StartSingleCycleLoop();
            }
            else // 수동 단일 검사
            {
                SLogger.Write("[InspStage] 수동 단일 검사 모드 실행");
                bool result = OneCycle();
                if (!result)
                {
                    SLogger.Write("[InspStage] 단일 검사 실패 또는 이미지 없음");
                }
            }
        }

      
        public bool OneCycle() 
        { 
            if (UseCamera) 
            { 
                if (!Grab(0)) return false; 
            } 
            else 
            { 
                if (!VirtualGrab()) return false; 
            } 
            ResetDisplay();

            //RunAISegAndShow();

            bool isDefect;
            int ngCrack, ngScratch, ngSqueeze, ngPrintDefect;
              if (!_inspWorker.RunInspect(out isDefect, out ngCrack, out ngScratch, out ngSqueeze, out ngPrintDefect))
                return false;
            // 여기서도 항상 검사 완료 이벤트를 올림
            try
            {
                string defectType = (_inspWorker != null && !string.IsNullOrEmpty(_inspWorker.LastDefectType))
                    ? _inspWorker.LastDefectType
                    : (isDefect ? "Scratch" : "OK");
                var h = InspectionCompleted;
                if (h != null) h(defectType);
            }
            catch { }
            
          
            return true; 

        }

        public void StopCycle()
        {
            if (_inspWorker != null)
                _inspWorker.Stop();
            VisionSequence.Inst.StopAutoRun();

            SetWorkingState(WorkingState.NONE);
        }

        public bool VirtualGrab()
        {
            if (_imageLoader is null)
                return false;

            string imagePath = _imageLoader.GetNextImagePath();
            if (imagePath == "")
                return false;

            Global.Inst.InspStage.SetImageBuffer(imagePath);

            _imageSpace.Split(0);

            DisplayGrabImage(0);

            return true;
        }

        //시퀀스 명령 처리
        private void SeqCommand(object sender, SeqCmd seqCmd, object Param)
        {
            switch (seqCmd)
            {
                case SeqCmd.InspStart:
                    {
                        SLogger.Write("MMI : 검사 진행", SLogger.LogType.Info);

                        string errMsg;

                        if (UseCamera)
                        {
                            if (!Grab(0))
                            {
                                errMsg = "이미지 촬영 실패";
                                SLogger.Write(errMsg, SLogger.LogType.Error);
                            }
                        }
                        else
                        {
                            if (!VirtualGrab())
                            {
                                errMsg = "가상 촬영 실패";
                                SLogger.Write(errMsg, SLogger.LogType.Error);
                            }
                        }
                        RunAISegAndShow();

                        bool isDefect;
                        int ngCrack, ngScratch, ngSqueeze, ngPrintDefect;
                        if (!_inspWorker.RunInspect(out isDefect, out ngCrack, out ngScratch, out ngSqueeze, out ngPrintDefect))
                        {
                            errMsg = "검사 실패";
                            SLogger.Write(errMsg, SLogger.LogType.Error);
                        }

                        VisionSequence.Inst.VisionCommand(Vision2Mmi.InspDone, isDefect);

                        // 세부 타입 문자열 가져와서 UI로 통지
                        string defectType = _inspWorker != null && !string.IsNullOrEmpty(_inspWorker.LastDefectType)
                                            ? _inspWorker.LastDefectType
                                            : (isDefect ? "Scratch" : "OK");

                        try
                        {
                            var handler = InspectionCompleted;
                            if (handler != null) handler(defectType);
                        }
                        catch { }
                    }
                    break;

                case SeqCmd.InspEnd:
                    {
                        SLogger.Write("MMI : 검사 종료", SLogger.LogType.Info);

                        //모든 검사 종료
                        string errMsg = "";

                        //검사 완료에 대한 처리
                        SLogger.Write("검사 종료");

                        VisionSequence.Inst.VisionCommand(Vision2Mmi.InspEnd, errMsg);
                    }
                    break;
            }
        }
        //검사를 위한 준비 작업
        public bool InspectReady(string lotNumber, string serialID)
        {
            _lotNumber = lotNumber;
            _serialID = serialID;

            LiveMode = false;
            UseCamera = SettingXml.Inst.CamType != CameraType.None ? true : false;

            Global.Inst.InspStage.CheckImageBuffer();

            ResetDisplay();

            return true;
        }

        public bool StartAutoRun()
        {
            SLogger.Write("동작 : 자동 검사 시작");

            string modelPath = CurModel.ModelPath;
            if (modelPath == "")
            {
                SLogger.Write("열려진 모델이 없습니다!", SLogger.LogType.Error);
                MessageBox.Show("열려진 모델이 없습니다!");
                return false;
            }

            LiveMode = false;
            UseCamera = SettingXml.Inst.CamType != CameraType.None ? true : false;

            SetWorkingState(WorkingState.INSPECT);
            // 자동검사 시작
            string modelName = Path.GetFileNameWithoutExtension(modelPath);
            VisionSequence.Inst.StartAutoRun(modelName);
            return true;
        }

        // 작업 상태 설정
        public void SetWorkingState(WorkingState workingState)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SetWorkingState(workingState);
            }
        }

        // 마지막 검사 결과를 기반으로, 불량 개수 계산 및 UI 업데이트
        private void PublishDistinctNg()
        {
            // Distinct-by-kind 값은 InspWorker에서 계산해 SetDistinctNgCount로 전달됨.
            OnUi(() => DistinctNgCountUpdated?.Invoke(LastDistinctNgCount));
        }
        public void SetDistinctNgCount(int count)
        {
            LastDistinctNgCount = count;
            OnUi(() => DistinctNgCountUpdated?.Invoke(count));
        }

        public void ShowSaigeResult()
        {
            if (_saigeAI is null)
                return;

            var result = _saigeAI.GetResultImage();
            if (result != null)
                UpdateDisplay(result);
        }

        public void RunAISegAndShow()
        {
            try
            {
                var bmp = GetBitmap();               // 현재 표시용 비트맵 (ImageSpace에서 꺼냄)
                if (bmp == null) return;

                var ai = AIModule;                   // SaigeAI 인스턴스 (지연 생성 프로퍼티)
                ai?.Inspect(bmp);                    // 세그먼트/검사 실행
                var result = ai?.GetResultImage();   // 그려진 결과 비트맵
                                                     // ★ 추가: DrawResult에서 반환된 defectInfos 가져오기 (DrawResult 호출 전에 resultImage 생성 필요)
                                                     // GetResultImage() 내부 DrawResult 호출로, defectInfos를 반환하도록 GetResultImage() 수정 or 별도 호출
                                                     // 간단히: ai.DrawResult(result); 대신 직접 호출
                List<DrawInspectInfo> defectInfos = ai.DrawResult(result); // 수정된 DrawResult 반환 사용

                if (result != null)
                    UpdateDisplay(result);

                // 불량 객체 그리기
                var cameraForm = MainForm.GetDockForm<CameraForm>();
                if (cameraForm != null && defectInfos != null && defectInfos.Count > 0)
                {
                    cameraForm.AddRect(defectInfos);
                }
            }
            catch (Exception ex)
            {
                SLogger.Write($"AI Inspect 실패: {ex.Message}", SLogger.LogType.Error);
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
                    //시퀀스 이벤트 해제
                    VisionSequence.Inst.SeqCommand -= SeqCommand;

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
                    _regKey.Close();
                }

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
