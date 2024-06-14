// This file is part of TorrentCore.
//   https://torrentcore.org
// Copyright (c) Samuel Fisher.
//
// Licensed under the GNU Lesser General Public License, version 3. See the
// LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace QuickLook.Plugin.TorrentViewer.Data;

internal interface IPieceCalculator
{
    void ComputePieces(List<ContainedFile> files, int pieceSize, IFileHandler fileHandler, List<Piece> pieces);
}
