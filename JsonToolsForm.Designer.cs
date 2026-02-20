namespace LanguageFileEditor;

partial class JsonToolsForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        btnImport = new Button();
        btnExport = new Button();
        lblInfo = new Label();
        SuspendLayout();
        // 
        // btnImport
        // 
        btnImport.Location = new Point(16, 54);
        btnImport.Name = "btnImport";
        btnImport.Size = new Size(160, 32);
        btnImport.TabIndex = 0;
        btnImport.Text = "JSON importieren...";
        btnImport.UseVisualStyleBackColor = true;
        btnImport.Click += BtnImport_Click;
        // 
        // btnExport
        // 
        btnExport.Location = new Point(192, 54);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(160, 32);
        btnExport.TabIndex = 1;
        btnExport.Text = "JSON exportieren...";
        btnExport.UseVisualStyleBackColor = true;
        btnExport.Click += BtnExport_Click;
        // 
        // lblInfo
        // 
        lblInfo.AutoSize = true;
        lblInfo.Location = new Point(16, 18);
        lblInfo.Name = "lblInfo";
        lblInfo.Size = new Size(316, 15);
        lblInfo.TabIndex = 2;
        lblInfo.Text = "JSON-Funktionen sind in eine eigene Werkzeuge-Form ausgelagert.";
        // 
        // JsonToolsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(370, 106);
        Controls.Add(lblInfo);
        Controls.Add(btnExport);
        Controls.Add(btnImport);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "JsonToolsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "JSON-Werkzeuge";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button btnImport;
    private Button btnExport;
    private Label lblInfo;
}
