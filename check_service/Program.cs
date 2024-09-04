using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Timers;

class GetCertificateInfo
{
    private static readonly RemoteCertificateValidationCallback s_certificateCallback = (_, _, _, _) => true;
public static void throw_alert (int data)
{
    int CRITICAL = 2;
    int WARNING = 1;
    int OK = 0;
    switch (data)
    {
        case 1 :
         Environment.Exit(OK);
         break;
        case 2 :
         Environment.Exit(WARNING);
         break;
        case 3:
        Console.WriteLine("CRITICAL: EL CERTIFICADO  HA CADUCADO");
        Environment.Exit(CRITICAL);
        break;
    }
    
}

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
    
        string domain = args[0];
       
         var get_cert =await GetCertificate(domain, 443);
         DateTime local = get_cert.NotAfter; 
         var newdate= ((DateTimeOffset)local).ToUnixTimeSeconds();
         Console.WriteLine(newdate);
    

}



}