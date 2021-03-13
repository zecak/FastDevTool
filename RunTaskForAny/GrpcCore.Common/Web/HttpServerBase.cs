using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcCore.Common.Web
{
    // HTTP服务类
    public abstract class HttpServerBase : IDisposable
    {
        private readonly HttpListener _listener;                        // HTTP 协议侦听器
        private readonly Thread _listenerThread;                        // 监听线程
        private readonly Thread[] _workers;                             // 工作线程组
        private readonly ManualResetEvent _stop, _ready;                // 通知停止、就绪
        private Queue<HttpListenerContext> _queue;                      // 请求队列
        private event Action<HttpListenerContext> ProcessRequest;       // 请求处理委托

        public HttpServerBase(int maxThreads)
        {
            _workers = new Thread[maxThreads];
            _queue = new Queue<HttpListenerContext>();
            _stop = new ManualResetEvent(false);
            _ready = new ManualResetEvent(false);
            _listener = new HttpListener();
            _listenerThread = new Thread(HandleRequests);
        }

        public void Start(int port)
        {
            // 注册处理函数
            ProcessRequest += ProcessHttpRequest;

            // 启动Http服务
            _listener.Prefixes.Add(String.Format("http://*:{0}/", port));

            try
            {
                _listener.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            _listenerThread.Start();

            // 启动工作线程
            for (int i = 0; i < _workers.Length; i++)
            {
                _workers[i] = new Thread(Worker);
                _workers[i].Start();
            }
        }

        // 请求处理函数
        protected abstract void ProcessHttpRequest(HttpListenerContext ctx);

        // 释放资源
        public void Dispose()
        {
            Stop();
        }

        // 停止服务
        public void Stop()
        {
            _stop.Set();
            _listenerThread.Join();
            foreach (Thread worker in _workers)
            {
                worker.Join();
            }
            _listener.Stop();
        }

        // 处理请求
        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                var context = _listener.BeginGetContext(ContextReady, null);
                if (0 == WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }))
                {
                    return;
                }
            }
        }

        // 请求就绪加入队列
        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (_queue)
                {
                    _queue.Enqueue(_listener.EndGetContext(ar));
                    _ready.Set();
                }
            }
            catch (Exception ex)
            {
                //Helper.Log.Error(ex);
            }
        }

        // 处理一个任务
        private void Worker()
        {
            WaitHandle[] wait = new[] { _ready, _stop };
            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext context;
                lock (_queue)
                {
                    if (_queue.Count > 0)
                        context = _queue.Dequeue();
                    else
                    {
                        _ready.Reset();
                        continue;
                    }
                }

                try
                {
                    ProcessRequest(context);
                }
                catch (Exception ex)
                {
                    //Helper.Log.Error(ex);
                }
            }
        }
    }

}
