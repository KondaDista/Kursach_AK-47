using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kursach_AK_47;

public class LexicalAnalyzer
{
    private StreamReader _streamReader;
    private bool _endAnalysis = false;
    private bool _errorAnalysis = false;
    private List<string> _errorMessage =new();

    private List<string> _resultLexicalAnalyze = new();
    
    private int typeOfNumber = 0;

    public LexicalAnalyzer(string path)
    {
        Analyzer(path);
    }

    private void Analyzer(string path)
    {
        try
        {
            using (_streamReader = File.OpenText(path))
            {
                while (_streamReader.Peek() != -1)
                {
                    char CH = (char)_streamReader.Read();

                    if (DataTokens.ContainsLetter(ref CH))
                    {
                        _readingWord(ref CH, out string word);
                        _verificationWord(word);
                    }
                    if (DataTokens.ContainsNumber(ref CH) || CH == '.')
                    {
                        _readingNumber(ref CH, out string word);
                        AddNumber(_verificationNumber(word));
                    }
                    if (DataTokens.ContainsSymbol(ref CH))
                    {
                        _readingSymbol(ref CH, out string word);
                        _verificationSymbol(word);
                    }

                    if (_endAnalysis)
                    {
                        _errorMessage.Add("Конец лексического анализа: Ошибок не обнаружено.");
                        break;
                    }

                }
            }
        }
        catch (Exception e)
        {
            _errorMessage.Add($"Ошибка чтения: Произошлая проблема во время чтения файла.\r\nТекст ошибки: {e}");
            Console.WriteLine(e);
            throw;
        }
    }

    private void _readingWord(ref char CH, out string word)
    {
        word = $"{CH}";
        while (_streamReader.Peek() != -1)
        {
            char nextCH = (char)_streamReader.Peek();
            if (DataTokens.ContainsLetter(ref nextCH) || DataTokens.ContainsNumber(ref nextCH) || nextCH == '_')
                word += (char)_streamReader.Read();
            else
                break;
        }
    }
    private void _verificationWord(string word)
    {
        if (DataTokens.ContainsTableServiceWords(word)) // Поиск в таблице служебных слов
        {
            int index = DataTokens.GetIndexTableServiceWords(word);
            _resultLexicalAnalyze.Add($"[0,{index}]");
            if (index == 8)
                _endAnalysis = true;
        }
        else
        {
            if (DataTokens.ContainsTableLimiters(word)) // Поиск в таблице разделителей
            {
                _resultLexicalAnalyze.Add($"[1,{DataTokens.GetIndexTableLimiters(word)}]");
            }
            else
            {
                // Поиск в таблице идентификаторов
                int index;
                if (!DataTokens.ContainsTableIdentification(word))
                {
                    index = DataTokens.GetCountTableIdentification();
                    DataTokens.AddedValueInTableIdentification(index, word);
                }
                else
                {
                    index = DataTokens.GetIndexTableIdentification(word);
                }

                _resultLexicalAnalyze.Add($"[2,{index}]");
            }
        }
    }

