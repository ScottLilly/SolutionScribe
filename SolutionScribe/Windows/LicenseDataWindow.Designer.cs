namespace SolutionScribe.Windows;

partial class LicenseDataWindow
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
            this.lblLicenseType = new System.Windows.Forms.Label();
            this.lblCopyrightYears = new System.Windows.Forms.Label();
            this.lblCopyrightHolder = new System.Windows.Forms.Label();
            this.lblFixedTextNote = new System.Windows.Forms.Label();
            this.cboLicenseTypes = new System.Windows.Forms.ComboBox();
            this.tbYears = new System.Windows.Forms.TextBox();
            this.tbCopyrightHolder = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblLicenseType
            //
            this.lblLicenseType.AutoSize = true;
            this.lblLicenseType.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicenseType.Location = new System.Drawing.Point(13, 13);
            this.lblLicenseType.Name = "lblLicenseType";
            this.lblLicenseType.Size = new System.Drawing.Size(99, 18);
            this.lblLicenseType.TabIndex = 0;
            this.lblLicenseType.Text = "License Type:";
            //
            // lblCopyrightYears
            //
            this.lblCopyrightYears.AutoSize = true;
            this.lblCopyrightYears.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyrightYears.Location = new System.Drawing.Point(13, 46);
            this.lblCopyrightYears.Name = "lblCopyrightYears";
            this.lblCopyrightYears.Size = new System.Drawing.Size(127, 18);
            this.lblCopyrightYears.TabIndex = 2;
            this.lblCopyrightYears.Text = "Copyright Year(s):";
            //
            // lblCopyrightHolder
            //
            this.lblCopyrightHolder.AutoSize = true;
            this.lblCopyrightHolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyrightHolder.Location = new System.Drawing.Point(13, 77);
            this.lblCopyrightHolder.Name = "lblCopyrightHolder";
            this.lblCopyrightHolder.Size = new System.Drawing.Size(123, 18);
            this.lblCopyrightHolder.TabIndex = 4;
            this.lblCopyrightHolder.Text = "Copyright Holder:";
            //
            // lblFixedTextNote
            //
            this.lblFixedTextNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFixedTextNote.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFixedTextNote.Location = new System.Drawing.Point(144, 107);
            this.lblFixedTextNote.Name = "lblFixedTextNote";
            this.lblFixedTextNote.Size = new System.Drawing.Size(394, 18);
            this.lblFixedTextNote.TabIndex = 6;
            this.lblFixedTextNote.Text = "This license text is fixed. The year and copyright holder are not used.";
            this.lblFixedTextNote.Visible = false;
            //
            // cboLicenseTypes
            //
            this.cboLicenseTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLicenseTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLicenseTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLicenseTypes.FormattingEnabled = true;
            this.cboLicenseTypes.Location = new System.Drawing.Point(147, 9);
            this.cboLicenseTypes.Name = "cboLicenseTypes";
            this.cboLicenseTypes.Size = new System.Drawing.Size(391, 26);
            this.cboLicenseTypes.TabIndex = 1;
            //
            // tbYears
            //
            this.tbYears.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbYears.Location = new System.Drawing.Point(147, 43);
            this.tbYears.Name = "tbYears";
            this.tbYears.Size = new System.Drawing.Size(150, 24);
            this.tbYears.TabIndex = 3;
            //
            // tbCopyrightHolder
            //
            this.tbCopyrightHolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCopyrightHolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCopyrightHolder.Location = new System.Drawing.Point(147, 77);
            this.tbCopyrightHolder.Name = "tbCopyrightHolder";
            this.tbCopyrightHolder.Size = new System.Drawing.Size(391, 24);
            this.tbCopyrightHolder.TabIndex = 5;
            //
            // btnOK
            //
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(313, 139);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(109, 27);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "Create File";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(428, 139);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(109, 27);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // LicenseDataWindow
            //
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(553, 178);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblFixedTextNote);
            this.Controls.Add(this.tbCopyrightHolder);
            this.Controls.Add(this.tbYears);
            this.Controls.Add(this.cboLicenseTypes);
            this.Controls.Add(this.lblCopyrightHolder);
            this.Controls.Add(this.lblCopyrightYears);
            this.Controls.Add(this.lblLicenseType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LicenseDataWindow";
            this.Text = "License Data";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblLicenseType;
    private System.Windows.Forms.Label lblCopyrightYears;
    private System.Windows.Forms.Label lblCopyrightHolder;
    private System.Windows.Forms.Label lblFixedTextNote;
    private System.Windows.Forms.ComboBox cboLicenseTypes;
    private System.Windows.Forms.TextBox tbYears;
    private System.Windows.Forms.TextBox tbCopyrightHolder;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
}
