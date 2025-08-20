using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking; // DockContent 을 사용 중이므로 유지
using PdfiumViewer;                 // PdfViewer, PdfDocument

namespace CapsuleInspect
{
    public partial class HelpForm : DockContent
    {
        private PdfViewer _pdfViewer;
        private PdfDocument _document;

        // 임시폴더에 풀어서 보여줄 파일명 (원하는 이름으로 변경 가능)
        private const string ExtractedPdfFileName = "Capsule-Inspection-MANUAL.pdf";

        public HelpForm()
        {
            InitializeComponent();
            InitPdfViewer();
            this.Load += (_, __) => LoadEmbeddedPdfToViewer();
        }

        private void InitPdfViewer()
        {
            _pdfViewer = new PdfViewer
            {
                Dock = DockStyle.Fill,
                ShowToolbar = true,               // 확대/축소/페이지 이동
                ShowBookmarks = false,
                ZoomMode = PdfViewerZoomMode.FitWidth
            };
            Controls.Add(_pdfViewer);
        }

        private void LoadEmbeddedPdfToViewer()
        {
            try
            {
                // 1) 어셈블리에서 PDF 리소스 찾기
                //    (프로젝트에 추가한 PDF의 Build Action = Embedded Resource 여야 함)
                var asm = Assembly.GetExecutingAssembly();

                // 프로젝트에서 추가한 PDF 파일명으로 끝나는 리소스를 찾아요.
                // 예: Properties:
                //    - Namespace: CapsuleInspect
                //    - 파일 경로: Docs/CapsuleInspection_Manual.pdf
                // 리소스 이름 예: "CapsuleInspect.Docs.CapsuleInspection_Manual.pdf"
                var resName = asm
                    .GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("Capsule-Inspection-MANUAL.pdf",
                                        StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(resName))
                {
                    MessageBox.Show(
                        "내장된 매뉴얼 리소스를 찾을 수 없습니다.\n" +
                        "PDF 파일이 프로젝트에 포함되어 있고 Build Action이 Embedded Resource인지 확인하세요.",
                        "Resource Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var resStream = asm.GetManifestResourceStream(resName))
                {
                    if (resStream == null)
                    {
                        MessageBox.Show("매뉴얼 리소스 스트림을 열 수 없습니다.",
                                        "Resource Stream Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2) 임시 폴더로 복사/추출
                    var tempPath = Path.Combine(Path.GetTempPath(), "CapsuleInspect");
                    Directory.CreateDirectory(tempPath);
                    var tempPdfPath = Path.Combine(tempPath, ExtractedPdfFileName);

                    // 기존 파일 있으면 덮어쓰기
                    using (var fs = new FileStream(tempPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        resStream.CopyTo(fs);
                    }

                    // 3) PdfiumViewer 로드
                    _document?.Dispose();
                    _document = PdfDocument.Load(tempPdfPath);
                    _pdfViewer.Document = _document;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"매뉴얼 로드 중 오류가 발생했습니다.\n{ex.Message}",
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _pdfViewer?.Dispose();
            _document?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
