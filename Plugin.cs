// Plugin.cs

using ClassIsland.Core.Attributes;
using ClassIsland.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.Hosting;

namespace LyricsComponent
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        private readonly HttpListenerServer _httpListenerServer;
        public Plugin()
        {
            _httpListenerServer = new HttpListenerServer("http://localhost:50063/");
        }
        
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            services.AddComponent<LyricsControl>();
            _httpListenerServer.Start();
        }
    }
}