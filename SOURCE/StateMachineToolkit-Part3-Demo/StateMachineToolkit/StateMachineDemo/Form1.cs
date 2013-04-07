using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using StateMachineToolkit;

namespace StateMachineDemo
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
        private EventQueue queue = new EventQueue();
        private TrafficLight light;
        private bool on = false;
        private Control currentPictureBox;
        private Control currentUmlPictureBox;
        private System.Windows.Forms.PictureBox offPictureBox;
        private System.Windows.Forms.PictureBox greenPictureBox;
        private System.Windows.Forms.PictureBox yellowPictureBox;
        private System.Windows.Forms.PictureBox redPictureBox;
        private System.Windows.Forms.Button onOffButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox umlOffPictureBox;
        private System.Windows.Forms.PictureBox umlGreenPictureBox;
        private System.Windows.Forms.PictureBox umlYellowPictureBox;
        private System.Windows.Forms.PictureBox umlRedPictureBox;
        private System.ComponentModel.IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			light = new TrafficLight(queue);
            
            light.EnteringOff += new EventHandler(light_EnteringOff);
            light.EnteringRed += new EventHandler(light_EnteringRed);
            light.EnteringYellow += new EventHandler(light_EnteringYellow);
            light.EnteringGreen += new EventHandler(light_EnteringGreen);

            currentPictureBox = offPictureBox;
            currentUmlPictureBox = umlOffPictureBox;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                queue.Dispose();

				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
            this.offPictureBox = new System.Windows.Forms.PictureBox();
            this.greenPictureBox = new System.Windows.Forms.PictureBox();
            this.yellowPictureBox = new System.Windows.Forms.PictureBox();
            this.redPictureBox = new System.Windows.Forms.PictureBox();
            this.onOffButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.umlOffPictureBox = new System.Windows.Forms.PictureBox();
            this.umlGreenPictureBox = new System.Windows.Forms.PictureBox();
            this.umlYellowPictureBox = new System.Windows.Forms.PictureBox();
            this.umlRedPictureBox = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // offPictureBox
            // 
            this.offPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.offPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("offPictureBox.Image")));
            this.offPictureBox.Location = new System.Drawing.Point(16, 16);
            this.offPictureBox.Name = "offPictureBox";
            this.offPictureBox.Size = new System.Drawing.Size(75, 192);
            this.offPictureBox.TabIndex = 0;
            this.offPictureBox.TabStop = false;
            // 
            // greenPictureBox
            // 
            this.greenPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.greenPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("greenPictureBox.Image")));
            this.greenPictureBox.Location = new System.Drawing.Point(16, 16);
            this.greenPictureBox.Name = "greenPictureBox";
            this.greenPictureBox.Size = new System.Drawing.Size(75, 192);
            this.greenPictureBox.TabIndex = 1;
            this.greenPictureBox.TabStop = false;
            // 
            // yellowPictureBox
            // 
            this.yellowPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.yellowPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("yellowPictureBox.Image")));
            this.yellowPictureBox.Location = new System.Drawing.Point(16, 16);
            this.yellowPictureBox.Name = "yellowPictureBox";
            this.yellowPictureBox.Size = new System.Drawing.Size(75, 192);
            this.yellowPictureBox.TabIndex = 2;
            this.yellowPictureBox.TabStop = false;
            // 
            // redPictureBox
            // 
            this.redPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.redPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("redPictureBox.Image")));
            this.redPictureBox.Location = new System.Drawing.Point(16, 16);
            this.redPictureBox.Name = "redPictureBox";
            this.redPictureBox.Size = new System.Drawing.Size(75, 192);
            this.redPictureBox.TabIndex = 3;
            this.redPictureBox.TabStop = false;
            // 
            // onOffButton
            // 
            this.onOffButton.Location = new System.Drawing.Point(16, 224);
            this.onOffButton.Name = "onOffButton";
            this.onOffButton.TabIndex = 4;
            this.onOffButton.Text = "On";
            this.onOffButton.Click += new System.EventHandler(this.onOffButton_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // umlOffPictureBox
            // 
            this.umlOffPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.umlOffPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("umlOffPictureBox.Image")));
            this.umlOffPictureBox.Location = new System.Drawing.Point(104, 8);
            this.umlOffPictureBox.Name = "umlOffPictureBox";
            this.umlOffPictureBox.Size = new System.Drawing.Size(280, 240);
            this.umlOffPictureBox.TabIndex = 5;
            this.umlOffPictureBox.TabStop = false;
            // 
            // umlGreenPictureBox
            // 
            this.umlGreenPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.umlGreenPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("umlGreenPictureBox.Image")));
            this.umlGreenPictureBox.Location = new System.Drawing.Point(104, 8);
            this.umlGreenPictureBox.Name = "umlGreenPictureBox";
            this.umlGreenPictureBox.Size = new System.Drawing.Size(280, 240);
            this.umlGreenPictureBox.TabIndex = 0;
            this.umlGreenPictureBox.TabStop = false;
            // 
            // umlYellowPictureBox
            // 
            this.umlYellowPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.umlYellowPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("umlYellowPictureBox.Image")));
            this.umlYellowPictureBox.Location = new System.Drawing.Point(104, 8);
            this.umlYellowPictureBox.Name = "umlYellowPictureBox";
            this.umlYellowPictureBox.Size = new System.Drawing.Size(280, 240);
            this.umlYellowPictureBox.TabIndex = 0;
            this.umlYellowPictureBox.TabStop = false;
            // 
            // umlRedPictureBox
            // 
            this.umlRedPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.umlRedPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("umlRedPictureBox.Image")));
            this.umlRedPictureBox.Location = new System.Drawing.Point(104, 8);
            this.umlRedPictureBox.Name = "umlRedPictureBox";
            this.umlRedPictureBox.Size = new System.Drawing.Size(280, 240);
            this.umlRedPictureBox.TabIndex = 0;
            this.umlRedPictureBox.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(392, 262);
            this.Controls.Add(this.umlOffPictureBox);
            this.Controls.Add(this.onOffButton);
            this.Controls.Add(this.offPictureBox);
            this.Name = "Form1";
            this.Text = "State Machine Demo";
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            light.Send((int)TrafficLight.EventType.TimerElapsed);        
        }

        private void light_EnteringOff(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler(light_EnteringOff));            
            }
            else
            {
                Controls.Remove(currentPictureBox);
                Controls.Add(offPictureBox);
                currentPictureBox = offPictureBox;

                Controls.Remove(currentUmlPictureBox);
                Controls.Add(umlOffPictureBox);
                currentUmlPictureBox = umlOffPictureBox;
            }
        }

        private void light_EnteringRed(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler(light_EnteringRed));            
            }
            else
            {
                Controls.Remove(currentPictureBox);
                Controls.Add(redPictureBox);
                currentPictureBox = redPictureBox;

                Controls.Remove(currentUmlPictureBox);
                Controls.Add(umlRedPictureBox);
                currentUmlPictureBox = umlRedPictureBox;
            }
        }

        private void light_EnteringYellow(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler(light_EnteringYellow));            
            }
            else
            {
                Controls.Remove(currentPictureBox);
                Controls.Add(yellowPictureBox);
                currentPictureBox = yellowPictureBox;

                Controls.Remove(currentUmlPictureBox);
                Controls.Add(umlYellowPictureBox);
                currentUmlPictureBox = umlYellowPictureBox;
            }
        }

        private void light_EnteringGreen(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler(light_EnteringGreen));            
            }
            else
            {
                Controls.Remove(currentPictureBox);
                Controls.Add(greenPictureBox);
                currentPictureBox = greenPictureBox;

                Controls.Remove(currentUmlPictureBox);
                Controls.Add(umlGreenPictureBox);
                currentUmlPictureBox = umlGreenPictureBox;
            }
        }
        
        private void onOffButton_Click(object sender, System.EventArgs e)
        {
            if(on)
            {
                on = false;
                onOffButton.Text = "On";
                light.Send((int)TrafficLight.EventType.TurnOff);
            }
            else
            {
                on = true;
                onOffButton.Text = "Off";
                light.Send((int)TrafficLight.EventType.TurnOn);
            }
        }
    }
}
