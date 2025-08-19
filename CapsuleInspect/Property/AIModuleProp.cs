using CapsuleInspect.Inspect;
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
using CapsuleInspect.Core;
namespace CapsuleInspect.Property
{
    public partial class AIModuleProp : UserControl
    {
        SaigeAI _saigeAI; // SaigeAI 인스턴스
        string _modelPath = string.Empty;
        //EngineType _engineType;

        public AIModuleProp()
        {
            InitializeComponent();
            InitializeFilterDataGridView();
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
            AddFilterRow("Area");
            AddFilterRow("Length");
            AddFilterRow("Width");
            AddFilterRow("Count");

            dataGridViewFilter.AllowUserToAddRows = false;
            dataGridViewFilter.RowHeadersVisible = false;
            dataGridViewFilter.AllowUserToResizeColumns = false;
            dataGridViewFilter.AllowUserToResizeRows = false;
            dataGridViewFilter.AllowUserToOrderColumns = false;
            dataGridViewFilter.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 이벤트 핸들러 추가
            dataGridViewFilter.CurrentCellDirtyStateChanged += dataGridViewFilter_CurrentCellDirtyStateChanged;
        }

        private void AddFilterRow(string itemName)
        {
            dataGridViewFilter.Rows.Add(itemName, false, "", "");
        }

        private void dataGridViewFilter_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewFilter.CurrentCell is DataGridViewCheckBoxCell)
            {
                dataGridViewFilter.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
            MessageBox.Show("모델이 성공적으로 로드되었습니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnInspAI_Click(object sender, EventArgs e)
        {
            if (_saigeAI == null)
            {
                MessageBox.Show("AI 모듈이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = Global.Inst.InspStage.GetBitmap();

            if (bitmap is null)
            {
                MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _saigeAI.Inspect(bitmap);

            Bitmap resultImage = _saigeAI.GetResultImage();

            Global.Inst.InspStage.UpdateDisplay(resultImage);
        }
    }
}