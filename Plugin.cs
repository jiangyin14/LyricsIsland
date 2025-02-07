// Plugin.cs

using ClassIsland.Core.Attributes;
using ClassIsland.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ClassIsland.Core.Extensions.Registry;
using HitokotoComponent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

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
            _httpListenerServer.Start();
            services.AddComponent<LyricsControl,InformationPage>();
            services.AddComponent<ExtraLyricsControl,InformationPage>();
        }
    }
}