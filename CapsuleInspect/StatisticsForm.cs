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
    public partial class StatisticsForm : DockContent
    {
        private DataGridView _grid;
        private Button _btnReset;
        private TableLayoutPanel _layout;

        public StatisticsForm()
        {
            Text = "통계창";
            Width = 300;

            InitStatisticsLayout();
            Global.Inst.InspStage.AccumChanged += OnAccumChanged;
        }

        private void InitStatisticsLayout()
        {
            // 전체 레이아웃
            _layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.White
            };
            _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 표 공간
            _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // 버튼 공간

            // 굵은 폰트 선언
            var boldFont = new Font("Noto Sans KR", 9F, FontStyle.Bold);

            // DataGridView 설정
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = boldFont,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Cursor = Cursors.Hand,
                GridColor = Color.LightGray,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 32,
                CellBorderStyle = DataGridViewCellBorderStyle.Single, // 셀 테두리
            };
       
            _grid.RowTemplate.Height = 32;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Noto Sans KR", 9F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = SystemColors.Highlight,
                SelectionForeColor = SystemColors.HighlightText,
                Padding = new Padding(0, 2, 0, 0) // 아래 여백 줄여 하단선 없앰
            };
            _grid.Columns.Add("c1", "Category");
            _grid.Columns.Add("c2", "Count");

            _grid.Rows.Add("Total", 0);
            _grid.Rows.Add("OK", 0);
            _grid.Rows.Add("NG", 0);
            _grid.Rows.Add("NG_Scratch", 0);
            _grid.Rows.Add("NG_Squeeze", 0);
            _grid.Rows.Add("NG_Crack", 0);
            _grid.Rows.Add("NG_PrintDefect", 0);

            // Reset 버튼
            _btnReset = new Button
            {
                Text = "Reset",
                Dock = DockStyle.Fill,
                Height = 36,
                Font = boldFont
            };
            _btnReset.Click += (s, e) => Global.Inst.InspStage.ResetAccum();

            // 배치
            _layout.Controls.Add(_grid, 0, 0);
            _layout.Controls.Add(_btnReset, 0, 1);

            Controls.Add(_layout);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Global.Inst.InspStage.AccumChanged -= OnAccumChanged;
            base.OnFormClosed(e);
        }

        private void OnAccumChanged(AccumCounter c)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnAccumChanged(c)));
                return;
            }

            _grid.Rows[0].Cells[1].Value = c.Total;
            _grid.Rows[1].Cells[1].Value = c.OK;
            _grid.Rows[2].Cells[1].Value = c.NG;
            _grid.Rows[3].Cells[1].Value = c.NG_Scratch;
            _grid.Rows[4].Cells[1].Value = c.NG_Squeeze;
            _grid.Rows[5].Cells[1].Value = c.NG_Crack;
            _grid.Rows[6].Cells[1].Value = c.NG_PrintDefect;
           
        }
    }
}
