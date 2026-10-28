# Terminal Edit

Windows 10/11 x64 小工具。界面只有一个 panel 和两个按钮；无编辑器、后台服务或托盘功能。

## 测试步骤

1. 完整解压 ZIP，将 `TerminalEdit.exe` 与 `TerminalEdit.exe.config` 放到准备长期保留的位置，再运行 EXE。程序使用系统的 **.NET Framework 4.8 或更高版本**，不再捆绑现代 .NET/WinForms 运行时；Windows 11 已自带，较旧的 Windows 10 可能需安装。不需要管理员权限。
2. 确保在终端中可以运行 `edit`。本工具不捆绑编辑器；没有 `edit` 时，可通过 `winget install Microsoft.Edit` 安装，然后重新运行本工具。
3. 点击 **注册关联**，会同时注册 `.txt` 关联、所有文件的右键菜单和通用“打开方式”应用项。确认提示后，程序会打开 Windows“默认应用 → Terminal Edit”页面；如需双击打开 `.txt`，再选择 **Terminal Edit** 并确认。只用右键菜单则无需更改默认应用。
4. 双击 `测试.txt` 或其他 `.txt` 文件。Windows 调用 `TerminalEdit.exe "文件路径"`；程序只启动下面的命令，随后立即退出，不显示设置面板：

   ```text
   edit.exe "文件绝对路径"
   ```

   编辑器在终端中运行。没有 `cmd`/PowerShell 中间层，中文、空格、`&`、`%`、括号等文件名字符不会作为命令执行或展开。无文件参数时才显示两个按钮的面板。
5. 打开其他文本文件（如 `.md`、`.log`、`.json`、源代码或无扩展名文件）：右键选择 **用 Terminal Edit 打开**。Windows 11 请先点 **显示更多选项**。也可在系统“打开方式 → 选择其他应用 → 更多选项”中选择 **Terminal Edit**；系统可能需要滚动列表才显示它。它只将文件交给 edit，不会执行所选程序，也不能把任意二进制文件变成文本。
   可先对包内的 `测试.log` 试用右键打开，无需先设置 `.txt` 默认应用。
6. 不再使用时点击 **解除注册**，会同时移除 `.txt` 注册、所有文件的右键项及通用“打开方式”应用注册，再删除程序。如果已将它选为默认应用，请在 Windows“设置 → 应用 → 默认应用”中重新选择原编辑器；Windows 不允许本工具自动恢复受保护的默认应用选择。

注册仅写入当前用户的 `OpenWithProgids`、`TerminalEdit.TextFile`、`*\shell\TerminalEdit`、`Applications\TerminalEdit.exe` 和 Default Apps 能力声明。不枚举或改写其他扩展名的默认程序，也不改写 `.txt` 默认值或受保护的 `UserChoice`。解除注册只删除本工具的注册项，保留其他编辑器的关联和右键菜单。重复注册/解除注册均可。

请勿在注册后移动或改名 `TerminalEdit.exe`；如需移动，先解除注册，在新位置运行并重新注册。

## 从源代码打包

需要 Windows 和 .NET SDK（本机使用 .NET 10 SDK 构建）；项目目标为 .NET Framework 4.8。构建时会恢复所需的引用程序集，运行时没有随包 DLL 或 NuGet 依赖。

```powershell
dotnet publish TerminalEdit.csproj -c Release -o artifacts/TerminalEdit-lightweight
```

输出为 `TerminalEdit.exe` 与 `TerminalEdit.exe.config`，使用系统 .NET Framework，保持 KiB 级体积。没有自包含发布、运行时解压或后台进程。

## 已验证

- 实际 WinForms 面板及“注册关联”“解除注册”按钮；成功提示采用短句分行，句号不再单独换行。
- `.txt` 关联、所有文件的“用 Terminal Edit 打开”右键项，以及非 `.txt` 的系统“打开方式 → 更多选项”应用列表。
- 通过 Windows Shell 的 `TerminalEdit.TextFile` 关联处理器调用真实 `edit.exe`，显示测试文档；文件路径包含中文、空格、`&`、`%TEMP%`、方括号。
- 调用后关联处理程序退出；解除注册删除本工具关联项，原有 `.txt` 默认值及 `UserChoice` 保持不变。
- 轻量版实际加载系统 .NET Framework CLR；连续注册两次、解除注册两次，通过注册的 Shell 处理器调用真实 edit，正确显示包含中文、空格、`&`、`%TEMP%`、方括号、圆括号路径的测试文档。
- 在真实资源管理器中右键 Markdown 文件并点击新增菜单项，确认真实 edit 显示该文件内容；解除注册后重新右键，该项消失，其他编辑器的菜单项仍保留。

测试未替你修改受保护的默认应用选择；首次设为默认后，请按上述步骤验证双击行为。
