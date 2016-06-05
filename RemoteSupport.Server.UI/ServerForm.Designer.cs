namespace RemoteSupport.Server.UI
{
    partial class ServerForm
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
            this.listClients = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.email = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.usage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.session = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listClients
            // 
            this.listClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listClients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.listClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.email,
            this.status,
            this.usage,
            this.session});
            this.listClients.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.listClients.ForeColor = System.Drawing.Color.Gainsboro;
            this.listClients.FullRowSelect = true;
            this.listClients.Location = new System.Drawing.Point(12, 12);
            this.listClients.MultiSelect = false;
            this.listClients.Name = "listClients";
            this.listClients.Size = new System.Drawing.Size(737, 346);
            this.listClients.TabIndex = 3;
            this.listClients.UseCompatibleStateImageBehavior = false;
            this.listClients.View = System.Windows.Forms.View.Details;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            this.ID.Width = 171;
            // 
            // email
            // 
            this.email.Text = "Email";
            this.email.Width = 217;
            // 
            // status
            // 
            this.status.Text = "Status";
            this.status.Width = 107;
            // 
            // usage
            // 
            this.usage.Text = "Total Bytes Usage";
            this.usage.Width = 104;
            // 
            // session
            // 
            this.session.Text = "In Session With";
            this.session.Width = 138;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(761, 370);
            this.Controls.Add(this.listClients);
            this.Name = "ServerForm";
            this.Text = "Server";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listClients;
        private System.Windows.Forms.ColumnHeader email;
        private System.Windows.Forms.ColumnHeader status;
        private System.Windows.Forms.ColumnHeader usage;
        private System.Windows.Forms.ColumnHeader ID;
        private System.Windows.Forms.ColumnHeader session;
    }
}

