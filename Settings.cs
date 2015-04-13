using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Newtonsoft.Json.Converters;
using System.Reflection;
using MFLibrary;

namespace HelloWorldPlugin
{
    public enum NTTEnum { Niv, The, Tool };
    public class Settings : NTTAppSettings<Settings>
    {
        public Settings()
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            Version = string.Format("{0}.{1}.{2} Revision {3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
            MyInt = 222;
            MyString = "Niv The Tool";
            MyEnum = NTTEnum.Niv;
            MyObject = new MyClass();
            MyArray = new List<MyClass>();
        }

        [Browsable(false)]
        public static HelloWorldPlugin plugin = null;

        [Category("Version Info")]
        [DisplayName("Version")]
        [Description("plugin version info")]
        [ReadOnly(true)]
        [JsonIgnore]
        public string Version { get; set; }

        [Category("Settings")]
        [DisplayName("1. Enable")]
        [Description("Enable Me")]
        public bool Enable { get; set; }

        [Category("My Category")]
        [DisplayName("1. My String")]
        [Description("parameter is a string")]
        public string MyString { get; set; }

        [Category("My Category")]
        [DisplayName("2. My Int")]
        [Description("parameter is an integer and it is read only")]
        [ReadOnly(true)]
        public int MyInt { get; set; }

        [Category("My Category")]
        [DisplayName("3. My Enum")]
        [Description("parameter is an enum")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NTTEnum MyEnum { get; set; }

        [Category("My Category")]
        [DisplayName("4. My Object")]
        [Description("parameter is an object")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MyClass MyObject { get; set; }

        [Category("My Category")]
        [DisplayName("5. My Array")]
        [Description("parameter is an array of objects")]
        [Editor(typeof(NTTCollectionTypeEditor), typeof(UITypeEditor))]
        public List<MyClass> MyArray { get; set; }
    }
    public class MyClass
    {
        public string param1 { get; set; }
        public int param2 { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
    //due to the fact that propertygrid does not fire change event on collection we need the following shit
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
                ((Settings)context.Instance).MyArray = (List<MyClass>)result;
                Settings.Save();
            }
            return result;
        }
    }
}
