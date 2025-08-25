using CapsuleInspect.Core;
using CapsuleInspect.Grab;
using CapsuleInspect.Sequence;
using CapsuleInspect.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace CapsuleInspect.Setting
{
    public partial class CommunicatorSetting : UserControl
    {
        private GrabModel _cachedGrab;
        public CommunicatorSetting()
        {
            InitializeComponent();

            //최초 로딩시, 환경설정 정보 로딩
            LoadSetting();
        }
        private GrabModel GetActiveGrab()
        {
            if (_cachedGrab != null) return _cachedGrab;

            var stage = Global.Inst?.InspStage;
            if (stage == null) return null;

            // 1) 프로퍼티에서 GrabModel 찾기
            var props = stage.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var p in props)
            {
                try
                {
                    if (typeof(GrabModel).IsAssignableFrom(p.PropertyType))
                    {
                        var val = p.GetValue(stage) as GrabModel;
                        if (val != null) { _cachedGrab = val; return _cachedGrab; }
                    }
                }
                catch { /* skip */ }
            }

            // 2) 필드에서 GrabModel 찾기
            var fields = stage.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                try
                {
                    if (typeof(GrabModel).IsAssignableFrom(f.FieldType))
                    {
                        var val = f.GetValue(stage) as GrabModel;
                        if (val != null) { _cachedGrab = val; return _cachedGrab; }
                    }
                }
                catch { /* skip */ }
            }

            return null;
        }

        /// <summary>
        /// 텍스트(숫자) → 노출값으로 파싱해 카메라에 적용
        /// ※ 단위: HikRobot은 μs(마이크로초)로 SetExposureTime을 사용합니다.
        /// </summary>
        private void ApplyExposureFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            // 숫자만 허용(정수). 소수 지원하려면 double.TryParse 후 (long)Math.Round(...)
            if (!long.TryParse(text.Trim(), out var exposure))
                return;

            // 장비 스펙에 맞춰 클램프 (예: 10 ~ 1,000,000 us)
            if (exposure < 10) exposure = 10;
            if (exposure > 1_000_000) exposure = 1_000_000;

            var grab = GetActiveGrab();
            if (grab == null)
            {
                SLogger.Write("카메라(GrabModel)를 찾을 수 없습니다.", SLogger.LogType.Error);
                return;
            }

            try
            {
                // GrabModel은 HikRobotCam/WebCam 모두 동일 시그니처를 구현
                if (grab.SetExposureTime(exposure))
                {
                    SLogger.Write($"노출(ExposureTime) 적용: {exposure} (μs 기준)");
                }
                else
                {
                    SLogger.Write("노출 설정 실패(드라이버가 거부함).", SLogger.LogType.Error);
                }
            }
            catch (Exception ex)
            {
                SLogger.Write($"노출 설정 중 예외: {ex}", SLogger.LogType.Error);
            }
        }

        /// <summary>
        /// 현재 카메라에서 노출값 읽어와 텍스트 박스에 표시
        /// </summary>
        private void RefreshExposureFromCamera()
        {
            var grab = GetActiveGrab();
            if (grab == null) return;

            try
            {
                if (grab.GetExposureTime(out long exp))
                {
                    // TextChanged 루프 방지용으로 값이 다를 때만 갱신
                    if (txtExposure.Text != exp.ToString())
                        txtExposure.Text = exp.ToString();
                }
            }
            catch { /* ignore */ }
        }
        private void LoadSetting()
        {
            cbCommType.DataSource = Enum.GetValues(typeof(CommunicatorType)).Cast<CommunicatorType>().ToList();

            txtMachine.Text = SettingXml.Inst.MachineName;
            //환경설정에서 현재 통신 타입 얻기
            cbCommType.SelectedIndex = (int)SettingXml.Inst.CommType;

            txtIpAddr.Text = SettingXml.Inst.CommIP;
            RefreshExposureFromCamera();
        }

        private void SaveSetting()
        {
            SettingXml.Inst.MachineName = txtMachine.Text;

            //환경설정에 통신 타입 설정
            SettingXml.Inst.CommType = (CommunicatorType)cbCommType.SelectedIndex;

            //통신 IP 설정
            SettingXml.Inst.CommIP = txtIpAddr.Text;

            //환경설정 저장
            SettingXml.Save();

            SLogger.Write($"통신 설정 저장");
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveSetting();
        }

        private void txtExposure_TextChanged(object sender, EventArgs e)
        {
            ApplyExposureFromText(txtExposure.Text);
        }
    }
}
