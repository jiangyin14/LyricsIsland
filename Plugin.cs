using ClassIsland.Core.Attributes;
using ClassIsland.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.Hosting;

namespace HitokotoComponent
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
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

        public override void Dispose()
        {
            _httpListenerServer.Stop();
        }
    }
}