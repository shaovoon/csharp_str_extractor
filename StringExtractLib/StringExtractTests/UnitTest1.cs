using NUnit.Framework;
using StringExtractLib;
using System.Collections.Generic;

namespace StringExtractTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void String()
        {
            string fmt = "ID:{}";

            string input = "ID:123";

            string id = "";

            StringExtractor.Extract(input, fmt, ref id);

            Assert.AreEqual(id, "123");
        }
        [Test]
        public void StringWithTimestamp()
        {
            string fmt = "ID:{}";

            string input = "2025-01-01 10:00:56 ID:123";

            string id = "";

            StringExtractor.Extract(input, fmt, ref id);

            Assert.AreEqual(id, "123");
        }
        [Test]
        public void StringWithSuffix()
        {
            string fmt = "ID:{} or OAuth Login";

            string input = "ID:123 or OAuth Login";

            string id = "";

            StringExtractor.Extract(input, fmt, ref id);

            Assert.AreEqual(id, "123");
        }
        [Test]
        public void TrimString()
        {
            string fmt = "Name:{t}";

            string input = "Name:  Sherry William  ";

            string name = "";

            StringExtractor.Extract(input, fmt, ref name);

            Assert.AreEqual(name, "Sherry William");
        }
        [Test]
        public void HexString()
        {
            string fmt = "Binary:{h}";

            string input = "Binary:0xA";

            string hex = "";

            StringExtractor.Extract(input, fmt, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void HexString2()
        {
            string fmt = "Binary:{h}";

            string input = "Binary:A";

            string hex = "";

            StringExtractor.Extract(input, fmt, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void HexString3()
        {
            string fmt = "Binary:{h}";

            string input = "Binary:a";

            string hex = "";

            StringExtractor.Extract(input, fmt, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void IgnoreVariable()
        {
            string fmt = "CustID:{x}, Binary:{h}";

            string input = "CustID:234, Binary:a";

            string hex = "";

            StringExtractor.Extract(input, fmt, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void ThreeVariable()
        {
            string fmt = "Name:{t}, Gender:{}, Salary:{}";

            string input = "Name:  Sherry William  , Gender:F, Salary:3600";

            string name = "";

            string gender = "A";

            string salary = "";

            StringExtractor.Extract(input, fmt, ref name, ref gender, ref salary);

            Assert.AreEqual(name, "Sherry William");

            Assert.AreEqual(gender, "F");

            Assert.AreEqual(salary, "3600");
        }
        //====================================================
        [Test]
        public void StringTokenized()
        {
            string fmt = "ID:{}";

            string input = "ID:123";

            string id = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref id);

            Assert.AreEqual(id, "123");
        }
        [Test]
        public void StringWithTimestampTokenized()
        {
            string fmt = "ID:{}";

            string input = "2025-01-01 10:00:56 ID:123";

            string id = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref id);

            Assert.AreEqual(id, "123");
        }
        [Test]
        public void StringWithSuffixTokenized()
        {
            string fmt = "ID:{} or OAuth Login";

            string input = "ID:123 or OAuth Login";

            string id = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref id);

            Assert.AreEqual(id, "123");
        }
        [Test]
        public void TrimStringTokenized()
        {
            string fmt = "Name:{t}";

            string input = "Name:  Sherry William  ";

            string name = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref name);

            Assert.AreEqual(name, "Sherry William");
        }
        [Test]
        public void HexStringTokenized()
        {
            string fmt = "Binary:{h}";

            string input = "Binary:0xA";

            string hex = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void HexStringTokenized2()
        {
            string fmt = "Binary:{h}";

            string input = "Binary:A";

            string hex = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void HexStringTokenized3()
        {
            string fmt = "Binary:{h}";

            string input = "Binary:a";

            string hex = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void IgnoreVariableTokenized()
        {
            string fmt = "CustID:{x}, Binary:{h}";

            string input = "CustID:234, Binary:a";

            string hex = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref hex);

            Assert.AreEqual(hex, "10");
        }
        [Test]
        public void ThreeVariableTokenized()
        {
            string fmt = "Name:{t}, Gender:{}, Salary:{}";

            string input = "Name:  Sherry William  , Gender:F, Salary:3600";

            string name = "";

            string gender = "A";

            string salary = "";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            StringExtractor.ExtractFromToken(input, tokens, ref name, ref gender, ref salary);

            Assert.AreEqual(name, "Sherry William");

            Assert.AreEqual(gender, "F");

            Assert.AreEqual(salary, "3600");
        }
        [Test]
        public void IsInputMatchedFmtTest()
        {
            string fmt = "Name:{t}, Gender:{}, Salary:{}";

            string input = "Name:  Sherry William  , Gender:F, Salary:3600";

            bool result = StringExtractor.IsInputMatchedFmt(input, fmt);

            Assert.IsTrue(result);
        }
        [Test]
        public void IsInputMatchedTokensTest()
        {
            string fmt = "Name:{t}, Gender:{}, Salary:{}";

            string input = "Name:  Sherry William  , Gender:F, Salary:3600";

            List<Token> tokens = StringExtractor.TokenizeFmtString(fmt);

            bool result = StringExtractor.IsInputMatchedTokens(input, tokens);

            Assert.IsTrue(result);
        }

    }
}