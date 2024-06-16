using System.Collections.Generic;
using System.Net;

namespace QuickLook.Plugin.TorrentViewer;

public static class Torrent2Magnet
{
    public static string Join(this IEnumerable<string> stringArray)
    {
        return string.Join(string.Empty, stringArray);
    }

    public static string GenerateMagnetLink(string infoHash, string fileName, string[] trackerUrls)
    {
        string encodedFileName = WebUtility.UrlEncode(fileName);
        string magnetLink = $"magnet:?xt=urn:btih:{infoHash}&dn={encodedFileName}";

        foreach (string trackerUrl in trackerUrls)
        {
            if (!string.IsNullOrEmpty(trackerUrl))
            {
                string encodedTrackerUrl = WebUtility.UrlEncode(trackerUrl);
                magnetLink += $"&tr={encodedTrackerUrl}";
            }
        }
        return magnetLink;
    }
}
