﻿using QuickLook.Plugin.TorrentViewer.Bencode.Exceptions;
using QuickLook.Plugin.TorrentViewer.Bencode.IO;
using QuickLook.Plugin.TorrentViewer.Bencode.Objects;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickLook.Plugin.TorrentViewer.Bencode.Parsing;

/// <summary>
/// Main class used for parsing bencode.
/// </summary>
public class BencodeParser : IBencodeParser
{
    /// <summary>
    /// List of parsers used by the <see cref="BencodeParser"/>.
    /// </summary>
    public BObjectParserList Parsers { get; }

    /// <summary>
    /// The encoding use for parsing.
    /// </summary>
    public Encoding Encoding
    {
        get => _encoding;
        set
        {
            _encoding = value ?? throw new ArgumentNullException(nameof(value));
            Parsers.GetSpecific<BStringParser>()?.ChangeEncoding(value);
        }
    }

    private Encoding _encoding;

    /// <summary>
    /// Creates an instance using the specified encoding and the default parsers.
    /// Encoding defaults to <see cref="System.Text.Encoding.UTF8"/> if not specified.
    /// </summary>
    /// <param name="encoding">The encoding to use when parsing.</param>
    public BencodeParser(Encoding encoding = null!)
    {
        _encoding = encoding ?? Encoding.UTF8;

        Parsers = new BObjectParserList
        {
            new BNumberParser(),
            new BStringParser(_encoding),
            new BListParser(this),
            new BDictionaryParser(this),
            new Torrents.TorrentParser(this),
        };
    }

    /// <summary>
    ///  Parses an <see cref="IBObject"/> from the reader.
    /// </summary>
    public virtual IBObject Parse(BencodeReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));

        switch (reader.PeekChar())
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9': return Parse<BString>(reader);
            case 'i': return Parse<BNumber>(reader);
            case 'l': return Parse<BList>(reader);
            case 'd': return Parse<BDictionary>(reader);
            case default(char): return null;
        }

        throw InvalidBencodeException<IBObject>.InvalidBeginningChar(reader.PeekChar(), reader.Position);
    }

    /// <summary>
    /// Parse an <see cref="IBObject"/> of type <typeparamref name="T"/> from the reader.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IBObject"/> to parse as.</typeparam>
    public virtual T Parse<T>(BencodeReader reader) where T : class, IBObject
    {
        var parser = Parsers.Get<T>();

        if (parser == null)
            throw new BencodeException($"Missing parser for the type '{typeof(T).FullName}'. Stream position: {reader.Position}");

        return parser.Parse(reader);
    }

    /// <summary>
    /// Parse an <see cref="IBObject"/> from the <see cref="PipeBencodeReader"/>.
    /// </summary>
    public virtual async ValueTask<IBObject> ParseAsync(PipeBencodeReader pipeReader, CancellationToken cancellationToken = default)
    {
        if (pipeReader == null) throw new ArgumentNullException(nameof(pipeReader));

        switch (await pipeReader.PeekCharAsync(cancellationToken).ConfigureAwait(false))
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9': return await ParseAsync<BString>(pipeReader, cancellationToken).ConfigureAwait(false);
            case 'i': return await ParseAsync<BNumber>(pipeReader, cancellationToken).ConfigureAwait(false);
            case 'l': return await ParseAsync<BList>(pipeReader, cancellationToken).ConfigureAwait(false);
            case 'd': return await ParseAsync<BDictionary>(pipeReader, cancellationToken).ConfigureAwait(false);
            case default(char): return null;
        }

        throw InvalidBencodeException<IBObject>.InvalidBeginningChar(
            await pipeReader.PeekCharAsync(cancellationToken).ConfigureAwait(false),
            pipeReader.Position);
    }

    /// <summary>
    /// Parse an <see cref="IBObject"/> of type <typeparamref name="T"/> from the <see cref="PipeBencodeReader"/>.
    /// </summary>
    public virtual async ValueTask<T> ParseAsync<T>(PipeBencodeReader pipeReader, CancellationToken cancellationToken = default) where T : class, IBObject
    {
        var parser = Parsers.Get<T>();

        if (parser == null)
            throw new BencodeException($"Missing parser for the type '{typeof(T).FullName}'. Stream position: {pipeReader.Position}");

        return await parser.ParseAsync(pipeReader, cancellationToken).ConfigureAwait(false);
    }
}
