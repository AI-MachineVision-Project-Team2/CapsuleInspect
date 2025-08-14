using CapsuleInspect.Algorithm;
using CapsuleInspect.Core;
using CapsuleInspect.Inspect;
using CapsuleInspect.Property2;
using CapsuleInspect.Util;
using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
namespace CapsuleInspect.Property
{
    public enum FilterType
    {
        None,
        HSVscale,
        Flip,
        Pyramid,
        Resize,
        CannyEdge,
        Morphology,
        Rotation
    }
    public partial class ImageFilterProp : UserControl
    {
        private FilterType _selectedFilter;
        public FilterType SelectedFilter => _selectedFilter;
        private FilterAlgorithm _filterAlgo;
        // 필터 적용 시 발생하는 이벤트 (예: PropertiesForm이나 CameraForm에서 구독 가능)
        public event EventHandler FilterApplied;
        public ImageFilterProp()
        {
            InitializeComponent();
            cbFilterType.DataSource = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList();
            cbFilterType.SelectedIndex = 0;
        }

        public void SetAlgorithm(FilterAlgorithm algo)
        {
            _filterAlgo = algo;

            // 알고리즘에 저장된 필터 타입을 콤보에 반영
            var filter = _filterAlgo?.Filter ?? FilterType.None;
            // 콤보박스가 이미 Enum 리스트로 바인딩되어 있으니 그대로 선택만 반영
            cbFilterType.SelectedItem = filter;
        }


        private void OnRotationPreview(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm == null) return;

            if (sender is RotateProp rotateProp)
            {
                int angle = rotateProp.Angle;
                cameraForm.PreviewFilter(FilterType.Rotation, new { Angle = angle });
            }
        }

