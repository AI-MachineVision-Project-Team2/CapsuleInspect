using CapsuleInspect.Core;
using CapsuleInspect.Grab;
using CapsuleInspect.Inspect;
using CapsuleInspect.Setting;
using CapsuleInspect.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace CapsuleInspect
{

    public partial class RunForm : DockContent
    {

        public RunForm()
        {
            InitializeComponent();
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            // 그랩시 이미지 버퍼를 먼저 설정하도록 변경

            var stage = Global.Inst.InspStage;
            SLogger.Write($"[RunForm] 촬상 클릭됨 CameraType: {stage.GetCurrentCameraType()}");

            if (stage.GetCurrentCameraType() == CameraType.None)
            {
                MessageBox.Show("현재 선택된 카메라가 없습니다.");
                return;
            }
            stage.CheckImageBuffer();
            stage.Grab(0);
        }

        private void btnLive_Click(object sender, EventArgs e)
        {
            var stage = Global.Inst.InspStage;
            stage.ToggleLiveMode();
            if (stage.LiveMode)
            {
                SLogger.Write($"[RunForm] 동영상 모드 클릭됨 CameraType:{stage.GetCurrentCameraType()}");
                stage.SetWorkingState(WorkingState.LIVE);
                stage.CheckImageBuffer();
                stage.Grab(0); // 최초 시작
            }
            else
            {
                SLogger.Write($"[RunForm] 동영상 모드 중지됨 CameraType:{stage.GetCurrentCameraType()}");
                stage.SetWorkingState(WorkingState.NONE);
            }
        }

        private void btnInsp_Click(object sender, EventArgs e)
        {
            SLogger.Write($"[RunForm] 검사 클릭됨");
            string serialID = $"{DateTime.Now:MM-dd HH:mm:ss}";
            var stage = Global.Inst.InspStage;
            stage.InspectReady("LOT_NUMBER", serialID);
            //string modelPath = SettingXml.Inst.AIModelPath;
            //bool isAIDefect = false;
            //bool isTeachingDefect = false;
            //int ngScratchFromAI = 0; // AI에서 Scratch 불량 여부 (1 or 0)
            //List<string> defectTypes = new List<string>();

            //// AI 검사
            //string modelPath = stage.CurModel.ModelPath;
            //if (!string.IsNullOrEmpty(modelPath))
            //{
            //    var saigeAI = stage.AIModule;
            //    saigeAI.LoadEngine(modelPath);
            //    Bitmap bitmap = stage.GetBitmap();
            //    if (bitmap != null)
            //    {
            //        if (saigeAI.Inspect(bitmap)) // Inspect에서 _isDefect 설정
            //        {
            //            Bitmap resultImage = saigeAI.GetResultImage(); // 내부 DrawResult 호출, _isDefect 확인
            //            if (resultImage != null)
            //            {
            //                stage.UpdateDisplay(resultImage);
            //                isAIDefect = saigeAI.IsDefect; // _isDefect 사용
            //                if (isAIDefect) ngScratchFromAI = 1; // AI defect 시 Scratch 1 증가 (개수 무관)
            //            }
            //            else
            //            {
            //                SLogger.Write("[RunForm] 결과 이미지를 가져오지 못했습니다.", SLogger.LogType.Error);
            //                MessageBox.Show("결과 이미지를 가져오지 못했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            }
            //        }
            //        else
            //        {
            //            SLogger.Write("[RunForm] AI 검사 실패", SLogger.LogType.Error);
            //            MessageBox.Show("AI 검사에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //    else
            //    {
            //        SLogger.Write("[RunForm] 현재 이미지가 없습니다.", SLogger.LogType.Error);
            //        MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}

            //// Teaching 검사
            //int ngCrack, ngScratchFromTeaching, ngSqueeze, ngPrintDefect;
            //stage.InspWorker.RunInspect(out isTeachingDefect, out ngCrack, out ngScratchFromTeaching, out ngSqueeze, out ngPrintDefect);

            //// 전체 불량 여부
            //bool isOverallDefect = isAIDefect || isTeachingDefect;

            //// 통계 업데이트
            //var accum = stage.Accum;
            //stage.AddAccumCount(1, isOverallDefect ? 0 : 1, isOverallDefect ? 1 : 0); // 이미지당 Total/OK/NG 1씩

            //// 불량 유형: AI + Teaching
            //int ngScratch = (ngScratchFromAI > 0 || ngScratchFromTeaching > 0) ? 1 : 0; // 중복 방지, 이미지당 1
            //stage.AddNgDetailCount(ngCrack, ngScratch, ngSqueeze, ngPrintDefect);

            //// 결과 표시
            //var resultForm = MainForm.GetDockForm<ResultForm>();
            //if (resultForm != null)
            //{
            //    resultForm.AddModelResult(stage.CurModel);
            //}

            // 단일 사이클 실행
            if (SettingXml.Inst.CamType == Grab.CameraType.None ||
                SettingXml.Inst.CommType == Sequence.CommunicatorType.None)
            {
                Global.Inst.InspStage.CycleInspect(true); // 무한 루프 검사
            }
            else
            {
                stage.StartAutoRun();
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.StopCycle();
            // 이미지 인덱스 초기화
            if (Global.Inst.InspStage.ImageLoader != null)
            {
                Global.Inst.InspStage.ImageLoader.Reset(); // ImageLoader에 ResetIndex 메서드 필요
                SLogger.Write("[RunForm] 이미지 인덱스 초기화 완료");
            }

            Global.Inst.InspStage.ShowSaigeResult();
        }
    }
}
