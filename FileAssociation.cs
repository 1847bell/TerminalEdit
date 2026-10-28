using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace TerminalEdit;

internal static class FileAssociation
{
    private const string ProgId = "TerminalEdit.TextFile";
    private const string ApplicationName = "TerminalEdit";
    private const string CapabilitiesPath = @"Software\TerminalEdit\Capabilities";
    private const string ContextMenuPath = @"*\shell\TerminalEdit";
    private const string OpenWithPath = @"Applications\TerminalEdit.exe";

    public static void Register()
    {
        var executablePath = Application.ExecutablePath;
        var openCommand = $"\"{executablePath}\" \"%1\"";
        var iconLocation = $"\"{executablePath}\",0";

        using var classes = Registry.CurrentUser.CreateSubKey(@"Software\Classes");
        using (var progId = classes.CreateSubKey(ProgId))
        {
            progId.SetValue(string.Empty, "Terminal Edit 文本文件");
            using var icon = progId.CreateSubKey("DefaultIcon");
            icon.SetValue(string.Empty, iconLocation);
            using var command = progId.CreateSubKey(@"shell\open\command");
            command.SetValue(string.Empty, openCommand);
            using var application = progId.CreateSubKey("Application");
            application.SetValue("ApplicationName", "Terminal Edit");
        }

        using (var openWith = classes.CreateSubKey(@".txt\OpenWithProgids"))
        {
            openWith.SetValue(ProgId, Array.Empty<byte>(), RegistryValueKind.None);
        }

        using (var contextMenu = classes.CreateSubKey(ContextMenuPath))
        {
            contextMenu.SetValue(string.Empty, "用 Terminal Edit 打开");
            contextMenu.SetValue("Icon", iconLocation);
            using var command = contextMenu.CreateSubKey("command");
            command.SetValue(string.Empty, openCommand);
        }

        using (var application = classes.CreateSubKey(OpenWithPath))
        {
            application.SetValue("FriendlyAppName", "Terminal Edit");
            using var icon = application.CreateSubKey("DefaultIcon");
            icon.SetValue(string.Empty, iconLocation);
            using var command = application.CreateSubKey(@"shell\open\command");
            command.SetValue(string.Empty, openCommand);
        }

        using (var capabilities = Registry.CurrentUser.CreateSubKey(CapabilitiesPath))
        {
            capabilities.SetValue("ApplicationName", "Terminal Edit");
            capabilities.SetValue("ApplicationDescription", "使用终端 edit 命令打开文本文件");
            capabilities.SetValue("ApplicationIcon", iconLocation);
            using var associations = capabilities.CreateSubKey("FileAssociations");
            associations.SetValue(".txt", ProgId);
        }

        using var applications = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications");
        applications.SetValue(ApplicationName, CapabilitiesPath);
        NotifyShell();
    }

    public static void Unregister()
    {
        using (var classes = Registry.CurrentUser.OpenSubKey(@"Software\Classes", writable: true))
        {
            if (classes is not null)
            {
                using (var extension = classes.OpenSubKey(".txt", writable: true))
                {
                    using (var openWith = extension?.OpenSubKey("OpenWithProgids", writable: true))
                    {
                        openWith?.DeleteValue(ProgId, throwOnMissingValue: false);
                    }

                    if (extension is not null)
                    {
                        DeleteEmptyKey(extension, "OpenWithProgids");
                    }
                }

                DeleteEmptyKey(classes, ".txt");
                classes.DeleteSubKeyTree(ProgId, throwOnMissingSubKey: false);
                classes.DeleteSubKeyTree(ContextMenuPath, throwOnMissingSubKey: false);
                classes.DeleteSubKeyTree(OpenWithPath, throwOnMissingSubKey: false);
            }
        }

        using var applications = Registry.CurrentUser.OpenSubKey(@"Software\RegisteredApplications", writable: true);
        applications?.DeleteValue(ApplicationName, throwOnMissingValue: false);
        Registry.CurrentUser.DeleteSubKeyTree(@"Software\TerminalEdit", throwOnMissingSubKey: false);
        NotifyShell();
    }

    private static void DeleteEmptyKey(RegistryKey parent, string name)
    {
        using (var key = parent.OpenSubKey(name))
        {
            if (key is null || key.ValueCount != 0 || key.SubKeyCount != 0)
            {
                return;
            }
        }

        parent.DeleteSubKey(name, throwOnMissingSubKey: false);
    }

    private static void NotifyShell()
    {
        SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(uint eventId, uint flags, IntPtr item1, IntPtr item2);
}
