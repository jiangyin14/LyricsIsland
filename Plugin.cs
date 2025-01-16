using ClassIsland.Core.Attributes;
using ClassIsland.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.Hosting;
using System;

namespace HitokotoComponent
{
    [PluginEntrance]
    public class Plugin : PluginBase, IDisposable
    {
        private bool _disposed = false;
        private readonly HttpListenerServer _httpListenerServer;

        public Plugin()
        {
            _httpListenerServer = new HttpListenerServer("http://localhost:50063/taskbar");
        }

        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            services.AddComponent<HitokotoControl>();
            _httpListenerServer.Start();
        }

        // 实现 IDisposable 接口
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // 重写 Dispose 方法
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 在这里释放托管资源
                    _httpListenerServer.Stop();
                }

                // 在这里释放非托管资源

                _disposed = true;
            }
        }
    }
}