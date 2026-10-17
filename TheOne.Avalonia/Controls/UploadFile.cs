using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace TheOne.Avalonia.Controls;

/// <summary>An immutable single-file selection. Every read returns a new stream owned by the caller.</summary>
public sealed class UploadFile : IDisposable
{
    private readonly object _lifetime = new();
    private readonly Func<CancellationToken, Task<Stream>> _openRead;
    private bool _disposed;
    private IDisposable? _ownedSource;
    private int _readers;

    public UploadFile(string name, Func<CancellationToken, Task<Stream>> openRead, long? size = null,
        string? contentType = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(openRead);
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
        Name = name;
        Size = size;
        ContentType = contentType;
        _openRead = openRead;
    }

    public string Name { get; }
    public long? Size { get; }
    public string? ContentType { get; }

    public string SizeText => Size is not { } size ? "大小未知" :
        size < 1024 ? $"{size} B" :
        size < 1024 * 1024 ? $"{size / 1024d:0.#} KB" : $"{size / (1024d * 1024):0.#} MB";

    /// <summary>Rejects new reads and releases an owned storage source after all open streams close.</summary>
    public void Dispose()
    {
        IDisposable? source = null;
        lock (_lifetime)
        {
            if (_disposed) return;
            _disposed = true;
            if (_readers == 0)
            {
                source = _ownedSource;
                _ownedSource = null;
            }
        }

        source?.Dispose();
    }

    public async Task<Stream> OpenReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lifetime)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _readers++;
        }

        try
        {
            var stream = await _openRead(cancellationToken).ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (!stream.CanRead)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
                throw new IOException("The file stream is not readable.");
            }

            return new ReadLease(stream, ReleaseReader);
        }
        catch
        {
            ReleaseReader();
            throw;
        }
    }

    /// <summary>The storage file must remain usable for as long as this selection is used.</summary>
    public static async Task<UploadFile> FromStorageFileAsync(IStorageFile file,
        CancellationToken cancellationToken = default, bool ownsStorageFile = false)
    {
        ArgumentNullException.ThrowIfNull(file);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var properties = await file.GetBasicPropertiesAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return new UploadFile(file.Name, async token =>
                {
                    token.ThrowIfCancellationRequested();
                    var stream = await file.OpenReadAsync().ConfigureAwait(false);
                    if (token.IsCancellationRequested)
                    {
                        await stream.DisposeAsync().ConfigureAwait(false);
                        token.ThrowIfCancellationRequested();
                    }

                    return stream;
                }, properties.Size is { } size && size <= long.MaxValue ? (long)size : null)
                { _ownedSource = ownsStorageFile ? file : null };
        }
        catch
        {
            if (ownsStorageFile) file.Dispose();
            throw;
        }
    }

    private void ReleaseReader()
    {
        IDisposable? source = null;
        lock (_lifetime)
        {
            _readers--;
            if (_disposed && _readers == 0)
            {
                source = _ownedSource;
                _ownedSource = null;
            }
        }

        source?.Dispose();
    }

    private sealed class ReadLease(Stream inner, Action release) : Stream
    {
        private Stream? _stream = inner;
        private Stream Inner => _stream ?? throw new ObjectDisposedException(nameof(ReadLease));
        public override bool CanRead => _stream?.CanRead == true;
        public override bool CanSeek => _stream?.CanSeek == true;
        public override bool CanWrite => false;
        public override long Length => Inner.Length;

        public override long Position
        {
            get => Inner.Position;
            set => Inner.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Inner.Read(buffer, offset, count);
        }

        public override int Read(Span<byte> buffer)
        {
            return Inner.Read(buffer);
        }

        public override int ReadByte()
        {
            return Inner.ReadByte();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            return Inner.ReadAsync(buffer, offset, count, token);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default)
        {
            return Inner.ReadAsync(buffer, token);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Inner.Seek(offset, origin);
        }

        public override void Flush()
        {
            Inner.Flush();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Interlocked.Exchange(ref _stream, null) is { } stream)
                try
                {
                    stream.Dispose();
                }
                finally
                {
                    release();
                }

            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _stream, null) is { } stream)
                try
                {
                    await stream.DisposeAsync().ConfigureAwait(false);
                }
                finally
                {
                    release();
                }

            GC.SuppressFinalize(this);
        }
    }
}

/// <summary>
///     Optional host picker service. Results transfer to the consumer if accepted; rejected results are disposed.
///     Null preserves selection.
/// </summary>
public interface IUploadFilePicker
{
    Task<UploadFile?> PickAsync(TopLevel owner, CancellationToken cancellationToken);
}