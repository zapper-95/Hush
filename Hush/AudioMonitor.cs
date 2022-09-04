using System;
using System.Linq;
using System.Threading;
using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hush
{
    class AudioMonitor : IDisposable //monitors the audio for a particular process
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        int pID;
        Task task;
        static double threshold_noise = 0.00001;
        private bool playing = false;

        public AudioMonitor(int pID)
        {
            this.pID = pID;
            task = new Task(() => ProcessPlayingAudio(pID, ref playing), cts.Token, TaskCreationOptions.LongRunning);
            task.Start();
            //thread = new Thread(() => ProcessPlayingAudio(pID, ref playing)); //starts a new thread that constantly checks if the audio is playing
            //thread.Name = pID + " audio monitor";
            //thread.IsBackground = true;
            //thread.Start();
        }

        public void ChangeProcess(int pID)
        {
            this.pID = pID;
        }
        private bool ProcessPlayingAudio(int pID, ref bool playing)
        {
            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {


                        foreach (var session in sessionEnumerator)
                        {
                            using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                            {
                                using (var session2 = session.QueryInterface<AudioSessionControl2>())
                                {
                                    if (pID == session2.ProcessID)
                                    {
                                        while (!cts.IsCancellationRequested)
                                        {
                                                //Debug.WriteLine(audioMeterInformation.GetPeakValue());
                                                if ((double)audioMeterInformation.GetPeakValue() > threshold_noise)
                                                {
                                                    playing = true;
                                                }
                                                else
                                                {
                                                    playing = false;
                                                }

                                        }

                                    }

                                    //Debug.WriteLine(session2.Process.ProcessName);

                                }
                            }
  
                        }

                }
                return false;
            }

        }

        public void Dispose()
        {
            cts.Cancel();
        }
        private AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }

        public bool IsPlaying()
        {
            return playing;
        }




    }

}
