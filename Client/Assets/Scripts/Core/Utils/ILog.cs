public interface ILog
{
    void Print(string logStr);
    void DoPrint();

    void PrintWarning(string logStr);
    void PrintError(string logStr);
    void PrintClientStates(string logStr);
    void PrintServerStates(string logStr);
    void PrintReceive(string logStr);
    void PrintSend(string logStr);
}

public abstract class LogBase
{
    public string LogStr;
    public string Time;
}