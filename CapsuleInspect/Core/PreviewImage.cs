using CapsuleInspect.Algorithm;
using CapsuleInspect.Property;
using CapsuleInspect.Teach;
using CapsuleInspect.Util;
using MvCameraControl;
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
    public class PreviewImage
    {
        private Mat _orinalImage = null;
        private Mat _previewImage = null;
        // 프리뷰를 위한 InspWindow 변수
        private InspWindow _inspWindow = null;
        private Mat _binaryResultImage = null;
        private bool _usePreview = true;
        public InspWindow CurrentInspWindow => _inspWindow;
        public void SetImage(Mat image)
        {
            _orinalImage = image;
            _previewImage = new Mat();
            _binaryResultImage = null;
        }
        public void SetBinaryResultImage(Mat binary)
        {
            _binaryResultImage = binary.Clone();
        }

        // 프리뷰를 위한 InspWindow 설정
        public void SetInspWindow(InspWindow inspwindow)
        {
            _inspWindow = inspwindow;
        }

        //ShowBinaryMode에 따라 이진화 프리뷰 진행
        public void SetBinary(int lowerValue, int upperValue, bool invert, ShowBinaryMode showBinMode)
        {
            if (_usePreview == false)
                return;

            if (_orinalImage == null)
                return;

            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm == null)
                return;

            Bitmap bmpImage;
            if (showBinMode == ShowBinaryMode.ShowBinaryNone)
            {
                bmpImage = BitmapConverter.ToBitmap(_orinalImage);
                cameraForm.UpdateDisplay(bmpImage);
                return;
            }

            Rect windowArea = new Rect(0, 0, _orinalImage.Width, _orinalImage.Height);
            // InspWindow가 있다면 프리뷰 설정 영역을 ROI로 변경
            if (_inspWindow != null)
            {
                windowArea = _inspWindow.WindowArea;
            }

            Mat orgRoi = _orinalImage[windowArea];
            //Cv2.ImShow("org", _orinalImage); // 디버깅용
            //Cv2.ImShow("orgRoi", orgRoi); // 디버깅용
            //Cv2.WaitKey(1); // OpenCV 창 업데이트

            Mat grayImage = new Mat();
            if (orgRoi.Type() == MatType.CV_8UC3)
                Cv2.CvtColor(orgRoi, grayImage, ColorConversionCodes.BGR2GRAY);
            else
                grayImage = orgRoi;

            Mat binaryMask = new Mat();
            Cv2.InRange(grayImage, lowerValue, upperValue, binaryMask);

            if (invert)
                binaryMask = ~binaryMask;

          
            Mat fullBinaryMask = Mat.Zeros(_orinalImage.Size(), MatType.CV_8UC1);
            binaryMask.CopyTo(new Mat(fullBinaryMask, windowArea));

            if (showBinMode == ShowBinaryMode.ShowBinaryOnly)
            {
                if (orgRoi.Type() == MatType.CV_8UC3)
                {
                    Mat colorBinary = new Mat();
                    Cv2.CvtColor(binaryMask, colorBinary, ColorConversionCodes.GRAY2BGR);
                    _previewImage = _orinalImage.Clone();
                    colorBinary.CopyTo(new Mat(_previewImage, windowArea));
                }
                else
                {
                    _previewImage = _orinalImage.Clone();
                    binaryMask.CopyTo(new Mat(_previewImage, windowArea));
                }

                bmpImage = BitmapConverter.ToBitmap(_previewImage);
                _binaryResultImage = fullBinaryMask.Clone();
                //Cv2.ImShow("Binary Result", _binaryResultImage); // 디버깅용

                cameraForm.UpdateDisplay(bmpImage);
                return;
            }

            Scalar highlightColor;
            if (showBinMode == ShowBinaryMode.ShowBinaryHighlightRed)
                highlightColor = new Scalar(0, 0, 255);
            else if (showBinMode == ShowBinaryMode.ShowBinaryHighlightGreen)
                highlightColor = new Scalar(0, 255, 0);
            else //(showBinMode == ShowBinaryMode.ShowBinaryHighlightBlue)
                highlightColor = new Scalar(255, 0, 0);

            // 원본 이미지 복사본을 만들어 이진화된 부분에만 색을 덧씌우기
            Mat overlayImage;
            if (_orinalImage.Type() == MatType.CV_8UC1)
            {
                overlayImage = new Mat();
                Cv2.CvtColor(_orinalImage, overlayImage, ColorConversionCodes.GRAY2BGR);

                Mat colorOrinal = overlayImage.Clone();

                overlayImage.SetTo(highlightColor, fullBinaryMask); // 빨간색으로 마스킹

                // 원본과 합성 (투명도 적용)
                Cv2.AddWeighted(colorOrinal, 0.7, overlayImage, 0.3, 0, _previewImage);
            }
            else
            {
                overlayImage = _orinalImage.Clone();
                overlayImage.SetTo(highlightColor, fullBinaryMask); // 빨간색으로 마스킹

                // 원본과 합성 (투명도 적용)
                Cv2.AddWeighted(_orinalImage, 0.7, overlayImage, 0.3, 0, _previewImage);
            }

            bmpImage = BitmapConverter.ToBitmap(_previewImage);

           

            cameraForm.UpdateDisplay(bmpImage);
        }

        public void SetCannyPreview(int min, int max)
        {
            // 1. 입력 검증: 원본 이미지 또는 ROI가 없으면 종료
            if (_orinalImage == null || _inspWindow == null)
            {
                SLogger.Write("SetCannyPreview: 원본 이미지 또는 InspWindow가 null입니다.", SLogger.LogType.Error);
                return;
            }

            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm == null)
            {
                SLogger.Write("SetCannyPreview: CameraForm을 찾을 수 없습니다.", SLogger.LogType.Error);
                return;
            }

            // 2. ROI 영역 가져오기
            Rect roi = _inspWindow.WindowArea;
            // baseImage: 현재 프리뷰 유지 (ShowBinaryOnly면 이진화 이미지)
            Mat baseImage;
            if (_orinalImage.Channels() == 3)
            {
                // 원본이 컬러일 경우 → 흑백 변환
                baseImage = new Mat();
                Cv2.CvtColor(_orinalImage, baseImage, ColorConversionCodes.BGR2GRAY);
            }
            else
            {
                // 원본이 이미 흑백이면 그대로 복사
                baseImage = _orinalImage.Clone();
            }

            Mat gray;
            if (_binaryResultImage != null && !_binaryResultImage.Empty())
            {
                gray = _binaryResultImage[roi].Clone(); // 🔧 ROI 크기로 잘라낸 마스크
            }
            else
            {
                Mat roiImage = baseImage[roi];
                gray = (roiImage.Channels() == 3) ? 
                    roiImage.CvtColor(ColorConversionCodes.BGR2GRAY) : roiImage.Clone();
            }
           
            // Canny 적용 (binary 마스크 기반, sharp 엣지 검출, ROI 내)
            Mat canny = new Mat();
            Cv2.Canny(gray, canny, min, max);
            
            // Canny 결과를 컬러로 변환 (baseImage는 CV_8UC3이므로)

            // baseImage의 ROI 영역을 Canny 이미지로 교체 (오버레이 대신)
            canny.CopyTo(new Mat(baseImage, roi));
           
            _previewImage=baseImage.Clone(); // 프리뷰 이미지 업데이트
            cameraForm.UpdateDisplay(_previewImage.ToBitmap());
        }

        public void SetMorphologyPreview(int kernelSize, MorphTypes morphType)
        {
            if (_orinalImage == null || _inspWindow == null)
            {

                SLogger.Write("SetMorphologyPreview: 원본 이미지 또는 InspWindow가 null입니다.", SLogger.LogType.Error);
                return;
            }

            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm == null)
            {
                SLogger.Write("SetMorphologyPreview: CameraForm을 찾을 수 없습니다.", SLogger.LogType.Error);
                return;
            }

            Rect roi = _inspWindow.WindowArea;
            Mat baseImage;
            if (_orinalImage.Channels() == 3)
            {
                // 원본이 컬러일 경우 → 흑백 변환
                baseImage = new Mat();
                Cv2.CvtColor(_orinalImage, baseImage, ColorConversionCodes.BGR2GRAY);
            }
            else
            {
                // 원본이 이미 흑백이면 그대로 복사
                baseImage = _orinalImage.Clone();
            }


            Mat gray;
            if (_binaryResultImage != null && !_binaryResultImage.Empty())
            {
                gray = _binaryResultImage[roi].Clone(); // 🔧 ROI 크기로 잘라낸 마스크
            }
            else
            {
                Mat roiImage = baseImage[roi];
                gray = (roiImage.Channels() == 3) ?
                    roiImage.CvtColor(ColorConversionCodes.BGR2GRAY) : roiImage.Clone();
            }
           
            // Morphology 커널 생성
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernelSize, kernelSize));

            // Morphology 적용
            Mat morph = new Mat();
            Cv2.MorphologyEx(gray, morph, morphType, kernel);
            // baseImage ROI 영역에 결과 적용
            morph.CopyTo(new Mat(baseImage, roi));

            _previewImage = baseImage.Clone();
            cameraForm.UpdateDisplay(_previewImage.ToBitmap());
            //_binaryResultImage = _previewImage.Clone(); // 이진화 결과 이미지 업데이트
            SLogger.Write("SetMorphologyPreview: Morphology 결과 적용 및 표시 완료", SLogger.LogType.Info);
        }
    }
}
