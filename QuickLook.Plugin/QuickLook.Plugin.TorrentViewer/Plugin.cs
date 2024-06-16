// Copyright © 2024 ema
//
// This file is part of QuickLook program.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using QuickLook.Common.Plugin;
using QuickLook.Plugin.TorrentViewer.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace QuickLook.Plugin.TorrentViewer;

public class Plugin : IViewer
{
    private ArchiveFileListView? _tvp;
    private string? _path;

    public int Priority => 0;

    public void Init()
    {
    }

    public bool CanHandle(string path)
    {
        if (File.Exists(path) && Path.GetExtension(path).Equals(".torrent", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        return false;
    }

    public void Prepare(string path, ContextObject context)
    {
        context.PreferredSize = new Size { Width = 840, Height = 600 };
    }

    public void View(string path, ContextObject context)
    {
        _path = path;
        _tvp = new ArchiveFileListView();

        Metainfo metainfo = LoadTorrent();
        TorrentFiles torrent = new()
        {
            Name = metainfo.Name,
            InfoHash = metainfo.InfoHash.Value.Select(v => $"{v:x2}").Join(),
            Files = metainfo.Files
            //.Where(f => !Regex.IsMatch(f.Name, @"_____padding_file_\d+_.*____"))
            .Select(f => new TorrentFile()
            {
                Name = f.Name,
                Size = f.Size,
            }),
            Trackers = metainfo.Trackers.SelectMany(t => t).Select(u => u.ToString()),
        };

        torrent.Magnet = Torrent2Magnet.GenerateMagnetLink(torrent.InfoHash, torrent.Name, torrent.Trackers.ToArray());

        _tvp.SetDataContext(torrent);
        _tvp.Tag = context;

        context.ViewerContent = _tvp;
        context.Title = $"{Path.GetFileName(path)}";
        context.IsBusy = false;
    }

    public void Cleanup()
    {
        GC.SuppressFinalize(this);

        _tvp = null;
    }

    public Metainfo LoadTorrent()
    {
        if (!File.Exists(_path))
        {
            return null!;
        }

        try
        {
            byte[] buffer = File.ReadAllBytes(_path);
            using MemoryStream stream = new(buffer);
            Metainfo metainfo = TorrentParser.ReadFromStream(stream);
            return metainfo;
        }
        catch
        {
        }

        return null!;
    }
}
