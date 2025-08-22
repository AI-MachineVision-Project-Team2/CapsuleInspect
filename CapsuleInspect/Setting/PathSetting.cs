using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapsuleInspect.Util;
using CapsuleInspect.Inspect;
namespace CapsuleInspect.Setting
{

    public partial class PathSetting : UserControl
    {
        SaigeAI _saigeAI; // SaigeAI 인스턴스
        string _modelPath = string.Empty;
        EngineType _engineType;
        public PathSetting()
        {
            InitializeComponent();
            LoadSetting();
        }
        private void LoadSetting()
        {
            //환경설정에서 모델 저장 경로 얻기
            txtModelDir.Text = SettingXml.Inst.ModelDir;
            txtImageDir.Text = SettingXml.Inst.ImageDir;
            txtAIModelPath.Text = SettingXml.Inst.AIModelPath;
        }
        private void SaveSetting()
        {
            //환경설정에 모델 저장 경로 설정
            SettingXml.Inst.ModelDir = txtModelDir.Text;
            SettingXml.Inst.ImageDir = txtImageDir.Text;
            SettingXml.Inst.AIModelPath = txtAIModelPath.Text;
            //환경설정 저장
            SettingXml.Save();

        }
        private void btnSelModelDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "폴더를 선택하세요.";
                folderDialog.ShowNewFolderButton = true;    //새 폴더 생성 버튼 활성화

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtModelDir.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSelImageDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "폴더를 선택하세요.";
                folderDialog.ShowNewFolderButton = true;    //새 폴더 생성 버튼 활성화

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtImageDir.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveSetting();
            SLogger.Write($"경로 설정 적용");
        }

        private void AIModelPath_Click(object sender, EventArgs e)
        {
            string filter = "seg |*.*;";

            switch (_engineType)
            {

                case EngineType.SEG:
                    filter = "seg |*.saigeseg;";
                    break;

            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "AI 모델 파일 선택";
                openFileDialog.Filter = filter;
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = @"C:\model\SEG_capsule.saigeseg";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _modelPath = openFileDialog.FileName;
                    txtAIModelPath.Text = _modelPath;
                }
            }
        }
    }
}
