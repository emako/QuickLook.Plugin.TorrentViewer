﻿// Copyright © 2017 Paddy Xu
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

using QuickLook.Common.ExtensionMethods;
using QuickLook.Common.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace QuickLook.Plugin.TorrentViewer;

/// <summary>
///     Interaction logic for ArchiveFileListView.xaml
/// </summary>
public partial class ArchiveFileListView : UserControl, IDisposable
{
    public ArchiveFileListView()
    {
        InitializeComponent();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        IconManager.ClearCache();
    }

    public void SetDataContext(TorrentFiles context)
    {
        treeTitle.Text = context.Name;
        treeTitle.ToolTip = context.Name;
        totalCount.Text = string.Format(TranslationHelper.Get("TOTAL_COUNT", domain: Assembly.GetExecutingAssembly().GetName().Name), context.Files.Count().ToString());
        totalSize.Text = string.Format(TranslationHelper.Get("TOTAL_SIZE", domain: Assembly.GetExecutingAssembly().GetName().Name), context.Files.Sum(x => x.Size).ToPrettySize(2));

        copyButton.ToolTip = context.Magnet;
        copyButton.Click += (_, _) =>
        {
            try
            {
                Clipboard.SetText(context.Magnet);
            }
            catch
            {
                ///
            }
        };

        treeGrid.DataContext = context.Files;

        treeView.LayoutUpdated += (sender, e) =>
        {
            // return when empty
            if (treeView.Items.Count == 0)
                return;

            // return when there are more than one root nodes
            if (treeView.Items.Count > 1)
                return;

            var root = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(treeView.Items[0]);
            if (root == null)
                return;

            root.IsExpanded = true;
        };
    }
}
