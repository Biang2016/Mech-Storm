using System.Net.Sockets;

public class SendMsg
{
    public SendMsg(Socket client, RequestBase req, int clientId)
    {
        Client = client;
        Req = req;
        ClientId = clientId;
    }

    public Socket Client;
    public RequestBase Req;
    public int ClientId;
}