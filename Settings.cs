using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace MFPlugins
{
    public enum NTTEnum { Niv, The, Tool };
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Settings
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string _json_file_path = Path.ChangeExtension(typeof(Settings).Assembly.Location, ".config.json");
        
        private static Settings _this = null;
        
        [Browsable(false)]
        public static HelloWorldPlugin plugin = null;
        
        private Settings()
        {
        }

        [Category("Version Info")]
        [DisplayName("Version")]
        [Description("plugin version info")]
        [ReadOnly(true)]
        [JsonIgnore]
        public string Version
        {
            get {
                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}.{2} Revision {3}", ver.Major, ver.Minor, ver.Build, ver.Revision); }
        }
        private bool _Enable = false;
        [Category("Settings")]
        [DisplayName("1. Enable")]
        [Description("Enable Me")]
        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; Save(); }
        }

        private string _Param1 = "Parameter 1";
        [Category("General")]
        [DisplayName("Parameter 1")]
        [Description("parameter 1 is a string")]
        public string Param1
        {
            get { return _Param1; }
            set { _Param1 = value; Save(); }
        }

        private int _Param2 = 222;
        [Category("General")]
        [DisplayName("Parameter 2")]
        [Description("parameter 2 is an integer and it is read only")]
        [ReadOnly(true)]
        public int Param2
        {
            get { return _Param2; }
            set { _Param2 = value; Save(); }
        }

        private NTTEnum _Param3 = NTTEnum.Niv;
        [Category("General")]
        [DisplayName("Parameter 3")]
        [Description("parameter 3 is an enum")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NTTEnum Param3
        {
            get { return _Param3; }
            set { _Param3 = value; Save(); }
        }

        private MyClass _Param4 = new MyClass();
        [Category("General")]
        [DisplayName("Parameter 4")]
        [Description("parameter 4 is an object")]
        public MyClass Param4
        {
            get { return _Param4; }
            set { _Param4 = value; Save(); }
        }

        private List<MyClass> _Param5 = new List<MyClass>();
        [Category("General")]
        [DisplayName("Parameter 5")]
        [Description("parameter 5 is an array of objects")]
        [Editor(typeof(NTTCollectionTypeEditor), typeof(UITypeEditor))]
        public List<MyClass> Param5
        {
            get { return _Param5; }
            set { _Param5 = value; Save(); }
        }


        public static Settings GetInstance()
        {
            if (_this != null)
                return _this;

            log.Info("reading " + _json_file_path.ToLower());
            if (File.Exists(_json_file_path))
            {
                _this = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_json_file_path));
            }
            else
            {
                _this = new Settings();
            }
            _this.Save();
            //Watch();
            return _this;
        }
        public void Save()
        {
            log.Info("writing " + _json_file_path.ToLower());
            File.WriteAllText(_json_file_path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        public static void Watch()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Directory.GetCurrentDirectory();
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Filter = _json_file_path;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            log.Info(_json_file_path + " changed (outside). reloading configuration");
            _this = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_json_file_path));
        }

    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MyClass
    {
        public string param1 { get; set; }
        public int param2 { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class NTTCollectionTypeEditor : CollectionEditor
    {
        public NTTCollectionTypeEditor(Type type) : base(type) { }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            object result = null;
            if (value is List<MyClass>)
            {
                result = base.EditValue(context, provider, value);
                // assign the temporary collection from the UI to the property
                ((Settings)context.Instance).Param5 = (List<MyClass>)result;
            }
            return result;
        }
    }
}
