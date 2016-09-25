namespace Launcher.UpdateListMaker
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDragDrop = new System.Windows.Forms.Label();
            this.lvUpdateList = new System.Windows.Forms.ListView();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chChecksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSaveList = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lblDragDrop
            // 
            this.lblDragDrop.Location = new System.Drawing.Point(12, 9);
            this.lblDragDrop.Name = "lblDragDrop";
            this.lblDragDrop.Size = new System.Drawing.Size(626, 47);
            this.lblDragDrop.TabIndex = 0;
            this.lblDragDrop.Text = "Arraste arquivos e solte na lista abaixo para adicionar eles.\r\nPara remover, sele" +
    "ciona um arquivo e pressione DELETE.";
            this.lblDragDrop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvUpdateList
            // 
            this.lvUpdateList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvUpdateList.AllowDrop = true;
            this.lvUpdateList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chChecksum,
            this.chSize});
            this.lvUpdateList.FullRowSelect = true;
            this.lvUpdateList.GridLines = true;
            this.lvUpdateList.Location = new System.Drawing.Point(12, 70);
            this.lvUpdateList.MultiSelect = false;
            this.lvUpdateList.Name = "lvUpdateList";
            this.lvUpdateList.Size = new System.Drawing.Size(626, 246);
            this.lvUpdateList.TabIndex = 1;
            this.lvUpdateList.UseCompatibleStateImageBehavior = false;
            this.lvUpdateList.View = System.Windows.Forms.View.Details;
            this.lvUpdateList.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvUpdateList_DragDrop);
            this.lvUpdateList.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvUpdateList_DragEnter);
            this.lvUpdateList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvUpdateList_KeyUp);
            // 
            // chFilename
            // 
            this.chFilename.Text = "Nome do arquivo";
            this.chFilename.Width = 300;
            // 
            // chChecksum
            // 
            this.chChecksum.Text = "Checksum";
            this.chChecksum.Width = 323;
            // 
            // btnSaveList
            // 
            this.btnSaveList.Location = new System.Drawing.Point(12, 322);
            this.btnSaveList.Name = "btnSaveList";
            this.btnSaveList.Size = new System.Drawing.Size(302, 53);
            this.btnSaveList.TabIndex = 2;
            this.btnSaveList.Text = "Salvar lista";
            this.btnSaveList.UseVisualStyleBackColor = true;
            this.btnSaveList.Click += new System.EventHandler(this.btnSaveList_Click);
            // 
            // btnClearList
            // 
            this.btnClearList.Location = new System.Drawing.Point(336, 322);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(302, 53);
            this.btnClearList.TabIndex = 3;
            this.btnClearList.Text = "Limpar lista";
            this.btnClearList.UseVisualStyleBackColor = true;
            this.btnClearList.Click += new System.EventHandler(this.btnClearList_Click);
            // 
            // chSize
            // 
            this.chSize.Text = "Tamanho";
            this.chSize.Width = 120;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 387);
            this.Controls.Add(this.btnClearList);
            this.Controls.Add(this.btnSaveList);
            this.Controls.Add(this.lvUpdateList);
            this.Controls.Add(this.lblDragDrop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UpdateList Maker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDragDrop;
        private System.Windows.Forms.ListView lvUpdateList;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chChecksum;
        private System.Windows.Forms.Button btnSaveList;
        private System.Windows.Forms.Button btnClearList;
        private System.Windows.Forms.ColumnHeader chSize;
    }
}