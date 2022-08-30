using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Hush
{

    public partial class Form1 : Form
    {
        Thread controlMixThread = null;
        private ProcessInfo TargetProcess;
        private ProcessInfo SpeakerProcess;
        string SpeakerText;
        string TargetText;
        int inertia_period = 0000;
        private List<ProcessInfo> processInfo = new List<ProcessInfo>();
        bool hush = false;
        public Form1()
        {
            InitializeComponent();
            RefreshProcesses();
        }


        public void RefreshProcesses()
        {
            processInfo.Clear();
            processInfo = VolumeMixer.GetProcessInfo();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //code here https://stackoverflow.com/questions/20938934/controlling-applications-volume-by-process-id
            if (controlMixThread != null) //to not abort an empty thread (i.e the first time this is run)
            {
                controlMixThread.Abort();
            }

            TargetText = Target.Text;
            SpeakerText = Speaker.Text;
            hush = true;
            controlMixThread = new Thread(ControlMixer);
            controlMixThread.IsBackground = true;
            controlMixThread.Start();

        }
        private void ControlMixer()
        {
            Thread thread = null;
            int SpeakerID = 0, TargetID = 0;
            SpeakerID = SpeakerProcess.ID;
            TargetID = TargetProcess.ID;
            float target_loud_volume = VolumeMixer.GetVolume(TargetID);
            string oldMixerState = "";
            while (hush && SpeakerProcess != null && TargetProcess != null)
            {
                //Console.WriteLine(target_loud_volume);



                string newMixerState = VolumeMixer.FindMixerState(SpeakerID, TargetID);


                if (oldMixerState != newMixerState) // we only want to fade if the state has changed
                {
                    Thread.Sleep(inertia_period);
                    if (VolumeMixer.FindMixerState(SpeakerID, TargetID) == newMixerState) //if it is the same state after the inertia period seconds fade
                    {
                        if (thread != null && thread.IsAlive)
                        {
                            thread.Abort();
                        }
                        thread = new Thread(() => Fade(SpeakerID, TargetID, target_loud_volume, newMixerState));
                        thread.IsBackground = true;
                        thread.Start();
                        oldMixerState = newMixerState;

                    }


                }







            }

        }
        private void DropDown(object sender, EventArgs e)
        {
            //code from https://stackoverflow.com/questions/4395405/how-can-i-obtain-a-list-of-currently-running-applications
            // and https://stackoverflow.com/questions/40070703/how-to-get-a-list-of-open-tabs-from-chrome-c-sharp
            // https://stackoverflow.com/questions/40070703/how-to-get-a-list-of-open-tabs-from-chrome-c-sharp

            ComboBox box = (ComboBox)sender;
            box.Items.Clear();
        


            foreach (var proc in processInfo)
            {
                if (!string.IsNullOrEmpty(proc.name))
                {
                    box.Items.Add(proc.name);
                }

            }
        }



        private void Fade(int SpeakerID, int TargetID, float target_loud_volume, string newMixerState)
        {
            if(newMixerState == "FadeIn")
            {
                VolumeMixer.FadeIn(SpeakerID, TargetID, target_loud_volume);
            }
            else if(newMixerState == "FadeOut")
            {
                VolumeMixer.FadeOut(SpeakerID, TargetID);
            }
        }




    
        private void SaveProcess(ref ProcessInfo proc, string procName)
        {
            proc = FindProcess(procName);
        }



        private ProcessInfo FindProcess(string name)
        {

            foreach (var proc in processInfo)
            {
                if (proc.name == name)
                {
                    return proc;
                }
            }
            return null;
        }



        private void Speaker_SelectedIndexChanged(object sender, EventArgs e)
        {
            hush = false;
            ComboBox box = (ComboBox)sender;
            if (box.Text != "")
            {
                SaveProcess(ref SpeakerProcess, box.Text);
            }
        }

        private void Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            hush = false;
            ComboBox box = (ComboBox)sender;
            if (box.Text != "")
            {
                SaveProcess(ref TargetProcess, box.Text);
            }
        }


        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshProcesses();
        }
    }
    



}
