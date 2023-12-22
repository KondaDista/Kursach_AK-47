using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kursach_AK_47;

public class SyntacticAnalyzer
{
    private bool _endAnalysis = false;
    private bool _errorAnalysis = false;
    private List<string> _errorMessage =new();

    private int indexElement = 0;
    private List<string> _resultSyntacticAnalyze = new();

    private Dictionary<string, string> _descriptions = new();
    private List<string> _operations = new();

    public SyntacticAnalyzer(List<string> _resultLexicalAnalyze)
    {
        _resultSyntacticAnalyze = _resultLexicalAnalyze;
        StartAnalyze();
    }
    
    private Ellement NextEllement()
    {
        if (indexElement < _resultSyntacticAnalyze.Count - 1)
            indexElement++;
        return new Ellement(_resultSyntacticAnalyze[indexElement]);
    }
    
    private Ellement NoSpaceNextEllement()
    {
        Ellement ellement = NextEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber == 23)
        {
            ellement = NextEllement();
        }
        return ellement;
    }
    
    private Ellement NoSpaceAndLineBrokeNextEllement()
    {
        Ellement ellement = NoSpaceNextEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber == 22)
        {
            ellement = NoSpaceNextEllement();
        }
        return ellement;
    }
    
    private Ellement PreviosEllement()
    {
        if (indexElement < _resultSyntacticAnalyze.Count - 1)
            indexElement--;
        return new Ellement(_resultSyntacticAnalyze[indexElement]);
    }
    
    private Ellement NoSpacePreviosEllement()
    {
        Ellement ellement = PreviosEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber == 23)
        {
            ellement = PreviosEllement();
        }
        return ellement;
    }
    
    private Ellement NoSpaceAndLineBrokePreviosEllement()
    {
        Ellement ellement = NoSpacePreviosEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber == 22)
        {
            ellement = NoSpacePreviosEllement();
        }
        return ellement;
    }

    private bool HasNext()
    {
        return indexElement < _resultSyntacticAnalyze.Count - 1;
    }

    private void StartAnalyze()
    {
        while (HasNext())
        {
            Ellement ellement = new Ellement(_resultSyntacticAnalyze[indexElement]);

            if (ellement.tableNumber == 1 && ellement.valueNumber is 22 or 23 or 16)
                ellement = NoSpaceAndLineBrokeNextEllement();
            
            VerifyDescription(ref ellement);
            
            if (ellement.tableNumber == 0 && ellement.valueNumber == 8)
                _endAnalysis = true;

            if (_endAnalysis)
                _errorMessage.Add("Конец синтаксического анализа: Ошибок не обнаружено.");
            if (_errorAnalysis)
            {
                _errorMessage.Add($"Ошибка синтаксического анализатора: считана не передусмотренная лексема [{ellement.tableNumber},{ellement.valueNumber}].");
                break;
            }
        }
    }

    private void VerifyDescription(ref Ellement ellement)
    {
        bool errorVerify = false;
        if (ellement.tableNumber == 0 && ellement.valueNumber == 3)
        {
            ellement = NoSpaceNextEllement();
            while (DataTokens.ContainsTableIdentification(ellement.valueNumber))
            {
                ellement = NoSpaceNextEllement();
                while (ellement.tableNumber == 1 && ellement.valueNumber == 17)
                {
                    ellement = NoSpaceNextEllement();
                    if (DataTokens.ContainsTableIdentification(ellement.valueNumber))
                        ellement = NoSpaceNextEllement();
                    else
                    {
                        _errorAnalysis = true;
                        _errorMessage.Add(
                            "Ошибка синтаксического анализатора: считана не реализованный идентификатор.");
                    }
                }
                if (ellement.tableNumber == 1 && ellement.valueNumber == 16)
                {
                    ellement = NoSpaceNextEllement();
                    if (ellement.tableNumber == 0 && (ellement.valueNumber == 15 || ellement.valueNumber == 16 ||
                                                      ellement.valueNumber == 17))
                    {
                        ellement = NoSpaceNextEllement();
                        if (ellement.tableNumber == 1 && ellement.valueNumber == 18)
                            ellement = NoSpaceNextEllement();
                        else
                        {
                            _errorAnalysis = true;
                            _errorMessage.Add("Ошибка синтаксического анализатора: жду ;");
                        }
                    }
                    else
                    {
                        _errorAnalysis = true;
                        _errorMessage.Add("Ошибка синтаксического анализатора: нет такого типа данных");
                    }
                }
            }
        }
        else
            VerifyOperator(ref ellement);
        if (ellement.tableNumber == 1 && ellement.valueNumber is 22 or 23 or 16 or 14)
            ellement = NoSpaceAndLineBrokeNextEllement();
    }

    private void VerifyOperator(ref Ellement ellement)
    {
        if (ellement.tableNumber == 1 && ellement.valueNumber == 19)
            CompoundOperator(ref ellement);
        if (ellement.tableNumber == 2)
            AssignmentOperatorForIdentification(ref ellement);
        else if (ellement.tableNumber == 0)
        {
            switch (ellement.valueNumber)
            {
                case 4:
                    AssignmentOperator(ref ellement);
                    break;
                case 5:
                    ConditionalOperator(ref ellement);
                    break;
                case 12:
                    FixedLoopOperator(ref ellement);
                    break;
                case 13:
                    break;
                case 8:
                    break;
                case 9:
                    ConditionalLoopOperator(ref ellement);
                    break;
                case 18:
                    InputOperator(ref ellement);
                    break;
                case 19:
                    OutputOperator(ref ellement);
                    break;
                default:
                    _errorAnalysis = true;
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры составного оператора не найдено ключевое слово.");
                    break;
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение оператора.");
        }
    }

    private void CompoundOperator(ref Ellement ellement)
    {
        ellement = NoSpaceAndLineBrokeNextEllement();
        VerifyOperator(ref ellement);

        ellement = NoSpaceAndLineBrokeNextEllement();
        while (indexElement < _resultSyntacticAnalyze.Count - 1)
        {
            if (isSemicolon(ref ellement))
            {
                ellement = NoSpaceAndLineBrokeNextEllement();
                VerifyOperator(ref ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
            }
            else if (ellement.tableNumber == 1 && ellement.valueNumber == 20)
            {
                ellement = NoSpaceAndLineBrokeNextEllement();
                return;
            }
            else
            {
                _errorAnalysis = true;
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры составного оператора.");
                return;
            }
        }
    }
    
    private void ConditionalOperator(ref Ellement ellement) 
    {
        ellement = NoSpaceNextEllement();
        _operations.Add(Expression(ref ellement));
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 0 && ellement.valueNumber == 6)
        {
            ellement = NoSpaceAndLineBrokeNextEllement();
            VerifyOperator(ref ellement);
            if (ellement.tableNumber == 0 && ellement.valueNumber == 7) {
                ellement = NoSpaceAndLineBrokeNextEllement();
                VerifyOperator(ref ellement);
                ellement = NoSpaceNextEllement();
            }
            if (ellement.tableNumber != 0 && ellement.valueNumber != 13)
            {
                _errorAnalysis = true;
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры условного оператора ожидается \"end_else\".");
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры условного оператора.");
        }
    }
    
    private void ConditionalLoopOperator(ref Ellement ellement) {
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 0 && ellement.valueNumber == 10)
        {
            Expression(ref ellement);
            if (!_errorAnalysis)
            {
                ellement = NoSpaceNextEllement();
                VerifyOperator(ref ellement);
                ellement = NoSpaceNextEllement();
                if (ellement.tableNumber != 0 && ellement.valueNumber != 11)
                {
                    _errorAnalysis = true;
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператор условного цикла ожидается \"loop\".");
                }
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператор условного, ожидается \"while\".");
        }
        
    }

    private void FixedLoopOperator(ref Ellement ellement)
    {
        ellement = NoSpaceAndLineBrokeNextEllement();
        if (ellement.tableNumber == 1 && ellement.valueNumber == 13)
        {
            ellement = NoSpaceAndLineBrokeNextEllement();
            if (!isSemicolon(ref ellement))
            {
                Expression(ref ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
                if (!isSemicolon(ref ellement))
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \";\".");
            }
            else
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \";\".");
            
            ellement = NoSpaceAndLineBrokeNextEllement();
            if (!isSemicolon(ref ellement))
            {
                Expression(ref ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
                if (!isSemicolon(ref ellement))
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \";\".");
            }
            else
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \";\".");

            ellement = NoSpaceAndLineBrokeNextEllement();
            if (!(ellement.tableNumber == 1 && ellement.valueNumber == 14))
            {
                Expression(ref ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
                if (!(ellement.tableNumber == 1 && ellement.valueNumber == 14))
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \")\".");
            }

            ellement = NoSpaceAndLineBrokeNextEllement();
            VerifyOperator(ref ellement);
        }
        else
            _errorMessage.Add(
                "Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \"(\".");
    }

    private bool isSemicolon(ref Ellement ellement)
    {
        return ellement.tableNumber == 1 && ellement.valueNumber == 18;
    }
    
    private void InputOperator(ref Ellement ellement) {
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 1 && ellement.valueNumber == 13)
        {
            ellement = NoSpaceNextEllement();
            if (ellement.tableNumber == 2)
            {
                ellement = NextEllement();
                while (ellement.tableNumber == 1 && ellement.valueNumber == 23)
                {
                    ellement = NextEllement();
                    if (ellement.tableNumber == 2)
                        ellement = NextEllement();
                    else
                    {
                        _errorAnalysis = true;
                        _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора ввода, не найден идентификатор.");
                        break;
                    }
                }

                if (ellement.tableNumber != 1 && ellement.valueNumber != 14)
                {
                    _errorAnalysis = true;
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора ввода, ожидается \")\".");
                }
            }
            else
            {
                _errorAnalysis = true;
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора ввода, не найден идентификатор.");
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора ввода, ожидается \"(\".");
        }
    }
    
    private void OutputOperator(ref Ellement ellement) {
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 1 && ellement.valueNumber == 13)
        {
            ellement = NoSpaceNextEllement();
            Expression(ref ellement);
            if (!_errorAnalysis)
            {
                ellement = NextEllement();
                while (ellement.tableNumber == 1 && ellement.valueNumber == 23)
                {
                    ellement = NextEllement();
                    Expression(ref ellement);
                    if (!_errorAnalysis)
                        ellement = NextEllement();
                    else
                    {
                        _errorAnalysis = true;
                        _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора вывода, ошибка в выражении.");
                        break;
                    }
                }

                if (ellement.tableNumber != 1 && ellement.valueNumber != 14)
                {
                    _errorAnalysis = true;
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора вывода, ожидается \")\".");
                }
            }
            else
            {
                _errorAnalysis = true;
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора вывода, ошибка в выражении.");
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора вывода, ожидается \"(\".");
        }
    }
    
    private void AssignmentOperatorForIdentification(ref Ellement ellement) 
    {
        if (DataTokens.ContainsTableIdentification(ellement.valueNumber))
        {
            _operations.Add(Expression(ref ellement));
            ellement = NoSpaceNextEllement();
            if (ellement.tableNumber == 1 && ellement.valueNumber == 21)
            {
                ellement = NoSpaceNextEllement();
                _operations.Add(Expression(ref ellement));
            }
            else
            {
                _errorAnalysis = true;
                _errorMessage.Add($"Ошибка синтаксического анализатора: нарушение структуры оператора присвоения var [{ellement.tableNumber },{ellement.valueNumber }].");
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add($"Ошибка синтаксического анализатора: нарушение структуры оператора присвоения var не найден идентификатор [{ellement.tableNumber },{ellement.valueNumber }].");
        }
    }

    private void AssignmentOperator(ref Ellement ellement)
    {
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 2 && DataTokens.ContainsTableIdentification(ellement.valueNumber))
        {
            //putIdentification(next.index, 4);
            ellement = NoSpaceNextEllement();
            if (ellement.tableNumber == 1 && ellement.valueNumber == 21)
            {
                ellement = NoSpaceNextEllement();
                _operations.Add(Expression(ref ellement));
                ellement = NoSpaceAndLineBrokeNextEllement();
            }
            else
            {
                _errorAnalysis = true;
                _errorMessage.Add($"Ошибка синтаксического анализатора: нарушение структуры оператора присвоения [{ellement.tableNumber },{ellement.valueNumber }].");
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add($"Ошибка синтаксического анализатора: нарушение структуры оператора присвоения не найден идентификатор [{ellement.tableNumber },{ellement.valueNumber }].");
        }
    }

    private String Expression(ref Ellement ellement)
    {
        StringBuilder operation = new StringBuilder();
        operation.Append(Operand(ref ellement));
        while (HasNext())
        {
            ellement = NoSpaceAndLineBrokeNextEllement();
            if (ellement.tableNumber == 1 && ellement.valueNumber is 1 or 2 or 3 or 4 or 5 or 6)
            {
                operation.Append(ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
                operation.Append(Operand(ref ellement));
            }
            else
            {
                ellement = NoSpaceAndLineBrokePreviosEllement();
                return operation.ToString();
            }
        }
        return operation.ToString();
    }

    private String Operand(ref Ellement ellement)
    {
        StringBuilder operation = new StringBuilder();
        operation.Append(Summand(ref ellement));
        while (HasNext())
        {
            ellement = NoSpaceAndLineBrokeNextEllement();
            if (ellement.tableNumber == 1 && ellement.valueNumber is 7 or 8 or 9)
            {
                operation.Append(ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
                operation.Append(Summand(ref ellement));
            }
            else
            {
                ellement = NoSpaceAndLineBrokePreviosEllement();
                return operation.ToString();
            }
        }
        return operation.ToString();
    }

    private String Summand(ref Ellement ellement)
    {
        StringBuilder operation = new StringBuilder();
        operation.Append(Multiplier(ref ellement));
        while (HasNext())
        {
            ellement = NoSpaceAndLineBrokeNextEllement();
            if (ellement.tableNumber == 1 && ellement.valueNumber is 10 or 11 or 12)
            {
                operation.Append(ellement);
                ellement = NoSpaceAndLineBrokeNextEllement();
                operation.Append(Multiplier(ref ellement));
            }
            else
            {
                ellement = NoSpaceAndLineBrokePreviosEllement();
                return operation.ToString();
            }
        }
        return operation.ToString();
    }

    private String Multiplier(ref Ellement ellement)
    {
        int stateParentheses = 0;
        StringBuilder multiplierString = new StringBuilder();
        if (ellement.tableNumber is 2 or 3 || (ellement.tableNumber == 0 && ellement.valueNumber is 1 or 2))
        {
            multiplierString.Append(ellement);
        }
        else if (ellement.tableNumber == 0 && ellement.valueNumber == 14)
        {
            multiplierString.Append(ellement).Append(Multiplier(ref ellement));
        }
        else if (ellement.tableNumber == 1)
        {
            if (ellement.valueNumber == 13)
            {
                stateParentheses = 1;
                multiplierString.Append(ellement).Append(Expression(ref ellement));
                ellement = NoSpaceAndLineBrokeNextEllement();
            }
            if (ellement.valueNumber == 14)
            {
                if (stateParentheses == 1)
                    multiplierString.Append(ellement);
            }

            if (stateParentheses != 1)
            {
                _errorAnalysis = true;
                _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры \" ( множитель ) \".");
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры множителя.");
        }

        return multiplierString.ToString();
    }

    public class Ellement
    {
        public int tableNumber { get; private set; }
        public int valueNumber { get; private set; }
        
        public string nameWord { get; private set; }

        public Ellement(string word)
        {
            string[] nums = word.Trim('[', ']').Split(',');
            tableNumber = Convert.ToInt32(nums[0]);
            valueNumber = Convert.ToInt32(nums[1]);
            nameWord = SetWord();
        }
        
        public String SetWord()
        {
            if (tableNumber == 0) 
            {
                return DataTokens.GetValueTableServiceWords(valueNumber);
            }
            else if (tableNumber == 1) 
            {
                return DataTokens.GetValueTableLimiters(valueNumber);
            }
            else if (tableNumber == 2) 
            {
                return DataTokens.GetValueTableIdentificator(valueNumber);
            }
            else if (tableNumber == 3) 
            {
                return DataTokens.GetValueTableNumber(valueNumber);
            }
            else
                return "";
        }
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