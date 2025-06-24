// Plugin.cs

using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using LycheeLib.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

namespace LyricsComponent
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            AppBase.Current.AppStarted += (_,_) =>
            {
                Rendezvous.Load(IAppHost.GetService<ILycheeLyrics>());
            };
            services.AddComponent<LyricsControl>();
            services.AddComponent<ExtraLyricsControl>();
        }
    }
}