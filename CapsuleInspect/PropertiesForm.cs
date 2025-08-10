﻿using CapsuleInspect.Property;
using CapsuleInspect.Core;
using CapsuleInspect.Algorithm;
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
    public enum PropertyType
    {
        Binary,
        Filter,
        AIModule
    }

    public partial class PropertiesForm : DockContent
    {
        // 속성탭을 관리하기 위한 딕셔너리
        Dictionary<string, TabPage> _allTabs = new Dictionary<string, TabPage>();
        public PropertiesForm()
        {
            InitializeComponent();
            // 속성 탭을 초기화
            LoadOptionControl(PropertyType.Binary);
            LoadOptionControl(PropertyType.Filter);
            LoadOptionControl(PropertyType.AIModule);
            tabPropControl.SelectedIndex = 0; // 첫 번째 탭 선택
        }
        private void LoadOptionControl(PropertyType propType)
        {
            string tabName = propType.ToString();

            // 이미 있는 TabPage인지 확인
            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Text == tabName)
                    return;
            }

            // 딕셔너리에 있으면 추가
            if (_allTabs.TryGetValue(tabName, out TabPage page))
            {
                tabPropControl.TabPages.Add(page);
                return;
            }

            // 새로운 UserControl 생성
            UserControl _inspProp = CreateUserControl(propType);
            if (_inspProp == null)
                return;

            // 새 탭 추가
            TabPage newTab = new TabPage(tabName)
            {
                Dock = DockStyle.Fill
            };
            _inspProp.Dock = DockStyle.Fill;
            newTab.Controls.Add(_inspProp);
            tabPropControl.TabPages.Add(newTab);
            tabPropControl.SelectedTab = newTab; // 새 탭 선택

            _allTabs[tabName] = newTab;
        }

        // 속성 탭을 생성하는 메서드
        private UserControl CreateUserControl(PropertyType propType)
        {
            UserControl curProp = null;
            switch (propType)
            {
                case PropertyType.Binary:
                    BinaryProp blobProp = new BinaryProp();
                    //이진화 속성 변경시 발생하는 이벤트 추가
                    blobProp.RangeChanged += RangeSlider_RangeChanged;
                    blobProp.PropertyChanged += PropertyChanged;

                    curProp = blobProp;
                    break;
                case PropertyType.Filter:
                    ImageFilterProp filterProp = new ImageFilterProp();
                    curProp = filterProp;
                    break;
                case PropertyType.AIModule:
                    AIModuleProp aiModuleProp = new AIModuleProp();
                    curProp = aiModuleProp;
                    break;
                default:
                    MessageBox.Show("유효하지 않은 옵션입니다.");
                    return null;
            }
            return curProp;
        }

        public void UpdateProperty(BlobAlgorithm blobAlgorithm)
        {
            if (blobAlgorithm is null)
                return;

            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Controls.Count > 0)
                {
                    UserControl uc = tabPage.Controls[0] as UserControl;

                    if (uc is BinaryProp binaryProp)
                    {
                        binaryProp.SetAlgorithm(blobAlgorithm);
                    }
                }
            }
        }

        // 이진화 속성 변경시 발생하는 이벤트 구현
        private void RangeSlider_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            // 속성값을 이용하여 이진화 임계값 설정
            int lowerValue = e.LowerValue;
            int upperValue = e.UpperValue;
            bool invert = e.Invert;
            ShowBinaryMode showBinMode = e.ShowBinMode;
            Global.Inst.InspStage.PreView?.SetBinary(lowerValue, upperValue, invert, showBinMode);
        }

        private void PropertyChanged(object sender, EventArgs e)
        {
            Global.Inst.InspStage.RedrawMainView();
        }
    }
}