        private void cbFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbFilterType.SelectedIndex)
            {
                case 0: _selectedFilter = FilterType.None; break;
                case 1: _selectedFilter = FilterType.HSVscale; break;
                case 2: _selectedFilter = FilterType.Flip; break;
                case 3: _selectedFilter = FilterType.Pyramid; break;
                case 4: _selectedFilter = FilterType.Resize; break;
                case 5: _selectedFilter = FilterType.CannyEdge; break;
                case 6: _selectedFilter = FilterType.Morphology; break;
                case 7: _selectedFilter = FilterType.Rotation; break;
                default: _selectedFilter = FilterType.None; break;
            }
            // None, HSVscale은 FilterForm 탭 호출 skip
            if (_selectedFilter == FilterType.None || _selectedFilter == FilterType.HSVscale)
                return;

            var filterForm = MainForm.SharedFilterForm;
            if (filterForm != null)
            {
                filterForm.SelectTab(_selectedFilter.ToString());
                // 회전 필터 선택 시 Preview 이벤트 연결
                if (_selectedFilter == FilterType.Rotation)
                {
                    var tab = filterForm.GetTabPage("Rotation");
                    if (tab != null && tab.Controls[0] is RotateProp rotateProp)
                    {
                        // 중복 연결 방지
                        rotateProp.Preview -= OnRotationPreview;
                        rotateProp.Preview += OnRotationPreview;
                    }
                }
            }
        }
        

        private void checkFilter_CheckedChanged(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SetFilterMode(checkFilter.Checked);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            dynamic options = null;
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm == null) return;

            if (_selectedFilter != FilterType.None)
            {
                switch (_selectedFilter)
                {
                    case FilterType.Flip:
                        {
                            var filterForm = MainForm.SharedFilterForm;
                            if (filterForm != null)
                            {
                                var tab = filterForm.GetTabPage("Flip");
                                if (tab != null && tab.Controls.Count > 0 && tab.Controls[0] is FlipProp flipProp)
                                {
                                    var selected = flipProp.SelectedFlipMode;
                                    if (selected == null)
                                    {
                                        MessageBox.Show("반전 모드를 선택하세요.");
                                        return;
                                    }

                                    options = new { FlipMode = selected.Value };
                                }
                                else
                                {
                                    MessageBox.Show("Flip 설정을 찾을 수 없습니다.");
                                    return;
                                }
                            }
                            break;
                        }

                    case FilterType.Pyramid:
                        {
                            var filterForm = MainForm.SharedFilterForm;
                            if (filterForm != null)
                            {
                                var tab = filterForm.GetTabPage("Pyramid");
                                if (tab != null && tab.Controls.Count > 0 && tab.Controls[0] is PyramidProp pyramidProp)
                                {
                                    string direction = pyramidProp.SelectedDirection;
                                    options = new { Direction = direction };
                                }
                                else
                                {
                                    MessageBox.Show("Pyramid 설정을 찾을 수 없습니다.");
                                    return;
                                }
                            }
                            break;
                        }

                    case FilterType.Resize:
                        {
                            var filterForm = MainForm.SharedFilterForm;
                            var tab = filterForm?.GetTabPage("Resize");
                            if (tab != null && tab.Controls[0] is ResizeProp resizeProp)
                            {
                                double fx = (double)resizeProp.NumericUpDownX.Value;
                                double fy = (double)resizeProp.NumericUpDownY.Value;

                                if (fx <= 0 || fx > 1000 || fy <= 0 || fy > 1000)
                                {
                                    MessageBox.Show("0보다 크고 1000 이하의 값을 입력하세요.");
                                    return;
                                }

                                options = new { Fx = fx, Fy = fy };
                            }
                            else
                            {
                                MessageBox.Show("Resize 설정을 찾을 수 없습니다.");
                                return;
                            }
                            break;
                        }
                    case FilterType.CannyEdge:
                        {
                            var filterForm = MainForm.SharedFilterForm;
                            var tab = filterForm.GetTabPage("CannyEdge");
                            if (tab != null && tab.Controls[0] is CannyEdgeProp cannyEdgeProp)
                            {
                                int min = cannyEdgeProp.Min;
                                int max = cannyEdgeProp.Max;
                                options = new { Min = min, Max = max };
                            }
                            break;
                        }
                    case FilterType.Morphology:
                        {
                            var filterForm = MainForm.SharedFilterForm;
                            var tab = filterForm?.GetTabPage("Morphology");

                            if (tab != null && tab.Controls[0] is MorphologyProp morphProp)
                            {
                                var morphType = morphProp.SelectedMorphType;
                                options = new { MorphType = morphType };
                            }
                            else
                            {
                                MessageBox.Show("Morphology 설정을 찾을 수 없습니다.");
                                return;
                            }
                            break;
                        }
                    case FilterType.Rotation:
                        {
                            var filterForm = MainForm.SharedFilterForm;
                            var tab = filterForm?.GetTabPage("Rotation");

                            if (tab != null && tab.Controls[0] is RotateProp rotateProp)
                            {
                                int angle = rotateProp.Angle;
                                options = new { Angle = angle };
                                rotateProp.Preview -= OnRotationPreview; // 중복 방지
                                rotateProp.Preview += OnRotationPreview;
                            }
                            else
                            {
                                MessageBox.Show("Rotation  설정을 찾을 수 없습니다.");
                                return;
                            }
                            break;
                        }
                }
                // 필터 적용 및 결과 이미지 저장
                Bitmap filteredImage = cameraForm.RunFilterAlgorithm(_selectedFilter, options);
                if (filteredImage != null)
                {
                    //Global.Inst.InspStage.SetFilteredImage(filteredImage); // 필터링된 이미지 저장
                    cameraForm.UpdateDisplay(filteredImage); // CameraForm에 표시
                    SLogger.Write($"필터 적용: {_selectedFilter}", SLogger.LogType.Info);
                }
                else
                {
                    MessageBox.Show("필터 적용 실패", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // None 상태일 때는 현재 이미지를 그대로 유지
                Bitmap currentImage = cameraForm.GetCurrentBitmap();
                if (currentImage != null)
                {
                    //Global.Inst.InspStage.SetFilteredImage(currentImage);
                    cameraForm.UpdateDisplay(currentImage); // 현재 이미지 다시 표시
                    SLogger.Write("현재 이미지 유지 (필터 없음)", SLogger.LogType.Info);
                }
                else
                {
                    MessageBox.Show("이미지를 찾을 수 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.Redo();
        }

        private void btnSrc_Click(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.RestoreOriginal();
        }

    }
}
