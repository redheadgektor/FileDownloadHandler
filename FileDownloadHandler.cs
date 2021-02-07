public class FileDownloadHandler : DownloadHandlerScript
{
    public string _downloadFilePath { get; private set; }
    public string _originalFilePath { get; private set; }
    private Stream _fileStream;
    private int received { get; private set; } = 0;
    public ulong contentSize { get; private set; } = 0;

    public FileDownloadHandler(string filepath) : base(new byte[4096])
    {
        _originalFilePath = filepath;
        _downloadFilePath = filepath + ".download";
    }

    protected override float GetProgress()
    {
        if (contentSize < 0) { return 0; };

        return (float)received / contentSize;
    }

    protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        contentSize = contentLength;
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (_fileStream == null)
        {
            _fileStream = File.OpenWrite(_downloadFilePath);
        }

        received += dataLength;
        _fileStream.Write(data, 0, dataLength);

        return true;
    }

    protected override void CompleteContent()
    {
        FreeFile();
    }

    public void ReplaceFile()
    {
        if (File.Exists(_originalFilePath))
        {
            File.Move(_originalFilePath, _originalFilePath + ".old");
        }
        File.Move(_downloadFilePath, _originalFilePath);
        if (File.Exists(_originalFilePath))
        {
            File.Delete(_originalFilePath + ".old");
        }
    }

    public void FreeFile()
    {
        _fileStream?.Close();
    }

    public void DeleteFile()
    {
        if (File.Exists(_downloadFilePath))
        {
            File.Delete(_downloadFilePath);
        }
    }
}
