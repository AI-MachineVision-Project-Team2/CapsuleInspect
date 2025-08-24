using CapsuleInspect.Core;
using CapsuleInspect.Inspect;
using CapsuleInspect.Property;
using CapsuleInspect.Util;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SaigeVision.Net.V2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CapsuleInspect.Algorithm
{
    //이진화 임계값 설정을 구조체로 만들기
    
    public class AIAlgorithm : BlobAlgorithm
    {
        private SaigeAI _saigeAI = null;
        private readonly List<DrawInspectInfo> _resultRects = new List<DrawInspectInfo>();

        public int OutBlobCount { get; private set; } = 0;

        public AIAlgorithm()
        {
            InspectType = InspectType.InspAI;

        }

        //InspWindow 복사를 위한 BlobAlgorithm 복사 함수
        public override InspAlgorithm Clone()
        {
            var cloneAlgo = new AIAlgorithm();

            // 공통 필드 복사
            this.CopyBaseTo(cloneAlgo);

            cloneAlgo.CopyFrom(this);

            return cloneAlgo;
        }

        public override bool CopyFrom(InspAlgorithm sourceAlgo)
        {
            BlobAlgorithm blobAlgo = (BlobAlgorithm)sourceAlgo;

            this.BinThreshold = blobAlgo.BinThreshold;
            this.BinMethod = blobAlgo.BinMethod;
            this.UseRotatedRect = blobAlgo.UseRotatedRect;

            this.BlobFilters = blobAlgo.BlobFilters
                               .Select(b => new BlobFilter
                               {
                                   name = b.name,
                                   isUse = b.isUse,
                                   min = b.min,
                                   max = b.max
                               })
                               .ToList();

            return true;
        }

        public void SetSaigeAI(SaigeAI saigeAI)
        {
            _saigeAI = saigeAI;
        }

        //BlobAlgorithm 생성시, 기본 필터 설정

        //InspAlgorithm을 상속받아, 구현하고, 인자로 입력받던 것을 부모의 _srcImage 이미지 사용
        //검사 시작전 IsInspected = false로 초기화하고, 검사가 정상적으로 완료되면,IsInspected = true로 설정
        //이진화 검사 알고리즘

        //측정 검사 알고리즘

        public override void SetInspData(Mat srcImage)
        {
            base.SetInspData(srcImage);
        }

        public override bool DoInspect()
        {
            /*
            ResetResult();              // 베이스 공통 결과 초기화
            _resultRects.Clear();
            OutBlobCount = 0;
            IsInspected = false;
            IsDefect = false;
            //Decision = DecisionType.Good;
            ResultString.Clear();

            if (_srcImage == null || _srcImage.Empty())
            {
                MessageBox.Show("소스 이미지를 가져올 수 없습니다!");
                return false;
            }


            //검사 영역이 검사 대상 이미지를 벗어나지 않는지 확인
            if (InspRect.Right > _srcImage.Width ||
                InspRect.Bottom > _srcImage.Height)
                return false;

            Mat targetImage = _srcImage[InspRect];
            // --- 변경: RunForm의 AI 검사 로직을 여기로 옮김 ---
            bool isAIDefect = false;


            // 이미지 변환 (Mat -> Bitmap)
            using (Bitmap bitmap = targetImage.ToBitmap())  // ROI 영역만 사용
            {
                if (bitmap == null)
                {
                    MessageBox.Show("이미지 변환 실패.");
                    return false;
                }

                // AI 검사 수행
                if (_saigeAI.Inspect(bitmap))
                {
                    OpenCvSharp.Point[][] contours = new OpenCvSharp.Point[0][];
                    if (_saigeAI.GetContours(ref contours))
                    {
                        if (_findArea is null)
                            _findArea = new List<DrawInspectInfo>();

                        _findArea.Clear();

                        int findBlobCount = 0;

                        foreach (var contour in contours)
                        {
                            double area = Cv2.ContourArea(contour);
                            if (area <= 0)
                                continue;

                            int showArea = 0;
                            int showWidth = 0;
                            int showHeight = 0;

                            BlobFilter areaFilter = BlobFilters[FILTER_AREA];

                            if (areaFilter.isUse)
                            {
                                if (areaFilter.min > 0 && area < areaFilter.min)
                                    continue;

                                if (areaFilter.max > 0 && area > areaFilter.max)
                                    continue;

                                showArea = (int)(area + 0.5f);
                            }

                            Rect boundingRect = Cv2.BoundingRect(contour);
                            RotatedRect rotatedRect = Cv2.MinAreaRect(contour);
                            Size2d blobSize = new Size2d(boundingRect.Width, boundingRect.Height);

                            // RotatedRect 정보 계산
                            if (UseRotatedRect)
                            {
                                // 너비와 높이 가져오기
                                float width = rotatedRect.Size.Width;
                                float height = rotatedRect.Size.Height;

                                // 장축과 단축 구분
                                blobSize.Width = Math.Max(width, height);
                                blobSize.Height = Math.Min(width, height);
                            }

                            BlobFilter widthFilter = BlobFilters[FILTER_WIDTH];
                            if (widthFilter.isUse)
                            {
                                if (widthFilter.min > 0 && blobSize.Width < widthFilter.min)
                                    continue;

                                if (widthFilter.max > 0 && blobSize.Width > widthFilter.max)
                                    continue;

                                showWidth = (int)(blobSize.Width + 0.5f);
                            }

                            BlobFilter heightFilter = BlobFilters[FILTER_HEIGHT];
                            if (heightFilter.isUse)
                            {
                                if (heightFilter.min > 0 && blobSize.Height < heightFilter.min)
                                    continue;

                                if (heightFilter.max > 0 && blobSize.Height > heightFilter.max)
                                    continue;

                                showHeight = (int)(blobSize.Height + 0.5f);
                            }


                            findBlobCount++;
                            Rect blobRect = boundingRect + InspRect.TopLeft;

                            string featureInfo = "";
                            if (showArea > 0)
                                featureInfo += $"A:{showArea}";

                            if (showWidth > 0)
                            {
                                if (featureInfo != "")
                                    featureInfo += "\r\n";

                                featureInfo += $"W:{showWidth}";
                            }

                            if (showHeight > 0)
                            {
                                if (featureInfo != "")
                                    featureInfo += "\r\n";

                                featureInfo += $"H:{showHeight}";
                            }

                            //검사된 정보를 문자열로 저장
                            string blobInfo;
                            blobInfo = $"Blob X:{blobRect.X}, Y:{blobRect.Y}, Size({blobRect.Width},{blobRect.Height})";
                            ResultString.Add(blobInfo);

                            //검사된 영역 정보를 DrawInspectInfo로 저장
                            DrawInspectInfo rectInfo = new DrawInspectInfo(blobRect, featureInfo, InspectType.InspAI, DecisionType.Info);

                            if (UseRotatedRect)
                            {
                                Point2f[] points = rotatedRect.Points().Select(p => p + InspRect.TopLeft).ToArray();
                                rectInfo.SetRotatedRectPoints(points);
                            }

                            _findArea.Add(rectInfo);
                        }

                        OutBlobCount = findBlobCount;

                        IsDefect = false;
                        string result = "OK";
                        BlobFilter countFilter = BlobFilters[FILTER_COUNT];

                        if (countFilter.isUse)
                        {
                            if (countFilter.min > 0 && findBlobCount < countFilter.min)
                                IsDefect = true;

                            if (IsDefect == false && countFilter.max > 0 && findBlobCount > countFilter.max)
                                IsDefect = true;
                        }

                        if (IsDefect)
                        {
                            string rectInfo = $"Count:{findBlobCount}";
                            _findArea.Add(new DrawInspectInfo(InspRect, rectInfo, InspectType.InspBinary, DecisionType.Defect));

                            result = "NG";

                            string resultInfo = "";
                            resultInfo = $"[{result}] Blob count [in : {countFilter.min},{countFilter.max},out : {findBlobCount}]";
                            ResultString.Add(resultInfo);
                        }
                    }
                }
            }

            IsInspected = true;

            return true;*/
            // === 초기화 ===
            ResetResult();              // 베이스 공통 결과 초기화
            _resultRects.Clear();
            OutBlobCount = 0;
            IsInspected = false;
            IsDefect = false;
            ResultString.Clear();

            // === 입력/ROI 유효성 ===
            if (_srcImage == null || _srcImage.Empty())
            {
                MessageBox.Show("소스 이미지를 가져올 수 없습니다!");
                return false;
            }

            if (InspRect.Right > _srcImage.Width || InspRect.Bottom > _srcImage.Height)
                return false;

            // ROI 잘라오기
            Mat targetImage = _srcImage[InspRect];

            // --- AI 검사 시작 ---
            using (Bitmap bitmap = targetImage.ToBitmap())  // ROI 영역만 사용
            {
                if (bitmap == null)
                {
                    MessageBox.Show("이미지 변환 실패.");
                    return false;
                }

                if (_saigeAI == null)
                {
                    MessageBox.Show("SaigeAI 인스턴스가 설정되지 않았습니다.");
                    return false;
                }

                // AI 실행
                if (_saigeAI.Inspect(bitmap))
                {
                    OpenCvSharp.Point[][] contours = new OpenCvSharp.Point[0][];
                    if (_saigeAI.GetContours(ref contours))
                    {
                        if (_findArea is null)
                            _findArea = new List<DrawInspectInfo>();
                        _findArea.Clear();

                        int findBlobCount = 0;

                        foreach (var contour in contours)
                        {
                            double area = Cv2.ContourArea(contour);
                            if (area <= 0)
                                continue;

                            int showArea = 0;
                            int showWidth = 0;
                            int showHeight = 0;

                            BlobFilter areaFilter = BlobFilters[FILTER_AREA];
                            if (areaFilter.isUse)
                            {
                                if (areaFilter.min > 0 && area < areaFilter.min)
                                    continue;
                                if (areaFilter.max > 0 && area > areaFilter.max)
                                    continue;

                                showArea = (int)(area + 0.5f);
                            }

                            Rect boundingRect = Cv2.BoundingRect(contour);
                            RotatedRect rotatedRect = Cv2.MinAreaRect(contour);
                            Size2d blobSize = new Size2d(boundingRect.Width, boundingRect.Height);

                            // RotatedRect 기반 W/H 구하기(장축/단축)
                            if (UseRotatedRect)
                            {
                                float width = rotatedRect.Size.Width;
                                float height = rotatedRect.Size.Height;
                                blobSize.Width = Math.Max(width, height);
                                blobSize.Height = Math.Min(width, height);
                            }

                            BlobFilter widthFilter = BlobFilters[FILTER_WIDTH];
                            if (widthFilter.isUse)
                            {
                                if (widthFilter.min > 0 && blobSize.Width < widthFilter.min)
                                    continue;
                                if (widthFilter.max > 0 && blobSize.Width > widthFilter.max)
                                    continue;

                                showWidth = (int)(blobSize.Width + 0.5f);
                            }

                            BlobFilter heightFilter = BlobFilters[FILTER_HEIGHT];
                            if (heightFilter.isUse)
                            {
                                if (heightFilter.min > 0 && blobSize.Height < heightFilter.min)
                                    continue;
                                if (heightFilter.max > 0 && blobSize.Height > heightFilter.max)
                                    continue;

                                showHeight = (int)(blobSize.Height + 0.5f);
                            }

                            // --- 필터 통과한 블롭만 카운트/표시 ---
                            findBlobCount++;

                            Rect blobRect = boundingRect + InspRect.TopLeft;

                            string featureInfo = "";
                            if (showArea > 0) featureInfo += $"A:{showArea}";
                            if (showWidth > 0)
                            {
                                if (featureInfo != "") featureInfo += "\r\n";
                                featureInfo += $"W:{showWidth}";
                            }
                            if (showHeight > 0)
                            {
                                if (featureInfo != "") featureInfo += "\r\n";
                                featureInfo += $"H:{showHeight}";
                            }

                            // 문자열 결과
                            string blobInfo = $"Blob X:{blobRect.X}, Y:{blobRect.Y}, Size({blobRect.Width},{blobRect.Height})";
                            ResultString.Add(blobInfo);

                            // 오버레이용 결과
                            DrawInspectInfo rectInfo = new DrawInspectInfo(blobRect, featureInfo, InspectType.InspAI, DecisionType.Info);
                            if (UseRotatedRect)
                            {
                                Point2f[] points = rotatedRect.Points().Select(p => p + InspRect.TopLeft).ToArray();
                                rectInfo.SetRotatedRectPoints(points);
                            }
                            _findArea.Add(rectInfo);
                        }

                        // === 최종 판정 규칙 (핵심): 잡히면 NG, 없으면 OK ===
                        OutBlobCount = findBlobCount;
                        IsDefect = (findBlobCount > 0);   // ★★ 여기 한 줄이 핵심 ★★

                        // 요약 박스/문구(선택)
                        if (IsDefect)
                        {
                            string rectInfo = $"Count:{findBlobCount}";
                            _findArea.Add(new DrawInspectInfo(InspRect, rectInfo, InspectType.InspAI, DecisionType.Defect));

                            string resultInfo = $"[NG] Blob count [out : {findBlobCount}]";
                            ResultString.Add(resultInfo);
                        }
                        else
                        {
                            ResultString.Add("[OK] No blob found by AI");
                        }
                    }
                }
            }

            IsInspected = true;
            return true;
        }
        //검사 결과 초기화
        public override void ResetResult()
        {
            base.ResetResult();
            if (_findArea != null)
                _findArea.Clear();
        }


        // 검사 결과 영역 영역 반환
        public override int GetResultRect(out List<DrawInspectInfo> resultArea)
        {
            resultArea = null;

            //검사가 완료되지 않았다면, 리턴
            if (!IsInspected)
                return -1;

            if (_findArea is null || _findArea.Count <= 0)
                return -1;

            resultArea = _findArea;
            return resultArea.Count;
        }

    }
}
