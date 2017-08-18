using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ModernHttpClient;
using System.Collections.Generic;

namespace CaSI
{
    public class BingSpeechService : IBingSpeechService
    {
        IAuthenticationService authenticationService;
        string operatingSystem;

        public BingSpeechService(IAuthenticationService authService, string os)
        {
            authenticationService = authService;
            operatingSystem = os;
        }

        public async Task InitAuth()
        {
            // Get Auth Token
            if (string.IsNullOrWhiteSpace(authenticationService.GetAccessToken()))
            {
                await authenticationService.InitializeAsync();
            }

        }

        public async Task<SpeechResult> RecognizeSpeechAsync(string filename)
        {
            // Get Auth Token
            if (string.IsNullOrWhiteSpace(authenticationService.GetAccessToken()))
            {
                await authenticationService.InitializeAsync();
            }

            System.Diagnostics.Debug.WriteLine("recognizespeechasync");
            // Read audio file to a stream
            var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(filename);
            var fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read);

            string accessToken = authenticationService.GetAccessToken();

            var response = await SendRequestAsync(fileStream, Constants.SpeechRecognitionEndpoint, accessToken, Constants.AudioContentType);

            var speechResults = JsonConvert.DeserializeObject<SpeechResult>(response);

            fileStream.Dispose();
            return speechResults; 
        }

        async Task<string> SendRequestAsync(Stream fileStream, string url, string bearerToken, string contentType)
    {
        try
        {
            using (var client = new HttpClient(new NativeMessageHandler()))
            {

                byte[] chunks = ReadToEnd(fileStream);
                ByteArrayContent content = new ByteArrayContent(chunks);
                content.Headers.TryAddWithoutValidation("Content-Type", contentType);
               
                client.DefaultRequestHeaders.TransferEncodingChunked = true;
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                HttpResponseMessage response = await client.PostAsync(url, content).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[1024];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
