using BrightIdeasSoftware;
using CapsuleInspect.Core;
using CapsuleInspect.Inspect;
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

namespace CapsuleInspect
{
    public partial class ResultForm : DockContent
    {
        private StatusStrip _status;                 // ← 추가
        private ToolStripStatusLabel _tsslDistinct;  // ← 추가

        //검사 결과를 보여주기 위한 컨트롤 추가
        private SplitContainer _splitContainer;
        private TreeListView _treeListView;
        private TextBox _txtDetails;

        public ResultForm()
        {
            InitializeComponent();
            //컨트롤 초기화, 아래 함수 구현할것
            InitTreeListView();
            // (A) 상태바(하단) 만들기
            _status = new StatusStrip() { SizingGrip = false };
            _tsslDistinct = new ToolStripStatusLabel("NG (종류별 1): 0");
            _status.Items.Add(_tsslDistinct);

            // 상태바는 Panel1(트리 영역) 아래에 붙임
            _splitContainer.Panel1.Controls.Add(_status);
            _status.Dock = DockStyle.Bottom;

            // (B) 이벤트 구독 (검사 사이클 끝에 값이 올라옴)
            Global.Inst.InspStage.DistinctNgCountUpdated += OnDistinctNgUpdated;

            // 폼 닫힐 때 구독 해제 (메모리 누수 방지)
            this.FormClosed += (s, e) =>
            {
                Global.Inst.InspStage.DistinctNgCountUpdated -= OnDistinctNgUpdated;
            };
        }
        private void InitTreeListView()
        {
            // SplitContainer 사용하여 상하 분할 레이아웃 구성
            _splitContainer = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 120,
                Panel1MinSize = 70,
                Panel2MinSize = 70,
                Cursor = Cursors.Hand,
            };

            //TreeListView 검사 결과 트리 생성
            _treeListView = new TreeListView()
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                ShowGroups = false,
                UseFiltering = true,
                OwnerDraw = true,
                MultiSelect = false,
                GridLines = true,
                Cursor = Cursors.Hand, // 커서 변경
                
            };
            _treeListView.SelectionChanged += TreeListView_SelectionChanged;
            _treeListView.MouseEnter += (s, e) =>
            {
                _treeListView.Cursor = Cursors.Hand;
            };

            _treeListView.MouseMove += (s, e) =>
            {
                if (_treeListView.Cursor != Cursors.Hand)
                    _treeListView.Cursor = Cursors.Hand;
            };

            _treeListView.CanExpandGetter = x => true;

            _treeListView.ChildrenGetter = x =>
            {
                if (x is InspWindow w)
                    return w.InspResultList;
                return new List<InspResult>();
            };

            //컬럼 추가
            var colUID = new OLVColumn("UID", "")
            {
                Width = 100,
                IsEditable = false,
                AspectGetter = obj =>
                {
                    if (obj is InspWindow win)
                        return win.UID;
                    if (obj is InspResult res)
                        return res.InspType.ToString();
                    return "";
                }
            };

            var colAlgo = new OLVColumn("Algorithm", "")
            {
                Width = 150,
                IsEditable = false,
                AspectGetter = obj =>
                {
                    if (obj is InspResult res)
                        return res.InspType.ToString();
                    return "";
                }
            };

            var colStatus = new OLVColumn("Status", "IsDefect")
            {
                Width = 80,
                TextAlign = HorizontalAlignment.Center,
                AspectGetter = obj =>
                {
                    if (obj is InspResult res)
                        return res.IsDefect ? "NG" : "OK";
                    return "";
                }
            };

            var colValue = new OLVColumn("Result", "Result")
            {
                Width = 80,
                TextAlign = HorizontalAlignment.Center,
                AspectGetter = obj =>
                {
                    if (obj is InspResult res)
                        return res.ResultValue;
                    return "";
                }
            };

            // 컬럼 추가
            _treeListView.Columns.AddRange(new OLVColumn[] { colUID, colAlgo, colStatus, colValue });


            // 검사 상세 정보 텍스트박스 생성
            _txtDetails = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand,

                ReadOnly = true
            };

            // 컨테이너에 컨트롤 추가
            _splitContainer.Panel1.Controls.Add(_treeListView);
            _splitContainer.Panel2.Controls.Add(_txtDetails);
            Controls.Add(_splitContainer);
        }

        public void AddModelResult(Model curModel)
        {
            if (curModel is null)
                return;

            _treeListView.SetObjects(curModel.InspWindowList);

            foreach (var window in curModel.InspWindowList)
            {
                _treeListView.Expand(window);
            }
        }

        public void AddWindowResult(InspWindow inspWindow)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action(() => AddWindowResult(inspWindow))); return; }

            if (inspWindow is null)
                return;

            _treeListView.SetObjects(new List<InspWindow> { inspWindow });
            _treeListView.Expand(inspWindow);

            if (inspWindow.InspResultList.Count > 0)
            {
                InspResult inspResult = inspWindow.InspResultList[0];
                ShowDetail(inspResult);
            }
        }

        //실제 검사가 되었을때, 검사 결과를 추가하는 함수
        public void AddInspResult(InspResult inspResult)
        {
            if (inspResult is null)
                return;

            // 현재 트리에 있는 객체 리스트 가져오기
            var existingResults = _treeListView.Objects as List<InspResult>;

            if (existingResults == null)
                existingResults = new List<InspResult>();

            // 기존 검사 결과에서 같은 BodyID를 가진 부모 찾기
            var parentResult = existingResults.FirstOrDefault(r => r.GroupID == inspResult.GroupID);

            existingResults.Add(inspResult);

            // TreeListView 업데이트
            _treeListView.SetObjects(existingResults);
        }

        //해당 트리 리스트 뷰 선택시, 상세 정보 텍스트 박스에 표시
        private void TreeListView_SelectionChanged(object sender, EventArgs e)
        {
            if (_treeListView.SelectedObject == null)
            {
                _txtDetails.Text = string.Empty;
                return;
            }

            if (_treeListView.SelectedObject is InspResult result)
            {
                ShowDetail(result);
            }
            else if (_treeListView.SelectedObject is InspWindow window)
            {
                var infos = window.InspResultList.Select(r => $" -{r.ObjectID}: {r.ResultInfos}").ToList();
                _txtDetails.Text = $"{window.UID}\r\n" +
                    string.Join("\r\n", infos);
            }
        }

        private void ShowDetail(InspResult result)
        {
            if (result is null)
                return;

            StringBuilder sb = new StringBuilder();
            if (result.ResultRectList != null)
            {
                foreach (var info in result.ResultRectList)
                {
                    sb.AppendLine($"{info.inspectType}: {info.info} (Defect)");
                }
            }
            _txtDetails.Text = sb.ToString();

            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null && result.ResultRectList != null)
            {
                cameraForm.AddRect(result.ResultRectList); // Defect 및 Count 필터 Defect 표시
            }
        }
        private void OnDistinctNgUpdated(int count)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => OnDistinctNgUpdated(count))); return; }
            _tsslDistinct.Text = $"NG (종류별 1): {count}";
        }
    }
}
