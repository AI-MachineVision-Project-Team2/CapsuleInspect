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
            _binaryResultImage = _previewImage.Clone();
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
            // 3. ROI 범위 유효성 검사 (오류 방지용)
            if (roi.X < 0 || roi.Y < 0 || roi.Right > _binaryResultImage.Width || roi.Bottom > _binaryResultImage.Height)
            {
                SLogger.Write("SetCannyPreview: ROI가 _binaryResultImage 범위를 초과함", SLogger.LogType.Error);
                return;
            }
            // 4. ROI 내에서 Canny Edge 수행
            Mat roiImage = _binaryResultImage[roi];
            Mat canny = new Mat();
            Cv2.Canny(roiImage, canny, min, max);

            // 5. Canny 결과를 컬러로 변환
            Mat cannyColor = new Mat();
            Cv2.CvtColor(canny, cannyColor, ColorConversionCodes.GRAY2BGR);
            // 6. _previewImage 초기화 (없으면 _orinalImage로부터 복사)
            if (_previewImage == null || _previewImage.Empty())
                _previewImage = _orinalImage.Clone();

            // 7. Canny 결과를 ROI 위치에 복사
            cannyColor.CopyTo(new Mat(_previewImage, roi));
            // 8. 화면 업데이트
            cameraForm.UpdateDisplay(_previewImage.ToBitmap());
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
