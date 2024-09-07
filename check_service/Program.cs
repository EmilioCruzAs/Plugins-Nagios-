using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


class GetCertificateInfo
{
    //private static int WARNING = 1;
    private static int CRITICAL = 2;
    //private static int OK =0;
    private static readonly RemoteCertificateValidationCallback s_certificateCallback = (_, _, _, _) => true;


public static async Task<X509Certificate2> GetCertificate(string domain, int port)
{
    TcpClient cliente = new TcpClient(domain, port);
    var Myssl = new SslStream(cliente.GetStream(),
    leaveInnerStreamOpen: true,
    s_certificateCallback );

    await Myssl.AuthenticateAsClientAsync(domain).ConfigureAwait(false);

    var server_certificate = Myssl.RemoteCertificate;
    if(server_certificate != null)
    {
         return new X509Certificate2(server_certificate);
    }
    
    return null;
}

public static async Task Main(string[] args)
{
    if(args.Length==0){
        Console.WriteLine("CRITICAL: Se requiere un argumento: -domain \n");
        Console.Write("-help para mas informacion");
        Environment.Exit(CRITICAL);
       }else if(args[0]=="-help" || args[0]=="-Help"){
        Console.WriteLine("Para el funcionamiento correcto del script se requiere el siguiente parametro:\t\t");
        Environment.Exit(0);
       }
    string domain = args[0];
       
    var get_cert =await GetCertificate(domain, 443);
    DateTime Externaldatime = get_cert.NotAfter; 
    DateTime LocalDatetime = DateTime.Now;
    TimeSpan difference =Externaldatime-LocalDatetime;
    var dias = difference.Days;
    Console.WriteLine(dias);


}



}