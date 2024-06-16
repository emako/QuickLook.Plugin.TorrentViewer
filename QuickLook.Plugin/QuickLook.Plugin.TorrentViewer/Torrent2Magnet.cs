using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace QuickLook.Plugin.TorrentViewer;

public static class Torrent2Magnet
{
    public static string GenerateMagnetLink(string infoHash, string fileName, string[] trackerUrls)
    {
        string encodedFileName = WebUtility.UrlEncode(fileName);
        string trackerUrl = trackerUrls.Select(tr => WebUtility.UrlEncode(tr)).Join("&tr=");
        string magnetLink = $"magnet:?xt=urn:btih:{infoHash}&dn={encodedFileName}&tr={trackerUrl}";

        return magnetLink;
    }

    public static string Join(this IEnumerable<string> values, string? separator = null)
    {
        return string.Join(separator ?? string.Empty, values);
    }
}
