// This file is part of TorrentCore.
//   https://torrentcore.org
// Copyright (c) Samuel Fisher.
//
// Licensed under the GNU Lesser General Public License, version 3. See the
// LICENSE file in the project root for full license information.

using QuickLook.Plugin.TorrentViewer.Bencode.Parsing;
using QuickLook.Plugin.TorrentViewer.Bencode.Torrents;
using QuickLook.Plugin.TorrentViewer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuickLook.Plugin.TorrentViewer;

/// <summary>
/// Reads .torrent files.
/// </summary>
internal static class TorrentParser
{
    /// <summary>
    /// Loads the specified Torrent file.
    /// </summary>
    /// <param name="input">Input stream to read.</param>
    /// <returns>Metainfo data.</returns>
    public static Metainfo ReadFromStream(Stream input)
    {
        var parser = new BencodeParser();
        var torrent = parser.Parse<Torrent>(input);

        var files = new List<ContainedFile>();
        if (torrent.File != null)
        {
            // Single file
            files.Add(new ContainedFile(torrent.File.FileName, torrent.File.FileSize));
        }
        else
        {
            // Multiple files
            files.AddRange(torrent.Files.Select(x => new ContainedFile(x.FullPath, x.FileSize)));
        }

        // Construct pieces
        var pieces = new List<Piece>();
        byte[] pieceHashes = torrent.Pieces;
        int pieceIndex = 0;
        for (long offset = 0; offset + torrent.PieceSize <= torrent.TotalSize; offset += torrent.PieceSize)
        {
            int length = (int)Math.Min(torrent.PieceSize, torrent.TotalSize - offset);
            byte[] hash = new byte[Sha1Hash.Length];
            Array.Copy(pieceHashes, pieceIndex * Sha1Hash.Length, hash, 0, Sha1Hash.Length);
            Piece piece = new(pieceIndex, length, new Sha1Hash(hash));
            pieces.Add(piece);
            pieceIndex++;
        }

        return new Metainfo(torrent.DisplayName,
                            new Sha1Hash(torrent.GetInfoHashBytes()),
                            files,
                            pieces,
                            torrent.Trackers.Select(x => x.Select(y => new Uri(y))),
                            []);
    }
}