    private void _readingNumber(ref char CH, out string word)
    {
        word = $"{CH}";
        while (_streamReader.Peek() != -1)
        {
            char nextCH = (char)_streamReader.Peek();
            if (DataTokens.ContainsSymbolInNumbers(ref nextCH))
            {
                if (nextCH is '+' or '-')
                {
                    int i = 0;
                    for (; i < word.Length; i++)
                        if (word[i] == 'E' || word[i] == 'e')
                            break;

                    if (i == word.Length)
                        break;
                }

                word += (char)_streamReader.Read();
            }
            else
                break;
        }
    }
    private string _verificationNumber(string word)
    {
        int index;
        string result;
        string substring = word;

        if (DataTokens.ContainsNumberSystem(word[word.Length - 1]))
            substring = word.Substring(0, word.Length - 1);

        if (IsBinary(word))
        {
            result = substring;
            typeOfNumber = 1;
        }
        else if (IsOct(word))
        {
            result = Convert.ToString(Convert.ToInt32(substring, 8), 2);
            typeOfNumber = 1;
        }
        else if (IsDec(word))
        {
            result = Convert.ToString(Convert.ToInt32(substring, 10), 2);
            typeOfNumber = 1;
        }
        else if (IsHex(word))
        {
            result = Convert.ToString(Convert.ToInt32(substring, 16), 2);
            typeOfNumber = 1;
        }
        else if (IsFloat(word))
        {
            result = ConvertFloatToBin(word);
            typeOfNumber = 2;
        }
        else if (IsExp(word))
        {
            char splitSymbol = word.Contains('E') ? 'E' : 'e';
            string[] expChapters = word.Split(splitSymbol);
            int degree = expChapters[1] == "" ? 1 : Convert.ToInt32(expChapters[1]);

            float expNum = (float)(Convert.ToDouble(expChapters[0].Replace('.', ',')) * Math.Pow(10, degree));

            if (expNum.ToString().Contains(','))
                result = ConvertFloatToBin(expNum.ToString());
            else
                result = Convert.ToString((int)expNum, 2);
            typeOfNumber = 2;
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add($"Ошибка: число \"{word}\" не поддерживается данным языком");
            return "Ошибка: число \"{word}\" не поддерживается данным языком";
        }

        if (!DataTokens.ContainsTableNumbers(result))
        {
            index = DataTokens.GetCountTableNumbers();
            DataTokens.AddedValueInTableNumbers(result);
        }
        else
            index = DataTokens.GetIndexTableNumbers(result);

        _resultLexicalAnalyze.Add($"[3,{index}]");
        return result;
    }
    
    private void AddNumber(string num) {
        int index = DataTokens.GetIndexTableNumbers(num);
        var a = DataTokens._tableNumbers;
        if (DataTokens.tableNumberTypes.ContainsKey(index))
            DataTokens.tableNumberTypes[index] = typeOfNumber;
        else
            DataTokens.tableNumberTypes.Add(index, typeOfNumber);
    }

    private bool IsBinary(string word)
    {
        int binId = 0;
        if (word[word.Length - 1] == 'B' || word[word.Length - 1] == 'b')
        {
            foreach (char ch in word)
            {
                if (!DataTokens.ContainsBin(ch))
                    return false;
                if (ch == 'B' || ch == 'b')
                    binId++;
                if (binId > 1)
                    return false;
            }
            return true;
        }
        return false;
    }
    private bool IsOct(string word)
    {
        int octId = 0;
        if (word[word.Length - 1] == 'O' || word[word.Length - 1] == 'o')
        {
            foreach (char ch in word)
            {
                if (!DataTokens.ContainsOct(ch))
                    return false;
                if (ch == 'O' || ch == 'o')
                    octId++;
                if (octId > 1)
                    return false;
            }
            return true;
        }
        return false;
    }
    private bool IsDec(string word)
    {
        int decId = 0;
        foreach (char ch in word)
        {
            if (!DataTokens.ContainsDec(ch))
                return false;
            if (ch == 'D' || ch == 'd')
                decId++;
            if (decId > 1)
                return false;
        }
        return true;
    }
    private bool IsHex(string word)
    {
        int hexId = 0;
        if (word[word.Length - 1] == 'H' || word[word.Length - 1] == 'h')
        {
            foreach(char ch in word)
            {
                if (!DataTokens.ContainsHex(ch)) 
                    return false;
                if (ch == 'H' || ch == 'h')
                    hexId++;
                if (hexId > 1)
                    return false;
            }
            return true;
        }
        return false;
    }
    private bool IsFloat(string word)
    {
        int dotSimbol = 0;
        for (int i = 0; i < word.Length; i++)
        {
            foreach(char ch in word)
            {
                if (!DataTokens.ContainsFloat(ch)) 
                    return false;
                if (ch == '.')
                    dotSimbol++;
                if (dotSimbol > 1)
                    return false;
            }
            return true;
        }
        return true;
    }
    private bool IsExp(string word)
    {
        int expSymbol = 0;
        int tokenSymbol = 0;
        int dotSymbol = 0;
        foreach (char ch in word)
        {
            if (!DataTokens.ContainsExp(ch))
                return false;
            if (ch is 'E' or 'e')
                expSymbol++;
            if (ch is '+' or '-')
                tokenSymbol++;
            if (ch == '.')
                dotSymbol++;
            if (tokenSymbol > 1 || expSymbol > 1 || dotSymbol > 1)
                return false;
        }
        return true;
    }

