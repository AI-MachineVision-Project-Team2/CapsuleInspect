using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Inspect;
using CapsuleInspect.Setting;
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
namespace CapsuleInspect.Property
{
    public partial class AIModuleProp : UserControl
    {
        //SaigeAI _saigeAI; // SaigeAI 인스턴스
        AIAlgorithm _aiAlgo = null;
        string _modelPath = string.Empty;
        private bool _updateDataGridView = true;
        private readonly int COL_USE = 1;
        private readonly int COL_MIN = 2;
        private readonly int COL_MAX = 3;
        private static readonly string[] _filterOrder = new[] { "Area", "Width", "Height", "Count" };

        //EngineType _engineType;
        public List<BlobFilter> BlobFilters { get; set; } = new List<BlobFilter>();
        private void EnsureAlgorithmFilters()
        {
            if (_aiAlgo == null) return;

            // 이름(소문자) 기준으로 사전화
            var dict = (_aiAlgo.BlobFilters ?? new List<BlobFilter>())
                       .Where(f => f != null && !string.IsNullOrWhiteSpace(f.name))
                       .ToDictionary(f => f.name.Trim().ToLowerInvariant(), f => f);

            var aligned = new List<BlobFilter>(4);
            foreach (var name in _filterOrder)
            {
                var key = name.ToLowerInvariant();
                if (!dict.TryGetValue(key, out var f))
                    f = new BlobFilter { name = name, isUse = false, min = 0, max = 0 };
                aligned.Add(f);
            }

            _aiAlgo.BlobFilters = aligned;   // ★ BlobAlgorithm의 인덱스(FILTER_AREA=0, ...)와 정렬 일치
        }

        private static bool ReadBool(object v, bool def = false)
        {
            try { return v is bool b ? b : Convert.ToBoolean(v); }
            catch { return def; }
        }
        private static int ReadInt(object v, int def = 0)
        {
            if (v == null) return def;
            if (v is int ii) return ii;
            int n; return int.TryParse(v.ToString(), out n) ? n : def;
        }
        public AIModuleProp()
        {
            InitializeComponent();
            InitializeFilterDataGridView();
            SetDefault(); // 생성자에서 초기화
        }
        public void SetDefault()
        {
            BlobFilters.Add(new BlobFilter { name = "Area", isUse = false, min = 200, max = 500 });
            BlobFilters.Add(new BlobFilter { name = "Width", isUse = false, min = 0, max = 0 });
            BlobFilters.Add(new BlobFilter { name = "Height", isUse = false, min = 0, max = 0 });
            BlobFilters.Add(new BlobFilter { name = "Count", isUse = false, min = 0, max = 0 });
        }
        private void InitializeFilterDataGridView()
        {
            // 스타일 설정
            dataGridViewFilter.EnableHeadersVisualStyles = false;
            dataGridViewFilter.ColumnHeadersHeight = 36;
            dataGridViewFilter.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewFilter.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewFilter.GridColor = Color.LightGray;
            dataGridViewFilter.BackgroundColor = Color.White;
            dataGridViewFilter.Font = new Font("Noto Sans KR", 9F, FontStyle.Bold);
            dataGridViewFilter.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewFilter.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Noto Sans KR", 9F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = SystemColors.Highlight,
                SelectionForeColor = SystemColors.HighlightText,
                Padding = new Padding(0, 2, 0, 0)
            };

