using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Sanford.StateMachineToolkit;

namespace TrafficLightDemo
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
        private TrafficLight light;
        private bool on = false;
        private Control currentPictureBox;
        private Control currentUmlPictureBox;
        private System.Windows.Forms.PictureBox offPictureBox;
        private System.Windows.Forms.PictureBox greenPictureBox;
        private System.Windows.Forms.PictureBox yellowPictureBox;
        private System.Windows.Forms.PictureBox redPictureBox;
        private System.Windows.Forms.Button onOffButton;
        private System.Windows.Forms.PictureBox umlOffPictureBox;
        private System.Windows.Forms.PictureBox umlGreenPictureBox;
        private System.Windows.Forms.PictureBox umlYellowPictureBox;
        private System.Windows.Forms.PictureBox umlRedPictureBox;
        private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			light = new TrafficLight();

            light.TransitionCompleted += new TransitionCompletedEventHandler(HandleTransitionCompleted);

            currentPictureBox = offPictureBox;
            currentUmlPictureBox = umlOffPictureBox;

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}

                light.Dispose();
                light.Execute();
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
            this.offPictureBox = new System.Windows.Forms.PictureBox();
            this.greenPictureBox = new System.Windows.Forms.PictureBox();
            this.yellowPictureBox = new System.Windows.Forms.PictureBox();
            this.redPictureBox = new System.Windows.Forms.PictureBox();
            this.onOffButton = new System.Windows.Forms.Button();
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
            this.greenPictureBox.Hide();
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
            this.yellowPictureBox.Hide();
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
            this.redPictureBox.Hide();
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
            this.umlGreenPictureBox.Hide();
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
            this.umlYellowPictureBox.Hide();
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
            this.umlRedPictureBox.Hide();
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
            this.Controls.Add(this.umlRedPictureBox);
            this.Controls.Add(this.umlGreenPictureBox);
            this.Controls.Add(this.umlYellowPictureBox);
            this.Controls.Add(this.redPictureBox);
            this.Controls.Add(this.greenPictureBox);
            this.Controls.Add(this.yellowPictureBox);
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

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void onOffButton_Click(object sender, System.EventArgs e)
        {
            if(on)
            {
                on = false;
                onOffButton.Text = "On";
                light.Send((int)TrafficLight.EventID.TurnOff);
            }
            else
            {
                on = true;
                onOffButton.Text = "Off";
                light.Send((int)TrafficLight.EventID.TurnOn);
            }

            light.Execute();
        }

        private void HandleTransitionCompleted(object sender, TransitionCompletedEventArgs e)
        {
            if(e.Error == null)
            {
                switch(e.StateID)
                {
                    case (int)TrafficLight.StateID.Off:
                        currentPictureBox.Hide();
                        offPictureBox.Show();
                        currentPictureBox = offPictureBox;

                        currentUmlPictureBox.Hide();
                        umlOffPictureBox.Show();
                        currentUmlPictureBox = umlOffPictureBox;
                        break;

                    case (int)TrafficLight.StateID.Red:
                        currentPictureBox.Hide();
                        redPictureBox.Show();
                        currentPictureBox = redPictureBox;

                        currentUmlPictureBox.Hide();
                        umlRedPictureBox.Show();
                        currentUmlPictureBox = umlRedPictureBox;
                        break;

                    case (int)TrafficLight.StateID.Yellow:
                        currentPictureBox.Hide();
                        yellowPictureBox.Show();
                        currentPictureBox = yellowPictureBox;

                        currentUmlPictureBox.Hide();
                        umlYellowPictureBox.Show();
                        currentUmlPictureBox = umlYellowPictureBox;
                        break;

                    case (int)TrafficLight.StateID.Green:
                        currentPictureBox.Hide();
                        greenPictureBox.Show();
                        currentPictureBox = greenPictureBox;

                        currentUmlPictureBox.Hide();
                        umlGreenPictureBox.Show();
                        currentUmlPictureBox = umlGreenPictureBox;
                        break;
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
