using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Teach;
using CapsuleInspect.Util;
using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CapsuleInspect.Inspect
{
    public class InspWorker
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private InspectBoard _inspectBoard = new InspectBoard();

        public bool IsRunning { get; set; } = false;

        public InspWorker()
        {
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public void StartCycleInspectImage()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => InspectionLoop(this, _cts.Token));
        }
        // 단일 사이클 루프 시작
        public void StartSingleCycleLoop()
        {
            if (IsRunning)
                return;

            if (!Global.Inst.InspStage.UseCamera)
            {
                string inspImagePath = Global.Inst.InspStage.CurModel.InspectImagePath;
                if (string.IsNullOrEmpty(inspImagePath))
                {
                    SLogger.Write("[InspWorker] 검사 이미지 경로가 비어있음", SLogger.LogType.Error);
                    return;
                }

                string inspImageDir = System.IO.Path.GetDirectoryName(inspImagePath);
                if (!System.IO.Directory.Exists(inspImageDir))
                {
                    SLogger.Write($"[InspWorker] 이미지 폴더가 존재하지 않음: {inspImageDir}", SLogger.LogType.Error);
                    return;
                }

                if (!Global.Inst.InspStage.ImageLoader.IsLoadedImages())
                    Global.Inst.InspStage.ImageLoader.LoadImages(inspImageDir);

                Global.Inst.InspStage.ImageLoader.CyclicMode = false; // 단일 사이클
            }

            _cts = new CancellationTokenSource();
            Task.Run(() => SingleCycleLoop(this, _cts.Token));
        }
        private void InspectionLoop(InspWorker inspWorker, CancellationToken token)
        {
            Global.Inst.InspStage.SetWorkingState(WorkingState.INSPECT);

            SLogger.Write("자동 반복 검사 시작");

            IsRunning = true;

            while (!token.IsCancellationRequested)
            {
                Global.Inst.InspStage.OneCycle();

                Thread.Sleep(200); // 주기 설정
            }

            IsRunning = false;

            SLogger.Write("자동 반복 검사 종료");
        }

        private void SingleCycleLoop(InspWorker inspWorker, CancellationToken token)
        {
            Global.Inst.InspStage.SetWorkingState(WorkingState.INSPECT);

            SLogger.Write("[InspWorker] 단일 사이클 검사 시작");

            IsRunning = true;

            while (!token.IsCancellationRequested)
            {
                bool result = Global.Inst.InspStage.OneCycle();
                if (!result)
                {
                    SLogger.Write("[InspWorker] 단일 사이클 검사 종료");
                    break;
                }
                Thread.Sleep(200); // 검사 간 지연
            }

            IsRunning = false;
            SLogger.Write("[InspWorker] 단일 사이클 검사 완료");
        }
        //InspStage내의 모든 InspWindow들을 검사하는 함수
        public bool RunInspect(out bool isDefect)
        {
            isDefect = false;
            Model curMode = Global.Inst.InspStage.CurModel;
            List<InspWindow> inspWindowList = curMode.InspWindowList;
            foreach (var inspWindow in inspWindowList)
            {
                if (inspWindow is null)
                    continue;

                UpdateInspData(inspWindow);
            }

            _inspectBoard.InspectWindowList(inspWindowList);

            int totalCnt = 0;
            int okCnt = 0;
            int ngCnt = 0;

            var allRects = new List<DrawInspectInfo>();

            // 누적 변수 선언
            int ngCrack = 0, ngScratch = 0, ngSqueeze = 0, ngPrintDefect = 0;

            foreach (var inspWindow in inspWindowList)
            {
                totalCnt++;

                if (inspWindow.IsDefect())
                {
                    if (!isDefect)
                        isDefect = true;

                    ngCnt++;

                    var kind = GetInspWindowKind(inspWindow); // ← 아래 헬퍼 추가

                    switch (kind)
                    {
                        case InspWindowType.Crack: ngCrack = 1; break;
                        case InspWindowType.Scratch: ngScratch = 1; break;
                        case InspWindowType.Squeeze: ngSqueeze = 1; break;
                        case InspWindowType.PrintDefect: ngPrintDefect = 1; break;
                        // 필요 시 다른 종류도 추가
                        default:
                            // 이름으로도 못 찾으면 로그 한번 남겨 원인 파악
                            // SLogger.Write($"[InspWorker] Unknown kind ROI: {inspWindow?.Name}", SLogger.LogType.Info);
                            break;
                    }
                }
                else
                {
                    okCnt++;
                }

                DisplayResult(inspWindow, InspectType.InspNone);
            }
            int distinctByKind = (ngCrack > 0 ? 1 : 0)
                   + (ngScratch > 0 ? 1 : 0)
                   + (ngSqueeze > 0 ? 1 : 0)
                   + (ngPrintDefect > 0 ? 1 : 0);
            Global.Inst.InspStage.SetDistinctNgCount(distinctByKind);

            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.AddRect(allRects);                        // (있다면)
                cameraForm.SetInspResultCount(totalCnt, okCnt, ngCnt);
            }

            // ★ 누적 카운트 갱신 (이미지 1장 단위로)
            Global.Inst.InspStage.AddAccumCount(1, isDefect ? 0 : 1, isDefect ? 1 : 0);
            // 🎯 세분화된 NG 카운트 반영
            Global.Inst.InspStage.AddNgDetailCount(ngCrack, ngScratch, ngSqueeze, ngPrintDefect);
            //if (totalCnt > 0)
            //{
            //    //찾은 위치를 이미지상에서 표시
            //    var cameraForm = MainForm.GetDockForm<CameraForm>();
            //    if (cameraForm != null)
            //    {
            //        cameraForm.SetInspResultCount(totalCnt, okCnt, ngCnt);
            //    }
            //    var resultForm = MainForm.GetDockForm<ResultForm>();
            //    if (resultForm != null)
            //    {
            //        if (resultForm.InvokeRequired)
            //            resultForm.BeginInvoke(new Action(() => resultForm.AddModelResult(curMode)));
            //        else
            //            resultForm.AddModelResult(curMode);
            //    }
            //}


            return true;
        }
        private InspWindowType GetInspWindowKind(InspWindow w)
        {
            // 1) 속성으로 직접 갖고 있으면 그걸 최우선 사용 (프로퍼티명 케이스 대응)
            var t = w.GetType();
            var p1 = t.GetProperty("InspWindowType"); // 보통 이 이름일 가능성 높음
            if (p1 != null && p1.PropertyType == typeof(InspWindowType))
                return (InspWindowType)p1.GetValue(w, null);

            var p2 = t.GetProperty("WindowType");     // 혹시 다른 이름으로 있을 수도 있음
            if (p2 != null && p2.PropertyType == typeof(InspWindowType))
                return (InspWindowType)p2.GetValue(w, null);

            // 2) 없으면 이름으로 후순위 판정 (대/소문자·한글 키워드 모두 처리)
            var name = (w?.Name ?? w?.UID ?? string.Empty).ToLowerInvariant();

            if (name.Contains("crack") || name.Contains("크랙") || name.Contains("균열"))
                return InspWindowType.Crack;

            if (name.Contains("scratch") || name.Contains("스크래치"))
                return InspWindowType.Scratch;

            if (name.Contains("squeeze") || name.Contains("찌그러") || name.Contains("변형"))
                return InspWindowType.Squeeze;

            if (name.Contains("printdefect") || name.Contains("인쇄") || name.Contains("프린트"))
                return InspWindowType.PrintDefect;

            return InspWindowType.None;
        }
        //특정 InspWindow에 대한 검사 진행
        //inspType이 있다면 그것만을 검사하고, 없다면 InpsWindow내의 모든 알고리즘 검사
        public bool TryInspect(InspWindow inspObj, InspectType inspType)
        {
            if (inspObj != null)
            {
                if (!UpdateInspData(inspObj))
                    return false;

                _inspectBoard.Inspect(inspObj);

                DisplayResult(inspObj, inspType);
            }
            else
            {
                bool isDefect = false;
                RunInspect(out isDefect);
            }

            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();
            if (resultForm != null)
            {
                if (inspObj != null)
                    resultForm.AddWindowResult(inspObj);
                else
                {
                    Model curMode = Global.Inst.InspStage.CurModel;
                    resultForm.AddModelResult(curMode);
                }
            }

            return true;
        }

        //각 알고리즘 타입 별로 검사에 필요한 데이터를 입력하는 함수
        private bool UpdateInspData(InspWindow inspWindow)
        {
            if (inspWindow is null)
                return false;

            Rect windowArea = inspWindow.WindowArea;

            inspWindow.PatternLearn();

            foreach (var inspAlgo in inspWindow.AlgorithmList)
            {
                //검사 영역 초기화
                inspAlgo.TeachRect = windowArea;
                inspAlgo.InspRect = windowArea;

                Mat srcImage = Global.Inst.InspStage.GetFilteredImage() ??
                  Global.Inst.InspStage.GetMat(0, inspAlgo.ImageChannel);
                inspAlgo.SetInspData(srcImage);
            }

            return true;
        }

        //InspWindow내의 알고리즘 중에서, 인자로 입력된 알고리즘과 같거나,
        //인자가 None이면 모든 알고리즘의 검사 결과(Rect 영역)를 얻어, cameraForm에 출력한다.
        private bool DisplayResult(InspWindow inspObj, InspectType inspType)
        {
            if (inspObj is null)
                return false;

            List<DrawInspectInfo> totalArea = new List<DrawInspectInfo>();

            List<InspAlgorithm> inspAlgorithmList = inspObj.AlgorithmList;
            foreach (var algorithm in inspAlgorithmList)
            {
                if (algorithm.InspectType != inspType && inspType != InspectType.InspNone)
                    continue;

                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                int resultCnt = algorithm.GetResultRect(out resultArea);
                if (resultCnt > 0)
                {
                    totalArea.AddRange(resultArea);
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

            return true;
        }
    }
}
