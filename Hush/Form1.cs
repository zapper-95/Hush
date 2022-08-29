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
using System.Threading;

namespace Hush
{

    public partial class Form1 : Form
    {
        private ProcessInfo TargetProcess;
        private ProcessInfo SpeakerProcess;
        string SpeakerText;
        string TargetText;
        int inertia_period = 3000;
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
            Process[] processes = Process.GetProcesses();
            processInfo = VolumeMixer.GetProcessInfo();
            foreach (var process in processInfo)
            {
                Console.WriteLine("Process ID: " + process.ID + " process name " + process.name);
                //processInfo.Add(new ProcessInfo(process.Id, process.MainWindowTitle));
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

        private void StartCounting()
        {
            int SpeakerID = 0, TargetID = 0;
            float priorMasterVolume = 0;
            string last_fade_state = "";
            while (hush)
            {
                if (SpeakerProcess != null && TargetProcess != null)
                {
                    SpeakerID = SpeakerProcess.ID;
                    TargetID = TargetProcess.ID;
                    try
                    {
                        Console.WriteLine(Process.GetProcessById(SpeakerID).ProcessName);
                        if (Process.GetProcessById(SpeakerID).ProcessName == SpeakerText && Process.GetProcessById(TargetID).ProcessName==TargetText) //checks that the process with that number corresponds to the text in the dropdown
                        {
                            bool speaker_noise = VolumeMixer.ProcessPlayingAudio(SpeakerID);
                            Console.WriteLine("Speaker noise " + speaker_noise);

                            if (speaker_noise && last_fade_state != "out")
                            {
                                Thread.Sleep(inertia_period);
                                if (VolumeMixer.ProcessPlayingAudio(SpeakerID))
                                {
                                    FadeOut(SpeakerID, TargetID, ref priorMasterVolume);
                                    last_fade_state = "out";
                                    Console.WriteLine("prior master volume " + priorMasterVolume);
                                }

                            }
                            else if(!speaker_noise && last_fade_state != "in")
                            {
                                Thread.Sleep(inertia_period);
                                if (!VolumeMixer.ProcessPlayingAudio(SpeakerID))
                                {
                                    FadeIn(SpeakerID, TargetID, priorMasterVolume);
                                    Console.WriteLine("prior master volume " + priorMasterVolume);
                                    last_fade_state = "in";
                                }

                            }
                        }

                    }
                    catch (Exception)
                    {

                        Debug.WriteLine("Process does not exist");
                    }
                }

            }
 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //code here https://stackoverflow.com/questions/20938934/controlling-applications-volume-by-process-id
            hush = true;
            TargetText = Target.Text;
            SpeakerText = Speaker.Text;
            var thread = new Thread(StartCounting);
            thread.IsBackground = true;
            thread.Start();
            if (hush)
            {

                //int SpeakerID = 0, TargetID = 0;

                //if (SpeakerProcess != null && TargetProcess != null)
                //{
                //    SpeakerID = SpeakerProcess.ID;
                //    TargetID = TargetProcess.ID;
                //    try
                //    {
                //        if (Process.GetProcessById(SpeakerID).MainWindowTitle == Speaker.Text && Process.GetProcessById(TargetID).MainWindowTitle == Target.Text) //checks that the process with that number corresponds to the text in the dropdown
                //        {
                //            bool speaker_noise = VolumeMixer.ProcessPlayingAudio(SpeakerID);

                //            if (speaker_noise)
                //            {
                //                FadeOut(SpeakerID, TargetID);
                //            }
                //        }

                //    }
                //    catch (Exception)
                //    {

                //        Debug.WriteLine("Process does not exist");
                //    }
                //}
            }


            
            
            
  

        }

        private void FadeOut(int SpeakerID, int TargetID, ref float priorMasterVolume) //method to reduce volume exponentially
        {
            /*
                                                            ----- EXPLANATION -----

             The method used here to fade out volume is to at fixed intervals, reduce the master volume by an exponential function.

                                     The exponential function grows by these time steps being its exponent
            More specifically  y = r^n , where n is the current time step, and y is the value to be subtracted from the master volume


            The sum of an exponential function from 1 to N is (1-r^N) / 1 - r
            We can set this equation equal to the current master volume

            Now we know after N time intervals, if we subtract r^n, we will eventually reduce the master volume to 0.

            The variables r, and the total volume reduction (TVR) i.e the 'current master volume', are fixed, so we can calculate the number of time steps by rearanging the equation for N.

            TVR = (1-r^N) / 1 - r  to make N the subject    N = (log(r-TVR(1-r)))/log(r) - 1

            
            The length of each time step will depend on how long we want the program to take to reduce the master volume to 0.
            Let us call the time to reduce the master volume to 0, t.

            t = alpha * N , where alpha is the length of each time step, and N is the number of time steps.

            We can solve alpha be rearranging the equation.

            */



            float r = 1.01f; 
            int t = 2;


            float tvr = VolumeMixer.GetVolume(TargetID) * 100; //easier to work with values 0 to 100 and r values look nicer


            priorMasterVolume = tvr;


            //derrive N
            int N = (int)Math.Floor((Math.Log(r - tvr * (1 - r)) / Math.Log(r)) - 1);

            Console.WriteLine("n " + N);
            //derrive alpha
            decimal alpha = decimal.Divide(t, N);
            Console.WriteLine("N " + N);
            Console.WriteLine("alpha " + alpha);

            //reduce the master volume
            for (int i = 1; i <= N; i++)
            {
                Thread.Sleep((int)Math.Round(alpha * (decimal)Math.Pow(10, 3)));
                float current_reduction = -(float)Math.Pow(r, i) / 100;
                VolumeMixer.ChangeVolume(TargetID, current_reduction);




            }


        }


        private void FadeIn(int SpeakerID, int TargetID, float priorMasterVolume) //method to reduce volume exponentially
        {
            /*
                                                            ----- EXPLANATION -----

             The method used here to fade out volume is to at fixed intervals, reduce the master volume by an exponential function.

                                     The exponential function grows by these time steps being its exponent
            More specifically  y = r^n , where n is the current time step, and y is the value to be subtracted from the master volume


            The sum of an exponential function from 1 to N is (1-r^N) / 1 - r
            We can set this equation equal to the current master volume

            Now we know after N time intervals, if we subtract r^n, we will eventually reduce the master volume to 0.

            The variables r, and the total volume reduction (TVR) i.e the 'current master volume', are fixed, so we can calculate the number of time steps by rearanging the equation for N.

            TVR = (1-r^N) / 1 - r  to make N the subject    N = (log(r-TVR(1-r)))/log(r) - 1

            
            The length of each time step will depend on how long we want the program to take to reduce the master volume to 0.
            Let us call the time to reduce the master volume to 0, t.

            t = alpha * N , where alpha is the length of each time step, and N is the number of time steps.

            We can solve alpha be rearranging the equation.

            */

            float r = 1.01f;
            int t = 5;



            if(VolumeMixer.GetVolume(TargetID) * 100 < priorMasterVolume) //only increase the volume if it is less than what the volume was fading it out
            {
                float tv = priorMasterVolume;
                //derrive N
                int N = (int)Math.Round((Math.Log(r - tv * (1 - r)) / Math.Log(r)) - 1);

                //derrive alpha
                decimal alpha = decimal.Divide(t, N);
                Console.WriteLine("N " + N);
                Console.WriteLine("alpha " + alpha);

                //increase the master volume
                for (int i = 1; i <= N; i++)
                {
                    Thread.Sleep((int)Math.Round(alpha * (decimal)Math.Pow(10, 3)));
                    float current_increase = (float)Math.Pow(r, i) / 100;
                    VolumeMixer.ChangeVolume(TargetID, current_increase);
                }
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
