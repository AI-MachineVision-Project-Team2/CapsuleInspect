using CapsuleInspect.Core;
using CapsuleInspect.Grab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapsuleInspect.Setting
{
    public partial class CameraSetting : UserControl
    {
        public CameraSetting()
        {
            InitializeComponent();
            LoadSetting();
        }
        private void LoadSetting()
        {
            cbCameraType.DataSource = Enum.GetValues(typeof(CameraType)).Cast<CameraType>().ToList();

            //환경설정에서 현재 카메라 타입 얻기
            cbCameraType.SelectedIndex = (int)SettingXml.Inst.CamType;
        }
        private void SaveSetting()
        {
            CameraType selectedType = (CameraType)cbCameraType.SelectedIndex;

            SettingXml.Inst.CamType = selectedType;
            SettingXml.Save();

            // 새로 추가된 부분!
            Global.Inst.InspStage.SetCameraType(selectedType);
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveSetting();
            CameraType selectedType = (CameraType)cbCameraType.SelectedItem;
            //SLogger.Write($"[CameraSetting] 적용된 카메라 타입: {selectedType}");
        }
    }
}
