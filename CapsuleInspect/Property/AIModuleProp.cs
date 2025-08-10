using CapsuleInspect.Inspect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using CapsuleInspect.Core;
namespace CapsuleInspect.Property
{
    public partial class AIModuleProp : UserControl
    {
        SaigeAI _saigeAI; // SaigeAI 인스턴스
        string _modelPath = string.Empty;
        EngineType _engineType;

        public AIModuleProp()
        {
            InitializeComponent();
            cbAIEngineType.DataSource = Enum.GetValues(typeof(EngineType)).Cast<EngineType>().ToList();
            cbAIEngineType.SelectedIndex = 0;
        }


        private void btnSelAIModel_Click(object sender, EventArgs e)
        {
            int selType = cbAIEngineType.SelectedIndex;
            string filter = "AI Files|*.*;";

            switch (selType)
            {
                case 0: //IAD
                    filter = "Anomaly Detection Files|*.saigeiad;";
                    break;
                case 1: // SEG
                    filter = "Segmentation Files|*.saigeseg;";
                    break;
                case 2: //DET
                    filter = "Detection Files|*.saigedet;";
                    break;
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "AI 모델 파일 선택";
                openFileDialog.Filter = filter;
                openFileDialog.Multiselect = false;
                try
                {
                    if (!string.IsNullOrEmpty(_modelPath) && Directory.Exists(Path.GetDirectoryName(_modelPath)))
                        openFileDialog.InitialDirectory = Path.GetDirectoryName(_modelPath);
                    else
                        openFileDialog.InitialDirectory = @"C:\Saige\SaigeVision\engine\Examples\data\sfaw2023\models";
                }
                catch (Exception)
                {
                    openFileDialog.InitialDirectory = @"C:\Saige\SaigeVision\engine\Examples\data\sfaw2023\models";
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _modelPath = openFileDialog.FileName;
                    txtAIModelPath.Text = _modelPath;
                }
            }
        }

        private void btnLoadModel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_modelPath))
            {
                MessageBox.Show("모델 파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_saigeAI == null)
            {
                _saigeAI = Global.Inst.InspStage.AIModule;
            }

            _saigeAI.LoadEngine(_modelPath);
            MessageBox.Show("모델이 성공적으로 로드되었습니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnInspAI_Click(object sender, EventArgs e)
        {
            if (_saigeAI == null)
            {
                MessageBox.Show("AI 모듈이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = Global.Inst.InspStage.GetBitmap();

            if (bitmap is null)
            {
                MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _saigeAI.Inspect(bitmap);

            Bitmap resultImage = _saigeAI.GetResultImage();

            Global.Inst.InspStage.UpdateDisplay(resultImage);
        }

        private void cbAIEngineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbAIEngineType.SelectedIndex)
            {
                case 0:
                    _engineType = EngineType.IAD;
                    break;
                case 1:
                    _engineType = EngineType.SEG;
                    break;
                case 2:
                    _engineType = EngineType.DET;
                    break;
            }

            if (_saigeAI != null)
            {
                _saigeAI.Dispose(); // 메모리 해제
                _saigeAI = null;
            }

            if (_saigeAI == null)
                _saigeAI = Global.Inst.InspStage.AIModule;

            _saigeAI.SetEngineType(_engineType);
        }
    }
}
