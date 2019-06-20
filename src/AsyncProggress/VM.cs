using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    public class VM 
    {
        private static readonly string[] URLS =
            {
            "https://source.unsplash.com/1200x1200/?dog",
            "https://source.unsplash.com/1200x1200/?cat",
                "http://r.ddmcdn.com/s_f/o_1/cx_633/cy_0/cw_1725/ch_1725/w_720/APL/uploads/2014/11/too-cute-doggone-it-video-playlist.jpg",
                "https://cdn.pixabay.com/photo/2016/02/19/15/46/dog-1210559_960_720.jpg",
                "http://r.ddmcdn.com/s_f/o_1/cx_633/cy_0/cw_1725/ch_1725/w_720/APL/uploads/2014/11/too-cute-doggone-it-video-playlist.jpg",
                "https://cdn.pixabay.com/photo/2016/02/19/15/46/dog-1210559_960_720.jpg",
                "http://r.ddmcdn.com/s_f/o_1/cx_633/cy_0/cw_1725/ch_1725/w_720/APL/uploads/2014/11/too-cute-doggone-it-video-playlist.jpg",
                "https://cdn.pixabay.com/photo/2016/02/19/15/46/dog-1210559_960_720.jpg",
                "http://r.ddmcdn.com/s_f/o_1/cx_633/cy_0/cw_1725/ch_1725/w_720/APL/uploads/2014/11/too-cute-doggone-it-video-playlist.jpg",
                "https://cdn.pixabay.com/photo/2016/02/19/15/46/dog-1210559_960_720.jpg",
                "http://r.ddmcdn.com/s_f/o_1/cx_633/cy_0/cw_1725/ch_1725/w_720/APL/uploads/2014/11/too-cute-doggone-it-video-playlist.jpg",
                "https://cdn.pixabay.com/photo/2016/02/19/15/46/dog-1210559_960_720.jpg",
                "http://r.ddmcdn.com/s_f/o_1/cx_633/cy_0/cw_1725/ch_1725/w_720/APL/uploads/2014/11/too-cute-doggone-it-video-playlist.jpg",
                "https://cdn.pixabay.com/photo/2016/02/19/15/46/dog-1210559_960_720.jpg",
            };
        public VM()
        {
            Data1 = new ObservableCollection<int>();
            Data2 = new ObservableCollection<int>();
            Data3 = new ObservableCollection<byte[]>();

            var prg = new Progress<int>(i => Data1.Add(i));
            AddingProgress(prg);
            ContinousAdding();
            Task t = DownloadAsync();
        }

        private async Task DownloadAsync()
        {
            //WebClient wc = new WebClient();
            using (var wc = new HttpClient())
            {
                foreach (var url in URLS)
                {
                    byte[] image = await wc.GetByteArrayAsync(url);
                    Data3.Add(image);

                }
            }
            //string data = await ImageProcess();
            // ON UI
        }

        //private async Task<string> ImageProcess()
        //{
        //    await Task.Delay(1000).ConfigureAwait(false);
        //    Trace.Write("Process");
        //    await Task.Delay(1000);
        //    Trace.Write("Process");
        //    return "Done";
        //}

        private void AddingProgress(IProgress<int> prg)
        {
            Task.Run(async () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(1000);
                        prg.Report(i);
                    }
                });
        }

        private async void ContinousAdding()
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                Data2.Add(i);
            }
        }

        public ObservableCollection<int> Data1 { get; set; }
        public ObservableCollection<int> Data2 { get; set; }
        public ObservableCollection<byte[]> Data3 { get; set; }
    }
}
