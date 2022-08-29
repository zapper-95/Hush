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
            Console.WriteLine(pID);
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
                                    Debug.WriteLine(audioMeterInformation.GetPeakValue());
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
                                        else if(newVolume > 100)
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




    }

}
