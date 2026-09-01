// Keyword identifiers and escaped naming overrides should generate valid C#

namespace @namespace;

[FastEnum(EnumNameOverride = "@class", EnumsClassName = "@struct", EnumsClassNamespace = "@namespace.Generated", ExtensionClassName = "@interface", ExtensionClassNamespace = "@namespace.Extensions")]
public enum @event
{
    @class = 1,
    @namespace = 2
}