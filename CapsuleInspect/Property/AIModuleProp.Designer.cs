namespace CapsuleInspect.Property
{
    partial class AIModuleProp
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbAIEngineType = new System.Windows.Forms.ComboBox();
            this.btnInspAI = new System.Windows.Forms.Button();
            this.btnLoadModel = new System.Windows.Forms.Button();
            this.btnSelAIModel = new System.Windows.Forms.Button();
            this.txtAIModelPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbAIEngineType
            // 
            this.cbAIEngineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAIEngineType.FormattingEnabled = true;
            this.cbAIEngineType.Items.AddRange(new object[] {
            "IAD",
            "DET",
            "SEG"});
            this.cbAIEngineType.Location = new System.Drawing.Point(8, 41);
            this.cbAIEngineType.Margin = new System.Windows.Forms.Padding(2);
            this.cbAIEngineType.Name = "cbAIEngineType";
            this.cbAIEngineType.Size = new System.Drawing.Size(210, 20);
            this.cbAIEngineType.TabIndex = 17;
            this.cbAIEngineType.SelectedIndexChanged += new System.EventHandler(this.cbAIEngineType_SelectedIndexChanged);
            // 
            // btnInspAI
            // 
            this.btnInspAI.Location = new System.Drawing.Point(164, 69);
            this.btnInspAI.Name = "btnInspAI";
            this.btnInspAI.Size = new System.Drawing.Size(53, 28);
            this.btnInspAI.TabIndex = 16;
            this.btnInspAI.Text = "AI 검사";
            this.btnInspAI.UseVisualStyleBackColor = true;
            this.btnInspAI.Click += new System.EventHandler(this.btnInspAI_Click);
            // 
            // btnLoadModel
            // 
            this.btnLoadModel.Location = new System.Drawing.Point(91, 69);
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(65, 29);
            this.btnLoadModel.TabIndex = 15;
            this.btnLoadModel.Text = "모델 로딩";
            this.btnLoadModel.UseVisualStyleBackColor = true;
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            // 
            // btnSelAIModel
            // 
            this.btnSelAIModel.Location = new System.Drawing.Point(8, 69);
            this.btnSelAIModel.Name = "btnSelAIModel";
            this.btnSelAIModel.Size = new System.Drawing.Size(76, 28);
            this.btnSelAIModel.TabIndex = 14;
            this.btnSelAIModel.Text = "AI모델 선택";
            this.btnSelAIModel.UseVisualStyleBackColor = true;
            this.btnSelAIModel.Click += new System.EventHandler(this.btnSelAIModel_Click);
            // 
            // txtAIModelPath
            // 
            this.txtAIModelPath.Location = new System.Drawing.Point(8, 10);
            this.txtAIModelPath.Name = "txtAIModelPath";
            this.txtAIModelPath.ReadOnly = true;
            this.txtAIModelPath.Size = new System.Drawing.Size(210, 21);
            this.txtAIModelPath.TabIndex = 13;
            // 
            // AIModuleProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbAIEngineType);
            this.Controls.Add(this.btnInspAI);
            this.Controls.Add(this.btnLoadModel);
            this.Controls.Add(this.btnSelAIModel);
            this.Controls.Add(this.txtAIModelPath);
            this.Name = "AIModuleProp";
            this.Size = new System.Drawing.Size(435, 235);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAIEngineType;
        private System.Windows.Forms.Button btnInspAI;
        private System.Windows.Forms.Button btnLoadModel;
        private System.Windows.Forms.Button btnSelAIModel;
        private System.Windows.Forms.TextBox txtAIModelPath;
    }
}
