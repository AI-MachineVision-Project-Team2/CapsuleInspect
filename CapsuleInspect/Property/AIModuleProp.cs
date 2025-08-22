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
        SaigeAI _saigeAI; // SaigeAI 인스턴스
        string _modelPath = string.Empty;
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
        private void GetProperty()
        {
            for (int i = 0; i < dataGridViewFilter.Rows.Count; i++)
            {
                var row = dataGridViewFilter.Rows[i];
                var filter = BlobFilters[i];
                filter.isUse = (bool)row.Cells[1].Value;// 사용 여부
                // min/max 파싱 (빈 값 처리: 0으로 fallback)
                filter.min = string.IsNullOrEmpty(row.Cells[2].Value?.ToString()) ? 0 : int.Parse(row.Cells[2].Value.ToString());
                filter.max = string.IsNullOrEmpty(row.Cells[3].Value?.ToString()) ? 0 : int.Parse(row.Cells[3].Value.ToString());
            }
        }
        private void dataGridViewFilter_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewFilter.CurrentCell is DataGridViewCheckBoxCell)
            {
                dataGridViewFilter.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void dataGridViewFilter_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GetProperty(); // 값 변경 시 즉시 반영
            // SaigeAI에 필터 즉시 적용
            if (_saigeAI != null)
            {
                _saigeAI.Filters = new List<BlobFilter>(BlobFilters.Select(f => new BlobFilter
                {
                    name = f.name,
                    isUse = f.isUse,
                    min = f.min,
                    max = f.max
                })); // 깊은 복사
            }
        }
        private void btnSelAIModel_Click(object sender, EventArgs e)
        {
            string filter = "Segmentation Files|*.saigeseg;";
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
                        openFileDialog.InitialDirectory = @"C:\model";
                }
                catch (Exception)
                {
                    openFileDialog.InitialDirectory = @"C:\model";
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
            _modelPath = SettingXml.Inst.AIModelPath;

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
            _saigeAI.Filters = new List<BlobFilter>(BlobFilters.Select(f => new BlobFilter
            {
                name = f.name,
                isUse = f.isUse,
                min = f.min,
                max = f.max
            })); // 깊은 복사
            MessageBox.Show("모델이 성공적으로 로드되었습니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}