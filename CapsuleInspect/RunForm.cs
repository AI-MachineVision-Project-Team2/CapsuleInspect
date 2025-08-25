using CapsuleInspect.Core;
using CapsuleInspect.Grab;
using CapsuleInspect.Inspect;
using CapsuleInspect.Setting;
using CapsuleInspect.Util;
using System;
using System.Drawing;
using System.Linq; // OfType<T>()
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace CapsuleInspect
{
    public partial class RunForm : DockContent
    {
        public RunForm()
        {
            InitializeComponent();
            // 생성자에서는 구독하지 않음 (초기화 타이밍 이슈 방지)
        }

        // 폼이 화면에 나타난 뒤에 안전하게 구독한다
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BindStage();
        }

        // 언제든 호출 가능: 최신 InspStage에 재구독
        public void BindStage()
        {
            var stage = Global.Inst.InspStage;
            if (stage == null) return;

            try { stage.InspectionCompleted -= OnInspectionCompleted; } catch { }
            stage.InspectionCompleted += OnInspectionCompleted;

            SLogger.Write("[RunForm] InspectionCompleted 구독 완료");
        }

        // DockContent 자원이 내려갈 때 구독 해제 (메모리 누수/중복 방지)
        protected override void OnHandleDestroyed(EventArgs e)
        {
            var stage = Global.Inst.InspStage;
            if (stage != null)
            {
                try { stage.InspectionCompleted -= OnInspectionCompleted; } catch { }
            }
            base.OnHandleDestroyed(e);
        }

        // 검사 완료 콜백: 세부 타입 문자열을 받는다 ("OK", "Scratch", "Crack", "Squeeze", "PrintDefect")
        private void OnInspectionCompleted(string defectType)
        {
            if (this.InvokeRequired)
            {
                try { this.BeginInvoke(new Action<string>(OnInspectionCompleted), defectType); } catch { }
                return;
            }

            var stage = Global.Inst.InspStage;
            if (stage == null) return;

            Bitmap bmp = stage.GetBitmap(); // 최신 버퍼/채널 기준
            if (bmp == null) return;

            // MainForm은 Form이므로 OpenForms로 찾는다
            var main = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (main != null)
            {
                // prefix는 필요에 맞게 변경 가능("Capsule")
                main.SaveFromInspection(bmp, defectType, "Capsule");
                SLogger.Write("[RunForm] SaveFromInspection 호출: " + defectType);
            }
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            var stage = Global.Inst.InspStage;
            SLogger.Write(string.Format("[RunForm] 촬상 클릭됨 CameraType: {0}", stage.GetCurrentCameraType()));

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
                SLogger.Write(string.Format("[RunForm] 동영상 모드 클릭됨 CameraType:{0}", stage.GetCurrentCameraType()));
                stage.SetWorkingState(WorkingState.LIVE);
                stage.CheckImageBuffer();
                stage.Grab(0); // 최초 시작
            }
            else
            {
                SLogger.Write(string.Format("[RunForm] 동영상 모드 중지됨 CameraType:{0}", stage.GetCurrentCameraType()));
                stage.SetWorkingState(WorkingState.NONE);
            }
        }

        private void btnInsp_Click(object sender, EventArgs e)
        {

            SLogger.Write($"[RunForm] 검사 클릭됨");
            string serialID = $"{DateTime.Now:MM-dd HH:mm:ss}";
            var stage = Global.Inst.InspStage;
            stage.InspectReady("LOT_NUMBER", serialID);
            // 초기화 뒤 재구독 보장 (안전망)
            BindStage();
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

            //Global.Inst.InspStage.ShowSaigeResult();
        }
    }
}