    private string ConvertFloatToBin(string word)
    {
        string[] floatNum = word.Replace('.',',').Split(',');
        string firstBinChapter = Convert.ToString(Convert.ToInt32(floatNum[0], 10), 2);
        float secondIntChapter = (float)(Convert.ToInt32(floatNum[1], 10) / Math.Pow(10, floatNum[1].Length));
        string secondBinChapter = "";

        int count = 10;
        while (count != 0)
        {
            secondIntChapter *= 2f;
            secondBinChapter += secondIntChapter.ToString()[0];

            floatNum = secondIntChapter.ToString().Split(',');
            secondIntChapter = (float)(Convert.ToInt32(floatNum[1], 10) / Math.Pow(10, floatNum[1].Length));
            count--;
        }
        return firstBinChapter + "." + secondBinChapter;
    }
    
    private void _readingSymbol(ref char CH, out string word)
    {
        word = $"{CH}";
        char nextCH = (char)_streamReader.Peek();

        switch (CH)
        {
            case '!':
                if (nextCH == '=')
                    word += (char)_streamReader.Read();
                break;
            case '<':
                if (nextCH == '=')
                    word += (char)_streamReader.Read();
                break;
            case '>':
                if (nextCH == '=')
                    word += (char)_streamReader.Read();
                break;
            case '=':
                if (nextCH == '=')
                    word += (char)_streamReader.Read();
                break;
            case '#':
                break;
            case '@':
                break;
            case '&':
                if (nextCH == '&')
                    word += (char)_streamReader.Read();
                break;
            case '\r':
                if (nextCH == '\n')
                    word += (char)_streamReader.Read();
                else
                {
                    _errorAnalysis = true;
                    _errorMessage.Add("Ошибка: проблема с переносом строки, после переноса каретки \\r отстутствует символ переноса строки \\n");
                }

                break;
            case '|':
                if (nextCH == '|')
                    word += (char)_streamReader.Read();
                else
                {
                    _errorAnalysis = true;
                    _errorMessage.Add("Ошибка: операции \"|\" нет в списке разделителей, возможно вы имели \"||\"");
                }

                break;
            case '(': // (* adc *)
                if (_streamReader.Peek() == '*')
                {
                    int commentState = 1;
                    word += (char)_streamReader.Read();
                    while (_streamReader.Peek() != -1)
                    {
                        char nextSymbolInComment = (char)_streamReader.Peek();
                        if (nextSymbolInComment == '*')
                        {
                            switch (commentState)
                            {
                                case 1:
                                    commentState = 2;
                                    break;
                                default:
                                    commentState = -1;
                                    break;
                            }
                        }

                        if (commentState == 2 && _streamReader.Peek() == ')')
                        {
                            word += (char)_streamReader.Read();
                            break;
                        }

                        word += (char)_streamReader.Read();
                    }

                    if (commentState != -1)
                        word = " ";
                    else
                    {
                        _errorAnalysis = true;
                        _errorMessage.Add("Ошибка: неверно написан комментарий");
                    }
                }

                break;
        }
    }
    private void _verificationSymbol(string word)
    {
        if (DataTokens.ContainsTableLimiters(word))
        {
            _resultLexicalAnalyze.Add($"[1,{DataTokens.GetIndexTableLimiters(word)}]");
        }
        else if (word is "!" or "#" or "@" or "&")
        {
            _resultLexicalAnalyze.Add($"[0,{DataTokens.GetIndexTableServiceWords(word)}]");
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add($"Ошибка: символ [{word}] не предусмотрен данным языком");
        }
    }

    public List<string> GetResult()
    {
        return _resultLexicalAnalyze;
    }
    public List<string> GetErrorMessage()
    {
        return _errorMessage;
    }
    public bool GetErrors()
    {
        return _errorAnalysis;
    }
}