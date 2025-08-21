using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SaigeVision.Net.V2;
using SaigeVision.Net.V2.Detection;
using SaigeVision.Net.V2.IAD;
using SaigeVision.Net.V2.Segmentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapsuleInspect.Inspect
{
    public enum EngineType 
    {
        [Description("Anomaly Detection")]
        IAD,
        [Description("Segmentation")]
        SEG,
        [Description("Detection")]
        DET,
    }
    public class SaigeAI : IDisposable
    {
        public EngineType _engineType;
        public List<BlobFilter> Filters { get; set; } = new List<BlobFilter>();
        IADEngine _iADEngine = null;
        IADResult _iADresult = null;
        SegmentationEngine _sEGEngine = null;
        SegmentationResult _sEGResult = null;
        DetectionEngine _dETEngine = null;
        DetectionResult _dETResult = null;

        Bitmap _inspImage = null;

        public SaigeAI()
        {
        }
        public void SetEngineType(EngineType engineType) => _engineType = engineType;

        public void LoadEngine(string modelPath)
        {

            DisposeMode();

            switch (_engineType)
            {
                case EngineType.IAD:
                    _iADEngine = new IADEngine(modelPath, 0);
                    var iadOption = _iADEngine.GetInferenceOption();
                    iadOption.CalcScoremap = false;
                    iadOption.CalcHeatmap = false;
                    iadOption.CalcMask = false;
                    iadOption.CalcObject = true;
                    iadOption.CalcObjectAreaAndApplyThreshold = true;
                    iadOption.CalcObjectScoreAndApplyThreshold = true;
                    iadOption.CalcTime = true;
                    _iADEngine.SetInferenceOption(iadOption);
                    break;

                case EngineType.SEG:
                    _sEGEngine = new SegmentationEngine(modelPath, 0);
                    var segOption = _sEGEngine.GetInferenceOption();
                    segOption.CalcTime = true;
                    segOption.CalcObject = true;
                    segOption.CalcScoremap = false;
                    segOption.CalcMask = false;
                    segOption.CalcObjectAreaAndApplyThreshold = true;
                    segOption.CalcObjectScoreAndApplyThreshold = true;
                    _sEGEngine.SetInferenceOption(segOption);
                    break;

                case EngineType.DET:
                    _dETEngine = new DetectionEngine(modelPath, 0);
                    var detOption = _dETEngine.GetInferenceOption();
                    detOption.CalcTime = true;
                    _dETEngine.SetInferenceOption(detOption);
                    break;
            }
        }
        public bool Inspect(Bitmap bmpImage)
        {
            // 필터링된 이미지를 우선적으로 사용
            Mat inputImage = Global.Inst.InspStage.GetFilteredImage() ?? BitmapConverter.ToMat(bmpImage);
            if (inputImage is null)
            {
                MessageBox.Show("이미지가 없습니다. 유효한 이미지를 입력해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _inspImage = inputImage.ToBitmap();
            SrImage srImage = new SrImage(_inspImage);
            Stopwatch sw = Stopwatch.StartNew();


            switch (_engineType)
            {
                case EngineType.IAD:
                    if (_iADEngine == null)
                    {
                        MessageBox.Show("IAD 엔진이 초기화되지 않았습니다.");
                        return false;
                    }
                    _iADresult = _iADEngine.Inspection(srImage);
                    break;
                case EngineType.SEG:
                    if (_sEGEngine == null)
                    {
                        MessageBox.Show("SEG 엔진이 초기화되지 않았습니다.");
                        return false;
                    }
                    _sEGResult = _sEGEngine.Inspection(srImage);
                    break;
                case EngineType.DET:
                    if (_dETEngine == null)
                    {
                        MessageBox.Show("DET 엔진이 초기화되지 않았습니다.");
                        return false;
                    }
                    _dETResult = _dETEngine.Inspection(srImage);
                    break;

                default:
                    MessageBox.Show("지원하지 않는 엔진 타입입니다.");
                    return false;
            }

            sw.Stop();
            return true;
        }
        public void DrawResult(Bitmap bmp)
        {
            if (bmp == null)
                return;

            Graphics g = Graphics.FromImage(bmp);
            int step = 10;

            List<object> filteredObjects = new List<object>(); // SEG/IAD/DET 객체 저장 (object로 다형성)

            switch (_engineType)
            {
                case EngineType.SEG:
                case EngineType.IAD:
                    {
                        var segmentedObjects = _engineType == EngineType.SEG
                            ? _sEGResult?.SegmentedObjects
                            : _iADresult?.SegmentedObjects;

                        if (segmentedObjects == null)
                            break;

                        foreach (var prediction in segmentedObjects)
                        {
                            if (prediction.Contour.Value.Count < 4) // 최소 contour 조건 (Blob 참고)
                                continue;

                            // List<PointF>를 OpenCvSharp.Point[]로 변환
                            var contourPoints = prediction.Contour.Value.Select(p => new OpenCvSharp.Point((int)p.X, (int)p.Y)).ToArray();
                            // Area 계산
                            double area = Cv2.ContourArea(contourPoints);

                            // Width/Height 계산
                            Rect boundingRect = Cv2.BoundingRect(contourPoints);
                            int width = boundingRect.Width;
                            int height = boundingRect.Height;

                            // 필터 적용 (BlobAlgorithm.RunBinary() 참고)
                            bool passFilter = true;
                            foreach (var filter in Filters)
                            {
                                if (!filter.isUse) continue;

                                if (filter.name == "Area" && ((filter.min > 0 && area < filter.min) || (filter.max > 0 && area > filter.max)))
                                {
                                    passFilter = false;
                                    break;
                                }
                                else if (filter.name == "Width" && ((filter.min > 0 && width < filter.min) || (filter.max > 0 && width > filter.max)))
                                {
                                    passFilter = false;
                                    break;
                                }
                                else if (filter.name == "Height" && ((filter.min > 0 && height < filter.min) || (filter.max > 0 && height > filter.max)))
                                {
                                    passFilter = false;
                                    break;
                                }
                            }

                            if (passFilter)
                                filteredObjects.Add(prediction);
                        }

                        // Count 필터 적용 (전체 객체 수, Blob처럼)
                        var countFilter = Filters.FirstOrDefault(f => f.name == "Count" && f.isUse);
                        if (countFilter != null)
                        {
                            int findCount = filteredObjects.Count;
                            if ((countFilter.min > 0 && findCount < countFilter.min) || (countFilter.max > 0 && findCount > countFilter.max))
                            {
                                filteredObjects.Clear(); // NG: 객체 비우기 (또는 NG 표시 로직 추가)
                            }
                        }

                        // 필터링된 객체 그리기
                        foreach (var obj in filteredObjects)
                        {
                            var prediction = (/* 적절한 타입 캐스트, 예: SegmentedObject */ obj as SegmentedObject);
                            using (var brush = new SolidBrush(Color.FromArgb(127, prediction.ClassInfo.Color)))
                            using (var gp = new GraphicsPath())
                            {
                                gp.AddPolygon(prediction.Contour.Value.ToArray());
                                foreach (var innerValue in prediction.Contour.InnerValue)
                                {
                                    gp.AddPolygon(innerValue.ToArray());
                                }
                                g.FillPath(brush, gp);
                            }
                            step += 50;
                        }
                    }
                    break;

                default:
                    MessageBox.Show("DrawResult: 지원하지 않는 엔진 타입입니다.");
                    break;
            }
        }

        public Bitmap GetResultImage()
        {
            if (_inspImage == null)
                return null;

            Bitmap resultImage = _inspImage.Clone(
                new Rectangle(0, 0, _inspImage.Width, _inspImage.Height),
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );
            DrawResult(resultImage);
            return resultImage;

        }

        private void DisposeMode()
        {
            //GPU에 여러개 모델을 넣을 경우, 메모리가 부족할 수 있으므로, 해제
            if (_iADEngine != null)
                _iADEngine.Dispose();

            if (_sEGEngine != null)
                _sEGEngine.Dispose();

            if (_dETEngine != null)
                _dETEngine.Dispose();
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

                    // 검사완료 후 메모리 해제를 합니다.
                    // 엔진 사용이 완료되면 꼭 dispose 해주세요
                    DisposeMode();
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
