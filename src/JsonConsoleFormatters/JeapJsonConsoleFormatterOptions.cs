//Adapted from 5fa2080e2fff7bb31a4235250ce2f7a4bb1b64cb of https://github.com/dotnet/runtime.git src/libraries/Microsoft.Extensions.Logging.Console/src/JsonConsoleFormatterOptions.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging.Console;

namespace JsonConsoleFormatters;

/// <summary>
///     Options for FlatJsonConsoleFormatter. Scopes are included by default.
/// </summary>
public class JeapJsonConsoleFormatterOptions : JsonConsoleFormatterOptions
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JeapJsonConsoleFormatterOptions" /> class.
    /// </summary>
    public JeapJsonConsoleFormatterOptions()
    {
        IncludeScopes = true;
    }

    public bool IncludeThreadName { get; set; }

    /// <summary>
    ///     Whether to include the seldom used EventId field. Defaults to false
    /// </summary>
    public bool IncludeEventId { get; set; }

    /// <summary>
    ///     If a log property is repeated from multiple sources, this controls whether the emitted log message has a single
    ///     property with the last logged value of the state key or whether a number is appended to deduplicate them.
    /// </summary>
    /// <remarks>
    ///     Given the following code:
    ///     <code>
    ///     using (_logger.BeginScope(new Dictionary&lt;string, object&gt; { { "x", 1 } }))
    ///     using (_logger.BeginScope(new Dictionary&lt;string, object&gt; { { "x", 2 } }))
    ///         _logger.LogDebug("x is {x}", 3);
    ///     </code>
    ///     Set this to true in order to log {x: 3}
    ///     Set this to false in order to log {x: 1, x_1: 2, x_2: 3}
    /// </remarks>
    public bool MergeDuplicateKeys { get; set; }
}
