using System;
using System.Linq;
using System.Threading;
using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Hush
{

    static class VolumeMixer
    {
        //https://stackoverflow.com/questions/27297577/get-processname-or-id-from-cscore-audiostream
        //https://stackoverflow.com/questions/21200825/getting-individual-windows-application-current-volume-output-level-as-visualized
        // https://github.com/filoe/cscore/blob/master/CSCore.Test/CoreAudioAPI/AudioSessionTests.cs
        private static double threshold_noise = 0.00001;
        public static bool ProcessPlayingAudio(int pID)
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
                                //Debug.WriteLine(session2.Process.ProcessName);
                                if(pID == session2.ProcessID)
                                {
                                    //Debug.WriteLine(audioMeterInformation.GetPeakValue());
                                    if((double)audioMeterInformation.GetPeakValue() > threshold_noise)
                                    {
                                        return true;
                                    }
                                    return false;
                                }
                            }
                        }
                    }
                }
                return false;
            }

        }

        public static float GetVolume(int pID)
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

                                //Debug.WriteLine(session2.Process.ProcessName);
                                if (pID == session2.ProcessID)
                                {
                                    //Debug.WriteLine(audioMeterInformation.);
                                    using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                                    {

                                        float volume = simpleVolume.MasterVolume;
                                        return volume;

                                    }
                                    
                                }
                            }
                        }
                    }
                }
                return 0;
            }

        }

        public static List<ProcessInfo>  GetProcessInfo()
        {
            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    List<ProcessInfo> processInfo = new List<ProcessInfo>();
                    foreach (var session in sessionEnumerator)
                    {
                        using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                        {
                            using (var session2 = session.QueryInterface<AudioSessionControl2>())
                            {

                                processInfo.Add(new ProcessInfo(session2.Process.Id, session2.Process.ProcessName));
                            }
                        }

                    }
                    return processInfo;
                }
            }
        }


        public static void ChangeVolume(int pID, float volume_change)
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
                                    using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                                    {

                                        float volume = simpleVolume.MasterVolume;
                                        float newVolume = simpleVolume.MasterVolume + volume_change;
                                        if (newVolume < 0)
                                        {
                                            simpleVolume.MasterVolume = 0;
                                        }
                                        else if(newVolume > 1)
                                        {
                                            simpleVolume.MasterVolume = 100;
                                        }
                                        else 
                                        {
                                            simpleVolume.MasterVolume = newVolume;
                                        }
                                        

                                    }
                                }
                            }
                        }
                       
                       
                    }
                }
            }
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
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


        public static string FindMixerState(int speakerID, int targetID)
        {
            string mixState = "";

            bool speakerOn = ProcessPlayingAudio(speakerID);
            bool targetOn = ProcessPlayingAudio(targetID);


            if (speakerOn && targetOn)
            {
                mixState =  "FadeOut";
            }
            else if (!speakerOn && targetOn)
            {
                mixState = "FadeIn";
            }
            return mixState;
        }



        public static void FadeOut(int SpeakerID, int TargetID) //method to reduce volume exponentially
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
            int t = 4;


            float tvr = GetVolume(TargetID) * 100; //easier to work with values 0 to 100 and r values look nicer



            //derrive N
            int N = (int)Math.Floor((Math.Log(r - tvr * (1 - r)) / Math.Log(r)) - 1);
            if(N != 0)
            {
                //Console.WriteLine("n " + N);
                //derrive alpha
                decimal alpha = decimal.Divide(t, N);
                //Console.WriteLine("N " + N);
                //Console.WriteLine("alpha " + alpha);

                //reduce the master volume
                for (int i = 1; i <= N; i++)
                {
                    Thread.Sleep((int)Math.Round(alpha * 1000));
                    float current_reduction = -(float)Math.Pow(r, i) / 100; //return values back between 1 and 0 for actual reduction
                    ChangeVolume(TargetID, current_reduction);




                }
            }



        }


        public static void FadeIn(int SpeakerID, int TargetID, float target_loud_volume) //method to reduce volume exponentially
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
            int t = 4;



            if (GetVolume(TargetID) < target_loud_volume) //only increase the volume if it is less than what the volume was fading it out
            {
                float tv = target_loud_volume - GetVolume(TargetID);
                //derrive N
                tv *= 100;
                int N = (int)Math.Round((Math.Log(r - tv * (1 - r)) / Math.Log(r)) - 1);
                if (N != 0)
                {
                    //derrive alpha
                    decimal alpha = decimal.Divide(t, N);
                    //Console.WriteLine("N " + N);
                    //Console.WriteLine("alpha " + alpha);

                    //increase the master volume
                    for (int i = 1; i <= N; i++)
                    {
                        Thread.Sleep((int)Math.Round(alpha * (decimal)Math.Pow(10, 3)));
                        float current_increase = (float)Math.Pow(r, i) / 100;
                        ChangeVolume(TargetID, current_increase);
                    }
                }

            }



        }


    }

}
