using CapsuleInspect.Core;
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
                BackColor = Color.FromArgb(41, 57, 85)
            };

            // DockPanel 생성 및 추가
            _dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(_dockPanel);
            Controls.Add(spacerPanel);
            Controls.SetChildIndex(spacerPanel, 0); // MenuStrip 아래
            Controls.SetChildIndex(_dockPanel, 1);  // spacer 아래

            _dockPanel.Theme = new VS2015BlueTheme();
            // 도킹 윈도우 로드 메서드 호출
            LoadDockingWindows();
            // Global 인스턴스 초기화
            Global.Inst.Initialize();
        }
        //도킹 윈도우를 로드하는 메서드
        private void LoadDockingWindows()
        {
            //도킹해제 금지 설정
            _dockPanel.AllowEndUserDocking = false;

            //메인폼 설정
            var cameraWindow = new CameraForm();
            cameraWindow.Show(_dockPanel, DockState.Document);

            var propWindow = new PropertiesForm();
            propWindow.Show(_dockPanel, DockState.DockRight);
            var runWindow = new RunForm();
            runWindow.Show(cameraWindow.Pane, DockAlignment.Bottom, 0.2);
            SharedFilterForm = new FilterForm();
            SharedFilterForm.Show(propWindow.Pane, DockAlignment.Bottom, 0.6);
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
                    cameraForm.LoadImage(filePath);
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.Inst.Dispose();
        }
    }
}
