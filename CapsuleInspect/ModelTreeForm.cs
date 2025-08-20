using CapsuleInspect.Core;
using CapsuleInspect.Teach;
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
using static OpenCvSharp.ML.DTrees;
namespace CapsuleInspect
{
    public partial class ModelTreeForm : DockContent
    {
        //개별 트리 노트에서 팝업 메뉴 보이기를 위한 메뉴
        private ContextMenuStrip _contextMenu;

        public ModelTreeForm()
        {
            InitializeComponent();

            tvModelTree.CheckBoxes = true;
            //추가한거
            tvModelTree.AfterCheck += tvModelTree_AfterCheck;
            //추가한거

            //초기 트리 노트의 기본값은 "Root", 체크되지 않은 상태로 초기화
            TreeNode rootNode = tvModelTree.Nodes.Add("Root");
            rootNode.Checked = false;


            // 컨텍스트 메뉴 초기화
            _contextMenu = new ContextMenuStrip();

            List<InspWindowType> windowTypeList;
            windowTypeList = new List<InspWindowType> { InspWindowType.Crack, InspWindowType.Scratch, 
                InspWindowType.Squeeze,InspWindowType.PrintDefect, InspWindowType.ID };

            foreach (InspWindowType windowType in windowTypeList)
                _contextMenu.Items.Add(new ToolStripMenuItem(windowType.ToString(), null, AddNode_Click) { Tag = windowType });

        }

        private void tvModelTree_MouseDown(object sender, MouseEventArgs e)
        {
            //Root 노드에서 마우스 오른쪽 버튼 클릭 시에, 팝업 메뉴 생성
            if (e.Button == MouseButtons.Right)
            {
                TreeNode clickedNode = tvModelTree.GetNodeAt(e.X, e.Y);
                if (clickedNode != null && clickedNode.Text == "Root")
                {
                    tvModelTree.SelectedNode = clickedNode;
                    _contextMenu.Show(tvModelTree, e.Location);
                }
            }
        }
        //팝업 메뉴에서, 메뉴 선택시 실행되는 함수
        private void AddNode_Click(object sender, EventArgs e)
        {
            if (tvModelTree.SelectedNode != null & sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                InspWindowType windowType = (InspWindowType)menuItem.Tag;
                AddNewROI(windowType);
            }
        }

        //imageViewer에 ROI 추가 기능 실행
        private void AddNewROI(InspWindowType inspWindowType)
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.AddRoi(inspWindowType);
            }
        }

        //현재 모델 전체의 ROI를 트리 모델에 업데이트
        public void UpdateDiagramEntity()
        {
            // 트리 갱신 중 깜빡임/이벤트 폭주 방지  //추가한거
            tvModelTree.BeginUpdate();                         //추가한거
            tvModelTree.AfterCheck -= tvModelTree_AfterCheck;  //추가한거

            try                                                //추가한거
            {
                tvModelTree.Nodes.Clear();
                TreeNode rootNode = tvModelTree.Nodes.Add("Root");
                rootNode.Checked = false;
                // 널 가드                                        //추가한거
                if (Global.Inst == null || Global.Inst.InspStage == null)
                    return;                                     //추가한거

                Model model = Global.Inst.InspStage.CurModel;
                if (model == null || model.InspWindowList == null)  //추가한거
                    return;                                         //추가한거

                List<InspWindow> windowList = model.InspWindowList;
                if (windowList.Count <= 0)
                    return;

                // UID 중복 대비를 위한 로컬 카운터(선택)               //추가한거
                var seen = new Dictionary<string, int>();             //추가한거

                foreach (InspWindow window in windowList)
                {
                    if (window == null)
                        continue;

                    // 표시에 쓸 라벨: UID 없으면 Name, 그것도 없으면 플레이스홀더  //추가한거
                    string baseLabel = window.UID ?? window.Name ?? "(ROI)";       //추가한거
                    string label = baseLabel;                                      //추가한거
                    if (seen.TryGetValue(baseLabel, out int n))                    //추가한거
                    {                                                              //추가한거
                        n++; seen[baseLabel] = n;                                   //추가한거
                        label = $"{baseLabel} ({n})";                               //추가한거
                    }                                                              //추가한거
                    else seen[baseLabel] = 1;                                      //추가한거

                    TreeNode node = new TreeNode(label)
                    {
                        Tag = window,         // ★ ROI 객체 연결
                                              // 현재 상태 반영: 검사 제외면 체크 해제(=숨김), 검사 대상이면 체크  //추가한거
                        Checked = !(window.IgnoreInsp)                              //추가한거
                    };
                    rootNode.Nodes.Add(node);
                }

                tvModelTree.ExpandAll();
            }
            finally                                            //추가한거
            {
                // 이벤트 재연결 + 업데이트 종료                   //추가한거
                tvModelTree.AfterCheck += tvModelTree_AfterCheck; //추가한거
                tvModelTree.EndUpdate();                          //추가한거
            }
        }
        private void tvModelTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            tvModelTree.AfterCheck -= tvModelTree_AfterCheck; // 재귀 방지
            try
            {
                // Root 노드 체크 시 하위 노드 모두 체크/해제
                if (e.Node.Text == "Root")
                {
                    foreach (TreeNode childNode in e.Node.Nodes)
                    {
                        childNode.Checked = e.Node.Checked;
                        var win = childNode.Tag as InspWindow;
                        if (win != null)
                        {
                            var viewerForm = MainForm.GetDockForm<CameraForm>();
                            if (viewerForm != null)
                            {
                                viewerForm.SetWindowVisible(win, childNode.Checked);
                            }
                            win.IgnoreInsp = !childNode.Checked;
                        }
                    }
                }
                // 하위 노드 체크 시 개별 처리
                else
                {
                    var win = e.Node.Tag as InspWindow;
                    if (win != null)
                    {
                        var viewerForm = MainForm.GetDockForm<CameraForm>();
                        if (viewerForm != null)
                        {
                            viewerForm.SetWindowVisible(win, e.Node.Checked);
                        }
                        win.IgnoreInsp = !e.Node.Checked;
                    }
                }
            }
            finally
            {
                tvModelTree.AfterCheck += tvModelTree_AfterCheck; // 이벤트 재연결
            }
        }
    }
}
