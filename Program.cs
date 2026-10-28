using System.Diagnostics;

namespace TerminalEdit;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            TerminalCommand.Open(args[0]);
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}

internal static class TerminalCommand
{
    public static void Open(string filePath)
    {
        try
        {
            // A console application started from this GUI gets its own terminal.
            // No command shell: filename characters such as '&' and '%' stay literal.
            var startInfo = new ProcessStartInfo("edit.exe")
            {
                UseShellExecute = false,
                Arguments = $"\"{Path.GetFullPath(filePath)}\"",
                CreateNoWindow = false
            };
            using var process = Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"无法使用 edit 打开文件。请先确认在终端中能运行 edit 命令。\n\n{ex.Message}",
                "Terminal Edit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
