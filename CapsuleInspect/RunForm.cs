using CapsuleInspect.Core;
using CapsuleInspect.Grab;
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
            SLogger.Write($"[RunForm] 촬상 클릭됨. CameraType: {stage.GetCurrentCameraType()}");

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
                SLogger.Write($"[RunForm] 동영상 모드 클릭됨. CameraType:{stage.GetCurrentCameraType()}");
                stage.SetWorkingState(WorkingState.LIVE);
                stage.CheckImageBuffer();
                stage.Grab(0); // 최초 시작
            }
            else
            {
                SLogger.Write($"[RunForm] 동영상 모드 중지됨. CameraType:{stage.GetCurrentCameraType()}");
                stage.SetWorkingState(WorkingState.NONE);
            }
        }

        private void btnInsp_Click(object sender, EventArgs e)
        {
            SLogger.Write($"[RunForm] 검사 클릭됨.");
            string serialID = $"{DateTime.Now:MM-dd HH:mm:ss}";
            Global.Inst.InspStage.InspectReady("LOT_NUMBER", serialID);

            if (SettingXml.Inst.CamType == Grab.CameraType.None)
            {
                bool cycleMode = SettingXml.Inst.CycleMode;
                Global.Inst.InspStage.CycleInspect(cycleMode);
            }
            else
            {
                Global.Inst.InspStage.StartAutoRun();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.StopCycle();
        }
    }
}
    