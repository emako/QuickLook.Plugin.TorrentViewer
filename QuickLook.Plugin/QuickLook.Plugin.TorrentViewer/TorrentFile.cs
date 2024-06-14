using System.Collections.Generic;

namespace QuickLook.Plugin.TorrentViewer;

public sealed class TorrentFile
{
    public string? Name { get; set; }
    public long Size { get; set; }
}

public sealed class TorrentFiles
{
    public string? Name { get; set; }
    public IEnumerable<TorrentFile> Files { get; set; } = [];
}
