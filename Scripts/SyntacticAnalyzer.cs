using System;
using System.Collections.Generic;
using System.Text;

namespace Kursach_AK_47;

public class SyntacticAnalyzer
{
    private bool _endAnalysis = false;
    private bool _errorAnalysis = false;
    private List<string> _errorMessage;

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

    private void StartAnalyze()
    {
        while (indexElement < _resultSyntacticAnalyze.Count)
        {
            Ellement ellement = new Ellement(_resultSyntacticAnalyze[indexElement]);

            VerifyDescription(ref ellement);
            ellement = NoSpaceNextEllement();

            if (ellement.tableNumber == 0 && ellement.valueNumber == 8)
                _endAnalysis = true;

            if (_endAnalysis)
                _errorMessage.Add("Конец синтаксического анализа: Ошибок не обнаружено.");
            if (_errorAnalysis)
            {
                _errorMessage.Add("Ошибка синтаксического анализатора: считана не передусмотренная лексема.");
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
                    _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры составного оператора.");
                    break;
            }
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры составного оператора.");
        }
    }

    private void CompoundOperator(ref Ellement ellement)
    {
        ellement = NoSpaceNextEllement();
        VerifyOperator(ref ellement);

        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 1 && ellement.valueNumber == 18)
        {
            ellement = NoSpaceNextEllement();
            VerifyOperator(ref ellement);
        }
        else if (ellement.tableNumber == 1 && ellement.valueNumber == 20)
            return;
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры составного оператора.");
        }
    }
    
    private void ConditionalOperator(ref Ellement ellement) 
    {
        _operations.Add(Expression());
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 0 && ellement.valueNumber == 6)
        {
            ellement = NoSpaceNextEllement();
            VerifyOperator(ref ellement);
            ellement = NoSpaceNextEllement();
            if (ellement.tableNumber == 0 && ellement.valueNumber == 7) {
                ellement = NoSpaceNextEllement();
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
            Expression();
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
            Expression();
            ellement = NoSpaceAndLineBrokeNextEllement();
            if (ellement.tableNumber == 1 && ellement.valueNumber == 18)
            {
                ellement = NoSpaceAndLineBrokeNextEllement();
                Expression();
                ellement = NoSpaceAndLineBrokeNextEllement();
                if (ellement.tableNumber == 1 && ellement.valueNumber == 18)
                {
                    ellement = NoSpaceAndLineBrokeNextEllement();
                    Expression();
                    ellement = NoSpaceAndLineBrokeNextEllement();
                    if (ellement.tableNumber == 1 && ellement.valueNumber == 14)
                    {
                        ellement = NoSpaceAndLineBrokeNextEllement();
                        VerifyOperator(ref ellement);
                    }
                    else
                        _errorMessage.Add(
                            "Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \")\".");
                }
                else
                    _errorMessage.Add(
                        "Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \";\".");
            }
            else
                _errorMessage.Add(
                    "Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \";\".");
        }
        else
            _errorMessage.Add(
                "Ошибка синтаксического анализатора: нарушение структуры оператора фиксированного цикла ожидается \")\".");
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
            Expression();
            if (!_errorAnalysis)
            {
                ellement = NextEllement();
                while (ellement.tableNumber == 1 && ellement.valueNumber == 23)
                {
                    ellement = NextEllement();
                    Expression();
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
            ellement = NoSpaceNextEllement();
        
        if(ellement.tableNumber == 1 && ellement.valueNumber == 21)
            _operations.Add(Expression());
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора присвоения.");
        }
    }

    private void AssignmentOperator(ref Ellement ellement)
    {
        ellement = NoSpaceNextEllement();
        if (ellement.tableNumber == 3 && DataTokens.ContainsTableIdentification(ellement.valueNumber))
        {
            //putIdentification(next.index, 4);
        }
        else
        {
            _errorAnalysis = true;
            _errorMessage.Add("Ошибка синтаксического анализатора: нарушение структуры оператора присвоения.");
        }
    }

    private String Expression()
    {
        StringBuilder operation = new StringBuilder();
        operation.Append(Operand());
        Ellement ellement = NoSpaceNextEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber is 1 or 2 or 3 or 4 or 5 or 6)
            operation.Append(ellement).Append(Operand());
        return operation.ToString();
    }

    private String Operand()
    {
        StringBuilder operation = new StringBuilder();
        operation.Append(Summand());
        Ellement ellement = NoSpaceNextEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber is 7 or 8 or 9)
            operation.Append(ellement).Append(Summand());
        return operation.ToString();
    }

    private String Summand()
    {
        StringBuilder operation = new StringBuilder();
        operation.Append(Multiplier());
        Ellement ellement = NoSpaceNextEllement();
        while (ellement.tableNumber == 1 && ellement.valueNumber is 10 or 11 or 12)
            operation.Append(ellement).Append(Multiplier());
        return operation.ToString();
    }

    private String Multiplier()
    {
        int stateParentheses = 0;
        StringBuilder multiplierString = new StringBuilder();
        Ellement ellement = NoSpaceNextEllement();
        if (ellement.tableNumber is 2 or 3 || (ellement.tableNumber == 0 && ellement.valueNumber is 1 or 2))
        {
            multiplierString.Append(ellement);
        }
        else if (ellement.tableNumber == 0 && ellement.valueNumber == 14)
        {
            multiplierString.Append(ellement).Append(Multiplier());
        }
        else if (ellement.tableNumber == 1)
        {
            if (ellement.valueNumber == 13)
            {
                stateParentheses = 1;
                multiplierString.Append(ellement).Append(Expression());
                ellement = NoSpaceNextEllement();
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

        public Ellement(string word)
        {
            string[] nums = word.Trim('[', ']').Split(',');
            tableNumber = Convert.ToInt32(nums[0]);
            valueNumber = Convert.ToInt32(nums[1]);
        }
    }
}