namespace AperyGenerateTeacherGUI
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.labelThreads = new System.Windows.Forms.Label();
			this.labelTeacherNodes = new System.Windows.Forms.Label();
			this.labelLoop = new System.Windows.Forms.Label();
			this.btn = new System.Windows.Forms.Button();
			this.labelLoopLog = new System.Windows.Forms.Label();
			this.boxLoopLog = new System.Windows.Forms.TextBox();
			this.labelLog = new System.Windows.Forms.Label();
			this.boxLog = new System.Windows.Forms.TextBox();
			this.boxTeacherNodes = new System.Windows.Forms.NumericUpDown();
			this.boxLoop = new System.Windows.Forms.NumericUpDown();
			this.boxThreads = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.boxTeacherNodes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxLoop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxThreads)).BeginInit();
			this.SuspendLayout();
			// 
			// labelThreads
			// 
			this.labelThreads.AutoSize = true;
			this.labelThreads.Location = new System.Drawing.Point(0, 10);
			this.labelThreads.Name = "labelThreads";
			this.labelThreads.Size = new System.Drawing.Size(400, 24);
			this.labelThreads.TabIndex = 0;
			this.labelThreads.Text = "使用するスレッド数を入力して下さい。\r\n(最初に入っている値が最も効率的です。数値が大きいほどPCに負荷が掛かります。)";
			// 
			// labelTeacherNodes
			// 
			this.labelTeacherNodes.AutoSize = true;
			this.labelTeacherNodes.Location = new System.Drawing.Point(0, 65);
			this.labelTeacherNodes.Name = "labelTeacherNodes";
			this.labelTeacherNodes.Size = new System.Drawing.Size(400, 12);
			this.labelTeacherNodes.TabIndex = 2;
			this.labelTeacherNodes.Text = "作成・送信する教師局面数を入力して下さい。(100万以上の値を入力して下さい。)";
			// 
			// labelLoop
			// 
			this.labelLoop.AutoSize = true;
			this.labelLoop.Location = new System.Drawing.Point(0, 105);
			this.labelLoop.Name = "labelLoop";
			this.labelLoop.Size = new System.Drawing.Size(223, 12);
			this.labelLoop.TabIndex = 4;
			this.labelLoop.Text = "作成・送信を繰り返す回数を入力して下さい。";
			// 
			// btn
			// 
			this.btn.Location = new System.Drawing.Point(100, 145);
			this.btn.Name = "btn";
			this.btn.Size = new System.Drawing.Size(100, 30);
			this.btn.TabIndex = 7;
			this.btn.Text = "作成開始";
			this.btn.UseVisualStyleBackColor = true;
			this.btn.Click += new System.EventHandler(this.btn_Click);
			// 
			// labelLoopLog
			// 
			this.labelLoopLog.AutoSize = true;
			this.labelLoopLog.Location = new System.Drawing.Point(0, 175);
			this.labelLoopLog.Name = "labelLoopLog";
			this.labelLoopLog.Size = new System.Drawing.Size(87, 12);
			this.labelLoopLog.TabIndex = 8;
			this.labelLoopLog.Text = "現在の作成回数";
			// 
			// boxLoopLog
			// 
			this.boxLoopLog.Location = new System.Drawing.Point(10, 190);
			this.boxLoopLog.Name = "boxLoopLog";
			this.boxLoopLog.ReadOnly = true;
			this.boxLoopLog.Size = new System.Drawing.Size(100, 19);
			this.boxLoopLog.TabIndex = 9;
			// 
			// labelLog
			// 
			this.labelLog.AutoSize = true;
			this.labelLog.Location = new System.Drawing.Point(0, 215);
			this.labelLog.Name = "labelLog";
			this.labelLog.Size = new System.Drawing.Size(23, 12);
			this.labelLog.TabIndex = 10;
			this.labelLog.Text = "ログ";
			// 
			// boxLog
			// 
			this.boxLog.Location = new System.Drawing.Point(10, 230);
			this.boxLog.Name = "boxLog";
			this.boxLog.ReadOnly = true;
			this.boxLog.Size = new System.Drawing.Size(400, 19);
			this.boxLog.TabIndex = 11;
			// 
			// boxTeacherNodes
			// 
			this.boxTeacherNodes.Location = new System.Drawing.Point(10, 80);
			this.boxTeacherNodes.Maximum = new decimal(new int[] {
            1661992959,
            1808227885,
            5,
            0});
			this.boxTeacherNodes.Name = "boxTeacherNodes";
			this.boxTeacherNodes.Size = new System.Drawing.Size(120, 19);
			this.boxTeacherNodes.TabIndex = 12;
			this.boxTeacherNodes.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			// 
			// boxLoop
			// 
			this.boxLoop.Location = new System.Drawing.Point(10, 120);
			this.boxLoop.Maximum = new decimal(new int[] {
            1661992959,
            1808227885,
            5,
            0});
			this.boxLoop.Name = "boxLoop";
			this.boxLoop.Size = new System.Drawing.Size(120, 19);
			this.boxLoop.TabIndex = 13;
			this.boxLoop.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// boxThreads
			// 
			this.boxThreads.Location = new System.Drawing.Point(10, 40);
			this.boxThreads.Maximum = new decimal(new int[] {
            1661992959,
            1808227885,
            5,
            0});
			this.boxThreads.Name = "boxThreads";
			this.boxThreads.Size = new System.Drawing.Size(120, 19);
			this.boxThreads.TabIndex = 14;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 411);
			this.Controls.Add(this.boxThreads);
			this.Controls.Add(this.boxLoop);
			this.Controls.Add(this.boxTeacherNodes);
			this.Controls.Add(this.boxLog);
			this.Controls.Add(this.labelLog);
			this.Controls.Add(this.boxLoopLog);
			this.Controls.Add(this.labelLoopLog);
			this.Controls.Add(this.btn);
			this.Controls.Add(this.labelLoop);
			this.Controls.Add(this.labelTeacherNodes);
			this.Controls.Add(this.labelThreads);
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "AperyGenerateTeacherGUI";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.boxTeacherNodes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxLoop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxThreads)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelThreads;
		private System.Windows.Forms.Label labelTeacherNodes;
		private System.Windows.Forms.Label labelLoop;
		private System.Windows.Forms.Button btn;
		private System.Windows.Forms.Label labelLoopLog;
		private System.Windows.Forms.TextBox boxLoopLog;
		private System.Windows.Forms.Label labelLog;
		private System.Windows.Forms.TextBox boxLog;
		private System.Windows.Forms.NumericUpDown boxTeacherNodes;
		private System.Windows.Forms.NumericUpDown boxLoop;
		private System.Windows.Forms.NumericUpDown boxThreads;
	}
}

