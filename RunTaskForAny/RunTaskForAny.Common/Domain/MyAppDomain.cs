using RunTaskForAny.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    public class MyAppDomain : IDisposable
    {
        string modulePrefix = null;

        string domainName = null;
        string path = null;
        string filter = null;
        bool includeSubDir;

        AppDomain ad = null;
        AppDomainSetup ads = null;
        RemoteLoader obj = null;

        DirectoryMonitor dir_monitor = null;

        bool isLoading = false;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
        }

        void Init()
        {
            ads = new AppDomainSetup();
            ads.ApplicationName = domainName;
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            ads.PrivateBinPath = path;

            ////设置缓存目录
            //ads.CachePath = ads.ApplicationBase;
            ////获取或设置指示影像复制是打开还是关闭
            //ads.ShadowCopyFiles = "true";
            ////获取或设置目录的名称，这些目录包含要影像复制的程序集
            //ads.ShadowCopyDirectories = ads.ApplicationBase+";"+ads.PrivateBinPath;

            System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;

            ad = AppDomain.CreateDomain(domainName + "_AppDomain", adevidence, ads);
            obj = (RemoteLoader)ad.CreateInstanceFromAndUnwrap(System.IO.Path.Combine(ads.ApplicationBase, typeof(RemoteLoader).Module.Name), typeof(RemoteLoader).FullName);

        }

        void MonitorFiles()
        {
            Init();

            dir_monitor = new DirectoryMonitor(path, filter, includeSubDir);
            dir_monitor.Change += Dir_monitor_Change;
            dir_monitor.Start();

            var files = System.IO.Directory.GetFiles(path, filter, includeSubDir ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                Load(file);
            }
        }

        public MyAppDomain(string _domainName, string _path, string _modulePrefix, bool _includeSubDir = true)
        {
            domainName = _domainName;
            path = _path;
            modulePrefix = _modulePrefix;
            filter = _modulePrefix + "*.dll";
            includeSubDir = _includeSubDir;
            MonitorFiles();
        }

        private void Dir_monitor_Change(string _path)
        {
            try
            {
                isLoading = true;
                if (ad != null)
                {
                    Unload();
                }
                Init();
                var files = System.IO.Directory.GetFiles(path, filter, includeSubDir ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    Load(file);
                }
                isLoading = false;

            }
            catch (Exception ex)
            {
                isLoading = false;
                Tool.Log.Error(ex.Message + " " + ex.StackTrace);
            }

        }

        public void Load(string filename)
        {
            try
            {
                obj.Load(filename);
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex.Message + " " + ex.StackTrace);
            }

        }

        public ResInfoModel Exec(InfoModel infoModel)
        {
            try
            {
                if (isLoading || obj == null) { return new ResInfoModel() { Code = -1, Msg = "类库加载中,无法调用" }; }
                var rez = obj.Invoke(modulePrefix + infoModel.TypeName + "." + infoModel.ClassName, infoModel.MethodName, new Type[] { typeof(string) }, new object[] { infoModel.JsonData }, new object[] { infoModel }) as ResInfoModel;
                if (rez == null) { return new ResInfoModel() { Code = -2, Msg = "返回值不是指定的类型(返回值应该为ResInfoModel类)" }; }
                return rez;
            }
            catch (Exception ex)
            {
                return new ResInfoModel() { Code = -3, Msg = ex.Message + System.Environment.NewLine + ex.StackTrace + System.Environment.NewLine };
            }

        }

        public void StartAction()
        {
            foreach (var filename in obj.FileNameModules)
            {
                try
                {
                    obj.Invoke(filename + ".Action", "Start", new Type[] { }, new object[] { }, new object[] { new InfoModel() { TypeName = filename, ClassName = "Action", MethodName = "Start", JsonData = "", SendTime = DateTime.Now, PluginName = "模块插件", PluginVersion = 1, Client = Tool.Client } });
                }
                catch (Exception ex)
                {
                    Tool.Log.Error(ex.Message + " " + ex.StackTrace);
                }
            }
        }

        public void Unload()
        {
            if (ad != null)
            {
                obj.UnLoad();
                AppDomain.Unload(ad);
                ads = null;
                ad = null;
                obj = null;
                System.GC.Collect();
            }
        }

        public void Dispose()
        {
            Unload();
        }
    }
}
