//Adapted from 5fa2080e2fff7bb31a4235250ce2f7a4bb1b64cb of https://github.com/dotnet/runtime.git src/libraries/Microsoft.Extensions.Logging.Console/src/JsonConsoleFormatterOptions.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging.Console;

namespace JsonConsoleFormatters;

/// <summary>
///     Options for <see cref="JeapJsonConsoleFormatter" />.
/// </summary>
/// <remarks>
///     By default, the formatter sets the Encoder to <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping" />
///     and includes scopes.
/// </remarks>
public class JeapJsonConsoleFormatterOptions : JsonConsoleFormatterOptions
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JeapJsonConsoleFormatterOptions" /> class.
    /// </summary>
    public JeapJsonConsoleFormatterOptions()
    {
        IncludeScopes = true;

        JsonWriterOptions = new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = false
        };
    }

    /// <summary>
    ///     Adds thread name to the log output. Defaults to false
    /// </summary>
    /// <remarks>
    ///     The thread name is stored in the json element "thread_name".
    /// </remarks>
    public bool IncludeThreadName { get; set; }

    // /// <summary>
    // ///     Whether to include the seldom used EventId field. Defaults to false
    // /// </summary>
    // public bool IncludeEventId { get; set; }

    /// <summary>
    ///     Determines whether to silently drop properties with duplicate keys. Defaults to false.
    ///     If a log property is repeated from multiple sources, this controls whether the formatter silently drops
    ///     the duplicated property, or if set to false will append a number to the key to deduplicate them.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Log properties can be added from a scope or from the log message itself.
    ///         This can lead to duplicated property keys.
    ///         If this is set to true, the formatter will silently drop the duplicated property.
    ///         Otherwise, the formatter will append a number to the key to deduplicate them.
    ///     </para>
    ///     <para>
    ///         The formatter uses the following evaluation order:
    ///         Standard log properties, scope properties, log message properties.
    ///     </para>
    ///     Given the following code:
    ///     <code>
    ///     using (_logger.BeginScope(new Dictionary&lt;string, object&gt; { { "x", 1 } }))
    ///     using (_logger.BeginScope(new Dictionary&lt;string, object&gt; { { "x", 2 } }))
    ///         _logger.LogDebug("x is {x}", 3);
    ///     </code>
    ///     DropDuplicatePropertyKey = true: {x: 3}
    ///     DropDuplicatePropertyKey = false:  {x: 3, x_1: 2, x_2: 3}
    /// </remarks>
    // TODO: Implement DropDuplicatePropertyKeys or remove it
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool DropDuplicatePropertyKeys { get; set; }
}
