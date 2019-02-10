namespace PreviewDot
{
	partial class XmlControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtXml = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtXml
			// 
			this.txtXml.BackColor = System.Drawing.SystemColors.Window;
			this.txtXml.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtXml.Location = new System.Drawing.Point(0, 0);
			this.txtXml.Multiline = true;
			this.txtXml.Name = "txtXml";
			this.txtXml.ReadOnly = true;
			this.txtXml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtXml.Size = new System.Drawing.Size(150, 150);
			this.txtXml.TabIndex = 1;
			this.txtXml.WordWrap = false;
			// 
			// XmlControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txtXml);
			this.Name = "XmlControl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtXml;
	}
}
