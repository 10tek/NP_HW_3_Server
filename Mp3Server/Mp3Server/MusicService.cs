using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Server
{
    public class MusicService
    {
        public async Task UpdateDbSongs()
        {
            var musicList = Directory.GetFiles(@$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\Music", "*.mp3").ToList();

            using (var context = new Context())
            {
                //var dbMusics = await context.MusicFiles.ToListAsync();
                //foreach (var music in dbMusics)
                //{
                //    foreach(var mus in musicList)
                //    {
                //        if(mus != music.Author + '-' + music.SongName)
                //        {
                //            context.MusicFiles.Remove(music);
                //        }
                //    }
                //}
                foreach (var music in musicList)
                {
                    var parts = Path.GetFileNameWithoutExtension(music).Split("-");
                    var isExist = await context.MusicFiles.SingleOrDefaultAsync(x => x.MusicPath == music);

                    if (isExist != null) continue;

                    context.MusicFiles.Add(new MusicFile
                    {
                        Author = parts.First(),
                        SongName = parts.Last(),
                        MusicPath = music,
                    });

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task SendMusic(string path, NetworkStream stream)
        {
            var data = await File.ReadAllBytesAsync(path);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public async Task SendMusicList(NetworkStream stream)
        {
            using (var context = new Context())
            {
                var musics = await context.MusicFiles.ToListAsync();
                var json = JsonConvert.SerializeObject(musics);
                var sendData = Encoding.UTF8.GetBytes(json);
                await stream.WriteAsync(sendData, 0, sendData.Length);
            }
        }
    }
}
