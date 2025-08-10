using CapsuleInspect.Property;
using CapsuleInspect.Property2;
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
    public partial class FilterForm : DockContent
    {
        Dictionary<string, TabPage> _allTabs = new Dictionary<string, TabPage>();

        public FilterForm()
        {
            InitializeComponent();
            LoadOptionControl(FilterType.Flip);
            LoadOptionControl(FilterType.Resize);
            LoadOptionControl(FilterType.Pyramid);
            LoadOptionControl(FilterType.CannyEdge);
            LoadOptionControl(FilterType.Morphology);
            LoadOptionControl(FilterType.Rotation);
        }
        private void LoadOptionControl(FilterType filterType)
        {
            string tabName = filterType.ToString();

            // 이미 있는 TabPage인지 확인
            foreach (TabPage tabPage in tabPropControl2.TabPages)
            {
                if (tabPage.Text == tabName)
                    return;
            }

            // 딕셔너리에 있으면 추가
            if (_allTabs.TryGetValue(tabName, out TabPage page))
            {
                tabPropControl2.TabPages.Add(page);
                return;
            }
            // 새로운 UserControl 생성
            UserControl _inspProp = CreateUserControl(filterType);
            if (_inspProp == null)
                return;

            // 새 탭 추가
            TabPage newTab = new TabPage(tabName)
            {
                Name = tabName,
                Dock = DockStyle.Fill
            };
            _inspProp.Dock = DockStyle.Fill;
            newTab.Controls.Add(_inspProp);
            tabPropControl2.TabPages.Add(newTab);
            tabPropControl2.SelectedTab = newTab; // 새 탭 선택

            _allTabs[tabName] = newTab;
        }
        public TabPage GetTabPage(string tabName)
        {
            foreach (TabPage tab in tabPropControl2.TabPages)
            {
                if (tab.Text == tabName)
                    return tab;
            }
            return null;
        }
        private UserControl CreateUserControl(FilterType filterType)
        {
            UserControl curProp = null;

            switch (filterType)
            {
                case FilterType.None:
                    return null; // None은 탭 생성 skip
                case FilterType.Flip:
                    curProp = new FlipProp(); // 클래스명 오타 수정 주의!
                    break;

                case FilterType.Resize:
                    curProp = new ResizeProp();
                    break;

                case FilterType.CannyEdge:
                    curProp = new CannyEdgeProp();
                    break;

                case FilterType.Morphology:
                    curProp = new MorphologyProp();
                    break;

                case FilterType.Pyramid:
                    curProp = new PyramidProp();
                    break;

                case FilterType.Rotation:
                    curProp = new RotateProp();
                    break;

                default:
                    MessageBox.Show("유효하지 않은 옵션입니다."); // 주석 처리하여 조용히 실패
                    return null;
            }

            return curProp;
        }
        public void SelectTab(string tabName)
        {
            foreach (TabPage tab in tabPropControl2.TabPages)
            {
                if (tab.Text == tabName || tab.Name == tabName)
                {
                    tabPropControl2.SelectedTab = tab;
                    return;
                }
            }

            // 못 찾았을 경우 시도적으로 추가
            if (Enum.TryParse(tabName, out FilterType type))
            {
                LoadOptionControl(type);
                SelectTab(tabName); // 재귀 호출로 다시 선택
            }
            else
            {
                MessageBox.Show($"'{tabName}' 탭을 찾을 수 없습니다.");
            }
        }
    }
}
