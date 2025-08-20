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
        CannyEdge,
        Morphology,
    }
    public partial class ImageFilterProp : UserControl
    {
        private FilterType _selectedFilter;
        public FilterType SelectedFilter => _selectedFilter;
        private FilterAlgorithm _filterAlgo;
        // 필터 적용 시 발생하는 이벤트 (예: PropertiesForm이나 CameraForm에서 구독 가능)
       
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
            Console.WriteLine($"SetAlgorithm: FilterType={filter}");
        }


        private void cbFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbFilterType.SelectedIndex)
            {
                case 0: _selectedFilter = FilterType.None; break;
                case 1: _selectedFilter = FilterType.CannyEdge; break;
                case 2: _selectedFilter = FilterType.Morphology; break;
                default: _selectedFilter = FilterType.None; break;
            }
            // None FilterForm 탭 호출 skip
            if (_selectedFilter == FilterType.None)
                return;

            var filterForm = MainForm.SharedFilterForm;
            if (filterForm != null)
            {
                filterForm.SelectTab(_selectedFilter.ToString());
            }
        }
        

        //private void btnApply_Click(object sender, EventArgs e)
        //{
        //    dynamic options = null;
        //    var cameraForm = MainForm.GetDockForm<CameraForm>();
        //    if (cameraForm == null) return;

        //    if (_selectedFilter != FilterType.None)
        //    {
        //        switch (_selectedFilter)
        //        {
        //            case FilterType.CannyEdge:
        //                {
        //                    var filterForm = MainForm.SharedFilterForm;
        //                    var tab = filterForm.GetTabPage("CannyEdge");
        //                    if (tab != null && tab.Controls[0] is CannyEdgeProp cannyEdgeProp)
        //                    {
        //                        int min = cannyEdgeProp.Min;
        //                        int max = cannyEdgeProp.Max;
        //                        options = new { Min = min, Max = max };

        //                        var curWindow = Global.Inst.InspStage.PreView?.CurrentInspWindow;
        //                        if (curWindow != null)
        //                        {
        //                            var blob = curWindow.FindInspAlgorithm(InspectType.InspBinary) as BlobAlgorithm;
        //                            if (blob != null)
        //                            {
        //                                blob.Filter = FilterType.CannyEdge;
        //                                blob.FilterOptions = options;
        //                                SLogger.Write($"btnApply: BlobAlgorithm 업데이트 (CannyEdge, Min={min}, Max={max})", SLogger.LogType.Info);
        //                            }
        //                        }

        //                        // Canny 적용 및 _previewImage 업데이트
        //                        var preview = Global.Inst.InspStage.PreView;
        //                        if (preview != null)
        //                        {
        //                            preview.SetCannyPreview(min, max);
        //                            Mat currentPreview = cameraForm.GetDisplayImage();
        //                            preview.UpdatePreviewImage(currentPreview);
        //                            SLogger.Write("btnApply: _previewImage를 Canny 결과로 업데이트", SLogger.LogType.Info);
        //                        }
        //                        else
        //                        {
        //                            SLogger.Write("btnApply: Preview 객체 null", SLogger.LogType.Error);
        //                        }

        //                        SLogger.Write($"Canny Edge 적용: Min={min}, Max={max}", SLogger.LogType.Info);
        //                    }
        //                    else
        //                    {
        //                        SLogger.Write("btnApply: CannyEdgeProp 탭 또는 컨트롤 null", SLogger.LogType.Error);
        //                    }
        //                    break;
        //                }
        //            case FilterType.Morphology:
        //                {
        //                    var filterForm = MainForm.SharedFilterForm;
        //                    var tab = filterForm?.GetTabPage("Morphology");

        //                    if (tab != null && tab.Controls[0] is MorphologyProp morphProp)
        //                    {
        //                        var morphType = morphProp.SelectedMorphType;
        //                        var kernelSize = morphProp.KernelSize;
                            
        //                        options = new { MorphType = morphType, KernelSize = kernelSize };
        //                        SLogger.Write($"options 설정: MorphType={morphType}, KernelSize={kernelSize}");
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("Morphology 설정을 찾을 수 없습니다.");
        //                        return;
        //                    }
        //                    break;
        //                } 
        //        }
        //        SLogger.Write($"필터 적용: {_selectedFilter}", SLogger.LogType.Info);
        //    }
        //    else
        //    {

        //        SLogger.Write("현재 이미지 유지 (필터 없음)", SLogger.LogType.Info);
        //    }
        //}

        private void btnSrc_Click(object sender, EventArgs e)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.RestoreOriginal();
        }

    }
}
