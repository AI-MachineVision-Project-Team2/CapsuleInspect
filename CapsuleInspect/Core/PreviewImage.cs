using CapsuleInspect.Algorithm;
using CapsuleInspect.Property;
using CapsuleInspect.Teach;
using CapsuleInspect.Util;
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

            Mat grayImage = new Mat();
            if (orgRoi.Type() == MatType.CV_8UC3)
                Cv2.CvtColor(orgRoi, grayImage, ColorConversionCodes.BGR2GRAY);
            else
                grayImage = orgRoi;

            Mat binaryMask = new Mat();
            Cv2.InRange(grayImage, lowerValue, upperValue, binaryMask);

            if (invert)
                binaryMask = ~binaryMask;

            // 추가: 순수 binaryMask를 _binaryResultImage에 저장 (ROI 크기)
            _binaryResultImage = binaryMask.Clone();
            SLogger.Write("SetBinary: _binaryResultImage 저장 완료 (순수 binary)", SLogger.LogType.Info);// binaryMask는 ROI 사이즈이므로 fullBinaryMask로 확장
          
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
            Mat baseImage = (_previewImage != null && !_previewImage.Empty()) ? _previewImage.Clone() : _orinalImage.Clone();
            SLogger.Write($"SetCannyPreview: baseImage 크기 (W={baseImage.Width}, H={baseImage.Height})", SLogger.LogType.Info);

            // _binaryResultImage (ROI 크기의 순수 binary) 사용
            Mat gray = new Mat();
            if (_binaryResultImage != null && !_binaryResultImage.Empty())
            {
                gray = _binaryResultImage.Clone(); // ROI 크기의 binary 마스크
                SLogger.Write($"SetCannyPreview: _binaryResultImage 사용 (Size: {gray.Width}x{gray.Height})", SLogger.LogType.Info);
            }
            else
            {
                // fallback: 원본 ROI에서 gray 생성
                Mat roiImage = baseImage[roi];
                if (roiImage.Channels() == 3)
                    Cv2.CvtColor(roiImage, gray, ColorConversionCodes.BGR2GRAY);
                else
                    gray = roiImage.Clone();
                SLogger.Write("SetCannyPreview: _binaryResultImage 없음, 원본 ROI로 gray 생성", SLogger.LogType.Error);
            }

            // Canny 적용 (binary 마스크 기반, sharp 엣지 검출)
            Mat canny = new Mat();
            Cv2.Canny(gray, canny, min, max);
            SLogger.Write($"SetCannyPreview: Canny 적용 (Min={min}, Max={max})", SLogger.LogType.Info);

            // Canny 결과를 전체 이미지 크기로 확장
            Mat fullCanny = Mat.Zeros(_orinalImage.Size(), MatType.CV_8UC1);
            canny.CopyTo(new Mat(fullCanny, roi));

            // Canny 결과를 컬러로 변환 (화면 표시용)
            Mat cannyColor = new Mat();
            Cv2.CvtColor(fullCanny, cannyColor, ColorConversionCodes.GRAY2BGR);

            // baseImage에 Canny 결과 오버레이
            Mat preview = baseImage.Clone();
            cannyColor.CopyTo(preview);
            // 또는 하이라이트 스타일로 오버레이: Cv2.AddWeighted(baseImage, 0.7, cannyColor, 0.3, 0, preview);

            cameraForm.UpdateDisplay(preview.ToBitmap());
            SLogger.Write("SetCannyPreview: CameraForm 업데이트 완료", SLogger.LogType.Info);
        }

        // 추가: _previewImage를 외부에서 업데이트 가능하도록
        public void UpdatePreviewImage(Mat newImage)
        {
            _previewImage = newImage.Clone();
            SLogger.Write($"UpdatePreviewImage: _previewImage 업데이트 (Size: {_previewImage.Width}x{_previewImage.Height})", SLogger.LogType.Info);
        }
    }
}
