using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Util;
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

        private bool _isDefect; // 불량 여부 저장
        public bool IsDefect => _isDefect; // 불량 여부 반환 프로퍼티
        Bitmap _inspImage = null;// ★ 항상 '입력받은 ROI 비트맵'을 저장
        private bool _isLoadedEngine = false;

        public SaigeAI()
        {
        }
        public void SetEngineType(EngineType engineType) => _engineType = engineType;

        public void LoadEngine(string modelPath)
        {
            if (_isLoadedEngine == true)
                return;

            DisposeMode();

            switch (_engineType)
            {
                case EngineType.SEG:
                    _sEGEngine = new SegmentationEngine(modelPath, -1);
                    var segOption = _sEGEngine.GetInferenceOption();
                    segOption.CalcTime = true;
                    segOption.CalcObject = true;
                    segOption.CalcScoremap = false;
                    segOption.CalcMask = false;
                    segOption.CalcObjectAreaAndApplyThreshold = true;
                    segOption.CalcObjectScoreAndApplyThreshold = true;
                    _sEGEngine.SetInferenceOption(segOption);
                    _isLoadedEngine = true;
                    break;
                default:
                    break;
            }
        }
        public bool Inspect(Bitmap bmpImage)
        {
            // 초기화: 검사 시작 시 _isDefect를 false로 리셋
            _isDefect = false;

            if (bmpImage == null)
            {
                MessageBox.Show("이미지가 없습니다. 유효한 이미지를 입력해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // ★ 좌표계를 ROI로 고정하기 위해 입력 비트맵을 그대로 사용
            _inspImage = (Bitmap)bmpImage.Clone();
            SrImage srImage = new SrImage(_inspImage);
            Stopwatch sw = Stopwatch.StartNew();


            switch (_engineType)
            {
                case EngineType.SEG:
                    if (_sEGEngine == null)
                    {
                        MessageBox.Show("SEG 엔진이 초기화되지 않았습니다.");
                        return false;
                    }
                    _sEGResult = _sEGEngine.Inspection(srImage);
                    break;
                default:
                    MessageBox.Show("지원하지 않는 엔진 타입입니다.");
                    return false;
            }

            sw.Stop();
            return true;
        }


        public bool GetContours(ref OpenCvSharp.Point[][] contours)
        {
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

                        contours = new OpenCvSharp.Point[segmentedObjects.Length][];

                        int i = 0;
                        foreach (var prediction in segmentedObjects)
                        {
                            contours[i++] = prediction.Contour.Value.Select(p => new OpenCvSharp.Point((int)p.X, (int)p.Y)).ToArray();
                        }
                    }
                    break;
            }

            return true;
        }


        public List<DrawInspectInfo> DrawResult(Bitmap bmp)
        {
            if (bmp == null)
                return null;

            Graphics g = Graphics.FromImage(bmp);
            int step = 10;

            List<object> filteredObjects = new List<object>(); // 통과 객체
            List<DrawInspectInfo> inspectInfos = new List<DrawInspectInfo>(); // Defect 정보 (Good은 화면 표시 안 함)

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

                            // 필터 적용 (필터 통과 = Defect)
                            bool passFilter = true;
                            string failReason = string.Empty;
                            foreach (var filter in Filters)
                            {
                                if (!filter.isUse) continue;
                                if (filter.name == "Area" && ((filter.min > 0 && area < filter.min) || (filter.max > 0 && area > filter.max)))
                                {
                                    passFilter = false;
                                    failReason = $"Area: {area}, Range=[{filter.min},{filter.max}]";
                                    break;
                                }
                                else if (filter.name == "Width" && ((filter.min > 0 && width < filter.min) || (filter.max > 0 && width > filter.max)))
                                {
                                    passFilter = false;
                                    failReason = $"Width: {width}, Range=[{filter.min},{filter.max}]";
                                    break;
                                }
                                else if (filter.name == "Height" && ((filter.min > 0 && height < filter.min) || (filter.max > 0 && height > filter.max)))
                                {
                                    passFilter = false;
                                    failReason = $"Height: {height}, Range=[{filter.min},{filter.max}]";
                                    break;
                                }
                            }

                            if (passFilter)
                            {
                                // 필터 통과 객체: Defect로 처리, DrawInspectInfo 생성, 화면 표시
                                string inspectInfo = $"Defect A:{(int)area}";
                                DrawInspectInfo drawInfo = new DrawInspectInfo(boundingRect, inspectInfo, InspectType.InspBinary, DecisionType.Defect);

                                // RotatedRect 계산 및 설정 (빨간색으로 표시)
                                RotatedRect rotatedRect = Cv2.MinAreaRect(contourPoints);
                                Point2f[] rotatedPoints = rotatedRect.Points();
                                drawInfo.SetRotatedRectPoints(rotatedPoints);

                                inspectInfos.Add(drawInfo);
                                filteredObjects.Add(prediction);
                            }
                            // 필터 미통과 객체 (Good): 화면 표시 및 로그 기록 안 함
                        }

                        // Count 필터
                        var countFilter = Filters.FirstOrDefault(f => f.name == "Count" && f.isUse);
                        int findCount = filteredObjects.Count;
                        bool isCountDefect = false;
                        if (countFilter != null && countFilter.min >= 1)
                        {
                            if (findCount >= countFilter.min && (countFilter.max == 0 || findCount <= countFilter.max))
                            {
                                isCountDefect = true; // Count 조건 충족 시 defect로 간주
                                _isDefect = true;
                            }
                            else
                            {
                                // Count 범위 밖: 전체 Defect로 표시 (기존 로직 유지)
                                if ((countFilter.max > 0 && findCount > countFilter.max))
                                {
                                    filteredObjects.Clear();
                                    string countInfo = $"Defect Count:{findCount}";
                                    DrawInspectInfo countDraw = new DrawInspectInfo(new Rect(0, 0, bmp.Width, bmp.Height), countInfo, InspectType.InspBinary, DecisionType.Defect);
                                    inspectInfos.Add(countDraw);
                                    _isDefect = true; // Count 불량도 defect로 플래그
                                }
                            }
                        }
                        else
                        {
                            _isDefect = findCount > 0; // Count 필터 없으면 기본
                        }

                        // 필터링된 객체 그리기 (Defect는 빨간색)
                        foreach (var obj in filteredObjects)
                        {
                            var prediction = obj as dynamic; // 실제 타입: SegmentedObject
                            using (var brush = new SolidBrush(Color.FromArgb(127, 255, 0, 0))) // 빨간색 계열
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
            return inspectInfos; // InspStage에서 CameraForm.AddRect(defectInfos) 호출
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
