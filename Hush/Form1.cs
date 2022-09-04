using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Hush
{

    public partial class Form1 : Form
    {
        VolumeMixer mixer;
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
            Console.WriteLine(Process.GetCurrentProcess().Threads.Count);
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
            controlMixThread.Name = "controlMixThread"; //thread so that the mixer can be controlled whilst the rest of the program is responsive
            controlMixThread.IsBackground = true;
            controlMixThread.Start();

        }
        private void ControlMixer()
        {
            Thread thread = null;
            int SpeakerID = 0, TargetID = 0;
            SpeakerID = SpeakerProcess.ID;
            TargetID = TargetProcess.ID;

            if(mixer!= null)
            {
                mixer.Dispose(); //dispose of the last mixer if it exists (can't achieve this destructor as .net framework            
            }
            mixer = new VolumeMixer(SpeakerID, TargetID);

            string oldMixerState = "";




            while (hush && SpeakerProcess != null && TargetProcess != null)
            {
                //    //Console.WriteLine(target_loud_volume);



                string newMixerState = mixer.FindMixerState();

                if (oldMixerState != newMixerState) // we only want to fade if the state has changed
                {
                    Thread.Sleep(inertia_period);
                    if (mixer.FindMixerState() == newMixerState) //if it is the same state after the inertia period seconds fade
                    {
                        if (thread != null)
                        {
                            thread.Abort();
                        }
                        thread = new Thread(() => Fade(mixer, newMixerState)); //thread to fade in/out but also to check that the the state doesn't change as this happens
                        thread.IsBackground = true;
                        thread.Name = "Fader";
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



        private void Fade(VolumeMixer mixer, string newMixerState)
        {
            if(newMixerState == "FadeIn")
            {
                //volu
                mixer.FadeIn();
            }
            else if(newMixerState == "FadeOut")
            {
                mixer.FadeOut();
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
