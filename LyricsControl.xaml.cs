using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace LyricsComponent
{
    [ComponentInfo(
        "8214141d-6b3b-4ad3-9674-389845493492",
        "歌词",
        PackIconKind.MusicNote,
        "在主界面上显示来自音乐软件的歌词。"
    )]
    public partial class LyricsControl : ComponentBase, IDisposable
    {
        public LyricsControl()
        {
            HttpListenerServer.Current ??= new HttpListenerServer();
            _listener = HttpListenerServer.Current;
            InitializeComponent();
            _listener.OnLyricsReceived += UpdateLyrics;
        }

        readonly HttpListenerServer _listener;
        
        private void UpdateLyricOnUI(string lyric)
        {
            Dispatcher.Invoke(() => LyricsText.Text = lyric);
        }

        void UpdateLyrics() {
            UpdateLyricOnUI(_listener.Lyrics);
        }

        /// <summary>
        /// 释放资源并停止 HTTP 监听器。
        /// </summary>
        public void Dispose()
        {
            _listener.OnLyricsReceived -= UpdateLyrics;
        }
    }
}
