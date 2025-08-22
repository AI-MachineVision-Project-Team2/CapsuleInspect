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
        //EngineType _engineType;
        public List<BlobFilter> BlobFilters { get; set; } = new List<BlobFilter>();
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
            // 이진화 알고리즘 필터값이 없을 경우, 기본값 설정
            if (_aiAlgo.BlobFilters.Count <= 0)
                _aiAlgo.SetDefault();
            SetProperty();
        }
        public void SetProperty()
        {
            if (_aiAlgo is null)
                return;
            UpdateDataGridView(true);
           
        }
        //UI컨트롤러 값을 이진화 알고리즘 클래스에 적용
        public void GetProperty()
        {
            if (_aiAlgo is null)
                return;

            //GridView 값을 _blobAlgo에 반영
            UpdateDataGridView(false);
        }
        private void UpdateDataGridView(bool update)
        {
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
            }
        }
    }
}