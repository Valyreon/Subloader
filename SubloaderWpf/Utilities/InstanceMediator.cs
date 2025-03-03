using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace SubloaderWpf.Utilities;

public class InstanceMediator
{
    private static readonly string NamedPipeName = "valyreon.subloader.pipe";
    private readonly CancellationTokenSource tokenSource = new();

    public event Action<string> ReceivedArgument;

    public void StartListening()
    {
        _ = Task.Run(() =>
        {
            while (!tokenSource.IsCancellationRequested)
            {
                using var server = new NamedPipeServerStream(NamedPipeName);
                server.WaitForConnection();

                using var reader = new StreamReader(server);
                var text = reader.ReadToEnd();

                ReceivedArgument?.Invoke(text);
            }
        }, tokenSource.Token);
    }

    public void StopListening()
    {
        tokenSource.Cancel();
    }

    public static void SendArgumentToRunningInstance(string arg)
    {
        using var client = new NamedPipeClientStream(NamedPipeName);
        try
        {
            client.Connect();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return;
        }

        if (!client.IsConnected)
        {
            return;
        }

        using var writer = new StreamWriter(client);
        writer.Write(arg);
        writer.Flush();
    }
}
