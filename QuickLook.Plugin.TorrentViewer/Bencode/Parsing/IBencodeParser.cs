using QuickLook.Plugin.TorrentViewer.Bencode.IO;
using QuickLook.Plugin.TorrentViewer.Bencode.Objects;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickLook.Plugin.TorrentViewer.Bencode.Parsing;

/// <summary>
/// Represents a parser capable of parsing bencode.
/// </summary>
public interface IBencodeParser
{
    /// <summary>
    /// List of parsers used by the <see cref="IBencodeParser"/>.
    /// </summary>
    BObjectParserList Parsers { get; }

    /// <summary>
    /// The encoding use for parsing.
    /// </summary>
    Encoding Encoding { get; }

    /// <summary>
    ///  Parses an <see cref="IBObject"/> from the reader.
    /// </summary>
    /// <param name="reader"></param>
    IBObject Parse(BencodeReader reader);

    /// <summary>
    /// Parse an <see cref="IBObject"/> of type <typeparamref name="T"/> from the reader.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IBObject"/> to parse as.</typeparam>
    /// <param name="reader"></param>
    T Parse<T>(BencodeReader reader) where T : class, IBObject;

    /// <summary>
    /// Parse an <see cref="IBObject"/> from the <see cref="PipeReader"/>.
    /// </summary>
    ValueTask<IBObject> ParseAsync(PipeBencodeReader pipeReader, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parse an <see cref="IBObject"/> of type <typeparamref name="T"/> from the <see cref="PipeBencodeReader"/>.
    /// </summary>
    ValueTask<T> ParseAsync<T>(PipeBencodeReader pipeReader, CancellationToken cancellationToken = default) where T : class, IBObject;
}
