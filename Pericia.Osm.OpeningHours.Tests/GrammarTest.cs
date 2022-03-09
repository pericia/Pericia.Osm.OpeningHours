using Antlr4.Runtime;
using System;
using System.IO;
using Xunit;
using static Pericia.Osm.OpeningHours.OpeningHoursParser;

namespace Pericia.Osm.OpeningHours.Tests
{
    public class GrammarTest
    {
        [Theory]
        [InlineData("24/7")]
        public void Open24_7(string input)
        {
            var parsed = ParseTimeDomain(input);

            Assert.Single(parsed.ruleSequence());

            var singleSequence = parsed.ruleSequence()[0];

            Assert.Null(singleSequence.ruleModifier());

            var selectorSequence = singleSequence.selectorSequence();
            Assert.NotNull(selectorSequence.twentyFourSeven());
            Assert.Null(selectorSequence.wideRangeSelectors());
            Assert.Null(selectorSequence.smallRangeSelectors());
        }

        [Theory]
        [InlineData("Mo-Fr 08:00-17:30")]
        public void OpenOnWeekDays(string input)
        {
            var parsed = ParseTimeDomain(input);

            Assert.Single(parsed.ruleSequence());

            var singleSequence = parsed.ruleSequence()[0];

            Assert.Null(singleSequence.ruleModifier());

            var selectorSequence = singleSequence.selectorSequence();
            Assert.Null(selectorSequence.twentyFourSeven());
            Assert.Null(selectorSequence.wideRangeSelectors());
            Assert.NotNull(selectorSequence.smallRangeSelectors());

            var smallRangeSelectors = selectorSequence.smallRangeSelectors();
            var weekDayRange = smallRangeSelectors.weekdaySelector()?.weekdaySequence()?.weekdayRange();

            Assert.NotNull(weekDayRange);
            Assert.Single(weekDayRange);

            var wdays = weekDayRange[0].wday();
            Assert.NotNull(wdays);
            Assert.Equal(2, wdays.Length);

            var monday = wdays[0];
            Assert.Equal("Mo", monday.GetText());
            var friday = wdays[1];
            Assert.Equal("Fr", friday.GetText());

            var timeRange = smallRangeSelectors.timeSelector();
            Assert.NotNull(timeRange);

            var timespans = timeRange.timespan();
            Assert.NotNull(timespans);
            Assert.Single(timespans);

            var timespan = timespans[0];
            var openTime = timespan.time();
            var closeTime = timespan.extendedTime();

            Assert.Equal("08", openTime.hourMinutes().hour().GetText());
            Assert.Equal("00", openTime.hourMinutes().minute().GetText());
            Assert.Equal("17", closeTime.extendedHourMinutes().extendedHour().GetText());
            Assert.Equal("30", closeTime.extendedHourMinutes().minute().GetText());
        }

        [Theory]
        [InlineData("Mo-Fr 08:00-12:00,13:00-17:30")]
        [InlineData("Mo-Fr 08:00-12:00, 13:00-17:30")]
        public void MultipleOpeningIntervals(string input)
        {
            var parsed = ParseTimeDomain(input);

            Assert.Single(parsed.ruleSequence());

            var singleSequence = parsed.ruleSequence()[0];

            var smallRangeSelectors = singleSequence.selectorSequence().smallRangeSelectors();

            var timeRange = smallRangeSelectors.timeSelector();
            Assert.NotNull(timeRange);

            var timespans = timeRange.timespan();
            Assert.NotNull(timespans);
            Assert.Equal(2, timespans.Length);

            var morning = timespans[0];
            Assert.Equal("08", morning.time().hourMinutes().hour().GetText());
            Assert.Equal("00", morning.time().hourMinutes().minute().GetText());
            Assert.Equal("12", morning.extendedTime().extendedHourMinutes().extendedHour().GetText());
            Assert.Equal("00", morning.extendedTime().extendedHourMinutes().minute().GetText());

            var afternoon = timespans[1];
            Assert.Equal("13", afternoon.time().hourMinutes().hour().GetText());
            Assert.Equal("00", afternoon.time().hourMinutes().minute().GetText());
            Assert.Equal("17", afternoon.extendedTime().extendedHourMinutes().extendedHour().GetText());
            Assert.Equal("30", afternoon.extendedTime().extendedHourMinutes().minute().GetText());
        }

        [Theory]
        [InlineData("Mo-Tu 08:00-17:30; Fr 08:00-12:00")]
        public void MultipleOpeningRules(string input)
        {
            var parsed = ParseTimeDomain(input);

            Assert.Equal(2, parsed.ruleSequence().Length);

            var firstRule = parsed.ruleSequence()[0].selectorSequence();

            var smallRangeSelectors = firstRule.smallRangeSelectors();
            var weekDayRange = smallRangeSelectors.weekdaySelector()?.weekdaySequence()?.weekdayRange();

            Assert.NotNull(weekDayRange);
            Assert.Single(weekDayRange);

            var wdays = weekDayRange[0].wday();
            Assert.NotNull(wdays);
            Assert.Equal(2, wdays.Length);

            var monday = wdays[0];
            Assert.Equal("Mo", monday.GetText());
            var tuesday = wdays[1];
            Assert.Equal("Tu", tuesday.GetText());


            var secondRule = parsed.ruleSequence()[1].selectorSequence();

            smallRangeSelectors = secondRule.smallRangeSelectors();
            weekDayRange = smallRangeSelectors.weekdaySelector()?.weekdaySequence()?.weekdayRange();

            Assert.NotNull(weekDayRange);
            Assert.Single(weekDayRange);

            wdays = weekDayRange[0].wday();
            Assert.NotNull(wdays);
            Assert.Single(wdays);

            var friday = wdays[0];
            Assert.Equal("Fr", friday.GetText());
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