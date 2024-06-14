using QuickLook.Plugin.TorrentViewer.Bencode.Exceptions;
using System;

namespace QuickLook.Plugin.TorrentViewer.Bencode.Torrents;

/// <summary>
/// Represents parse errors when parsing torrents.
/// </summary>
public class InvalidTorrentException : BencodeException
{
    public string InvalidField { get; set; }

    public InvalidTorrentException()
    {
    }

    public InvalidTorrentException(string message)
        : base(message)
    {
    }

    public InvalidTorrentException(string message, string invalidField)
        : base(message)
    {
        InvalidField = invalidField;
    }

    public InvalidTorrentException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

#pragma warning restore 1591
