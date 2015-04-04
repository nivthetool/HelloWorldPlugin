﻿using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using MFLibrary;
using System;
using System.ComponentModel;
using System.IO;

namespace MFPlugins
{
    public class HelloWorldPlugin : IMFChannelPlugin
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string MainConfigFile = "";
        private string MyConfigSection = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();

#if(DEBUG)
        /*
        * To run this plugin in debug mode change project properties 
        *  - "Output type" to "Console Application"
        *  - "Startup object" to this class
        * When done dont forget to switch back project properties 
        *  - "Output type" "Class Library"
        */
        static void Main(string[] args)
        {
            //NTT : let's configure log4net for coloredconsole output and debug trace
            //var layout = new log4net.Layout.SimpleLayout();
            //var layout = new log4net.Layout.PatternLayout("%-4timestamp [%thread] %-5level %logger %ndc - %message%newline");
            var layout = new log4net.Layout.PatternLayout("%date %-5level %message%newline");
            var ta = new TraceAppender { Layout = layout };
            var ca = new ColoredConsoleAppender { Threshold = Level.All, Layout = layout };
            var cca = new ColoredConsoleAppender { Threshold = Level.All, Layout = layout };
            cca.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Debug, ForeColor = ColoredConsoleAppender.Colors.Cyan | ColoredConsoleAppender.Colors.HighIntensity });
            cca.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Info, ForeColor = ColoredConsoleAppender.Colors.Green | ColoredConsoleAppender.Colors.HighIntensity });
            cca.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Warn, ForeColor = ColoredConsoleAppender.Colors.Purple | ColoredConsoleAppender.Colors.HighIntensity });
            cca.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Error, ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity });
            cca.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Fatal, ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity, BackColor = ColoredConsoleAppender.Colors.Red });
            cca.ActivateOptions();
            BasicConfigurator.Configure(cca);
            Logger l = (Logger)log.Logger;
            l.AddAppender(ta);


            HelloWorldPlugin plugin = new HelloWorldPlugin();
            
            string config_file = "test.ini";
            File.AppendAllText(config_file, "");
            
            plugin.Init(config_file);

            Track track = new Track();
            track.status = Track.TrackStatus.TRACK_LOST;
            track.time = DateTime.Now.ToString();
            track.channel_id = "ntt";
            track.artist = "Niv Maman";
            track.title = "";
            track.position = 0;
            track.duration = 111;
            track.length = 111;

            Console.WriteLine("options:\nr - OnRecognition\ns - OnSilence\nf - OnFingerprintChange\nt - OnTest\nor any other key to exit...");
            string code = Console.ReadLine();

            switch (code)
            {
                case "r":
                    plugin.OnRecognition(track);
                    break;
                case "s":
                    plugin.OnSilence(true, "ntt", "ntt", "ntt",Environment.MachineName);
                    break;
                case "f":
                    plugin.OnFingerprintChange("niv", "ntt");
                    break;
                case "t":
                    plugin.OnTest();
                    break;
            }
        }
#endif

        #region PLUGIN CONFIGURATION

        private bool _Enable = false;
        [Category("Settings")]
        [DisplayName("1. Enable")]
        [Description("Enable Me")]
        [ReadOnly(false)]
        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; WriteConfiguration(); }
        }

        #endregion

        private IMFChannelPluginHost _PluginHost;
        [Browsable(false)]
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
            return @"Hello World Plugin Version 1.0
This hello world plugin demonstrate how to write plugin to MFChannel.";
        }
        public void ReadConfiguration()
        {
            INIFile Ini = new INIFile(MainConfigFile);

            _Enable = Ini.GetValue(MyConfigSection, "Enable", false);
        }
        public void WriteConfiguration()
        {
            INIFile Ini = new INIFile(MainConfigFile);

            Ini.SetValue(MyConfigSection, "Enable", _Enable);

            Ini.Flush();
        }
        public void Init(string config_file)
        {
            try
            {
                log.DebugFormat("{0} Init: ",GetName());

                MainConfigFile = config_file;

                ReadConfiguration();

                //optional: this will cause writing the plugin to flush its configuration section to the file
                WriteConfiguration();
            
                //DO YOUR INIT STUFF DUDE
            
            
            
            }
            catch (Exception ex)
            {
                log.Error(GetName() + " init:", ex);
            }
        }

        public void OnRecognition(Track track)
        {
            if (!Enable)
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
