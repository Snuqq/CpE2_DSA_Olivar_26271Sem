using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;
using CpE2_DSA_Olivar_26271Sem;

// Transport never touches Unity objects. The main thread drains Commands.
public sealed class SearchConnection : IDisposable
{
    public readonly ConcurrentQueue<string> Commands = new ConcurrentQueue<string>();
    readonly BlockingCollection<string> replies = new BlockingCollection<string>(128);
    NamedPipeClientStream pipe;
    volatile bool disposed;
    public bool Embedded { get; private set; }
    volatile string error;
    public string Error { get { return error; } }
    public SearchConnection()
    {
        var args = Environment.GetCommandLineArgs();
        int at = Array.IndexOf(args, "-searchPipe");
        Embedded = at >= 0 && at + 1 < args.Length;
        if (!Embedded) return;
        string name = args[at + 1];
        new Thread(() => Connect(name)) { IsBackground = true, Name = "Search command reader" }.Start();
    }
    void Connect(string name)
    {
        try
        {
            pipe = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            pipe.Connect(15000);
            SearchWire.Write(pipe, "HELLO|" + SearchWire.Version);
            new Thread(() => {
                try { foreach (string reply in replies.GetConsumingEnumerable()) SearchWire.Write(pipe, reply); }
                catch (Exception ex) { if (!disposed) error = ex.Message; }
            }) { IsBackground = true, Name = "Search event writer" }.Start();
            while (!disposed) Commands.Enqueue(SearchWire.Read(pipe));
        }
        catch (Exception ex) { if (!disposed) error = ex.Message; }
    }
    public void Send(string message) { if (Embedded && !disposed && !replies.TryAdd(message)) error = "The host stopped reading events."; }
    public void Dispose() { disposed = true; replies.CompleteAdding(); if (pipe != null) pipe.Dispose(); }
}
