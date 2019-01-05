using RunTaskForAny.Common.Domain;
using RunTaskForAny.Common.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RunTaskForAny.Helper
{
    public class MyTask
    {
        Task task;
        CancellationTokenSource cancellationTokenSource;
        Stopwatch stopwatch;

        MyAppDomain mydomain = null;
        public MyTask()
        {
            mydomain = new MyAppDomain("Shadow", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules"), "RunTaskForAny.Module.");

            stopwatch = new Stopwatch();
            cancellationTokenSource = new CancellationTokenSource();
            task = new Task(DoWork, cancellationTokenSource.Token);

            //TaskManager.Start();
        }

        public void Start()
        {
            if (task.IsCompleted || cancellationTokenSource.Token.IsCancellationRequested)
            {
                Tool.Log.Debug("准备重新创建任务");
                stopwatch = new Stopwatch();
                cancellationTokenSource = new CancellationTokenSource();
                task = new Task(DoWork, cancellationTokenSource.Token);
                Tool.Log.Debug("任务创建完成");
            }
            task.Start();
            Tool.Log.Debug("任务开始");
        }

        public void Stop()
        {
            Tool.Log.Debug("准备取消任务");
            cancellationTokenSource.Cancel();
            task.Wait();
            Tool.Log.Debug("任务结束");
        }

        private void DoWork()
        {
            try
            {
                Tool.Log.Debug("准备开始");
                stopwatch.Restart();
                mydomain.StartAction();
                //while (!cancellationTokenSource.Token.IsCancellationRequested)
                //{
                //    try
                //    {
                //        if (mydomain.IsLoading)
                //        {
                //            Tool.Log.Debug("正在加载类库..");
                //            System.Threading.Thread.Sleep(100);
                //            continue;
                //        }

                //        ResInfoModel resInfoModel = mydomain.Exec(new InfoModel() { TypeName = "Test", ClassName = "Class1", MethodName = "Init", JsonData = "", SendTime = DateTime.Now, PluginName = "测试调用", PluginVersion = 1, Client = Tool.Client });

                //        Tool.Log.Debug(resInfoModel.Msg);
                //        System.Threading.Thread.Sleep(100);
                //    }
                //    catch (Exception ex)
                //    {
                //        Tool.Log.Error("错误信息:" + ex.Message);
                //        Tool.Log.Error(ex.StackTrace);
                //    }

                //}
                stopwatch.Stop();
                Tool.Log.Debug("工作中..");
                //Tool.Log.Debug("任务运行时间:" + stopwatch.Elapsed.Days + "天" + stopwatch.Elapsed.Hours + "小时" + stopwatch.Elapsed.Minutes + "分" + stopwatch.Elapsed.Seconds + "秒" + stopwatch.Elapsed.Milliseconds + "毫秒");
            }
            catch (Exception ex)
            {
                Tool.Log.Error("错误信息:" + ex.Message+" "+ex.StackTrace);
            }
           
        }
    }
}
