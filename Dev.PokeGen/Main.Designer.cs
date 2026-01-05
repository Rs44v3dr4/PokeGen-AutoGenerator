namespace Dev.PokeGen
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            grpConfig = new GroupBox();
            lblRol = new Label();
            txtRol = new TextBox();
            lblCanal = new Label();
            txtCanal = new TextBox();
            lblToken = new Label();
            txtToken = new TextBox();
            btnIniciar = new Button();
            lblEstado = new Label();
            rtbLog = new RichTextBox();
            lblLogTitle = new Label();
            label1 = new Label();
            label2 = new Label();
            txtGeminiKey = new TextBox();
            grpConfig.SuspendLayout();
            SuspendLayout();
            // 
            // grpConfig
            // 
            grpConfig.Controls.Add(label2);
            grpConfig.Controls.Add(txtGeminiKey);
            grpConfig.Controls.Add(lblRol);
            grpConfig.Controls.Add(txtRol);
            grpConfig.Controls.Add(lblCanal);
            grpConfig.Controls.Add(txtCanal);
            grpConfig.Controls.Add(lblToken);
            grpConfig.Controls.Add(txtToken);
            grpConfig.ForeColor = Color.White;
            grpConfig.Location = new Point(10, 11);
            grpConfig.Name = "grpConfig";
            grpConfig.Size = new Size(402, 206);
            grpConfig.TabIndex = 0;
            grpConfig.TabStop = false;
            grpConfig.Text = "Configuración";
            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRol.Location = new Point(214, 84);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(102, 15);
            lblRol.TabIndex = 5;
            lblRol.Text = "ID Rol Permitido:";
            // 
            // txtRol
            // 
            txtRol.BackColor = Color.FromArgb(45, 45, 48);
            txtRol.BorderStyle = BorderStyle.FixedSingle;
            txtRol.ForeColor = Color.White;
            txtRol.Location = new Point(214, 106);
            txtRol.Name = "txtRol";
            txtRol.Size = new Size(166, 23);
            txtRol.TabIndex = 4;
            // 
            // lblCanal
            // 
            lblCanal.AutoSize = true;
            lblCanal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCanal.Location = new Point(18, 84);
            lblCanal.Name = "lblCanal";
            lblCanal.Size = new Size(113, 15);
            lblCanal.TabIndex = 3;
            lblCanal.Text = "ID Canal Permitido:";
            // 
            // txtCanal
            // 
            txtCanal.BackColor = Color.FromArgb(45, 45, 48);
            txtCanal.BorderStyle = BorderStyle.FixedSingle;
            txtCanal.ForeColor = Color.White;
            txtCanal.Location = new Point(21, 106);
            txtCanal.Name = "txtCanal";
            txtCanal.Size = new Size(173, 23);
            txtCanal.TabIndex = 2;
            // 
            // lblToken
            // 
            lblToken.AutoSize = true;
            lblToken.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblToken.Location = new Point(18, 28);
            lblToken.Name = "lblToken";
            lblToken.Size = new Size(89, 15);
            lblToken.TabIndex = 1;
            lblToken.Text = "Discord Token:";
            // 
            // txtToken
            // 
            txtToken.BackColor = Color.FromArgb(45, 45, 48);
            txtToken.BorderStyle = BorderStyle.FixedSingle;
            txtToken.ForeColor = Color.White;
            txtToken.Location = new Point(21, 50);
            txtToken.Name = "txtToken";
            txtToken.PasswordChar = '•';
            txtToken.Size = new Size(359, 23);
            txtToken.TabIndex = 0;
            // 
            // btnIniciar
            // 
            btnIniciar.BackColor = Color.SeaGreen;
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.FlatStyle = FlatStyle.Flat;
            btnIniciar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnIniciar.ForeColor = Color.White;
            btnIniciar.Location = new Point(10, 223);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(402, 38);
            btnIniciar.TabIndex = 1;
            btnIniciar.Text = "INICIAR BOT";
            btnIniciar.UseVisualStyleBackColor = false;
            btnIniciar.Click += btnIniciar_Click;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.ForeColor = Color.Gray;
            lblEstado.Location = new Point(10, 267);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(123, 15);
            lblEstado.TabIndex = 2;
            lblEstado.Text = "Estado: Desconectado";
            // 
            // rtbLog
            // 
            rtbLog.BackColor = Color.Black;
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtbLog.ForeColor = Color.Lime;
            rtbLog.Location = new Point(-1, 307);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(402, 206);
            rtbLog.TabIndex = 3;
            rtbLog.Text = "Esperando inicio...";
            // 
            // lblLogTitle
            // 
            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLogTitle.ForeColor = Color.White;
            lblLogTitle.Location = new Point(10, 289);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Size = new Size(100, 15);
            lblLogTitle.TabIndex = 4;
            lblLogTitle.Text = "Monitor en Vivo:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 6.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(306, 518);
            label1.Name = "label1";
            label1.Size = new Size(106, 12);
            label1.TabIndex = 5;
            label1.Text = "By Rafiña DevPokePark";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(18, 147);
            label2.Name = "label2";
            label2.Size = new Size(96, 15);
            label2.TabIndex = 7;
            label2.Text = "Gemini API Key:";
            // 
            // txtGeminiKey
            // 
            txtGeminiKey.BackColor = Color.FromArgb(45, 45, 48);
            txtGeminiKey.BorderStyle = BorderStyle.FixedSingle;
            txtGeminiKey.ForeColor = Color.White;
            txtGeminiKey.Location = new Point(21, 169);
            txtGeminiKey.Name = "txtGeminiKey";
            txtGeminiKey.PasswordChar = '•';
            txtGeminiKey.Size = new Size(359, 23);
            txtGeminiKey.TabIndex = 6;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 28);
            ClientSize = new Size(424, 532);
            Controls.Add(label1);
            Controls.Add(lblLogTitle);
            Controls.Add(rtbLog);
            Controls.Add(lblEstado);
            Controls.Add(btnIniciar);
            Controls.Add(grpConfig);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PokeGenBot - Monitor";
            Load += Main_Load;
            grpConfig.ResumeLayout(false);
            grpConfig.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConfig;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.TextBox txtRol;
        private System.Windows.Forms.Label lblCanal;
        private System.Windows.Forms.TextBox txtCanal;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.RichTextBox rtbLog; // La consola visual
        private System.Windows.Forms.Label lblLogTitle;
        private Label label1;
        private Label label2;
        private TextBox txtGeminiKey;
    }
}
