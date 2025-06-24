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
            InitializeComponent();
            LycheeLib.Interface.Rendezvous.OnLyricsChanged += UpdateLyrics;
        }

        private void UpdateLyrics(List<string> lyrics)
        {
            UpdateLyricOnUI(lyrics[0]);
        }
        
        private void UpdateLyricOnUI(string lyric)
        {
            Dispatcher.Invoke(() => LyricsText.Text = lyric);
        }

        public void Dispose()
        {
            LycheeLib.Interface.Rendezvous.OnLyricsChanged -= UpdateLyrics;
        }
    }
}
