using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using MFLibrary;
using System;
using System.Windows.Forms;

namespace MFPlugins
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        /*
        * To run this plugin in debug mode change project properties 
        *  - "Output type" to "Console Application"
        *  - "Startup object" to this class
        * When done dont forget to switch back project properties 
        *  - "Output type" "Class Library"
        */
        static void Main(string[] args)
        {
            InitLogger();

            HelloWorldPlugin plugin = new HelloWorldPlugin();

            plugin.Init();

            Application.EnableVisualStyles();
            Application.Run(new Form
            {
                Controls = { new PropertyGrid { Dock = DockStyle.Fill,
                                            SelectedObject = plugin.Settings
                            }
                }
            });

            
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
                    plugin.OnSilence(true, "ntt", "ntt", "ntt", Environment.MachineName);
                    break;
                case "f":
                    plugin.OnFingerprintChange("niv", "ntt");
                    break;
                case "t":
                    plugin.OnTest();
                    break;
            }
        }
        public static void InitLogger()
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
        }


    }
}
