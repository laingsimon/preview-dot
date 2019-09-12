namespace PreviewDot
{
    partial class PreviewControl
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
            this.components = new System.ComponentModel.Container();
            this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itmDrawingDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.itmPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.itmFitImage = new System.Windows.Forms.ToolStripMenuItem();
            this.itmZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.itmZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.pnlScroller = new System.Windows.Forms.Panel();
            this.itmCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.pnlScroller.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuContext
            // 
            this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmDrawingDetails,
            this.itmPrint,
            this.itmCopy,
            this.itmFitImage,
            this.itmZoomIn,
            this.itmZoomOut});
            this.mnuContext.Name = "mnuContext";
            this.mnuContext.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuContext.Size = new System.Drawing.Size(181, 158);
            this.mnuContext.Opening += new System.ComponentModel.CancelEventHandler(this.mnuContext_Opening);
            // 
            // itmDrawingDetails
            // 
            this.itmDrawingDetails.Enabled = false;
            this.itmDrawingDetails.Name = "itmDrawingDetails";
            this.itmDrawingDetails.Size = new System.Drawing.Size(180, 22);
            this.itmDrawingDetails.Text = "Drawing Details";
            // 
            // itmPrint
            // 
            this.itmPrint.Name = "itmPrint";
            this.itmPrint.Size = new System.Drawing.Size(180, 22);
            this.itmPrint.Text = "Print";
            this.itmPrint.Click += new System.EventHandler(this.itmPrint_Click);
            // 
            // itmFitImage
            // 
            this.itmFitImage.Checked = true;
            this.itmFitImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.itmFitImage.Name = "itmFitImage";
            this.itmFitImage.Size = new System.Drawing.Size(180, 22);
            this.itmFitImage.Text = "Fit Image";
            this.itmFitImage.Click += new System.EventHandler(this.itmFitImage_Click);
            // 
            // itmZoomIn
            // 
            this.itmZoomIn.Name = "itmZoomIn";
            this.itmZoomIn.ShortcutKeyDisplayString = "Ctrl + +";
            this.itmZoomIn.Size = new System.Drawing.Size(180, 22);
            this.itmZoomIn.Text = "Zoom In";
            this.itmZoomIn.Click += new System.EventHandler(this.itmZoomIn_Click);
            // 
            // itmZoomOut
            // 
            this.itmZoomOut.Name = "itmZoomOut";
            this.itmZoomOut.ShortcutKeyDisplayString = "Ctrl + -";
            this.itmZoomOut.Size = new System.Drawing.Size(180, 22);
            this.itmZoomOut.Text = "Zoom Out";
            this.itmZoomOut.Click += new System.EventHandler(this.itmZoomOut_Click);
            // 
            // picPreview
            // 
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(0, 0);
            this.picPreview.Margin = new System.Windows.Forms.Padding(0);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(150, 150);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 1;
            this.picPreview.TabStop = false;
            this.picPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseDown);
            this.picPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseMove);
            this.picPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseUp);
            // 
            // pnlScroller
            // 
            this.pnlScroller.AutoScroll = true;
            this.pnlScroller.Controls.Add(this.picPreview);
            this.pnlScroller.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScroller.Location = new System.Drawing.Point(0, 0);
            this.pnlScroller.Name = "pnlScroller";
            this.pnlScroller.Size = new System.Drawing.Size(150, 150);
            this.pnlScroller.TabIndex = 2;
            // 
            // itmCopy
            // 
            this.itmCopy.Name = "itmCopy";
            this.itmCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.itmCopy.Size = new System.Drawing.Size(180, 22);
            this.itmCopy.Text = "Copy";
            this.itmCopy.Click += new System.EventHandler(this.itmCopy_Click);
            // 
            // PreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.mnuContext;
            this.Controls.Add(this.pnlScroller);
            this.DoubleBuffered = true;
            this.Name = "PreviewControl";
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.PreviewControl_Scroll);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PreviewControl_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PreviewControl_KeyUp);
            this.mnuContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.pnlScroller.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip mnuContext;
        private System.Windows.Forms.ToolStripMenuItem itmPrint;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel pnlScroller;
        private System.Windows.Forms.ToolStripMenuItem itmFitImage;
        private System.Windows.Forms.ToolStripMenuItem itmZoomIn;
        private System.Windows.Forms.ToolStripMenuItem itmZoomOut;
        private System.Windows.Forms.ToolStripMenuItem itmDrawingDetails;
        private System.Windows.Forms.ToolStripMenuItem itmCopy;
    }
}
