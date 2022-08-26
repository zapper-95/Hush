using CSCore.CoreAudioAPI;
using System;

namespace Hush
{
    partial class Form1
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
            this.Speaker = new System.Windows.Forms.ComboBox();
            this.sp = new System.Windows.Forms.Label();
            this.Target = new System.Windows.Forms.ComboBox();
            this.tg = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.Refresh = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Speaker
            // 
            this.Speaker.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Speaker.FormattingEnabled = true;
            this.Speaker.Location = new System.Drawing.Point(335, 95);
            this.Speaker.Name = "Speaker";
            this.Speaker.Size = new System.Drawing.Size(180, 25);
            this.Speaker.TabIndex = 0;
            this.Speaker.DropDown += new System.EventHandler(this.DropDown);
            this.Speaker.SelectedIndexChanged += new System.EventHandler(this.Speaker_SelectedIndexChanged);
            // 
            // sp
            // 
            this.sp.AutoSize = true;
            this.sp.Font = new System.Drawing.Font("American Captain", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sp.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sp.Location = new System.Drawing.Point(167, 76);
            this.sp.Name = "sp";
            this.sp.Size = new System.Drawing.Size(162, 58);
            this.sp.TabIndex = 1;
            this.sp.Text = "Speaker";
            // 
            // Target
            // 
            this.Target.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Target.FormattingEnabled = true;
            this.Target.Location = new System.Drawing.Point(335, 173);
            this.Target.Name = "Target";
            this.Target.Size = new System.Drawing.Size(180, 25);
            this.Target.TabIndex = 2;
            this.Target.DropDown += new System.EventHandler(this.DropDown);
            this.Target.SelectedIndexChanged += new System.EventHandler(this.Target_SelectedIndexChanged);
            // 
            // tg
            // 
            this.tg.AutoSize = true;
            this.tg.Font = new System.Drawing.Font("American Captain", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tg.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tg.Location = new System.Drawing.Point(167, 152);
            this.tg.Name = "tg";
            this.tg.Size = new System.Drawing.Size(138, 58);
            this.tg.TabIndex = 3;
            this.tg.Text = "Target";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("American Captain", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(335, 264);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(149, 62);
            this.button1.TabIndex = 4;
            this.button1.Text = "Hush";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Refresh
            // 
            this.Refresh.Font = new System.Drawing.Font("Bebas Neue", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Refresh.Location = new System.Drawing.Point(600, 143);
            this.Refresh.Name = "Refresh";
            this.Refresh.Size = new System.Drawing.Size(107, 32);
            this.Refresh.TabIndex = 5;
            this.Refresh.Text = "Refresh";
            this.Refresh.UseVisualStyleBackColor = true;
            this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(650, 284);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(81, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(827, 397);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Refresh);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tg);
            this.Controls.Add(this.Target);
            this.Controls.Add(this.sp);
            this.Controls.Add(this.Speaker);
            this.Font = new System.Drawing.Font("Montserrat Hairline", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox Speaker;
        private System.Windows.Forms.Label sp;
        private System.Windows.Forms.ComboBox Target;
        private System.Windows.Forms.Label tg;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Refresh;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

