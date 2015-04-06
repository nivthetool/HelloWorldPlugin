using MFLibrary;
using System;
using System.ComponentModel;
using System.Reflection;

namespace MFPlugins
{
    public class HelloWorldPlugin : IMFChannelPlugin
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Settings Settings = Settings.GetInstance();

        private IMFChannelPluginHost _PluginHost;
        public IMFChannelPluginHost PluginHost
        {
            get { return _PluginHost; }
            set
            {
                _PluginHost = value;
                _PluginHost.Register(this);
            }
        }
        public string GetName()
        {
            return "Hello World Plugin";
        }
        public string GetDescription()
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            string description = GetName();
            description += Environment.NewLine + string.Format("Version {0}.{1}.{2} Revision {3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
            description += Environment.NewLine + "Hello world plugin demonstrate how to write plugin to MFChannel.";
            return description;
        }
        public object GetSettings() { return Settings; }
        public void Init()
        {
            try
            {
                log.DebugFormat("{0} Init: ",GetName());
                Settings.plugin = this;
                //DO YOUR INIT STUFF DUDE
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " init:", ex);
            }
        }
        public void OnRecognition(Track track)
        {
            if (!Settings.Enable)
                return;

            try
            {
                log.DebugFormat("{0} OnRecognition: {1}, {2} {3} {4} {5} {6}", GetName(), track.status, track.time, track.channel_name, track.artist, track.title, track.duration);
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " OnRecognition:", ex);
            }
        }
        public void OnSilence(bool is_silence, string audio_device_name, string chnnel_mf_id, string channel_name, string machine_name)
        {
            try
            {
                log.DebugFormat("{0} OnSilence: {1} {2} {3}", GetName(), is_silence ? "detected on" : "removed from", channel_name, machine_name);
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " OnSilence:", ex);
            }
        }
        public void OnFingerprintChange(string old_path, string new_path)
        {
            try
            {
                log.DebugFormat("{0} OnFingerprintChange: from {1} to {2}", GetName(), old_path, new_path);
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " OnFingerprintChange:", ex);
            }
        }
        public bool OnTest()
        {
            try
            {
                log.DebugFormat("{0} OnTest: + ", GetName());
                return true;
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " OnTest:", ex);
            }

            return false;
        }
        public void OnExit()
        {
            try
            {
                log.DebugFormat("{0} OnExit: + ", GetName());
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " OnExit:", ex);
            }
        }
    }
}
