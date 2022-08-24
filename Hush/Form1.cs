using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Automation;
using CSCore;
using CSCore.CoreAudioAPI;
using System.Runtime.InteropServices;



namespace Hush
{

    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);


        private Process TargetProcess;
        private Process SpeakerProcess;

        public Form1()
        {
            InitializeComponent();
        }


        private void DropDown(object sender, EventArgs e)
        {
            //code from https://stackoverflow.com/questions/4395405/how-can-i-obtain-a-list-of-currently-running-applications
            // and https://stackoverflow.com/questions/40070703/how-to-get-a-list-of-open-tabs-from-chrome-c-sharp


            ComboBox box = (ComboBox)sender;
            box.Items.Clear();

            Process[] processes = Process.GetProcesses();

            foreach (var proc in processes)
            {
                if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                {
                    box.Items.Add(proc.MainWindowTitle);
                }

            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //code here https://stackoverflow.com/questions/20938934/controlling-applications-volume-by-process-id
            int SpeakerID, TargetID = 0;
            
            SpeakerID = SpeakerProcess.Id;
            TargetID = TargetProcess.Id;
            //checks that the text name of the speaker and target are still the same as the ID (this might change if the process is shut down))
            if(Process.GetProcessById(SpeakerID).MainWindowTitle == Speaker.Text && Process.GetProcessById(TargetID).MainWindowTitle == Target.Text)
            {
                //question mark means nullable
                float? i = VolumeMixer.GetApplicationVolume(SpeakerID);


                VolumeMixer.SetApplicationVolume(TargetID, 50f);
            }
            
  

        }


        private void SaveProcess(ref Process proc, string procName)
        {
            proc = FindProcess(procName);
        }



        private Process FindProcess(string name)
        {
            Process[] processes = Process.GetProcesses();

            foreach (var proc in processes)
            {
                if (proc.MainWindowTitle == name)
                {
                    return proc;
                }
            }
            return null;
        }



        private void Speaker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            if (box.Text != "")
            {
                SaveProcess(ref SpeakerProcess, box.Text);
            }
        }

        private void Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            if (box.Text != "")
            {
                SaveProcess(ref TargetProcess, box.Text);
            }
        }






    }
    



}
