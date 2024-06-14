Remove-Item ..\QuickLook.Plugin.TorrentViewer.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\QuickLook.Plugin\QuickLook.Plugin.TorrentViewer\bin\Release\ -Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.TorrentViewer.zip
Move-Item ..\QuickLook.Plugin.TorrentViewer.zip ..\QuickLook.Plugin.TorrentViewer.qlplugin