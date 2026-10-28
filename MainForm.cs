using System.Diagnostics;

namespace TerminalEdit;

internal sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "Terminal Edit";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(360, 140);
        AutoScaleMode = AutoScaleMode.Dpi;

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24)
        };

        var registerButton = new Button
        {
            Text = "注册关联",
            Width = 130,
            Height = 42,
            Location = new Point(24, 38)
        };
        registerButton.Click += (_, _) => Execute(
            FileAssociation.Register,
            "已注册文件关联和右键菜单。\n\n"
                + "其他文件可右键选择：\n"
                + "“用 Terminal Edit 打开”\n"
                + "Windows 11 请先点“显示更多选项”。\n\n"
                + "接下来将打开默认应用设置。\n"
                + "如需双击打开 .txt，请选择本程序。\n"
                + "默认应用仍需由你确认。",
            openSettings: true);

        var unregisterButton = new Button
        {
            Text = "解除注册",
            Width = 130,
            Height = 42,
            Location = new Point(182, 38)
        };
        unregisterButton.Click += (_, _) => Execute(
            FileAssociation.Unregister,
            "已解除文件关联和右键菜单注册。\n\n"
                + "如果曾将本程序设为默认应用，\n"
                + "请在系统设置中重新选择原编辑器。");

        panel.Controls.Add(registerButton);
        panel.Controls.Add(unregisterButton);
        Controls.Add(panel);
    }

    private void Execute(Action action, string successMessage, bool openSettings = false)
    {
        try
        {
            action();
            MessageBox.Show(
                this,
                successMessage,
                "Terminal Edit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            if (openSettings)
            {
                using var process = Process.Start(new ProcessStartInfo(
                    "ms-settings:defaultapps?registeredAppUser=TerminalEdit")
                {
                    UseShellExecute = true
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "操作失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
