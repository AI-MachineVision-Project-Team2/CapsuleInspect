using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Teach;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace CapsuleInspect.Inspect
{
    public class InspectBoard
    {
        public InspectBoard()
        {
        }

        public bool Inspect(InspWindow window)
        {
            if (window is null)
                return false;

            if (!InspectWindow(window))
                return false;

            return true;
        }

        private bool InspectWindow(InspWindow window)
        {
            window.ResetInspResult();
            foreach (InspAlgorithm algo in window.AlgorithmList)
            {
                if (algo.IsUse == false)
                    continue;
                // ❶ 검사영역(Teach/InspRect) 기본값 보정
                algo.TeachRect = window.WindowArea;
                algo.InspRect = window.InspArea.Width > 0 ? window.InspArea : window.WindowArea;

                // ❷ 검사 입력 이미지 주입 (필터된 이미지 우선)
                Mat src = Global.Inst.InspStage.GetFilteredImage() ??
                          Global.Inst.InspStage.GetMat(0, algo.ImageChannel);
                algo.SetInspData(src);

                // ❸ InspAI라면 Saige 엔진 주입
                if (algo.InspectType == InspectType.InspAI)
                {
                    AIAlgorithm aiAlgo = algo as AIAlgorithm;
                    if (aiAlgo != null && Global.Inst?.InspStage?.AIModule != null)
                    {
                        // 모델 로딩은 Stage 초기화 시점 또는 최초 1회만
                        aiAlgo.SetSaigeAI(Global.Inst.InspStage.AIModule);
                    }
                }
                if (!algo.DoInspect())
                    return false;

                string resultInfo = string.Join("\r\n", algo.ResultString);

                InspResult inspResult = new InspResult
                {
                    ObjectID = window.UID,
                    InspType = algo.InspectType,
                    IsDefect = algo.IsDefect,
                    ResultInfos = resultInfo
                };

                switch (algo.InspectType)
                {
                    case InspectType.InspMatch:
                        MatchAlgorithm matchAlgo = algo as MatchAlgorithm;
                        inspResult.ResultValue = $"{matchAlgo.OutScore}";
                        break;
                    case InspectType.InspBinary:
                        BlobAlgorithm blobAlgo = algo as BlobAlgorithm;
                        int min = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT].min;
                        int max = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT].max;

                        inspResult.ResultValue = $"{blobAlgo.OutBlobCount}/{min}~{max}";
                        break;
                    case InspectType.InspAI:
                        AIAlgorithm aiAlgo = algo as AIAlgorithm;
                        int min2 = aiAlgo.BlobFilters[aiAlgo.FILTER_COUNT].min;
                        int max2 = aiAlgo.BlobFilters[aiAlgo.FILTER_COUNT].max;

                        inspResult.ResultValue = $"{aiAlgo.OutBlobCount}/{min2}~{max2}";
                        break;
                }

                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                int resultCnt = algo.GetResultRect(out resultArea);
                inspResult.ResultRectList = resultArea;

                window.AddInspResult(inspResult);
            }

            return true;
        }

        public bool InspectWindowList(List<InspWindow> windowList)
        {
            if (windowList.Count <= 0)
                return false;

            //ID 윈도우가 매칭알고리즘이 있고, 검사가 되었다면, 오프셋을 얻는다.
            Point alignOffset = new Point(0, 0);
            InspWindow idWindow = windowList.Find(w => w.InspWindowType == Core.InspWindowType.ID);
            if (idWindow != null)
            {
                MatchAlgorithm matchAlgo = (MatchAlgorithm)idWindow.FindInspAlgorithm(InspectType.InspMatch);
                if (matchAlgo != null && matchAlgo.IsUse)
                {
                    if (!InspectWindow(idWindow))
                        return false;

                    if (matchAlgo.IsInspected)
                    {
                        alignOffset = matchAlgo.GetOffset();
                        idWindow.InspArea = idWindow.WindowArea + alignOffset;
                    }
                }
            }

            foreach (InspWindow window in windowList)
            {
                //모든 윈도우에 오프셋 반영
                window.SetInspOffset(alignOffset);
                if (!InspectWindow(window))
                    return false;
            }

            return true;
        }
    }
}
