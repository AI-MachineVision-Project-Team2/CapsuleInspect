using CapsuleInspect.Core;
using CapsuleInspect.Setting;
using Common.Util.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CapsuleInspect.Teach
{
    public class Model
    {
        public bool Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.ModelPath) ||
                    string.IsNullOrWhiteSpace(this.ModelName))
                    return false;

                string modelXmlPath = Path.GetFullPath(this.ModelPath);
                string modelRootDir = Path.GetDirectoryName(modelXmlPath);
                string imagesDir = Path.Combine(modelRootDir, "Images");

                // 폴더 보장
                Directory.CreateDirectory(modelRootDir);
                Directory.CreateDirectory(imagesDir);

                // 모델(XML) 저장
                XmlHelper.SaveXml(modelXmlPath, this);

                // 윈도우별 부가 리소스(패턴/템플릿) 저장
                if (this.InspWindowList != null)
                {
                    foreach (var win in this.InspWindowList)
                    {
                        if (win == null) continue;
                        // 프로젝트에 구현된 시그니처에 맞춰 저장
                        // 보통 모델 루트나 Images 폴더 경로를 넘깁니다.
                        win.SaveInspWindow(this);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //모델 정보 저장을 위해 추가한 프로퍼티
        public string ModelName { get; set; } = "";
        public string ModelInfo { get; set; } = "";
        public string ModelPath { get; set; } = "";

        public string InspectImagePath { get; set; } = "";

        [XmlElement("InspWindow")]
        public List<InspWindow> InspWindowList { get; set; }

        public Model()
        {
            InspWindowList = new List<InspWindow>();
        }

        public InspWindow AddInspWindow(InspWindowType windowType)
        {
            InspWindow inspWindow = InspWindowFactory.Inst.Create(windowType);
            InspWindowList.Add(inspWindow);

            return inspWindow;
        }

        public bool AddInspWindow(InspWindow inspWindow)
        {
            if (inspWindow is null)
                return false;

            InspWindowList.Add(inspWindow);

            return true;
        }

        public bool DelInspWindow(InspWindow inspWindow)
        {
            if (InspWindowList.Contains(inspWindow))
            {
                InspWindowList.Remove(inspWindow);
                return true;
            }
            return false;
        }

        public bool DelInspWindowList(List<InspWindow> inspWindowList)
        {
            int before = InspWindowList.Count;
            InspWindowList.RemoveAll(w => inspWindowList.Contains(w));
            return InspWindowList.Count < before;
        }

        //신규 모델 생성
        public void CreateModel(string path, string modelName, string modelInfo)
        {
            ModelPath = path;
            ModelName = modelName;
            ModelInfo = modelInfo;
        }


        //모델 파일 Load,Save,SaveAs
        //모델 로딩함수
        public Model Load(string path)
        {
            Model model = XmlHelper.LoadXml<Model>(path);
            if (model == null)
                return null;
            ModelPath = path;
            foreach (var window in model.InspWindowList)
            {
                window.LoadInspWindow(model);
            }

            return model;
        }

        //모델 저장함수
        public bool SaveAs()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = SettingXml.Inst.ModelDir;
                sfd.Title = "모델 파일 저장";
                sfd.Filter = "Model Files (*.xml)|*.xml";
                sfd.DefaultExt = "xml";
                sfd.AddExtension = true;
                sfd.FileName = string.IsNullOrWhiteSpace(this.ModelName) ? "" : this.ModelName;

                if (sfd.ShowDialog() == DialogResult.OK)
                    return SaveAs(sfd.FileName);   // ▶️ 아래 오버로드로 연결

                return false;
            }
        }


        //모델 다른 이름으로 저장함수
        // 실제 저장 로직 (경로 인자로 받음)
        public bool SaveAs(string selectedPath)
        {
            if (string.IsNullOrWhiteSpace(selectedPath))
                return false;

            // 경로 정규화
            string full = Path.GetFullPath(selectedPath.Trim());

            // 사용자가 ".xml"을 붙여도 폴더명은 파일명(확장자 제외)로
            string pickedDir = Path.GetDirectoryName(full);
            string pickedName = Path.GetFileNameWithoutExtension(full);

            // ...\{pickedName}\{pickedName}.xml + Images 구조
            string targetRootDir = Path.Combine(pickedDir, pickedName);
            string targetXmlPath = Path.Combine(targetRootDir, pickedName + ".xml");
            string imagesDir = Path.Combine(targetRootDir, "Images");

            Directory.CreateDirectory(targetRootDir);
            Directory.CreateDirectory(imagesDir);

            // 모델 메타 갱신 (프로퍼티명은 프로젝트에 맞게)
            this.ModelName = pickedName;
            this.ModelPath = targetXmlPath;  // 파일 경로
            this.InspectImagePath = imagesDir;

            return this.Save(); // 기존 Save() 내부에서 XmlHelper.SaveXml 호출
        }

    }
}
