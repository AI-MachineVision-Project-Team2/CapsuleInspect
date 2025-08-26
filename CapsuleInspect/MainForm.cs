using CapsuleInspect.Core;
using CapsuleInspect.Setting;
using CapsuleInspect.Teach;
using CapsuleInspect.UIControl;
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
using static CapsuleInspect.ImageSaveHelper; // ★ Category enum 사용

namespace CapsuleInspect
{
    public partial class MainForm : Form
    {
        private static DockPanel _dockPanel;
        public static FilterForm SharedFilterForm;

        // RunForm 참조를 필드로 보관 (BindStage 호출용)
        private RunForm _runWindow;

        //  실시간 저장기
        private readonly RealtimeImageSaver _realtimeSaver = new RealtimeImageSaver(boundedCapacity: 500, useDateFolder: true);

        public MainForm()
        {
            InitializeComponent();
            if (DesignModeDetector.IsDesignMode)
                return; // 디자이너에서는 여기서 중단

            mainMenu.Dock = DockStyle.Top;
            var spacerPanel = new Panel
            {
                Height = 5,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(243, 244, 246)
            };
            var toolboxCtrl = new ToolboxCtrl
            {
                Dock = DockStyle.Top,
                Height = 20
            };

            _dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(_dockPanel);
            Controls.Add(toolboxCtrl);
            Controls.Add(mainMenu);
            //Controls.Add(spacerPanel);

            //Controls.SetChildIndex(spacerPanel, 0); // MenuStrip 아래
            Controls.SetChildIndex(_dockPanel, 1);
            _dockPanel.Theme = new VS2015LightTheme();

            // 도킹 윈도우 생성/표시
            LoadDockingWindows();

            // InspStage 초기화
            Global.Inst.Initialize();

            // 환경설정 반영
            LoadSetting();

            // Initialize 직후 RunForm가 InspStage 이벤트를 구독하도록 보장
            _runWindow?.BindStage();

            // 저장 완료 시 경로 UI 갱신
            _realtimeSaver.OnSaved += path => { try { UpdateFilePathTextBox(path); } catch { } };
        }

        private void LoadDockingWindows()
        {
            _dockPanel.AllowEndUserDocking = false;

            var cameraWindow = new CameraForm();
            cameraWindow.Show(_dockPanel, DockState.Document);

            // 필드에 보관
            _runWindow = new RunForm();
            _runWindow.Show(cameraWindow.Pane, DockAlignment.Bottom, 0.3);

            var resultWindow = new ResultForm();
            resultWindow.Show(_dockPanel, DockState.DockRight);

            var statisicsWindow = new StatisticsForm();
            statisicsWindow.Show(_dockPanel, DockState.DockRight);

            var helpWindow = new HelpForm();
            helpWindow.Show(_dockPanel, DockState.DockRight);

            var propWindow = new PropertiesForm();
            propWindow.Show(_dockPanel, DockState.DockLeft);

            var logWindow = new LogForm();
            logWindow.Show(resultWindow.Pane, DockAlignment.Bottom, 0.3);

            SharedFilterForm = new FilterForm();
            SharedFilterForm.Show(propWindow.Pane, DockAlignment.Bottom, 0.3);

            var modelTreeWindow = new ModelTreeForm();
            modelTreeWindow.Show(_runWindow.Pane, DockAlignment.Bottom, 0.6);
        }

        private void LoadSetting()
        {
            cycleModeMenuItem.Checked = SettingXml.Inst.CycleMode;
            cycleModeMenuItem2.Checked = SettingXml.Inst.CycleMode2;
        }

        public static T GetDockForm<T>() where T : DockContent
        {
            var findForm = _dockPanel.Contents.OfType<T>().FirstOrDefault();
            return findForm;
        }

