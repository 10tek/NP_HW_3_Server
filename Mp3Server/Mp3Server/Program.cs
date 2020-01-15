using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Server
{
    class Program
    {
        private const string ADDRESS = "127.0.0.1";
        private const int PORT = 3222;
        static async Task Main(string[] args)
        {
            var audioManager = new MusicService();
            await audioManager.UpdateDbSongs();

            var listener = new TcpListener(IPAddress.Parse(ADDRESS), PORT);
            listener.Start();
            Console.WriteLine("СЕРВЕР НАЧАЛ РАБОТАТЬ!");
            while (true)
            {
                using var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Поступил запрос!");
                using var stream = client.GetStream();

                var buffer = new byte[1024];
                var builder = new StringBuilder();

                do
                {
                    int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                }
                while (stream.DataAvailable);

                if (builder.ToString() == "Update")
                {
                    await audioManager.SendMusicList(stream);
                }
                else
                {
                    var music = JsonConvert.DeserializeObject<MusicFile>(builder.ToString());
                    if (music is null)
                    {
                        Console.WriteLine("Ошибка! music is null");
                        continue;
                    }
                    await audioManager.SendMusic(music.MusicPath, stream);
                    Console.WriteLine("Музыка успешно отправлена");
                }
                Console.WriteLine("Запрос обработан!");
            }
        }
    }
}
