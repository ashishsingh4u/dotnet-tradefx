namespace TradeFx.Connection.Client
{
    partial class ConnectionClient
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
            this._latencyLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _latencyLabel
            // 
            this._latencyLabel.AutoSize = true;
            this._latencyLabel.Location = new System.Drawing.Point(103, 99);
            this._latencyLabel.Name = "_latencyLabel";
            this._latencyLabel.Size = new System.Drawing.Size(45, 13);
            this._latencyLabel.TabIndex = 1;
            this._latencyLabel.Text = "Latency";
            // 
            // ConnectionClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this._latencyLabel);
            this.Name = "ConnectionClient";
            this.Text = "Client Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _latencyLabel;

    }
}

