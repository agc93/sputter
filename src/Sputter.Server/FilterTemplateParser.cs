using Parlot.Fluent;
using static Parlot.Fluent.Parsers;
using Sputter.Core;
using Parlot;

namespace Sputter.Server;

public class FilterTemplateParser(ILogger<FilterTemplateParser>? logger) {
    public DiscoveryTemplate? ParseTemplate(string filter) {
        var parser = ZeroOrOne(Terms.Identifier(extraPart: c => c == '-').AndSkip(Terms.Char(':'))).And(OneOf(Terms.String(), GetPathParser()));
        var res = parser.Parse(filter);
        if (!string.IsNullOrWhiteSpace(res.Item1.ToString()) && !string.IsNullOrWhiteSpace(res.Item2.ToString())) {
            return new DiscoveryTemplate(res.Item2.ToString()) {
                SourceAdapter = res.Item1.ToString()
            };
        }
        if (!string.IsNullOrWhiteSpace(res.Item2.ToString())) {
            return new DiscoveryTemplate(res.Item2.ToString());
        }
        logger?.LogError("Error parsing template '{Filter}' as drive specification!", filter);
        return null;
    }

    private static Parser<TextSpan> GetPathParser() {
        var divided = Terms.Char('/');
        var times = Terms.Char('*');
        var minus = Terms.Char('-');
        var underscore = Terms.Char('_');
        var plus = Terms.Char('+');
        var openParen = Terms.Char('(');
        var closeParen = Terms.Char(')');
        return Capture(OneOrMany(Terms.Identifier((c) => c == '/' || c == '*').And(ZeroOrMany(Literals.WhiteSpace().Then(w => w.ToString()).Or(OneOf(divided, times, minus, plus, openParen, closeParen, underscore).Then(c => c.ToString()))))));
    }
}
