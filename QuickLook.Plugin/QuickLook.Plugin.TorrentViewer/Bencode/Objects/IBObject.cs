﻿using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace QuickLook.Plugin.TorrentViewer.Bencode.Objects;

/// <summary>
/// Represent a bencode value that can be encoded to bencode.
/// </summary>
public interface IBObject
{
    /// <summary>
    /// Calculates the (encoded) size of the object in bytes.
    /// </summary>
    int GetSizeInBytes();

    /// <summary>
    /// Writes the object as bencode to the specified stream.
    /// </summary>
    /// <typeparam name="TStream">The type of stream.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <returns>The used stream.</returns>
    TStream EncodeTo<TStream>(TStream stream) where TStream : Stream;

    /// <summary>
    /// Writes the object as bencode to the specified <see cref="PipeWriter"/> without flushing the writer,
    /// you should do that manually.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    void EncodeTo(PipeWriter writer);

    /// <summary>
    /// Writes the object as bencode to the specified <see cref="PipeWriter"/> and flushes the writer afterwards.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="cancellationToken"></param>
    ValueTask<FlushResult> EncodeToAsync(PipeWriter writer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the object asynchronously as bencode to the specified <see cref="Stream"/> using a <see cref="PipeWriter"/>.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="writerOptions">The options for the <see cref="PipeWriter"/>.</param>
    /// <param name="cancellationToken"></param>
    ValueTask<FlushResult> EncodeToAsync(Stream stream, StreamPipeWriterOptions writerOptions = null, CancellationToken cancellationToken = default);
}
