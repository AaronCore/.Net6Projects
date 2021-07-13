
namespace WinFormsGrpc.Client.Demo
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_sample = new System.Windows.Forms.Button();
            this.btn_server = new System.Windows.Forms.Button();
            this.btn_client = new System.Windows.Forms.Button();
            this.btn_double = new System.Windows.Forms.Button();
            this.txt_result = new System.Windows.Forms.TextBox();
            this.lab_result = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txt_condition = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_server_token = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_sample
            // 
            this.btn_sample.Location = new System.Drawing.Point(136, 37);
            this.btn_sample.Name = "btn_sample";
            this.btn_sample.Size = new System.Drawing.Size(76, 42);
            this.btn_sample.TabIndex = 0;
            this.btn_sample.Text = "简单模式";
            this.btn_sample.UseVisualStyleBackColor = true;
            this.btn_sample.Click += new System.EventHandler(this.btn_sample_Click);
            // 
            // btn_server
            // 
            this.btn_server.Location = new System.Drawing.Point(277, 37);
            this.btn_server.Name = "btn_server";
            this.btn_server.Size = new System.Drawing.Size(95, 42);
            this.btn_server.TabIndex = 1;
            this.btn_server.Text = "服务端流模式";
            this.btn_server.UseVisualStyleBackColor = true;
            this.btn_server.Click += new System.EventHandler(this.btn_server_Click);
            // 
            // btn_client
            // 
            this.btn_client.Location = new System.Drawing.Point(423, 37);
            this.btn_client.Name = "btn_client";
            this.btn_client.Size = new System.Drawing.Size(90, 42);
            this.btn_client.TabIndex = 2;
            this.btn_client.Text = "客户端流模式";
            this.btn_client.UseVisualStyleBackColor = true;
            this.btn_client.Click += new System.EventHandler(this.btn_client_Click);
            // 
            // btn_double
            // 
            this.btn_double.Location = new System.Drawing.Point(567, 37);
            this.btn_double.Name = "btn_double";
            this.btn_double.Size = new System.Drawing.Size(90, 42);
            this.btn_double.TabIndex = 3;
            this.btn_double.Text = "双向流模式";
            this.btn_double.UseVisualStyleBackColor = true;
            this.btn_double.Click += new System.EventHandler(this.btn_double_Click);
            // 
            // txt_result
            // 
            this.txt_result.Location = new System.Drawing.Point(52, 147);
            this.txt_result.Multiline = true;
            this.txt_result.Name = "txt_result";
            this.txt_result.Size = new System.Drawing.Size(623, 235);
            this.txt_result.TabIndex = 4;
            // 
            // lab_result
            // 
            this.lab_result.AutoSize = true;
            this.lab_result.Location = new System.Drawing.Point(52, 127);
            this.lab_result.Name = "lab_result";
            this.lab_result.Size = new System.Drawing.Size(68, 17);
            this.lab_result.TabIndex = 5;
            this.lab_result.Text = "调用结果：";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txt_condition
            // 
            this.txt_condition.Location = new System.Drawing.Point(126, 85);
            this.txt_condition.Name = "txt_condition";
            this.txt_condition.Size = new System.Drawing.Size(100, 23);
            this.txt_condition.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "条     件：";
            // 
            // btn_server_token
            // 
            this.btn_server_token.Location = new System.Drawing.Point(277, 88);
            this.btn_server_token.Name = "btn_server_token";
            this.btn_server_token.Size = new System.Drawing.Size(95, 42);
            this.btn_server_token.TabIndex = 1;
            this.btn_server_token.Text = "服务端流模式(Token)";
            this.btn_server_token.UseVisualStyleBackColor = true;
            this.btn_server_token.Click += new System.EventHandler(this.btn_server_token_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_condition);
            this.Controls.Add(this.lab_result);
            this.Controls.Add(this.txt_result);
            this.Controls.Add(this.btn_double);
            this.Controls.Add(this.btn_client);
            this.Controls.Add(this.btn_server_token);
            this.Controls.Add(this.btn_server);
            this.Controls.Add(this.btn_sample);
            this.Name = "Form1";
            this.Text = "FormGrpcClient";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_sample;
        private System.Windows.Forms.Button btn_server;
        private System.Windows.Forms.Button btn_client;
        private System.Windows.Forms.Button btn_double;
        private System.Windows.Forms.TextBox txt_result;
        private System.Windows.Forms.Label lab_result;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txt_condition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_server_token;
    }
}

