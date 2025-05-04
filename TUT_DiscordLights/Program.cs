
// Required DSharpPlus NuGet Dependency
// https://dsharpplus.github.io/DSharpPlus/

// Requires local philips hue devices
// https://developers.meethue.com/develop/get-started-2/


namespace TUT_DiscordLights
{

    internal class Program
    {
        //Your bot token here
        public static string YOUR_TOKEN_HERE = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLM_-NOPQR";
        //Your application URL on the local philips hue bridge
        public static string YOUR_PHILIPS_HUE_PATH_HERE = "https://192.168.1.145/api/ABCDEFGHIJKLMNOPQRSTUVWX-YZABCDEFGHIJKLM";
                                                       


        //Entry
        static void Main(string[] args)
        {
            //This never returns a result to keep program running.
            MainAsync().GetAwaiter().GetResult();
        }

                




        static async Task MainAsync()
        {
            // Create a list of users with light data
            List<VIPI> vipii = new List<VIPI>();

            //ID, Light No, Hue, Bri, Sat
            vipii.Add(new VIPI(123456789012345678, 5, 45540, 255, 255));
            vipii.Add(new VIPI(123456789012345678, 3, 558, 255, 255));
            vipii.Add(new VIPI(123456789012345678, 4, 24386, 255, 255));
            vipii.Add(new VIPI(123456789012345678, 4, 8105, 255, 255));
    


            // Setup the login type for the Discord API
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = YOUR_TOKEN_HERE, // Login credentials
                TokenType = TokenType.Bot, // User is a bot account
                Intents = DiscordIntents.AllUnprivileged // With access rights
            });


            // Callback for when a user joins/leaves/mutes/unmutes
            discord.VoiceStateUpdated += async (s, e) =>
            {
                // Cache
                VIPI user = null;

                // Find if this user is a VIP
                for (int i = 0; i < vipii.Count; i++)
                {
                    if (e.User.Id == vipii[i].id)
                    {
                        Console.WriteLine("Oh Hi " + e.User.Username);
                        user = vipii[i];
                        break;
                    }
                }

                // Return if not VIP
                if (user == null)
                    return;

                // Check if the user left the server or not
                bool lightState = false;
                if (e.After.Channel != null)
                {
                    lightState = true;
                }

                // Create a URL request to send to Philips Hue API
                string url = $"{YOUR_PHILIPS_HUE_PATH_HERE}/lights/{user.lightNo}/state/"; // Updating which light
                string cmd = $"{{\"on\":{lightState.ToString().ToLower()}, \"sat\":{user.sat}, \"bri\":{user.bri},\"hue\":{user.hue}}}"; // Updating the on/off state with colour               
                string method = "PUT"; // A put request

                Console.WriteLine(cmd);

                HttpSend(url, cmd, method); // Send the request

            };




            await discord.ConnectAsync(); // Connect to Discord server
            await Task.Delay(-1); // Never return to keep program running


        }



        public static void HttpSend(string url, string cmd, string method) // HTTP PUT, POST methods
        {
            // Spoof check
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            
            try
            {
                // Convert the command string to bytes for encoding
                byte[] dataBytes = Encoding.UTF8.GetBytes(cmd);

                //Setup the HTTP request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;
                request.ContentType = "application/json";
                request.Method = method; // PUT / POST / GET

                // Send data
                using (Stream requestBody = request.GetRequestStream())
                {
                    requestBody.Write(dataBytes, 0, dataBytes.Length);
                }

                // Await response code and display
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }







    }
}
