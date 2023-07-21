
namespace KeyAssist
{
    partial class KeyAssist
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_textMessage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_timer = new System.Windows.Forms.Timer(this.components);
            this.m_btnStart = new System.Windows.Forms.Button();
            this.m_grid = new System.Windows.Forms.DataGridView();
            this.m_btnEdit = new System.Windows.Forms.Button();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.m_inputTimeInterval = new System.Windows.Forms.TextBox();
            this.m_async = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
            this.SuspendLayout();
            // 
            // m_textMessage
            // 
            this.m_textMessage.AutoSize = true;
            this.m_textMessage.Location = new System.Drawing.Point(83, 9);
            this.m_textMessage.Name = "m_textMessage";
            this.m_textMessage.Size = new System.Drawing.Size(11, 12);
            this.m_textMessage.TabIndex = 0;
            this.m_textMessage.Text = "A";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前按键：";
            // 
            // m_timer
            // 
            this.m_timer.Tick += new System.EventHandler(this.m_timer_Tick);
            // 
            // m_btnStart
            // 
            this.m_btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnStart.Location = new System.Drawing.Point(368, 4);
            this.m_btnStart.Name = "m_btnStart";
            this.m_btnStart.Size = new System.Drawing.Size(75, 23);
            this.m_btnStart.TabIndex = 2;
            this.m_btnStart.Text = "启动";
            this.m_btnStart.UseVisualStyleBackColor = true;
            this.m_btnStart.Click += new System.EventHandler(this.m_btnStart_Click);
            // 
            // m_grid
            // 
            this.m_grid.AllowUserToAddRows = false;
            this.m_grid.AllowUserToDeleteRows = false;
            this.m_grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_grid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_grid.Location = new System.Drawing.Point(14, 33);
            this.m_grid.Name = "m_grid";
            this.m_grid.ReadOnly = true;
            this.m_grid.RowTemplate.Height = 23;
            this.m_grid.Size = new System.Drawing.Size(429, 306);
            this.m_grid.TabIndex = 3;
            this.m_grid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_grid_CellContentClick);
            this.m_grid.SizeChanged += new System.EventHandler(this.m_grid_SizeChanged);
            // 
            // m_btnEdit
            // 
            this.m_btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnEdit.Location = new System.Drawing.Point(208, 345);
            this.m_btnEdit.Name = "m_btnEdit";
            this.m_btnEdit.Size = new System.Drawing.Size(75, 23);
            this.m_btnEdit.TabIndex = 4;
            this.m_btnEdit.Text = "编辑";
            this.m_btnEdit.UseVisualStyleBackColor = true;
            this.m_btnEdit.Click += new System.EventHandler(this.m_btnEdit_Click);
            // 
            // m_btnAdd
            // 
            this.m_btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnAdd.Location = new System.Drawing.Point(289, 345);
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.Size = new System.Drawing.Size(75, 23);
            this.m_btnAdd.TabIndex = 5;
            this.m_btnAdd.Text = "添加";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
            // 
            // m_btnRemove
            // 
            this.m_btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnRemove.Location = new System.Drawing.Point(368, 345);
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.m_btnRemove.TabIndex = 6;
            this.m_btnRemove.Text = "移除";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.m_btnRemove_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 350);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "检测间隔(毫秒):";
            // 
            // m_inputTimeInterval
            // 
            this.m_inputTimeInterval.Location = new System.Drawing.Point(113, 345);
            this.m_inputTimeInterval.Name = "m_inputTimeInterval";
            this.m_inputTimeInterval.Size = new System.Drawing.Size(80, 21);
            this.m_inputTimeInterval.TabIndex = 8;
            this.m_inputTimeInterval.TextChanged += new System.EventHandler(this.m_inputTimeInterval_TextChanged);
            // 
            // m_async
            // 
            this.m_async.DoWork += new System.ComponentModel.DoWorkEventHandler(this.m_async_DoWork);
            // 
            // KeyAssist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 368);
            this.Controls.Add(this.m_inputTimeInterval);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_btnRemove);
            this.Controls.Add(this.m_btnAdd);
            this.Controls.Add(this.m_btnEdit);
            this.Controls.Add(this.m_grid);
            this.Controls.Add(this.m_btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_textMessage);
            this.KeyPreview = true;
            this.Name = "KeyAssist";
            this.Text = "按键精灵";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyAssist_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_textMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer m_timer;
        private System.Windows.Forms.Button m_btnStart;
        private System.Windows.Forms.DataGridView m_grid;
        private System.Windows.Forms.Button m_btnEdit;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox m_inputTimeInterval;
        private System.ComponentModel.BackgroundWorker m_async;
    }
}