            dataGridViewFilter.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // 컬럼 설정
            dataGridViewFilter.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "필터명",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 70
            });

            dataGridViewFilter.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText = "사용",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 40
            });

            dataGridViewFilter.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "최소값",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 65
            });

            dataGridViewFilter.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "최대값",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 65
            });

            // 항목 추가
            AddFilterRow("Area", false, 200, 500);
            AddFilterRow("Width", false, 0, 0);
            AddFilterRow("Height", false, 0, 0);
            AddFilterRow("Count", false, 0, 0);

            dataGridViewFilter.AllowUserToAddRows = false;
            dataGridViewFilter.RowHeadersVisible = false;
            dataGridViewFilter.AllowUserToResizeColumns = false;
            dataGridViewFilter.AllowUserToResizeRows = false;
            dataGridViewFilter.AllowUserToOrderColumns = false;
            dataGridViewFilter.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 이벤트 핸들러 추가
            dataGridViewFilter.CurrentCellDirtyStateChanged += dataGridViewFilter_CurrentCellDirtyStateChanged;
            dataGridViewFilter.CellValueChanged += dataGridViewFilter_CellValueChanged;
        }

        // GridView 행 추가 메서드 수정 (초기값 반영)
        private void AddFilterRow(string itemName, bool isUse, int min, int max)
        {
            dataGridViewFilter.Rows.Add(itemName, isUse, min.ToString(), max.ToString());
        }
        // BinaryProp처럼 GridView 값을 BlobFilters에 반영
      
        private void dataGridViewFilter_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewFilter.CurrentCell is DataGridViewCheckBoxCell)
            {
                dataGridViewFilter.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void dataGridViewFilter_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_updateDataGridView == true)
                UpdateDataGridView(false);


        }
       
        public void SetAlgorithm(AIAlgorithm aiAlgo)
        {
            _aiAlgo = aiAlgo;
            if (_aiAlgo == null) return;

            // 알고리즘에 필터가 없으면 기본 생성
            if (_aiAlgo.BlobFilters == null || _aiAlgo.BlobFilters.Count == 0)
                _aiAlgo.SetDefault();

            // ★ 필수: 알고리즘 필터 순서/구성 보정(Area/Width/Height/Count)
            EnsureAlgorithmFilters();

            // 그리드 <- 알고리즘
            SetProperty();
            chkUse.Checked = _aiAlgo.IsUse;
            ApplyUseState(_aiAlgo.IsUse);
        }
        public void SetProperty()
        {
            if (_aiAlgo is null)
                return;

            // ★ 순서 보장 후 그리드 업데이트
            EnsureAlgorithmFilters();
            UpdateDataGridView(true);
            chkUse.Checked = _aiAlgo.IsUse;
            ApplyUseState(_aiAlgo.IsUse);
        }
        //UI컨트롤러 값을 이진화 알고리즘 클래스에 적용
        public void GetProperty()
        {
            if (_aiAlgo is null)
                return;
            _aiAlgo.IsUse = chkUse.Checked;
            //GridView 값을 _blobAlgo에 반영
            UpdateDataGridView(false);
        }
        private void UpdateDataGridView(bool update)
        {
            /*
            if (_aiAlgo is null)
                return;

            if (update)
            {
                _updateDataGridView = false;
                List<BlobFilter> blobFilters = _aiAlgo.BlobFilters;

                for (int i = 0; i < blobFilters.Count; i++)
                {
                    if (i >= dataGridViewFilter.Rows.Count)
                        break;

                    dataGridViewFilter.Rows[i].Cells[COL_USE].Value = blobFilters[i].isUse;
                    dataGridViewFilter.Rows[i].Cells[COL_MIN].Value = blobFilters[i].min;
                    dataGridViewFilter.Rows[i].Cells[COL_MAX].Value = blobFilters[i].max;
                }
                _updateDataGridView = true;
            }
            else
            {
                if (_updateDataGridView == false)
                    return;

                List<BlobFilter> blobFilters = _aiAlgo.BlobFilters;

                for (int i = 0; i < blobFilters.Count; i++)
                {
                    BlobFilter blobFilter = blobFilters[i];
                    blobFilter.isUse = (bool)dataGridViewFilter.Rows[i].Cells[COL_USE].Value;

                    object value = dataGridViewFilter.Rows[i].Cells[COL_MIN].Value;

                    int min = 0;
                    if (value != null && int.TryParse(value.ToString(), out min))
                        blobFilter.min = min;

                    value = dataGridViewFilter.Rows[i].Cells[COL_MAX].Value;

                    int max = 0;
                    if (value != null && int.TryParse(value.ToString(), out max))
                        blobFilter.max = max;
                }
            }*/

            if (_aiAlgo is null)
                return;

            // 항상 먼저 알고리즘 쪽 필터 순서를 보정
            EnsureAlgorithmFilters();
            var blobFilters = _aiAlgo.BlobFilters;

            if (update)
            {
                // 알고리즘 -> 그리드
                _updateDataGridView = false;
                // 그리드가 4행(Area/Width/Height/Count)이라는 전제 하에 값만 채움
                for (int i = 0; i < _filterOrder.Length && i < dataGridViewFilter.Rows.Count; i++)
                {
                    dataGridViewFilter.Rows[i].Cells[COL_USE].Value = blobFilters[i].isUse;
                    dataGridViewFilter.Rows[i].Cells[COL_MIN].Value = blobFilters[i].min;
                    dataGridViewFilter.Rows[i].Cells[COL_MAX].Value = blobFilters[i].max;
                }
                _updateDataGridView = true;
            }
            else
            {
                // 그리드 -> 알고리즘
                if (_updateDataGridView == false)
                    return;

                for (int i = 0; i < _filterOrder.Length && i < dataGridViewFilter.Rows.Count; i++)
                {
                    var row = dataGridViewFilter.Rows[i];
                    var bf = blobFilters[i];

                    bf.isUse = ReadBool(row.Cells[COL_USE].Value, bf.isUse);
                    bf.min = ReadInt(row.Cells[COL_MIN].Value, bf.min);
                    bf.max = ReadInt(row.Cells[COL_MAX].Value, bf.max);
                }

                // (선택) 레퍼런스 유지가 싫다면 아래처럼 재대입
                // _aiAlgo.BlobFilters = blobFilters;
            }
        }
        private void ApplyUseState(bool enabled)
        {
            // 알고리즘 사용 여부 동기화
            if (_aiAlgo != null)
                _aiAlgo.IsUse = enabled;

            // UI 비활성화(체크 해제 시 그리드 필터를 아예 못 건드리게)
            dataGridViewFilter.Enabled = enabled;
        }
        private void chkUse_CheckedChanged(object sender, EventArgs e)
        {
            ApplyUseState(chkUse.Checked);
        }
    }
}