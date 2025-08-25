using CapsuleInspect.Core;
using CapsuleInspect.Inspect;
using CapsuleInspect.Teach;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CapsuleInspect.Util
{
    public class ReportSession : IDisposable
    {
        private readonly Document _doc;
        private readonly Model _model;
        private readonly string _title;
        private readonly List<string> _tempFiles = new List<string>();

        // 공통 페이지 설정 템플릿 (복제해서 각 섹션에 적용)
        private readonly PageSetup _pageSetupTemplate;

        // 맨 앞 통계용 섹션(처음에 만들어 두고 채워 넣음)
        private readonly Section _statsSection;

        private bool _disposed;

        public ReportSession(string title, Model model)
        {
            _title = title ?? "Inspection Report";
            _model = model ?? throw new ArgumentNullException(nameof(model));

            _doc = new Document();
            DefineStyles(_doc);

            // 기본 페이지 설정 복제 후 공통 여백 지정
            _pageSetupTemplate = _doc.DefaultPageSetup.Clone();
            _pageSetupTemplate.LeftMargin = Unit.FromMillimeter(12);
            _pageSetupTemplate.RightMargin = Unit.FromMillimeter(12);
            _pageSetupTemplate.TopMargin = Unit.FromMillimeter(10);
            _pageSetupTemplate.BottomMargin = Unit.FromMillimeter(12);

            // 통계 섹션을 먼저 하나 만들어 둔다(맨 앞 페이지가 됨)
            _statsSection = CreateSectionWithFooter();
        }

        private static void DefineStyles(Document doc)
        {
            var normal = doc.Styles["Normal"];
            normal.Font.Name = "Malgun Gothic"; // 한글 환경
            normal.Font.Size = 10;
        }

        private Section CreateSectionWithFooter()
        {
            if (_doc == null)
                throw new InvalidOperationException("Document is not initialized.");

            // 섹션 생성
            var sec = _doc.AddSection();
            // 페이지 설정 복제 적용 (널 가드)
            sec.PageSetup = _pageSetupTemplate != null ? _pageSetupTemplate.Clone() : _doc.DefaultPageSetup.Clone();

            // Footers.Primary가 내부에서 보장되더라도, 혹시 모를 NRE 방지
            var primary = sec.Footers?.Primary;
            if (primary == null)
            {
                // MigraDoc 1.32에서도 Primary는 접근 시 생성되지만,
                // 널이면 한 번 강제로 접근해 초기화
                primary = sec.Footers.Primary;
            }

            var footer = primary.AddParagraph();
            footer.AddText("Page ");
            footer.AddPageField();
            footer.AddText(" of ");
            footer.AddNumPagesField();
            footer.Format.Alignment = ParagraphAlignment.Right;
            footer.Format.Font.Size = 8;

            return sec;
        }

        private string SaveTemp(Bitmap bmp)
        {
            if (bmp == null) return null;
            string p = Path.Combine(Path.GetTempPath(), $"caps_{Guid.NewGuid():N}.png");
            bmp.Save(p, System.Drawing.Imaging.ImageFormat.Png);
            _tempFiles.Add(p);
            return p;
        }

        public class RowItem
        {
            public string Time { get; set; }
            public string Window { get; set; }
            public string Type { get; set; }
            public string Decision { get; set; }
            public string Value { get; set; }
            public string Detail { get; set; }
        }

        public static List<RowItem> CollectRowsFromModel(Model model)
        {
            var rows = new List<RowItem>();
            if (model?.InspWindowList == null) return rows;

            foreach (var w in model.InspWindowList)
            {
                if (w == null || w.InspResultList == null) continue;
                string winName = !string.IsNullOrWhiteSpace(w.Name) ? w.Name : w.UID;

                foreach (var r in w.InspResultList)
                {
                    if (r == null) continue;
                    rows.Add(new RowItem
                    {
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        Window = winName,
                        Type = r.InspType.ToString(),
                        Decision = r.IsDefect ? "NG" : "OK",
                        Value = r.ResultValue ?? "",
                        Detail = r.ResultInfos ?? ""
                    });
                }
            }
            return rows;
        }

        private void AddRowsTable(Section section, IEnumerable<RowItem> rows, string caption = null)
        {
            if (section == null) return;

            if (!string.IsNullOrEmpty(caption))
            {
                var cp = section.AddParagraph(caption);
                cp.Format.Font.Bold = true;
                cp.Format.SpaceBefore = Unit.FromPoint(6);
                cp.Format.SpaceAfter = Unit.FromPoint(2);
            }

            var tbl = section.AddTable();
            tbl.Borders.Width = 0.5;

            var cols = new[]
            {
                ("Time",   20.0),
                ("Window", 38.0),
                ("Type",   22.0),
                ("Decision", 22.0),
                ("Value",  22.0),
                ("Detail", 0.0) // 남는 폭
            };

            Unit totalFixed = Unit.FromMillimeter(cols.Where(c => c.Item2 > 0).Sum(c => c.Item2));
            Unit usable = section.PageSetup.PageWidth - section.PageSetup.LeftMargin - section.PageSetup.RightMargin;
            Unit detailWidth = usable - totalFixed;
            if (detailWidth < Unit.FromMillimeter(30)) detailWidth = Unit.FromMillimeter(30);

            foreach (var c in cols)
            {
                var col = tbl.AddColumn(c.Item2 > 0 ? Unit.FromMillimeter(c.Item2) : detailWidth);
                col.Format.Alignment = ParagraphAlignment.Left;
            }

            var header = tbl.AddRow();
            header.HeadingFormat = true;
            header.Shading.Color = Colors.LightGray;
            SetRow(header, "Time", "Window", "Type", "Decision", "Value", "Detail");

            foreach (var r in rows)
            {
                var row = tbl.AddRow();
                SetRow(row, r.Time, r.Window, r.Type, r.Decision, r.Value, r.Detail);
            }
        }

        private static void SetRow(Row row, params string[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                row.Cells[i].AddParagraph(cells[i] ?? "");
                row.Cells[i].Format.Font.Size = 9;
                row.Cells[i].VerticalAlignment = VerticalAlignment.Center;
            }
        }

        public void AddCyclePage(Bitmap originalImage, Bitmap resultImage, List<RowItem> newRows, int pageIndex)
        {
            var sec = CreateSectionWithFooter();

            var pTitle = sec.AddParagraph($"Cycle {pageIndex}");
            pTitle.Format.Font.Size = 14;
            pTitle.Format.Font.Bold = true;
            pTitle.Format.SpaceAfter = Unit.FromPoint(6);

            if (originalImage != null)
            {
                var p = SaveTemp(originalImage);
                if (!string.IsNullOrEmpty(p))
                {
                    var img = sec.AddImage(p);
                    img.LockAspectRatio = true;
                    img.Width = sec.PageSetup.PageWidth - sec.PageSetup.LeftMargin - sec.PageSetup.RightMargin;
                    var cap = sec.AddParagraph("Original Image");
                    cap.Format.Font.Size = 9;
                    cap.Format.SpaceAfter = Unit.FromPoint(4);
                }
            }

            if (resultImage != null)
            {
                var p = SaveTemp(resultImage);
                if (!string.IsNullOrEmpty(p))
                {
                    var img = sec.AddImage(p);
                    img.LockAspectRatio = true;
                    img.Width = sec.PageSetup.PageWidth - sec.PageSetup.LeftMargin - sec.PageSetup.RightMargin;
                    var cap = sec.AddParagraph("Result Image (Contours)");
                    cap.Format.Font.Size = 9;
                    cap.Format.SpaceAfter = Unit.FromPoint(4);
                }
            }

            if (newRows != null && newRows.Count > 0)
            {
                AddRowsTable(sec, newRows, "Results (New)");
            }
        }

        public void InsertStatsFrontPage(Bitmap statsGridImage, StatsSummary statsFallback)
        {
            
            var sec = _statsSection; // 이미 맨 앞 섹션

            var pTitle = sec.AddParagraph(_title);
            pTitle.Format.Font.Size = 16;
            pTitle.Format.Font.Bold = true;
            pTitle.Format.SpaceAfter = Unit.FromPoint(6);

            var pMeta = sec.AddParagraph(
                $"Model: {Path.GetFileNameWithoutExtension(_model.ModelPath)}   " +
                $"Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            pMeta.Format.Font.Size = 9;
            pMeta.Format.SpaceAfter = Unit.FromPoint(8);

            if (statsGridImage != null)
            {
                var p = SaveTemp(statsGridImage);
                if (!string.IsNullOrEmpty(p))
                {
                    var img = sec.AddImage(p);
                    img.LockAspectRatio = true;
                    img.Width = sec.PageSetup.PageWidth - sec.PageSetup.LeftMargin - sec.PageSetup.RightMargin;
                }
            }
            else if (statsFallback != null)
            {
                AddStatsTable(sec, statsFallback);
            }
        }
        private static void SetRowCompact(Row row, params string[] cells)
{
    for (int i = 0; i < cells.Length; i++)
    {
        var para = row.Cells[i].AddParagraph(cells[i] ?? "");
        row.Cells[i].Format.Font.Size = 8;                 // 작은 폰트
        row.Cells[i].VerticalAlignment = VerticalAlignment.Center;

        // 문단 여백 최소화
        para.Format.SpaceBefore = Unit.FromPoint(0);
        para.Format.SpaceAfter  = Unit.FromPoint(0);
    }
}
        private static void AddStatsTable(Section section, StatsSummary s)
        {
            var p = section.AddParagraph("Summary");
            p.Format.Font.Bold = true;
            p.Format.SpaceAfter = Unit.FromPoint(4);

            var tbl = section.AddTable();
            tbl.Borders.Width = 0.5;

            var c1 = tbl.AddColumn(Unit.FromMillimeter(30));
            var c2 = tbl.AddColumn(Unit.FromMillimeter(30));
            var c3 = tbl.AddColumn(Unit.FromMillimeter(30));
            var c4 = tbl.AddColumn(Unit.FromMillimeter(35));
            var c5 = tbl.AddColumn(Unit.FromMillimeter(35));
            var c6 = tbl.AddColumn(Unit.FromMillimeter(35));

            var header = tbl.AddRow();
            header.HeadingFormat = true;
            header.Shading.Color = Colors.LightGray;
            SetRow(header, "Total", "OK", "NG", "NG-Crack", "NG-Scratch", "NG-Squeeze / Print");

            var row = tbl.AddRow();
            SetRow(row, s.Total.ToString(), s.OK.ToString(), s.NG.ToString(),
                         s.NgCrack.ToString(), s.NgScratch.ToString(),
                         $"{s.NgSqueeze} / {s.NgPrintDefect}");
        }
        public void AddCyclePageCompact(
            string pageTitle,                 // ← "Cycle n" 대신 이미지 이름
            Bitmap originalImage,
            Bitmap resultImage,
            List<RowItem> rowsForThisCycle,   // 이번 사이클의 표(누적 아님)
            int pageIndex,                    // 필요하면 사용, 안 쓰면 무시 가능
            double maxImageHeightMm = 60,
            int rowsLimit = 12
        )
        {
            var sec = CreateSectionWithFooter();
            sec.PageSetup.PageFormat = PageFormat.A4;

            var pTitle = sec.AddParagraph(string.IsNullOrWhiteSpace(pageTitle) ? $"Cycle {pageIndex}" : pageTitle);
            pTitle.Format.Font.Size = 12;
            pTitle.Format.Font.Bold = true;
            pTitle.Format.SpaceAfter = Unit.FromPoint(4);

            Unit usableWidth = sec.PageSetup.PageWidth - sec.PageSetup.LeftMargin - sec.PageSetup.RightMargin;
            Unit colWidth = (usableWidth - Unit.FromMillimeter(2)) / 2;

            var tblImg = sec.AddTable();
            tblImg.Borders.Width = 0;
            tblImg.AddColumn(colWidth);
            tblImg.AddColumn(colWidth);
            var r = tblImg.AddRow();

            if (originalImage != null)
            {
                var p = SaveTemp(originalImage);
                if (!string.IsNullOrEmpty(p))
                {
                    var img = r.Cells[0].AddImage(p);
                    img.LockAspectRatio = true;
                    img.Height = Unit.FromMillimeter(maxImageHeightMm);
                    r.Cells[0].AddParagraph("Original").Format.Font.Size = 8;
                }
            }

            if (resultImage != null)
            {
                var p2 = SaveTemp(resultImage);
                if (!string.IsNullOrEmpty(p2))
                {
                    var img2 = r.Cells[1].AddImage(p2);
                    img2.LockAspectRatio = true;
                    img2.Height = Unit.FromMillimeter(maxImageHeightMm);
                    r.Cells[1].AddParagraph("Contours").Format.Font.Size = 8;
                }
            }

            var sep = sec.AddParagraph();
            sep.Format.SpaceBefore = Unit.FromPoint(2);
            sep.Format.SpaceAfter = Unit.FromPoint(2);

            var rowsToUse = (rowsForThisCycle ?? new List<RowItem>()).Take(rowsLimit).ToList();
            if (rowsToUse.Count > 0)
            {
                // 콤팩트 표(작은 폰트/여백)
                var tbl = sec.AddTable();
                tbl.Borders.Width = 0.25;
                tbl.Format.SpaceBefore = Unit.FromPoint(0);
                tbl.Format.SpaceAfter = Unit.FromPoint(2);

                var defs = new[]
                {
                    ("Time",   18.0),
                    ("Window", 32.0),
                    ("Type",   18.0),
                    ("Decision", 18.0),
                    ("Value",  18.0),
                    ("Detail", 0.0)
                };

                Unit tf = Unit.FromMillimeter(defs.Where(d => d.Item2 > 0).Sum(d => d.Item2));
                Unit dw = usableWidth - tf;
                if (dw < Unit.FromMillimeter(25)) dw = Unit.FromMillimeter(25);

                foreach (var d in defs)
                    tbl.AddColumn(d.Item2 > 0 ? Unit.FromMillimeter(d.Item2) : dw);

                var header = tbl.AddRow();
                header.HeadingFormat = true;
                header.Shading.Color = Colors.WhiteSmoke;
                SetRowCompact(header, "Time", "Window", "Type", "Decision", "Value", "Detail");

                foreach (var rr in rowsToUse)
                {
                    var row = tbl.AddRow();
                    SetRowCompact(row, rr.Time, rr.Window, rr.Type, rr.Decision, rr.Value, rr.Detail);
                }
            }
        }
        public void Save(string filePath)
        {
            var renderer = new PdfDocumentRenderer(true); // MigraDoc 1.32 호환
            renderer.Document = _doc;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);

            foreach (var f in _tempFiles.ToList())
            {
                try { File.Delete(f); } catch { }
                _tempFiles.Remove(f);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var f in _tempFiles.ToList())
            {
                try { File.Delete(f); } catch { }
            }
            _tempFiles.Clear();
        }

        // ====== 캡처/통계 유틸 ======
        public static Bitmap CaptureCameraImageSafe(CameraForm cam)
        {
            if (cam == null) return null;
            if (cam.InvokeRequired)
            {
                Bitmap bmp = null;
                cam.Invoke(new Action(() => bmp = CaptureControl(cam)));
                return bmp;
            }
            return CaptureControl(cam);
        }

        public static Bitmap CaptureControl(Control ctrl)
        {
            if (ctrl == null) return null;
            var size = ctrl.ClientRectangle.Size;
            if (size.Width <= 0 || size.Height <= 0) return null;

            var bmp = new Bitmap(size.Width, size.Height);
            ctrl.DrawToBitmap(bmp, new System.Drawing.Rectangle(System.Drawing.Point.Empty, size));
            return bmp;
        }

        public static Bitmap CaptureControlSafe(Control ctrl)
        {
            if (ctrl == null) return null;
            if (ctrl.InvokeRequired)
            {
                Bitmap bmp = null;
                ctrl.Invoke(new Action(() => bmp = CaptureControl(ctrl)));
                return bmp;
            }
            return CaptureControl(ctrl);
        }

        public static Bitmap MatToBitmap(Mat mat)
        {
            if (mat == null || mat.Empty()) return null;
            return mat.ToBitmap();
        }

        public class StatsSummary
        {
            public int Total { get; set; }
            public int OK { get; set; }
            public int NG { get; set; }
            public int NgCrack { get; set; }
            public int NgScratch { get; set; }
            public int NgSqueeze { get; set; }
            public int NgPrintDefect { get; set; }
        }

        public static StatsSummary GetStatsFromStageSafe()
        {
            var stage = Global.Inst?.InspStage;
            if (stage == null) return null;

            int total = GetInt(stage, "TotalCount", "Total", "CntTotal");
            int ok = GetInt(stage, "OkCount", "OK", "CntOK");
            int ng = GetInt(stage, "NgCount", "NG", "CntNG");

            int ngCr = GetInt(stage, "NgCrack", "CrackCount", "CntCrack");
            int ngSc = GetInt(stage, "NgScratch", "ScratchCount", "CntScratch");
            int ngSq = GetInt(stage, "NgSqueeze", "SqueezeCount", "CntSqueeze");
            int ngPr = GetInt(stage, "NgPrintDefect", "PrintDefectCount", "CntPrintDefect");

            return new StatsSummary
            {
                Total = total,
                OK = ok,
                NG = ng,
                NgCrack = ngCr,
                NgScratch = ngSc,
                NgSqueeze = ngSq,
                NgPrintDefect = ngPr
            };
        }
        public void AddCyclePageCompact(
    Bitmap originalImage,
    Bitmap resultImage,
    List<RowItem> newRows,
    int pageIndex,
    double maxImageHeightMm = 70,   // 이미지 최대 높이 제한
    int rowsLimit = 20              // 표에 넣을 최대 행수(페이지에 다 들어가게)
)
        {
            var sec = CreateSectionWithFooter();

            // 제목
            var pTitle = sec.AddParagraph($"Cycle {pageIndex}");
            pTitle.Format.Font.Size = 14;
            pTitle.Format.Font.Bold = true;
            pTitle.Format.SpaceAfter = Unit.FromPoint(6);

            // 가용 폭 계산
            Unit usableWidth = sec.PageSetup.PageWidth - sec.PageSetup.LeftMargin - sec.PageSetup.RightMargin;
            Unit colWidth = usableWidth / 2 - Unit.FromMillimeter(2); // 좌/우 여백 조금

            // 2열 테이블 만들어 이미지 나란히 배치
            var tblImg = sec.AddTable();
            tblImg.Borders.Width = 0;
            var c1 = tblImg.AddColumn(colWidth);
            var c2 = tblImg.AddColumn(colWidth);
            var r = tblImg.AddRow();

            // 왼쪽: 원본
            if (originalImage != null)
            {
                var p = SaveTemp(originalImage);
                if (!string.IsNullOrEmpty(p))
                {
                    var img = r.Cells[0].AddImage(p);
                    img.LockAspectRatio = true;
                    img.Width = colWidth;                       // 열 폭에 맞춤
                    img.Height = Unit.FromMillimeter(maxImageHeightMm); // 너무 크면 높이 제한
                    r.Cells[0].AddParagraph("Original").Format.Font.Size = 9;
                }
            }

            // 오른쪽: 결과(컨투어)
            if (resultImage != null)
            {
                var p2 = SaveTemp(resultImage);
                if (!string.IsNullOrEmpty(p2))
                {
                    var img2 = r.Cells[1].AddImage(p2);
                    img2.LockAspectRatio = true;
                    img2.Width = colWidth;
                    img2.Height = Unit.FromMillimeter(maxImageHeightMm);
                    r.Cells[1].AddParagraph("Contours").Format.Font.Size = 9;
                }
            }

            // 이미지와 표 사이 여백
            sec.AddParagraph().Format.SpaceAfter = Unit.FromPoint(6);

            // 결과 표(신규 행만, rowsLimit 만큼 잘라서)
            var rowsToUse = (newRows ?? new List<RowItem>()).Take(rowsLimit).ToList();
            if (rowsToUse.Count > 0)
            {
                AddRowsTable(sec, rowsToUse, "Results (New)");
            }
        }
        private static int GetInt(object obj, params string[] names)
        {
            foreach (var n in names)
            {
                var p = obj.GetType().GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && (p.PropertyType == typeof(int)))
                    return (int)(p.GetValue(obj) ?? 0);
            }
            return 0;
        }
    }
}
