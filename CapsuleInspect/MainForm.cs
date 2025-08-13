using CapsuleInspect.Core;
using CapsuleInspect.Setting;
using CapsuleInspect.Teach;
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

    public partial class MainForm : Form
    {
        private static DockPanel _dockPanel;
        public static FilterForm SharedFilterForm;

        public MainForm()
        {
            InitializeComponent();
            var spacerPanel = new Panel
            {
                Height = 5,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(255, 221, 221) // 상단 메뉴와 도킹 패널 사이의 간격을 위한 패널
            };
           
            // DockPanel 생성 및 추가
            _dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(_dockPanel);
            //Controls.Add(spacerPanel);
            
           
            //Controls.SetChildIndex(spacerPanel, 0); // MenuStrip 아래
            Controls.SetChildIndex(_dockPanel, 1);  // spacer 아래
            _dockPanel.Theme = new VS2015LightTheme();
            // 도킹 윈도우 로드 메서드 호출
            LoadDockingWindows();
            // Global 인스턴스 초기화
            Global.Inst.Initialize();
            LoadSetting();
        }
        //도킹 윈도우를 로드하는 메서드
        private void LoadDockingWindows()
        {
            //도킹해제 금지 설정
            _dockPanel.AllowEndUserDocking = false;

            //메인폼 설정
            var cameraWindow = new CameraForm();
            cameraWindow.Show(_dockPanel, DockState.Document);


            var runWindow = new RunForm();
            runWindow.Show(cameraWindow.Pane, DockAlignment.Bottom, 0.3);
            var resultWindow = new ResultForm();
            resultWindow.Show(_dockPanel, DockState.DockRight);
            var propWindow = new PropertiesForm();
            propWindow.Show(_dockPanel, DockState.DockLeft);

            var logWindow = new LogForm();
            logWindow.Show(resultWindow.Pane, DockAlignment.Bottom, 0.3);
            SharedFilterForm = new FilterForm();
            SharedFilterForm.Show(propWindow.Pane, DockAlignment.Bottom, 0.3);

            //#11_MODEL_TREE#1 검사 결과창 우측에 40% 비율로 모델트리 추가
            var modelTreeWindow = new ModelTreeForm();
            modelTreeWindow.Show(runWindow.Pane, DockAlignment.Bottom, 0.6);
            
        }
        private void LoadSetting()
        {
            cycleModeMenuItem.Checked = SettingXml.Inst.CycleMode;
        }

        //쉽게 도킹패널에 접근하기 위한 정적 함수
        //제네릭 함수 사용를 이용해 입력된 타입의 폼 객체 얻기
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
                    Global.Inst.InspStage.SetImageBuffer(filePath);
                    Global.Inst.InspStage.CurModel.InspectImagePath = filePath;
                    txtFilePath.Text = filePath;
                }
            }
        }
        private void SetupMenuItem_Click(object sender, EventArgs e)
        {
            SLogger.Write($"환경설정창 열기");
            SetupForm setupForm = new SetupForm();
            setupForm.ShowDialog();
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.Inst.Dispose();
        }

        // 모델 파일 열기,저장, 다른 이름으로 저장 기능 구현
        private string GetMdoelTitle(Model curModel)
        {
            if (curModel is null)
                return "";

            string modelName = curModel.ModelName;
            return $"{Define.PROGRAM_NAME} - MODEL : {modelName}";
        }

        private void modelNewMenuItem_Click(object sender, EventArgs e)
        {
            //신규 모델 추가를 위한 모델 정보를 받기 위한 창 띄우기
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
            //모델 파일 열기
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

        private void modelSaveMenuItem_Click(object sender, EventArgs e)
        {
            //모델 파일 저장
            Global.Inst.InspStage.SaveModel("");
        }

        private void modelSaveAsMenuItem_Click(object sender, EventArgs e)
        {
            //다른이름으로 모델 파일 저장
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = SettingXml.Inst.ModelDir;
                saveFileDialog.Title = "모델 파일 선택";
                saveFileDialog.Filter = "Model Files|*.xml;";
                saveFileDialog.DefaultExt = "xml";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    Global.Inst.InspStage.SaveModel(filePath);
                }
            }
        }
        //Cycle 모드 설정
        private void cycleModeMenuItem_Click(object sender, EventArgs e)
        {
            SLogger.Write($"Cycle Mode Click");
            // 현재 체크 상태 확인
            bool isChecked = cycleModeMenuItem.Checked;
            SettingXml.Inst.CycleMode = isChecked;
        }
        
    }
}
