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
        DataGridView _grid;
        Button _btnReset; // 원치 않으면 제거 가능

        public StatisticsForm()
        {
            Text = "통계창";
            Width = 260;

            _grid = new DataGridView
            {
                Dock = DockStyle.Top,
              
                Height = 130,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ColumnHeadersVisible = false
            };
            _grid.Columns.Add("c1", "항목");
            _grid.Columns.Add("c2", "값");
            _grid.Rows.Add("Total", 0);
            _grid.Rows.Add("OK", 0);
            _grid.Rows.Add("NG", 0);
            //_grid.Rows.Add("NG_Squeeze", 0);
            //_grid.Rows.Add("NG_FaultyPrint", 0);
            //_grid.Rows.Add("NG_Crack", 0);



            _btnReset = new Button { Dock = DockStyle.Top, Height = 32, Text = "Reset" };
            _btnReset.Click += (s, e) => Global.Inst.InspStage.ResetAccum();

            Controls.Add(_btnReset);   // Reset 버튼을 빼고 싶으면 이 줄 주석
            Controls.Add(_grid);

            Global.Inst.InspStage.AccumChanged += OnAccumChanged;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Global.Inst.InspStage.AccumChanged -= OnAccumChanged;
            base.OnFormClosed(e);
        }

        void OnAccumChanged(AccumCounter c)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => OnAccumChanged(c))); return; }

            _grid.Rows[0].Cells[1].Value = c.Total;
            _grid.Rows[1].Cells[1].Value = c.OK;
            _grid.Rows[2].Cells[1].Value = c.NG;
            //_grid.Rows[3].Cells[1].Value = c.NG_Squeeze;
            //_grid.Rows[4].Cells[1].Value = c.NG_FaultyPrint;
            //_grid.Rows[5].Cells[1].Value = c.NG_Crack;

        }
    }
}
