using System;
using System.ServiceProcess;
using System.Threading;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientService : ServiceBase
    {
        private const string ServiceDisplayName = "XHS 打印客户端服务";
        private readonly object _timerLock = new object();
        private Timer _timer;
        private PrintClientWorker _worker;
        private PrintClientFileLogger _logger;
        private PrintClientSettings _settings;

        public PrintClientService()
        {
            ServiceName = "XHSPrintClientService";
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            StartWorker();
        }

        protected override void OnStop()
        {
            StopWorker();
        }

        public void StartAsConsole()
        {
            StartWorker();
        }

        public void StopAsConsole()
        {
            StopWorker();
        }

        private void StartWorker()
        {
            lock (_timerLock)
            {
                if (_timer != null)
                {
                    return;
                }

                _settings = PrintClientSettings.Load();
                _logger = new PrintClientFileLogger(_settings.LogFolder);
                _worker = new PrintClientWorker(_settings, _logger.Write, _logger.WriteFetch);

                _logger.Write(ServiceDisplayName + "已启动，轮询间隔 " + _settings.PollIntervalSeconds + " 秒。");
                _timer = new Timer(ProcessPrintTasks, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.PollIntervalSeconds));
            }
        }

        private void StopWorker()
        {
            lock (_timerLock)
            {
                if (_timer != null)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer.Dispose();
                    _timer = null;
                }

                if (_logger != null)
                {
                    _logger.Write(ServiceDisplayName + "已停止。");
                }

                _worker = null;
            }
        }

        private void ProcessPrintTasks(object state)
        {
            PrintClientWorker currentWorker = _worker;
            if (currentWorker == null)
            {
                return;
            }

            currentWorker.ProcessOnce();
        }
    }
}
