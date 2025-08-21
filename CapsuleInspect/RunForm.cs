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

            bool isAIDefect = false;
            bool isTeachingDefect = false;
            List<string> defectTypes = new List<string>(); // 불량 유형 추적
            List<InspResult> teachingResults = new List<InspResult>(); // Teaching 검사 결과 저장

            // 1. AI 검사 (Scratch만 처리)
            string modelPath = stage.CurModel.ModelPath;
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
                            isAIDefect = saigeAI.IsDefect; // SaigeAI의 불량 여부 확인 (findCount >= 1)
                            if (isAIDefect)
                            {
                                defectTypes.Add("Scratch");
                                SLogger.Write("[RunForm] AI Scratch 불량 감지", SLogger.LogType.Info);
                            }
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

            // 2. Teaching 기반 검사 (Crack, Squeeze, PrintDefect)
            if (SettingXml.Inst.CamType == Grab.CameraType.None ||
                SettingXml.Inst.CommType == Sequence.CommunicatorType.None)
            {
                stage.CycleInspect(true); // 단일 사이클 검사
            }
            else
            {
                stage.StartAutoRun();
            }

            // 3. Teaching 검사 결과 수집
            foreach (var window in stage.CurModel.InspWindowList)
            {
                if (window.IgnoreInsp) continue; // 검사 제외된 ROI는 스킵
                foreach (var result in window.InspResultList)
                {
                    teachingResults.Add(result);
                    if (result.IsDefect)
                    {
                        isTeachingDefect = true;
                        switch (window.InspWindowType)
                        {
                            case InspWindowType.Crack:
                                defectTypes.Add("Crack");
                                break;
                            case InspWindowType.Squeeze:
                                defectTypes.Add("Squeeze");
                                break;
                            case InspWindowType.PrintDefect:
                                defectTypes.Add("PrintDefect");
                                break;
                        }
                    }
                }
            }

            // 4. 통계 업데이트
            var accum = stage.Accum; // InspStage의 Accum 프로퍼티 사용
            accum.Total++; // 총 검사 횟수 증가

            if (!isAIDefect && !isTeachingDefect)
            {
                accum.OK++; // 모든 검사에서 불량이 없으면 OK 카운트
                stage.AddAccumCount(1, 1, 0); // Total=1, OK=1, NG=0
            }
            else
            {
                accum.NG++; // 불량이 하나라도 있으면 NG 카운트
                stage.AddAccumCount(1, 0, 1); // Total=1, OK=0, NG=1

                // 불량 유형별 카운트
                int crack = defectTypes.Contains("Crack") ? 1 : 0;
                int scratch = defectTypes.Contains("Scratch") ? 1 : 0;
                int squeeze = defectTypes.Contains("Squeeze") ? 1 : 0;
                int printDefect = defectTypes.Contains("PrintDefect") ? 1 : 0;
                stage.AddNgDetailCount(crack, scratch, squeeze, printDefect);
            }

            // 5. 결과 표시
            var resultForm = MainForm.GetDockForm<ResultForm>();
            if (resultForm != null)
            {
                resultForm.AddModelResult(stage.CurModel);
            }

            // 6. 불량 종류별 고유 카운트 업데이트
            stage.SetDistinctNgCount(defectTypes.Distinct().Count());
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.StopCycle();
        }
    }
}
