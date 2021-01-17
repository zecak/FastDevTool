using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Common
{
    public enum ResourceDictionaryType
    {
        None = 0,
        Language = 1,
        Theme = 2
    }
    public class ResourceDictionaryInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public ResourceDictionaryType ResourceType { get; set; }
        public ResourceDictionary Dictionary { get; set; }
    }

    public class ResourceDictionaryHelper
    {
        static List<ResourceDictionaryInfo> resourceDictionaries;
        public static List<ResourceDictionaryInfo> ResourceDictionaries
        {
            get
            {
                if (resourceDictionaries == null)
                {
                    resourceDictionaries = new List<ResourceDictionaryInfo>();

                    {
                        //加载默认语言
                        var dict = new ResourceDictionary();
                        dict.Source = new Uri(@"Language\中文.xaml", UriKind.Relative);
                        var languageInfo = new ResourceDictionaryInfo() { ResourceType = ResourceDictionaryType.Language, Name = "中文", Title = "中文", FilePath = @"Language\中文.xaml", Dictionary = dict };
                        resourceDictionaries.Add(languageInfo);
                        //加载所有语言包
                        var langList = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Language"));
                        foreach (var lang in langList)
                        {
                            var fileinfo = new FileInfo(lang);
                            using (FileStream fs = new FileStream(lang, FileMode.Open))
                            {
                                var rootElement = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);
                                resourceDictionaries.Add(new ResourceDictionaryInfo() { ResourceType = ResourceDictionaryType.Language, Name = fileinfo.Name.Replace(fileinfo.Extension, ""), Title = fileinfo.Name.Replace(fileinfo.Extension, ""), FilePath = lang, Dictionary = rootElement });
                            }
                        }
                    }

                    {
                        //加载默认皮肤
                        var dict = new ResourceDictionary();
                        dict.Source = new Uri(@"Themes\Default.xaml", UriKind.Relative);
                        var languageInfo = new ResourceDictionaryInfo() { ResourceType = ResourceDictionaryType.Theme, Name = "Default", Title = "Default", FilePath = @"Themes\Default.xaml", Dictionary = dict };
                        resourceDictionaries.Add(languageInfo);
                        //加载所有皮肤包
                        var langList = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes"));
                        foreach (var lang in langList)
                        {
                            var fileinfo = new FileInfo(lang);
                            using (FileStream fs = new FileStream(lang, FileMode.Open))
                            {
                                var rootElement = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);
                                resourceDictionaries.Add(new ResourceDictionaryInfo() { ResourceType = ResourceDictionaryType.Theme, Name = fileinfo.Name.Replace(fileinfo.Extension, ""), Title = fileinfo.Name.Replace(fileinfo.Extension, ""), FilePath = lang, Dictionary = rootElement });
                            }
                        }
                    }

                    SetDefault(LanguageDefault, ThemeDefault);
                }
                return resourceDictionaries;
            }
        }
        static ResourceDictionaryInfo languageInfo;
        public static ResourceDictionaryInfo LanguageDefault
        {
            get
            {
                if (languageInfo == null)
                {
                    languageInfo = ResourceDictionaries.FirstOrDefault(l => l.ResourceType == ResourceDictionaryType.Language);
                }
                return languageInfo;
            }
            set
            {
                languageInfo = value;
            }
        }

        static ResourceDictionaryInfo themeInfo;
        public static ResourceDictionaryInfo ThemeDefault
        {
            get
            {
                if (themeInfo == null)
                {
                    themeInfo = ResourceDictionaries.FirstOrDefault(l => l.ResourceType == ResourceDictionaryType.Theme);
                }
                return themeInfo;
            }
            set
            {
                themeInfo = value;
            }
        }

        public static void SetDefault(ResourceDictionaryInfo language, ResourceDictionaryInfo theme)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            if (language != null)
            {
                LanguageDefault = language;
                Application.Current.Resources.MergedDictionaries.Add(language.Dictionary);
            }
            if (theme != null)
            {
                ThemeDefault = theme;
                Application.Current.Resources.MergedDictionaries.Add(theme.Dictionary);
            }
        }

    }
}
