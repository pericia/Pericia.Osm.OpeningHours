using Antlr4.Runtime;
using Pericia.Osm.OpeningHours;
using System;
using System.IO;
using Xunit;
using static Pericia.Osm.OpeningHours.OpeningHoursParser;

namespace Pericia.Osm.OpeningHours.Tests
{
    public class GrammarTest
    {
        [Fact]
        public void Test24_7()
        {
            var parsed = ParseTimeDomain("24/7");

            Assert.Single(parsed.ruleSequence());

            var singleSequence = parsed.ruleSequence()[0];

            Assert.Null(singleSequence.ruleModifier());

            var selectorSequence = singleSequence.selectorSequence();
            Assert.NotNull(selectorSequence.twentyFourSeven());
            Assert.Null(selectorSequence.wideRangeSelectors());
            Assert.Null(selectorSequence.smallRangeSelectors());
        }

        [Fact]
        public void TestOneTimeRange()
        {
            var parsed = ParseTimeDomain("Mo-Fr 08:00-18:00");

            Assert.Single(parsed.ruleSequence());

            var singleSequence = parsed.ruleSequence()[0];

            //Assert.Null(singleSequence.ruleModifier());

            var selectorSequence = singleSequence.selectorSequence();
            Assert.Null(selectorSequence.twentyFourSeven());
            Assert.Null(selectorSequence.wideRangeSelectors());
            Assert.NotNull(selectorSequence.smallRangeSelectors());

            var smallRangeSelectors = selectorSequence.smallRangeSelectors();


            
            //Assert.Single(smallRangeSelectors.weekdaySelector());
            //Assert.Single(smallRangeSelectors.timeRangeSelector());


        }

        static TimeDomainContext ParseTimeDomain(string input)
        {
            var str = new AntlrInputStream(input);
            var lexer = new OpeningHoursLexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new OpeningHoursParser(tokens);

            var listener_lexer = new ErrorListener<int>();
            var listener_parser = new ErrorListener<IToken>();
            lexer.AddErrorListener(listener_lexer);
            parser.AddErrorListener(listener_parser);

            var tree = parser.timeDomain();
            if (listener_lexer.had_error || listener_parser.had_error)
            {
                throw new ArgumentException("Parse error", nameof(input));
            }

            return tree;
        }
        
            
    }
    internal class ErrorListener<T> : IAntlrErrorListener<T>
    {
        public bool had_error;
        
        public void SyntaxError(TextWriter output, IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            had_error = true;
        }
    }
}