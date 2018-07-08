using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class ServerLog
{
    public static void Print(string log)
    {
        LogQueue.Enqueue(log);
    }

    static Queue<string> LogQueue = new Queue<string>();

    public static void Update(){
        if(LogQueue.Count>0){
            Console.WriteLine(LogQueue.Dequeue());
        }
    }
}