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

            // AI 검사 추가
            string modelPath = SettingXml.Inst.AIModelPath;
            if (!string.IsNullOrEmpty(modelPath))
            {
                var saigeAI = stage.AIModule;
                saigeAI.LoadEngine(modelPath);

                Bitmap bitmap = stage.GetBitmap();
                if (bitmap != null)
                {
                    if (saigeAI.Inspect(bitmap))
                    {
                        Bitmap resultImage = saigeAI.GetResultImage();
                        if (resultImage != null)
                        {
                            stage.UpdateDisplay(resultImage);
                        }
                        else
                        {
                            SLogger.Write("[RunForm] 결과 이미지를 가져오지 못했습니다.", SLogger.LogType.Error);
                            MessageBox.Show("결과 이미지를 가져오지 못했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        SLogger.Write("[RunForm] AI 검사 실패", SLogger.LogType.Error);
                        MessageBox.Show("AI 검사에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    SLogger.Write("[RunForm] 현재 이미지가 없습니다.", SLogger.LogType.Error);
                    MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (SettingXml.Inst.CamType == Grab.CameraType.None ||
                SettingXml.Inst.CommType == Sequence.CommunicatorType.None)
            {

                Global.Inst.InspStage.CycleInspect(true); // 무한 루프 검사

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
