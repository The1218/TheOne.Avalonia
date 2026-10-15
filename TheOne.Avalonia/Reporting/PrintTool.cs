using System.Diagnostics;

namespace TheOne.Avalonia.Reporting;

public enum ReportPrintPlatform { Windows, Linux, MacOS }
public sealed record ReportPrintOptions(string? PrinterName = null, int Copies = 1);
/// <summary>Submission is accepted by the OS handler/spooler, not confirmation that paper was printed.</summary>
public sealed record ReportPrintResult(string Message, bool KeepDocument)
{
    /// <summary>Temporary PDF retained for an asynchronous Windows handler, if applicable.</summary>
    public string? DocumentPath { get; init; }
}

public static class PrintTool
{
    public static ReportPrintPlatform CurrentPlatform => OperatingSystem.IsWindows() ? ReportPrintPlatform.Windows : OperatingSystem.IsMacOS() ? ReportPrintPlatform.MacOS
        : OperatingSystem.IsLinux() ? ReportPrintPlatform.Linux : throw new PlatformNotSupportedException("当前平台不支持桌面打印。");

    /// <summary>Builds shell-free CUPS arguments, or the Windows registered PDF print verb.</summary>
    public static ProcessStartInfo CreateStartInfo(string pdfPath, ReportPrintPlatform platform, ReportPrintOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfPath);
        options ??= new();
        if (options.Copies is < 1 or > 999) throw new ArgumentOutOfRangeException(nameof(options));
        var path = Path.GetFullPath(pdfPath);
        if (platform == ReportPrintPlatform.Windows)
        {
            if (options.Copies != 1 || !string.IsNullOrWhiteSpace(options.PrinterName)) throw new NotSupportedException("Windows PDF 处理程序管理打印机和份数，请在其打印设置中选择。");
            return new ProcessStartInfo(path) { UseShellExecute = true, Verb = "print" };
        }
        if (platform is not (ReportPrintPlatform.Linux or ReportPrintPlatform.MacOS)) throw new ArgumentOutOfRangeException(nameof(platform));
        var start = new ProcessStartInfo(platform == ReportPrintPlatform.MacOS ? "/usr/bin/lp" : "lp")
        { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        if (!string.IsNullOrWhiteSpace(options.PrinterName)) { start.ArgumentList.Add("-d"); start.ArgumentList.Add(options.PrinterName); }
        start.ArgumentList.Add("-n"); start.ArgumentList.Add(options.Copies.ToString(System.Globalization.CultureInfo.InvariantCulture));
        start.ArgumentList.Add("--"); start.ArgumentList.Add(path); return start;
    }
    public static async Task<ReportPrintResult> PrintPdfAsync(string pdfPath, ReportPrintOptions? options = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(pdfPath)) throw new FileNotFoundException("打印 PDF 不存在。", pdfPath);
        var platform = CurrentPlatform; var start = CreateStartInfo(pdfPath, platform, options);
        // The print verb depends on the installed PDF application. Successful shell invocation does
        // not prove a job reached a printer, and its process may be shared with an existing viewer.
        if (platform == ReportPrintPlatform.Windows && !start.Verbs.Contains("print", StringComparer.OrdinalIgnoreCase))
            throw new NotSupportedException("请先安装并注册支持 print 动作的 Windows PDF 应用，再调用打印。");
        Process? started;
        try { started = Process.Start(start); }
        catch (System.ComponentModel.Win32Exception exception) when (platform != ReportPrintPlatform.Windows)
        { throw new InvalidOperationException("无法启动系统打印命令，请确认已安装 CUPS、lp 命令及可用打印机。", exception); }
        using var process = started;
        if (platform == ReportPrintPlatform.Windows) return new("已调用 Windows PDF 打印处理程序，尚未确认打印任务或纸张输出。", true);
        if (process is null) throw new InvalidOperationException("无法启动 CUPS 打印服务。");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        var output = process.StandardOutput.ReadToEndAsync(timeout.Token); var error = process.StandardError.ReadToEndAsync(timeout.Token);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
            var message = (await output).Trim(); var failure = (await error).Trim();
            if (process.ExitCode != 0) throw new InvalidOperationException("打印提交失败：" + failure);
            return new("系统已接收打印提交，尚未确认纸张输出。" + (message.Length > 0 ? " " + message : ""), false);
        }
        catch (OperationCanceledException exception)
        {
            try { if (!process.HasExited) process.Kill(); }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) { }
            // Drain canceled output tasks so they cannot become unobserved faults.
            try { await Task.WhenAll(output, error); }
            catch (OperationCanceledException) { }
            catch (IOException) { }
            if (!cancellationToken.IsCancellationRequested) throw new TimeoutException("打印提交超时；系统可能已经接收任务，请检查打印队列。", exception);
            throw;
        }
    }

    private const string TemporaryPrefix = "TheOne.ReportPrint-";
    internal static string CreateTemporaryPdfPath() => Path.Combine(Directory.CreateTempSubdirectory(TemporaryPrefix).FullName, "report.pdf");
    internal static void DeleteTemporaryDocument(string path)
    {
        try
        {
            File.Delete(path);
            var directory = Path.GetDirectoryName(path);
            if (directory is not null && !Directory.EnumerateFileSystemEntries(directory).Any()) Directory.Delete(directory);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
    internal static void ScheduleTemporaryCleanup(string path) => _ = CleanupLaterAsync(path);
    private static async Task CleanupLaterAsync(string path)
    { await Task.Delay(TimeSpan.FromHours(24)).ConfigureAwait(false); DeleteTemporaryDocument(path); }

    /// <summary>Removes print PDFs older than 24 hours, including files left by a previous process.
    /// Call at application startup if printing is infrequent. Only this library's temporary folders are inspected.</summary>
    public static void CleanupTemporaryDocuments()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);
        try
        {
            foreach (var directory in Directory.EnumerateDirectories(Path.GetTempPath(), TemporaryPrefix + "*"))
            {
                try
                {
                    if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) continue;
                    var path = Path.Combine(directory, "report.pdf");
                    if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0 && File.GetLastWriteTimeUtc(path) < cutoff)
                        DeleteTemporaryDocument(path);
                    else if (!File.Exists(path) && Directory.GetLastWriteTimeUtc(directory) < cutoff && !Directory.EnumerateFileSystemEntries(directory).Any())
                        Directory.Delete(directory);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