        private void imageOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraForm cameraForm = GetDockForm<CameraForm>();
            if (cameraForm is null)
                return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "이미지 파일 선택";
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    cameraForm.LoadImage(filePath);
                    Global.Inst.InspStage.SetImageBuffer(filePath);
                    Global.Inst.InspStage.CurModel.InspectImagePath = filePath;
                    txtFilePath.Text = filePath;
                }
            }
        }

        private void SetupMenuItem_Click(object sender, EventArgs e)
        {
            SLogger.Write("환경설정창 열기");
            SetupForm setupForm = new SetupForm();
            setupForm.ShowDialog();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.Inst.Dispose();

            // 백그라운드 저장기 정리
            _realtimeSaver.Dispose();
        }

        private string GetMdoelTitle(Model curModel)
        {
            if (curModel is null)
                return "";

            string modelName = curModel.ModelName;
            return $"{Define.PROGRAM_NAME} - MODEL : {modelName}";
        }

        private void modelNewMenuItem_Click(object sender, EventArgs e)
        {
            NewModel newModel = new NewModel();
            newModel.ShowDialog();

            Model curModel = Global.Inst.InspStage.CurModel;
            if (curModel != null)
            {
                this.Text = GetMdoelTitle(curModel);
            }
        }

        private void modelOpenMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "모델 파일 선택";
                openFileDialog.Filter = "Model Files|*.xml;";
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = SettingXml.Inst.ModelDir;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (Global.Inst.InspStage.LoadModel(filePath))
                    {
                        Model curModel = Global.Inst.InspStage.CurModel;
                        if (curModel != null)
                        {
                            this.Text = GetMdoelTitle(curModel);
                        }
                    }
                }
            }
        }

        public void DoModelOpen()
        {
            modelOpenMenuItem_Click(this, EventArgs.Empty);
        }

        private void modelSaveMenuItem_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.SaveModel("");
        }

        private void modelSaveAsMenuItem_Click(object sender, EventArgs e)
        {
            var model = Global.Inst.InspStage.CurModel;
            if (model != null)
                model.SaveAs();
        }
        

        public void DoModelSave()
        {
            modelSaveMenuItem_Click(this, EventArgs.Empty);
        }

        private void cycleModeMenuItem_Click(object sender, EventArgs e)
        {
            SLogger.Write("자동 반복 검사 클릭됨");
            bool isChecked = cycleModeMenuItem.Checked;
            SettingXml.Inst.CycleMode = isChecked;
            if (isChecked)
            {
                cycleModeMenuItem2.Checked = false;
                SettingXml.Inst.CycleMode2 = false;
            }
        }

        private void cycleModeMenuItem2_Click(object sender, EventArgs e)
        {
            SLogger.Write("자동 반복 검사(한 번) 클릭됨");
            bool isChecked = cycleModeMenuItem2.Checked;
            SettingXml.Inst.CycleMode2 = isChecked;
            if (isChecked)
            {
                cycleModeMenuItem.Checked = false;
                SettingXml.Inst.CycleMode = false;
            }
        }

        public static ImageViewCtrl GetImageViewCtrl()
        {
            var camForm = GetDockForm<CameraForm>();
            return camForm?.GetImageViewControl();
        }

        public void UpdateFilePathTextBox(string path)
        {
            if (txtFilePath.InvokeRequired)
            {
                txtFilePath.Invoke(new Action(() => txtFilePath.Text = path));
            }
            else
            {
                txtFilePath.Text = path;
            }
        }

        // 검사 결과가 나올 때 호출해서 저장
        public void SaveFromInspection(OpenCvSharp.Mat resultMat, string defectType, string prefix = "Capsule")
        {
            if (resultMat == null) return;

            Category cat;
            if (defectType == "OK") cat = Category.OK;
            else if (defectType == "Scratch") cat = Category.NG_Scratch;
            else if (defectType == "PrintDefect") cat = Category.NG_PrintDefect;
            else if (defectType == "Crack") cat = Category.NG_Crack;
            else if (defectType == "Squeeze") cat = Category.NG_Squeeze;
            else cat = Category.OK;

            _realtimeSaver.Enqueue(resultMat, cat, prefix);
        }

    }
}
