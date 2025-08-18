using System;
using System.Linq;
using System.Runtime.InteropServices; // ← 추가!
using System.Windows.Forms;
using CapsuleInspect.Core;

namespace CapsuleInspect.UIControl
{
    public partial class ToolboxCtrl : UserControl
    {
        // ── Win32: 마우스 휠 메시지 보내기용 ──────────────────────────────
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WHEEL_DELTA = 120;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        // ────────────────────────────────────────────────────────────────

        public ToolboxCtrl()
        {
            InitializeComponent();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var helpForm = MainForm.GetDockForm<HelpForm>();
            helpForm?.Show();
        }

        private void btnModelOpen_Click(object sender, EventArgs e)
        {
            var main = this.FindForm() as CapsuleInspect.MainForm;
            if (main == null) { MessageBox.Show("MainForm를 찾지 못했습니다."); return; }
            main.DoModelOpen();
        }

        private void btnModelSave_Click(object sender, EventArgs e)
        {
            var main = this.FindForm() as CapsuleInspect.MainForm;
            if (main == null) { MessageBox.Show("MainForm를 찾지 못했습니다."); return; }
            main.DoModelSave();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            var viewer = FindImageViewer();
            if (viewer == null) { MessageBox.Show("Image 뷰어를 찾지 못했습니다."); return; }
            SendWheel(viewer, +WHEEL_DELTA);  // 확대
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            var viewer = FindImageViewer();
            if (viewer == null) { MessageBox.Show("Image 뷰어를 찾지 못했습니다."); return; }
            SendWheel(viewer, -WHEEL_DELTA);  // 축소
        }

        // ── 여기서부터 내부 헬퍼 ────────────────────────────────────────
        private Control FindImageViewer()
        {
            var cam = CapsuleInspect.MainForm.GetDockForm<CameraForm>();
            if (cam == null) return null;

            // CameraForm 내부 컨트롤 중 이름이 "imageViewer" 인 걸 찾음 (디자이너 Name 기준)
            var found = cam.Controls.Find("imageViewer", true);
            return found.Length > 0 ? found[0] : null;
        }

        private void SendWheel(Control ctrl, int delta)
        {
            if (ctrl == null || !ctrl.IsHandleCreated) return;

            // 휠 메시지는 화면 좌표(screen)로 좌표를 넣어야 함 (중앙 기준으로 보냄)
            var centerClient = new System.Drawing.Point(ctrl.ClientSize.Width / 2, ctrl.ClientSize.Height / 2);
            var centerScreen = ctrl.PointToScreen(centerClient);

            // wParam: HIWORD=delta(120 단위), LOWORD=키 플래그(0)
            IntPtr wParam = (IntPtr)((delta << 16) | 0);

            // lParam: Y(high 16) | X(low 16)  (화면 좌표, signed 16-bit)
            int lp = ((centerScreen.Y & 0xFFFF) << 16) | (centerScreen.X & 0xFFFF);
            IntPtr lParam = (IntPtr)lp;

            // 포커스 주고 메시지 전송
            ctrl.Focus();
            SendMessage(ctrl.Handle, WM_MOUSEWHEEL, wParam, lParam);
        }
        // ────────────────────────────────────────────────────────────────
    }
}
