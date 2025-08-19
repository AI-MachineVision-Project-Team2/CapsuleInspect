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
            this.btnInspAI = new System.Windows.Forms.Button();
            this.btnLoadModel = new System.Windows.Forms.Button();
            this.btnSelAIModel = new System.Windows.Forms.Button();
            this.txtAIModelPath = new System.Windows.Forms.TextBox();
            this.lbl_SEG = new System.Windows.Forms.Label();
            this.dataGridViewFilter = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInspAI
            // 
            this.btnInspAI.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnInspAI.Location = new System.Drawing.Point(232, 62);
            this.btnInspAI.Name = "btnInspAI";
            this.btnInspAI.Size = new System.Drawing.Size(67, 27);
            this.btnInspAI.TabIndex = 16;
            this.btnInspAI.Text = "AI 검사";
            this.btnInspAI.UseVisualStyleBackColor = true;
            this.btnInspAI.Click += new System.EventHandler(this.btnInspAI_Click);
            // 
            // btnLoadModel
            // 
            this.btnLoadModel.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnLoadModel.Location = new System.Drawing.Point(116, 62);
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(76, 27);
            this.btnLoadModel.TabIndex = 15;
            this.btnLoadModel.Text = "모델 로딩";
            this.btnLoadModel.UseVisualStyleBackColor = true;
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            // 
            // btnSelAIModel
            // 
            this.btnSelAIModel.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnSelAIModel.Location = new System.Drawing.Point(8, 62);
            this.btnSelAIModel.Name = "btnSelAIModel";
            this.btnSelAIModel.Size = new System.Drawing.Size(76, 28);
            this.btnSelAIModel.TabIndex = 14;
            this.btnSelAIModel.Text = "AI모델 선택";
            this.btnSelAIModel.UseVisualStyleBackColor = true;
            this.btnSelAIModel.Click += new System.EventHandler(this.btnSelAIModel_Click);
            // 
            // txtAIModelPath
            // 
            this.txtAIModelPath.BackColor = System.Drawing.Color.White;
            this.txtAIModelPath.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.txtAIModelPath.Location = new System.Drawing.Point(8, 34);
            this.txtAIModelPath.Name = "txtAIModelPath";
            this.txtAIModelPath.ReadOnly = true;
            this.txtAIModelPath.Size = new System.Drawing.Size(292, 25);
            this.txtAIModelPath.TabIndex = 13;
            // 
            // lbl_SEG
            // 
            this.lbl_SEG.AutoSize = true;
            this.lbl_SEG.BackColor = System.Drawing.Color.Transparent;
            this.lbl_SEG.Font = new System.Drawing.Font("Noto Sans KR", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbl_SEG.ForeColor = System.Drawing.SystemColors.MenuText;
            this.lbl_SEG.Location = new System.Drawing.Point(3, 0);
            this.lbl_SEG.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_SEG.Name = "lbl_SEG";
            this.lbl_SEG.Size = new System.Drawing.Size(176, 30);
            this.lbl_SEG.TabIndex = 18;
            this.lbl_SEG.Text = "SEGMENTATION";
            // 
            // dataGridViewFilter
            // 
            this.dataGridViewFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFilter.Location = new System.Drawing.Point(8, 99);
            this.dataGridViewFilter.Name = "dataGridViewFilter";
            this.dataGridViewFilter.RowHeadersWidth = 62;
            this.dataGridViewFilter.RowTemplate.Height = 23;
            this.dataGridViewFilter.Size = new System.Drawing.Size(291, 118);
            this.dataGridViewFilter.TabIndex = 19;
            // 
            // AIModuleProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewFilter);
            this.Controls.Add(this.lbl_SEG);
            this.Controls.Add(this.btnInspAI);
            this.Controls.Add(this.btnLoadModel);
            this.Controls.Add(this.btnSelAIModel);
            this.Controls.Add(this.txtAIModelPath);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "AIModuleProp";
            this.Size = new System.Drawing.Size(435, 235);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnInspAI;
        private System.Windows.Forms.Button btnLoadModel;
        private System.Windows.Forms.Button btnSelAIModel;
        private System.Windows.Forms.TextBox txtAIModelPath;
        private System.Windows.Forms.Label lbl_SEG;
        private System.Windows.Forms.DataGridView dataGridViewFilter;
    }
}
