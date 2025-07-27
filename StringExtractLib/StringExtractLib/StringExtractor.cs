using StringExtractLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringExtractLib
{
    public enum TokenType
    {
        None,
        Hex,
        Matter,
        Trim
    }
    public class Token
    {
        public Token(int _index, int _start, int _size, TokenType _type, string _prefix, string _postfix)
        {
            index = _index;
            start = _start;
            size = _size;
            type = _type;
            prefix = _prefix;
            postfix = _postfix;
        }

        public int index;
        public int start;
        public int size;
        public TokenType type;
        public string prefix;
        public string postfix;
    }
    public class StringExtractor
    {
        static List<Token> Find(string input, string find, TokenType type)
        {
            List<Token> vec = new List<Token>();
            if (find.Count() == 0)
                return null;
            ;

            int pos = 0;
            pos = input.IndexOf(find);
            while (pos != -1)
            {
                vec.Add(new Token(-1, pos, find.Count(), type, "", ""));
                pos += find.Count();
                pos = input.IndexOf(find, pos);
            }
            return vec;
        }

        public static List<Token> TokenizeFmtString(string fmt)
        {
            List<Token> vecMatter = Find(fmt, "{}", TokenType.Matter);
            List<Token> vecHex = Find(fmt, "{h}", TokenType.Hex);
            List<Token> vecX = Find(fmt, "{x}", TokenType.None);
            List<Token> vecTrim = Find(fmt, "{t}", TokenType.Trim);

            List<Token> vec = new List<Token>();
            foreach (var a in vecMatter)
            {
                vec.Add(a);
            }
            foreach (var a in vecHex)
            {
                vec.Add(a);
            }
            foreach (var a in vecX)
            {
                vec.Add(a);
            }
            foreach (var a in vecTrim)
            {
                vec.Add(a);
            }

            int countDiffTokenType = 0;
            if (vecMatter.Count() > 0)
                countDiffTokenType++;
            if (vecHex.Count() > 0)
                countDiffTokenType++;
            if (vecX.Count() > 0)
                countDiffTokenType++;
            if (vecTrim.Count() > 0)
                countDiffTokenType++;

            if (countDiffTokenType > 1)
            {
                vec.Sort(delegate (Token a, Token b) { return a.start.CompareTo(b.start); });
            }

            for (int i = 0; i < vec.Count(); ++i)
            {
                var curr = vec[i];

                if (i == 0)
                {
                    if (curr.start == 0)
                    {
                        curr.prefix = "";
                    }
                    else
                    {
                        curr.prefix = fmt.Substring(0, curr.start);
                    }

                    if (i + 1 < vec.Count())
                    {
                        var next = vec[i + 1];
                        if (curr.start + curr.size == next.start)
                        {
                            Console.WriteLine("Error: Format specifier {} cannot be side by side! For example: {}{}");
                            return null;
                        }
                        else if (curr.start + curr.size < next.start)
                        {
                            curr.postfix = fmt.Substring(curr.start + curr.size, next.start - (curr.start + curr.size));
                        }
                    }
                    else
                    {
                        curr.postfix = fmt.Substring(curr.start + curr.size);
                    }
                    continue;
                }

                if (i + 1 < vec.Count() && i > 0)
                {
                    var prev = vec[i - 1];

                    curr.prefix = fmt.Substring(prev.start + prev.size, curr.start - (prev.start + prev.size));

                    var next = vec[i + 1];
                    if (curr.start + curr.size == next.start)
                    {
                        Console.WriteLine("Error: Format specifier {} cannot be side by side! For example: {}{}");
                        return null;
                    }
                    else if (curr.start + curr.size < next.start)
                    {
                        curr.postfix = fmt.Substring(curr.start + curr.size, next.start - (curr.start + curr.size));
                    }
                    continue;
                }

                if (i == (vec.Count() - 1) && i > 0)
                {
                    var prev = vec[i - 1];

                    curr.prefix = fmt.Substring(prev.start + prev.size, curr.start - (prev.start + prev.size));
                    curr.postfix = fmt.Substring(curr.start + curr.size);
                }
            }

            int index = 0;
            foreach (var a in vec)
            {
                if (a.type != TokenType.None)
                {
                    a.index = index;

                    ++index;
                }
            }

            return vec;
        }

        static void ValuesExtractHelp(string input, List<Token> tokens, ref List<string> results)
        {
            int token_size = 0;

            for (int i = 0; i < tokens.Count(); ++i)
            {
                if (tokens[i].type != TokenType.None)
                    ++token_size;
            }

            int prefix_pos = 0;
            int postfix_pos = 0;
            for (int i = 0; i < tokens.Count(); ++i)
            {
                var curr = tokens[i];

                if (String.IsNullOrEmpty(curr.prefix) == false)
                {
                    prefix_pos = input.IndexOf(curr.prefix, prefix_pos);

                    if (prefix_pos == -1)
                    {
                        Console.WriteLine("prefix_pos Error");
                        return;
                    }
                    prefix_pos += curr.prefix.Count();
                }

                postfix_pos = prefix_pos + 1;
                if (String.IsNullOrEmpty(curr.postfix) == false)
                {
                    postfix_pos = input.IndexOf(curr.postfix, postfix_pos);

                    if (postfix_pos == -1)
                    {
                        postfix_pos = prefix_pos;
                        if (String.IsNullOrEmpty(curr.postfix) == false)
                        {
                            postfix_pos = input.IndexOf(curr.postfix, postfix_pos);
                        }

                        if (postfix_pos == -1)
                        {
                            Console.WriteLine("postfix_pos Error");
                            return;
                        }
                    }
                }
                else
                {
                    postfix_pos = -1;
                }

                string res = "";
                if (prefix_pos != -1)
                {
                    if (postfix_pos == -1)
                        res = input.Substring(prefix_pos);
                    else
                        res = input.Substring(prefix_pos, postfix_pos - prefix_pos);

                    if (curr.index != -1)
                    {
                        if (curr.type == TokenType.Trim)
                            res = res.Trim();
                        if (curr.type == TokenType.Hex)
                        {
                            if (res.StartsWith("0x"))
                            {
                                res = res.Substring(2);
                            }
                            int intValue = Convert.ToInt32(res, 16);
                            res = intValue.ToString();
                        }

                        results.Add(res);
                    }
                }
                prefix_pos += res.Count();
            }
        }
        public static void Extract(string input, string fmt, ref string output1)
        {
            List<Token> tokens = TokenizeFmtString(fmt);

            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
        }
        public static void Extract(string input, string fmt, ref string output1, ref string output2)
        {
            List<Token> tokens = TokenizeFmtString(fmt);

            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
        }
        public static void Extract(string input, string fmt, ref string output1, ref string output2, ref string output3)
        {
            List<Token> tokens = TokenizeFmtString(fmt);

            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
            output3 = results[2];
        }
        public static void Extract(string input, string fmt, ref string output1, ref string output2, ref string output3, ref string output4)
        {
            List<Token> tokens = TokenizeFmtString(fmt);

            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
            output3 = results[2];
            output4 = results[3];
        }
        public static void Extract(string input, string fmt, ref string output1, ref string output2, ref string output3, ref string output4, ref string output5)
        {
            List<Token> tokens = TokenizeFmtString(fmt);

            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
            output3 = results[2];
            output4 = results[3];
            output5 = results[4];
        }
        public static void ExtractFromToken(string input, List<Token> tokens, ref string output1)
        {
            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
        }
        public static void ExtractFromToken(string input, List<Token> tokens, ref string output1, ref string output2)
        {
            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
        }
        public static void ExtractFromToken(string input, List<Token> tokens, ref string output1, ref string output2, ref string output3)
        {
            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
            output3 = results[2];
        }
        public static void ExtractFromToken(string input, List<Token> tokens, ref string output1, ref string output2, ref string output3, ref string output4)
        {
            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
            output3 = results[2];
            output4 = results[3];
        }
        public static void ExtractFromToken(string input, List<Token> tokens, ref string output1, ref string output2, ref string output3, ref string output4, ref string output5)
        {
            List<string> results = new List<string>();

            ValuesExtractHelp(input, tokens, ref results);

            output1 = results[0];
            output2 = results[1];
            output3 = results[2];
            output4 = results[3];
            output5 = results[4];
        }

        public static bool IsInputMatchedFmt(string input, string fmt)
        {
            List<Token> tokens = TokenizeFmtString(fmt);

            int pos = 0;
            int prev_pos = 0;
            foreach (var token in tokens)
            {
                if (String.IsNullOrWhiteSpace(token.prefix) == false)
                {
                    pos = input.IndexOf(token.prefix, prev_pos);

                    if (pos != -1)
                    {
                        if (pos < prev_pos)
                            return false;
                    }
                    else
                        return false;

                    prev_pos = pos;
                }

                if (String.IsNullOrWhiteSpace(token.postfix) == false)
                {
                    pos = input.IndexOf(token.postfix, prev_pos);

                    if (pos != -1)
                    {
                        if (pos < prev_pos)
                            return false;
                    }
                    else
                        return false;

                    prev_pos = pos;
                }
            }
            return true;
        }
        public static bool IsInputMatchedTokens(string input, List<Token> tokens)
        {
            int pos = 0;
            int prev_pos = 0;
            foreach (var token in tokens)
            {
                if (String.IsNullOrWhiteSpace(token.prefix) == false)
                {
                    pos = input.IndexOf(token.prefix, prev_pos);

                    if (pos != -1)
                    {
                        if (pos < prev_pos)
                            return false;
                    }
                    else
                        return false;

                    prev_pos = pos;
                }

                if (String.IsNullOrWhiteSpace(token.postfix) == false)
                {
                    pos = input.IndexOf(token.postfix, prev_pos);

                    if (pos != -1)
                    {
                        if (pos < prev_pos)
                            return false;
                    }
                    else
                        return false;

                    prev_pos = pos;
                }
            }
            return true;
        }
    }
}
