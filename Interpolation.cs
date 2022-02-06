using System;
using System.Collections.Generic;
using Xunit;
using System.Text.RegularExpressions;
using System.Text;

namespace StringInterpolationExperiments
{
    public class Interpolation
    {
        public static IEnumerable<object[]> InvalidInputStrings => new List<object[]>
          {
                new object[] { string.Empty },
                new object[] { null },
                new object[] { " " },
          };

        public static IEnumerable<object[]> ValidInputStringsWithoutSquareBrackets => new List<object[]>
          {
            new object[] { "Hello my name is Jim" },
            new object[] { "Hello my name@email.com is Jim" },
            new object[] { "name" },
            new object[] { "Hello my {name} is Jim" },
            new object[] { "Hello my (name) is Jim" },
            new object[] { "Hello my <name> is Jim" },
            new object[] { "Hello my *name* is Jim" },
            new object[] { "Hello my *name* is Jim" },
            new object[] { "Hello my [name is Jim" },
            new object[] { "Hello my name] is Jim" },
          };

        public static IEnumerable<object[]> SpeicalCharacterInputStrings => new List<object[]>
          {
            new object[] { "+" },
            new object[] { "." },
            new object[] { "," },
            new object[] { "-" },
            new object[] { "!" },
            new object[] { "?" },
            new object[] { "/" },
            new object[] { "|" },
            new object[] { @"\" },
            new object[] { "$" },
            new object[] { "%" },
            new object[] { "&" },
            new object[] { "*" },
            new object[] { "(" },
            new object[] { ")" },
            new object[] { "{" },
            new object[] { "}" },
            new object[] { "<" },
            new object[] { ">" },
            new object[] { "`" },
            new object[] { "~" },
            new object[] { "@" },
            new object[] { "#" },
            new object[] { "_" },
            new object[] { "=" },
            new object[] { ":" },
            new object[] { ";" },
            new object[] { "'" },
        };


        [Theory]
        [MemberData(nameof(InvalidInputStrings))]
        public void Interpolate_Should_Throw_ArgumentExceptionWhenInvalidInputStringIsProvided(string invalidInput)
        {
            Assert.Throws<ArgumentException>(() => Interpolate(invalidInput, new Dictionary<string, string> { { "name", "Jim" } }));
        }

        [Fact]
        public void Interpolate_Should_Throw_ArgumentNullExceptionWhenANullValuesDictionaryIsProvided()
        {
            Assert.Throws<ArgumentNullException>(() => Interpolate("Hello world", null));
        }

        [Theory]
        [MemberData(nameof(ValidInputStringsWithoutSquareBrackets))]
        public void Interpolate_Should_Not_SubstituteSingleValueWhenThereAreNoSquareBrackets(string input)
        {
            Assert.Equal(input, Interpolate(input, new Dictionary<string, string> { { "name", "Error" } }));
        }

        [Fact]
        public void Interpolate_Should_Not_SubstituteNullValues()
        {
            Assert.Equal("Hello [name]", Interpolate("Hello [name]", new Dictionary<string, string> { { "name", null } }));
        }

        [Fact]
        public void Interpolate_Should_Not_SubstituteEmptyValues()
        {
            Assert.Equal("Hello [name]", Interpolate("Hello [name]", new Dictionary<string, string> { { "name", string.Empty } }));
        }

        [Fact]
        public void Interpolate_Should_Not_SubstituteEmptyKeys()
        {
            Assert.Equal("Hello [name]", Interpolate("Hello [name]", new Dictionary<string, string> { { string.Empty, "Jane" } }));
        }

        [Fact]
        public void Interpolate_Should_SubstituteSingleValueInSquareBrackets()
        {
            Assert.Equal("Hello Jim", Interpolate("Hello [name]", new Dictionary<string, string> { { "name", "Jim" } }));
        }

        [Fact]
        public void Interpolate_Should_SubstituteMultipleValueInSquareBrackets()
        {
            Assert.Equal("Obviously you could write an enumerator in multiple ways.",
                Interpolate("Obviously [first] could [second] an [third] in [fourth] ways.",
                new Dictionary<string, string>
                {
                    { "first", "you" },   { "second", "write" },   { "third", "enumerator" },   { "fourth", "multiple" }
                }));
        }

        [Fact]
        public void Interpolate_Should_SubstituteMultipleValueInSquareBracketsInMultipleLines()
        {
            Assert.Equal($"Obviously you could write{Environment.NewLine} an enumerator in{Environment.NewLine} multiple ways.",
                Interpolate($"Obviously [first] could [second]{Environment.NewLine} an [third] in{Environment.NewLine} [fourth] ways.",
                new Dictionary<string, string>
                {
                    { "first", "you" },   { "second", "write" },   { "third", "enumerator" },   { "fourth", "multiple" }
                }));
        }

        [Fact]
        public void Interpolate_Should_Not_SubstituteMissingValuesInSquareBrackets()
        {
            Assert.Equal("You can submit your code in any [missing] you like.",
                Interpolate("You can [first] your [second] in any [missing] you like.",
                new Dictionary<string, string>
                {
                    { "first", "submit" },   { "second", "code" },
                }));
        }

        [Fact]
        public void Interpolate_Should_Not_SubstituteValuesWhenNoneAreProvided()
        {
            Assert.Equal("We [a] [b] [c] example", Interpolate("We [a] [b] [c] example", new Dictionary<string, string> { }));
        }

        [Fact]
        public void Interpolate_Should_EscapeDoubledSquareBrackets()
        {
            Assert.Equal("Hello Jim [author]", Interpolate("Hello [name] [[author]]", new Dictionary<string, string> { { "name", "Jim" } }));
        }

        [Fact]
        public void Interpolate_Should_EscapeMulitpleDoubledSquareBrackets()
        {
            Assert.Equal("Hello Sue [author]. There are [10] items in your shopping cart. Would you like to go to the check out?",
                Interpolate("Hello [name] [[author]]. There are [[10]] items in your [shopping-cart-name]. Would you like to go to the [check out-name]?",
                new Dictionary<string, string> { { "name", "Sue" }, { "shopping-cart-name", "shopping cart" }, { "check out-name", "check out" } }));
        }

        [Fact]
        public void Interpolate_Should_SubstituteSingleDuplicatedValuesInSquareBrackets()
        {
            Assert.Equal("Hello Jim Jim", Interpolate("Hello [name] [name]", new Dictionary<string, string> { { "name", "Jim" } }));
        }

        [Fact]
        public void Interpolate_Should_SubstituteMultipleDuplicatedValuesInSquareBrackets()
        {
            Assert.Equal("This is my special string special string",
                Interpolate("This is my [one] [two] [one] [two]",
                new Dictionary<string, string> { { "one", "special" }, { "two", "string" } }));
        }

        [Theory]
        [MemberData(nameof(SpeicalCharacterInputStrings))]
        public void Interpolate_Should_SubstituteValuesInSquareBrackets(string specialCharacter)
        {
            Assert.Equal($"The speical character is {specialCharacter}", Interpolate("The speical character is [character]", new Dictionary<string, string> { { "character", specialCharacter } }));
        }

        [Fact]
        public void Interpolate_Should_SubstituteNonEnglishValuesInSquareBrackets()
        {
            Assert.Equal("Hello 标记", Interpolate("Hello [name]", new Dictionary<string, string> { { "name", "标记" } }));
        }


        /// <summary>
        /// The regex matches: 
        ///     \[ matches the character [ (case sensitive)
        ///     . matches any character (except for line terminators)
        ///     +? matches the previous token between one and unlimited times, as few times as possible, expanding as needed (lazy)
        ///     \] matches the character ] (case sensitive)
        /// /// </summary>
        private readonly Regex regex = new Regex(@"\[.+?\]", RegexOptions.Compiled | RegexOptions.Multiline, TimeSpan.FromSeconds(15));

        /// <summary>
        /// Interpolates the input string with the provided substitutions.
        /// Square brackets can be escaped by doubling them. E.g [[hello]] would return [hello]
        /// </summary>
        /// <example>
        /// Input 'Hello [name]' with substitution Key:name Value:Jane would return 'Hello Jane'
        /// </example>
        /// <param name="input">The string to interpolate.</param>
        /// <param name="substitutions">A dictionary of values to be subsituted into the input string's [] placeholders.</param>
        /// <exception cref="ArgumentException">The input string is null, empty or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The substitutions dictionary is null.</exception>
        /// <returns>A copy of the input string, but with the dictionary values subsituted into the [] placeholders.</returns>
        public string Interpolate(string input, Dictionary<string, string> substitutions)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException(nameof(input), "An input string is required to interpolate.");
            }

            if (substitutions == null)
            {
                throw new ArgumentNullException(nameof(substitutions), "A dictionary of string substitutions is required.");
            }

            StringBuilder builder = new StringBuilder(input).Replace("[[", "[").Replace("]]", "]");
            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                foreach (Group group in match.Groups)
                {
                    string key = group.Value.Trim('[', ']');
                    if (substitutions.TryGetValue(key, out string substitution) && !string.IsNullOrWhiteSpace(substitution))
                    {
                        builder.Replace(match.Value, substitution);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
