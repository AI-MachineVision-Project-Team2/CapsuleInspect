using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Setting;
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

        // 마지막 판정의 세부 NG 타입(또는 "OK")
        public string LastDefectType { get; private set; } = "OK";

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
                    SLogger.Write(string.Format("[InspWorker] 이미지 폴더가 존재하지 않음: {0}", inspImageDir), SLogger.LogType.Error);
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
        public bool RunInspect(out bool isDefect, out int ngCrack, out int ngScratch, out int ngSqueeze, out int ngPrintDefect)
        {
            /*
            // 기본 out 값
            isDefect = false;
            ngCrack = ngScratch = ngSqueeze = ngPrintDefect = 0;
            LastDefectType = "OK";

            Model currentModel = Global.Inst.InspStage.CurModel;
            if (currentModel == null || currentModel.InspWindowList == null)
            {
                var cam0 = MainForm.GetDockForm<CameraForm>();
                cam0?.SetInspResultCount(0, 0, 0);
                Global.Inst.InspStage.SetDistinctNgCount(0);
                return true;
            }

            // 검사 대상 ROI만 추출
            var activeWindows = currentModel.InspWindowList
                .Where(w => w != null && !w.IgnoreInsp)
                .ToList();

            // 활성 ROI가 없으면 UI 초기화 후 종료
            if (activeWindows.Count == 0)
            {
                var cam1 = MainForm.GetDockForm<CameraForm>();
                cam1?.SetInspResultCount(0, 0, 0);
                Global.Inst.InspStage.SetDistinctNgCount(0);
                return true;
            }

            // 검사 데이터 준비
            foreach (var w in activeWindows)
                UpdateInspData(w);

            // 실제 검사 실행
            _inspectBoard.InspectWindowList(activeWindows);

            // ROI별 OK/NG 카운트(상단 카운터 표시용)
            int totalRoi = activeWindows.Count;
            int ngRoi = activeWindows.Count(w => w.IsDefect());
            int okRoi = totalRoi - ngRoi;

            // === 유형별 '존재 여부'로 1회만 집계 (PrintDefect가 여러 ROI여도 1) ===
            bool bCrack = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.Crack && w.IsDefect());
            bool bScratch = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.Scratch && w.IsDefect());
            bool bSqueeze = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.Squeeze && w.IsDefect());
            bool bPrint = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.PrintDefect && w.IsDefect());

            ngCrack = bCrack ? 1 : 0;
            ngScratch = bScratch ? 1 : 0;
            ngSqueeze = bSqueeze ? 1 : 0;
            ngPrintDefect = bPrint ? 1 : 0;

            isDefect = bCrack || bScratch || bSqueeze || bPrint;

            // ROI별 결과 표시(도형/박스)
            foreach (var w in activeWindows)
                DisplayResult(w, InspectType.InspNone);

            // 종류별 distinct 카운트 (표시에 사용)
            Global.Inst.InspStage.SetDistinctNgCount(
                (bCrack ? 1 : 0) + (bScratch ? 1 : 0) + (bSqueeze ? 1 : 0) + (bPrint ? 1 : 0)
            );

            // 상단 카운터 갱신(ROI 기준)
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.SetInspResultCount(totalRoi, okRoi, ngRoi);

            // 누적 카운트 갱신(이미지 1장 단위)
            Global.Inst.InspStage.AddAccumCount(
                1,                 // Total +1
                isDefect ? 0 : 1,  // 모두 정상이면 OK +1
                isDefect ? 1 : 0   // 하나라도 불량이면 NG +1
            );

            // 세분화된 NG 카운트(유형별 존재 시 1씩)
            Global.Inst.InspStage.AddNgDetailCount(ngCrack, ngScratch, ngSqueeze, ngPrintDefect);

            // 최종 세부 타입(우선순위: Crack > Squeeze > Scratch > PrintDefect)
            if (!isDefect)
            {
                LastDefectType = "OK";
            }
            else
            {
                if (bCrack) LastDefectType = "Crack";
                else if (bSqueeze) LastDefectType = "Squeeze";
                else if (bScratch) LastDefectType = "Scratch";
                else LastDefectType = "PrintDefect";
            }

            return true;*/
            // 기본 out 값
            isDefect = false;
            ngCrack = ngScratch = ngSqueeze = ngPrintDefect = 0;
            LastDefectType = "OK";

            Model currentModel = Global.Inst.InspStage.CurModel;
            if (currentModel == null || currentModel.InspWindowList == null)
            {
                var cam0 = MainForm.GetDockForm<CameraForm>();
                cam0?.SetInspResultCount(0, 0, 0);
                Global.Inst.InspStage.SetDistinctNgCount(0);  
                // --- ResultForm 갱신: 이번 검사 결과(모델 전체)로 UI 리프레시 ---
                var resultForm1 = MainForm.GetDockForm<ResultForm>();
                resultForm1?.AddModelResult(Global.Inst.InspStage.CurModel);

                return true;
            }

            // 검사 대상 ROI만 추출
            var activeWindows = currentModel.InspWindowList
                .Where(w => w != null && !w.IgnoreInsp)
                .ToList();

            // 활성 ROI가 없으면 UI 초기화 후 종료
            if (activeWindows.Count == 0)
            {
                var cam1 = MainForm.GetDockForm<CameraForm>();
                cam1?.SetInspResultCount(0, 0, 0);
                Global.Inst.InspStage.SetDistinctNgCount(0);
                // --- ResultForm 갱신 ---
                var resultForm1 = MainForm.GetDockForm<ResultForm>();
                resultForm1?.AddModelResult(Global.Inst.InspStage.CurModel);
                return true;
            }

            // 검사 데이터 준비
            foreach (var w in activeWindows)
                UpdateInspData(w);

            // 실제 검사 실행
            _inspectBoard.InspectWindowList(activeWindows);

            // ROI별 OK/NG 카운트(상단 카운터 표시용)
            int totalRoi = activeWindows.Count;
            int ngRoi = activeWindows.Count(w => w.IsDefect());
            int okRoi = totalRoi - ngRoi;

            // === 유형별 '존재 여부'로 1회만 집계 (PrintDefect가 여러 ROI여도 1) ===
            bool bCrack = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.Crack && w.IsDefect());
            bool bScratch = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.Scratch && w.IsDefect());
            bool bSqueeze = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.Squeeze && w.IsDefect());
            bool bPrint = activeWindows.Any(w => GetInspWindowKind(w) == InspWindowType.PrintDefect && w.IsDefect());

            ngCrack = bCrack ? 1 : 0;
            ngScratch = bScratch ? 1 : 0;
            ngSqueeze = bSqueeze ? 1 : 0;
            ngPrintDefect = bPrint ? 1 : 0;

            isDefect = bCrack || bScratch || bSqueeze || bPrint;

            // ROI별 결과 표시(도형/박스)
            foreach (var w in activeWindows)
                DisplayResult(w, InspectType.InspNone);

            // 종류별 distinct 카운트 (표시에 사용)
            Global.Inst.InspStage.SetDistinctNgCount(
                (bCrack ? 1 : 0) + (bScratch ? 1 : 0) + (bSqueeze ? 1 : 0) + (bPrint ? 1 : 0)
            );

            // 상단 카운터 갱신(ROI 기준)
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.SetInspResultCount(totalRoi, okRoi, ngRoi);

            // 누적 카운트 갱신(이미지 1장 단위)
            Global.Inst.InspStage.AddAccumCount(
                1,                 // Total +1
                isDefect ? 0 : 1,  // 모두 정상이면 OK +1
                isDefect ? 1 : 0   // 하나라도 불량이면 NG +1
            );

            // 세분화된 NG 카운트(유형별 존재 시 1씩)
            Global.Inst.InspStage.AddNgDetailCount(ngCrack, ngScratch, ngSqueeze, ngPrintDefect);

            // 최종 세부 타입(우선순위: Crack > Squeeze > Scratch > PrintDefect)
            if (!isDefect)
            {
                LastDefectType = "OK";
            }
            else
            {
                if (bCrack) LastDefectType = "Crack";
                else if (bSqueeze) LastDefectType = "Squeeze";
                else if (bScratch) LastDefectType = "Scratch";
                else LastDefectType = "PrintDefect";
            }
             var resultForm = MainForm.GetDockForm<ResultForm>();
            if (resultForm != null)
            {
                resultForm.AddModelResult(Global.Inst.InspStage.CurModel);
            }

            return true;
        }

        private InspWindowType GetInspWindowKind(InspWindow w)
        {
            
            var t = w.GetType();
            var p1 = t.GetProperty("InspWindowType");
            if (p1 != null && p1.PropertyType == typeof(InspWindowType))
                return (InspWindowType)p1.GetValue(w, null);

            var p2 = t.GetProperty("WindowType");
            if (p2 != null && p2.PropertyType == typeof(InspWindowType))
                return (InspWindowType)p2.GetValue(w, null);

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
                int ngCrack, ngScratch, ngSqueeze, ngPrintDefect;
                RunInspect(out isDefect, out ngCrack, out ngScratch, out ngSqueeze, out ngPrintDefect);
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


                if (inspAlgo.InspectType == InspectType.InspAI)
                {
                    Mat srcImage = Global.Inst.InspStage.GetMat(0,eImageChannel.Color);
                    inspAlgo.SetInspData(srcImage);

                    Global.Inst.InspStage.AIModule.LoadEngine(SettingXml.Inst.AIModelPath);
                    AIAlgorithm aiAlgo = inspAlgo as AIAlgorithm;
                    aiAlgo.SetSaigeAI(Global.Inst.InspStage.AIModule);
                }
                else
                {
                    Mat srcImage = Global.Inst.InspStage.GetFilteredImage() ??
                    Global.Inst.InspStage.GetMat(0, inspAlgo.ImageChannel);
                    inspAlgo.SetInspData(srcImage);


                }
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
