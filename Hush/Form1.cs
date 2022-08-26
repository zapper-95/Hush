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
        private ProcessInfo TargetProcess;
        private ProcessInfo SpeakerProcess;
        private List<ProcessInfo> processInfo = new List<ProcessInfo>();

        public Form1()
        {
            InitializeComponent();
            RefreshProcesses();
        }


        public void RefreshProcesses()
        {
            processInfo.Clear();
            Process[] processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                processInfo.Add(new ProcessInfo(process.Id, process.MainWindowTitle));
            }

            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                Console.WriteLine("Chrome is not running");
            }
            else
            {
                foreach (Process proc in procsChrome)
                {
                    // the chrome process must have a window 
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }
                    AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);

                    string windowTitle = proc.MainWindowTitle;
                    windowTitle = windowTitle.Remove(windowTitle.Length - 16); //gets rid of the 16 characters that is associated with the appened " - Google Chrome"
                    Condition condNewTab = new PropertyCondition(AutomationElement.NameProperty, windowTitle);
                    AutomationElement elmNewTab = root.FindFirst(TreeScope.Subtree, condNewTab);
                    if(elmNewTab == null)
                    {
                        //elmNewTab = root.
                    }
                    // get the tabstrip by getting the parent of the 'new tab' button 
                    TreeWalker treewalker = TreeWalker.ControlViewWalker;
                    AutomationElement elmTabStrip = treewalker.GetParent(elmNewTab);
                    // loop through all the tabs and get the names which is the page title 
                    Condition condTabItem = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                    foreach (AutomationElement tabitem in elmTabStrip.FindAll(TreeScope.Children, condTabItem))
                    {
                        processInfo.Add(new ProcessInfo(proc.Id, tabitem.Current.Name));
                        Console.WriteLine(tabitem.Current.Name);
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



        private void button1_Click(object sender, EventArgs e)
        {
            //code here https://stackoverflow.com/questions/20938934/controlling-applications-volume-by-process-id
            int SpeakerID = 0, TargetID = 0;

            if (SpeakerProcess != null && TargetProcess != null)
            {
                SpeakerID = SpeakerProcess.ID;
                TargetID = TargetProcess.ID;
                try
                {                  
                    if (Process.GetProcessById(SpeakerID).MainWindowTitle == Speaker.Text && Process.GetProcessById(TargetID).MainWindowTitle == Target.Text) //checks that the process with that number corresponds to the text in the dropdown
                    {
                        bool speaker_noise = VolumeMixer.ProcessPlayingAudio(SpeakerID);
                        speaker_noise = true;
                        if (speaker_noise)
                        {
                            VolumeMixer.ChangeVolume(TargetID, -0.2f);
                        }
                    }

                }
                catch (Exception)
                {

                    throw;
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


        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshProcesses();
        }
    }
    



}
