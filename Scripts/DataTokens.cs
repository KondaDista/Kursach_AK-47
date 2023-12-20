using System.Collections.Generic;

namespace Kursach_AK_47
{
    public static class DataTokens
    {
        private static HashSet<char> _numbers = new()
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private static HashSet<char> _alphabet = new()
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
            'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
            'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
            'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        private static HashSet<char> _symbolsInNums = new()
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'O', 'H', 'a', 'b', 'c', 'd', 'e', 'f', 'o', 'h', '0', '1', '2', '3', '4',
            '5', '6', '7', '8', '9', '.', '+', '-'
        };
        
        private static HashSet<char> _numberSystems = new()
        {
            'B', 'b', 'O', 'o', 'D', 'd', 'H', 'h' 
        };

        private static HashSet<char> _bin = new()
        {
            'B', 'b', '0', '1'
        };

        private static HashSet<char> _oct = new()
        {
            'O', 'o', '0', '1', '2', '3', '4', '5', '6', '7'
        };

        private static HashSet<char> _dec = new()
        {
            'D', 'd', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private static HashSet<char> _hex = new()
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'H', 'a', 'b', 'c', 'd', 'e', 'f', 'h', '0', '1', '2', '3', '4', '5', '6',
            '7', '8', '9'
        };

        private static HashSet<char> _float = new()
        {
            '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private static HashSet<char> _exp = new()
        {
            '.', 'E', 'e', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-'
        };

        private static HashSet<char> _symbols = new()
        {
            '!', '=', '<', '>', '+', '-', '|', '*', '/',
            '(', ')', '.', ':', ';', ',', '{',
            '}', '#', '@', '&', '\r', '\n', ' '
        };

        private static Dictionary<string, int> _tableServiceWords = new()
        {
            { "true", 1 },
            { "false", 2 },
            { "var", 3 },
            { "let", 4 },
            { "if", 5 },
            { "then", 6 },
            { "else", 7 },
            { "end", 8 },
            { "do", 9 },
            { "while", 10 },
            { "loop", 11 },
            { "for", 12 },
            { "end_else", 13 },
            { "!", 14 },
            { "#", 15 },
            { "@", 16 },
            { "&", 17 }
        };


        private static Dictionary<string, int> _tableLimiters = new()
        {
            { "!=", 1 },
            { "==", 2 },
            { "<", 3 },
            { "<=", 4 },
            { ">", 5 },
            { ">=", 6 },
            { "+", 7 },
            { "-", 8 },
            { "||", 9 },
            { "*", 10 },
            { "/", 11 },
            { "&&", 12 },
            { "(", 13 },
            { ")", 14 },
            { ".", 15 },
            { ":", 16 },
            { ",", 17 },
            { ";", 18 },
            { "{", 19 },
            { "}", 20 },
            { "=", 21 },
            { "\r\n", 22 },
            { " ", 23 },
        };

        public static bool ContainsTableServiceWords(string word)
        {
            if (_tableServiceWords.ContainsKey(word))
                return true;
            return false;
        }
        public static int GetIndexTableServiceWords(string word)
        {
            return _tableServiceWords[word];
        }
        
        public static bool ContainsTableLimiters(string word)
        {
            if (_tableLimiters.ContainsKey(word))
                return true;
            return false;
        }
        public static int GetIndexTableLimiters(string word)
        {
            return _tableLimiters[word];
        }
        
        public static bool ContainsNumber(ref char CH)
        {
            if (_numbers.Contains(CH) || CH == '.')
                return true;
            return false;
        }
        public static bool ContainsLetter(ref char CH)
        {
            if (_alphabet.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsSymbol(ref char CH)
        {
            if (_symbols.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsSymbolInNumbers(ref char CH)
        {
            if (_symbolsInNums.Contains(CH))
                return true;
            return false;
        }
        
        public static bool ContainsBin(char CH)
        {
            if (_bin.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsOct(char CH)
        {
            if (_oct.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsDec(char CH)
        {
            if (_dec.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsHex(char CH)
        {
            if (_hex.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsFloat(char CH)
        {
            if (_float.Contains(CH))
                return true;
            return false;
        }
        public static bool ContainsExp(char CH)
        {
            if (_exp.Contains(CH))
                return true;
            return false;
        }
        
        public static bool ContainsNumberSystem(char CH)
        {
            if (_numberSystems.Contains(CH))
                return true;
            return false;
        }
    }
}