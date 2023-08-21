using Serilog;
using System.Diagnostics;
using System.IO;

namespace Core8.Utilities;

public class Assembler
{
    private readonly string palbart;

    public Assembler(string palbart)
    {
        if (!File.Exists(palbart))
        {
            throw new FileNotFoundException(palbart);
        }

        this.palbart = palbart;
    }

    public bool TryAssemble(string source, out string binary)
    {
        if (!File.Exists(source))
        {
            throw new FileNotFoundException(source);
        }

        binary = Path.ChangeExtension(source, ".bin");

        File.Delete(binary);

        using Process process = new();

        ProcessStartInfo info = new()
        {
            FileName = palbart,
            UseShellExecute = false,
            Arguments = $"\"{source}\"",
            RedirectStandardError = true
        };

        process.StartInfo = info;
        process.OutputDataReceived += Process_OutputDataReceived;
        process.ErrorDataReceived += Process_ErrorDataReceived;

        process.Start();

        if (!process.WaitForExit(10000))
        {
            process.Kill(true);
        }

        string error = Path.ChangeExtension(source, ".err");

        return File.Exists(binary) && !File.Exists(error);
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        Log.Error(e.Data);
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Log.Information(e.Data);
    }
}
