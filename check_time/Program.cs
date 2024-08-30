
using System.Text.Json;
using System.Web;

public class TimeResponse
{
    public double time{get; set;}
} 

public class Program
    {
       
        public static int OK= 0;
        public static int WARNING= 1;
        public static int CRITICAL = 2;

        public static async Task<double> CreateClient(string ip_direction, string token)
        {   

            string encode_token = HttpUtility.UrlEncode(token);
            string url = $"https://{ip_direction}:5693/api/system/time?token={encode_token}";
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (HttpResponseMessage, certificate, chain, SslPolicyErrors) => true;
                using var client = new HttpClient(handler);
            try{
                    var response = await client.GetStringAsync(url);
                    TimeResponse? jsonresponse = JsonSerializer.Deserialize<TimeResponse>(response);
                    if(jsonresponse != null){

                            double check = jsonresponse.time;
                            DateTime date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(check)).DateTime;
                            DateTime local_date = date.ToLocalTime();
                            return check;
                    }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
           return 0;
        }


        public static void GetDifference(double check, string hostname)
        {
            DateTime local = DateTime.Now;
            long unixTimestamp = ((DateTimeOffset)local).ToUnixTimeSeconds();
            double diff = Math.Abs(unixTimestamp-check);
            
            if(diff >=120 && diff<300)
            {
                Console.WriteLine($"WARNING: EL SERVIDOR {hostname} tiene una diferencia de al menos 2 minutos");
                Environment.Exit(WARNING);
            }else if(diff>300)
            {
                Console.WriteLine($"CRIICAL: EL SERVIDOR {hostname} tiene una diferencia mayor a 5 minutos");
                Environment.Exit(CRITICAL);
            }
            else{
                Console.WriteLine($"OK: El SERVIDOR {hostname} esta configurado con la hora correcta");
                Environment.Exit(OK);
            }
        }

        public static  async Task  Main(string[] args)
        {
            if(args.Length==0)
            {
                Console.WriteLine("No se especifico ningun argumento");
                Environment.Exit(CRITICAL);
            }
            string direction_ip = args[0];
            string token = args[1];
            Console.WriteLine($"https://{direction_ip}:5693/system/time?token={token}");
            Console.WriteLine($"Esperando respuesta de {direction_ip}");
            var date_host = await CreateClient(direction_ip, token);
           GetDifference(date_host,direction_ip);
        }    


       


    }

