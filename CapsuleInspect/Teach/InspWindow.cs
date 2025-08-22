using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Inspect;
using CapsuleInspect.Util;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CapsuleInspect.Teach
{
    public partial class InspWindow
    {
        public InspWindowType InspWindowType { get; set; }

        public string Name { get; set; }
        public string UID { get; set; }

        public Rect WindowArea { get; set; }
        public Rect InspArea { get; set; }
        public bool IsTeach { get; set; } = false;

        // Xml Serialize를 위해서, Element을 명확하게 알려줘야 함        
        [XmlElement("InspAlgorithm")]
        public List<InspAlgorithm> AlgorithmList { get; set; } = new List<InspAlgorithm>();

        // 검사 결과를 저장하기 위한 리스트
        public List<InspResult> InspResultList { get; set; } = new List<InspResult>();


        // 패턴매칭에 필요한 티칭 이미지 관리 기능
        // Xml Serialize를 하지 않도록 설정
        [XmlIgnore]
        public List<Mat> _windowImages = new List<Mat>();
        public void AddWindowImage(Mat image)
        {
            if (image is null)
                return;

            _windowImages.Add(image.Clone());
        }

        public void ResetWindowImages()
        {
            _windowImages.Clear();
        }

        public void SetWindowImage(Mat image, int index)
        {
            if (image is null)
                return;

            if (index < 0 || index >= _windowImages.Count)
                return;

            _windowImages[index] = image.Clone();
        }

        public void DelWindowImage(int index)
        {
            if (index < 0 || index >= _windowImages.Count)
                return;

            _windowImages.RemoveAt(index);

            IsPatternLearn = false;
            PatternLearn();
        }

        public bool IsPatternLearn { get; set; } = false;

        public InspWindow()
        {
        }

        public InspWindow(InspWindowType windowType, string name)
        {
            InspWindowType = windowType;
            Name = name;
        }

        public InspWindow Clone(OpenCvSharp.Point offset, bool includeChildren = true)
        {
            InspWindow cloneWindow = InspWindowFactory.Inst.Create(this.InspWindowType, false);
            cloneWindow.WindowArea = this.WindowArea + offset;
            cloneWindow.IsTeach = false;
            cloneWindow.IsPatternLearn = false;

            foreach (var img in _windowImages.ToList())
                cloneWindow._windowImages.Add(img?.Clone());

            foreach (InspAlgorithm algo in AlgorithmList)
            {
                var cloneAlgo = algo.Clone();
                cloneWindow.AlgorithmList.Add(cloneAlgo);
            }

            return cloneWindow;
        }
        //InspWindow에 있는 템플릿 이미지를 MatchAlgorithm에 등록하는 함수
        public bool PatternLearn()
        {
            if (IsPatternLearn == true)
                return true;

            foreach (var algorithm in AlgorithmList)
            {
                if (algorithm.InspectType == InspectType.InspMatch)
                {
                    MatchAlgorithm matchAlgo = (MatchAlgorithm)algorithm;
                    matchAlgo.ResetTemplateImages();

                    for (int i = 0; i < _windowImages.Count; i++)
                    {
                        Mat tempImage = _windowImages[i];
                        if (tempImage is null)
                            continue;

                        if (tempImage.Type() == MatType.CV_8UC3)
                        {
                            Mat grayImage = new Mat();
                            Cv2.CvtColor(tempImage, grayImage, ColorConversionCodes.BGR2GRAY);
                            matchAlgo.AddTemplateImage(grayImage);
                        }
                        else
                        {
                            matchAlgo.AddTemplateImage(tempImage);
                        }
                    }
                }
            }

            IsPatternLearn = true;

            return true;
        }

        //타입에 따라 알고리즘을 추가하는 함수
        public bool AddInspAlgorithm(InspectType inspType)
        {

            InspAlgorithm inspAlgo = null;

            switch (inspType)
            {
                case InspectType.InspBinary:
                    inspAlgo = new BlobAlgorithm();
                    break;
                case InspectType.InspMatch:
                    inspAlgo = new MatchAlgorithm();
                    break;
                case InspectType.InspFilter:
                    inspAlgo = new FilterAlgorithm();
                    break;
                case InspectType.InspAI:
                    inspAlgo = new AIAlgorithm();
                    break;
            }

            if (inspAlgo is null)
                return false;

            AlgorithmList.Add(inspAlgo);

            return true;
        }

        //알고리즘을 리스트로 관리하므로, 필요한 타입의 알고리즘을 찾는 함수
        public InspAlgorithm FindInspAlgorithm(InspectType inspType)
        {
            return AlgorithmList.Find(algo => algo.InspectType == inspType);
        }

        //클래스 내에서, 인자로 입력된 타입의 알고리즘을 검사하거나,
        ///모든 알고리즘을 검사하는 옵션을 가지는 검사 함수
        public virtual bool DoInpsect(InspectType inspType)
        {
            // 필터링된 이미지를 가져옴
            Mat inputImage = Global.Inst.InspStage.GetFilteredImage() ?? Global.Inst.InspStage.GetMat(0, eImageChannel.Color);
            if (inputImage == null)
            {
                SLogger.Write("검사에 사용할 이미지가 없습니다.", SLogger.LogType.Error);
                return false;
            }
            // 1. 먼저 InspFilter 실행하여 _filteredImage 설정
            var filterAlgo = AlgorithmList.Find(algo => algo.InspectType == InspectType.InspFilter);
            if (filterAlgo != null && filterAlgo.IsUse && (inspType == InspectType.InspNone || inspType == InspectType.InspFilter))
            {
                filterAlgo.SetInspData(inputImage);
                if (filterAlgo.DoInspect())
                {
                    // 필터링된 이미지 갱신
                    inputImage = Global.Inst.InspStage.GetFilteredImage()?.Clone() ?? inputImage;
                }
            }

            // 2. 나머지 알고리즘(InspMatch, InspBinary) 실행
            foreach (var inspAlgo in AlgorithmList)
            {
                if (inspAlgo == filterAlgo) continue; // 필터는 이미 실행됨
                if (inspAlgo.IsUse && (inspType == InspectType.InspNone || inspAlgo.InspectType == inspType))
                {
                    inspAlgo.SetInspData(inputImage);
                    inspAlgo.DoInspect();
                }
            }

            // 3. InspAIModule 별도 처리
            if (inspType == InspectType.InspNone || inspType == InspectType.InspAI)
            {
                Bitmap inputBitmap = inputImage.ToBitmap();
                Global.Inst.InspStage.AIModule.Inspect(inputBitmap);
            }
            return true;
        }

        public bool IsDefect()
        {
            foreach (InspAlgorithm algo in AlgorithmList)
            {
                if (!algo.IsInspected)
                    continue;

                if (algo.IsDefect)
                    return true;
            }
            return false;
        }

        public virtual bool OffsetMove(OpenCvSharp.Point offset)
        {
            Rect windowRect = WindowArea;
            windowRect.X += offset.X;
            windowRect.Y += offset.Y;
            WindowArea = windowRect;
            return true;
        }

        public bool SetInspOffset(OpenCvSharp.Point offset)
        {
            InspArea = WindowArea + offset;
            AlgorithmList.ForEach(algo => algo.InspRect = algo.TeachRect + offset);
            return true;
        }


        // InspWindow가 가지고 있는 이미지를 모델 폴더에 저장과 로딩
        public virtual bool SaveInspWindow(Model curModel)
        {
            if (curModel is null)
                return false;

            string imgDir = Path.Combine(Path.GetDirectoryName(curModel.ModelPath), "Images");
            if (!Directory.Exists(imgDir))
            {
                Directory.CreateDirectory(imgDir);
            }

            for (int i = 0; i < _windowImages.Count; i++)
            {
                Mat img = _windowImages[i];
                if (img is null)
                    continue;

                string targetPath = Path.Combine(imgDir, $"{UID}_{i}.png");
                Cv2.ImWrite(targetPath, img);
            }

            return true;
        }

        public virtual bool LoadInspWindow(Model curModel)
        {
            if (curModel is null)
                return false;

            string imgDir = Path.Combine(Path.GetDirectoryName(curModel.ModelPath), "Images");

            foreach (InspAlgorithm algo in AlgorithmList)
            {
                if (algo is null)
                    continue;

                if (algo.InspectType == InspectType.InspMatch)
                {
                    MatchAlgorithm matchAlgo = algo as MatchAlgorithm;

                    int i = 0;
                    while (true)
                    {
                        string targetPath = Path.Combine(imgDir, $"{UID}_{i}.png");
                        if (!File.Exists(targetPath))
                            break;

                        Mat windowImage = Cv2.ImRead(targetPath);
                        if (windowImage != null)
                        {
                            AddWindowImage(windowImage);
                        }

                        i++;
                    }
                    IsPatternLearn = false;
                }
            }

            return true;
        }

        // 검사 결과를 초기화 및 추가 함수
        public void ResetInspResult()
        {
            foreach (var algorithm in AlgorithmList)
            {
                algorithm.ResetResult();
            }

            InspResultList.Clear();
        }

        public void AddInspResult(InspResult inspResult)
        {
            InspResultList.Add(inspResult);
        }
    }
}
