using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kursach_AK_47
{
    internal class SemanticAnalyzer
    {
        private bool _errorAnalysis = false;
        private List<string> _errorMessage = new();
        
        public SemanticAnalyzer()
        {
            StartAnalyze();
        }

        private void StartAnalyze()
        {
            if (VerifyExpressions() && VerifyAssigment())
                _errorMessage.Add("Конец семантического анализа: Ошибок не обнаружено.");
        }

        private bool VerifyExpressions() // проверка выражения [0,1][2,15][2,0][3,4] 
        {
            foreach (var expression in DataTokens.tableExpression)
            {
                if (!DefineExpressionsType(expression)) return false;
            }
            
            return true;
        }

        private bool VerifyAssigment() 
        {
            foreach (var assigment in DataTokens.tableAssigment)
            {
                if (!DefineAssigmentType(assigment)) return false;
            }
            return true;
        }

        private bool DefineAssigmentType(String assigment) // проверка присвоения 
        {
            List<String> ellements = new (assigment.Replace("][", "];[").Split(';'));
            Iterator iterator = new Iterator(ellements);
            EllementSemantic next = new(iterator.Next());
            int firstEllement;
            
            if (next.tableNumber == 0 && next.indexNumber == 4)
            {
                next = new EllementSemantic(iterator.Next());
                if (next.tableNumber != 2)
                {
                    _errorMessage.Add($"Ошибка семантического анализатора. Ожидается переменная");
                    return false;
                }
                int index = next.indexNumber;
                firstEllement = DataTokens.tableIdTypes[next.indexNumber];
                if (isEqualsSemicolon(new EllementSemantic(iterator.Next())))
                {
                    _errorMessage.Add(
                        $"Ошибка семантического анализатора. Ожидается точка с запятой вместо '{next.word}'");
                    return false;
                }
                int secondEllement = Expression(iterator);
                if (!VerifyTypes(firstEllement, secondEllement))
                {
                    _errorMessage.Add($"Ошибка семантического анализатора. Неправильное преобразование типа");
                    return false;
                }
                if (DataTokens.tableIdTypes.ContainsKey(index))
                    DataTokens.tableIdTypes[index] = secondEllement;
                else
                    DataTokens.tableIdTypes.Add(index, secondEllement);
                return true;
            }
            else
            {
                if (next.tableNumber != 2)
                {
                    _errorMessage.Add($"Ошибка семантического анализатора. Ожидается переменная");
                    return false;
                }
                firstEllement = DataTokens.tableIdTypes[next.indexNumber];
                if (isEqualsSemicolon(new EllementSemantic(iterator.Next())))
                {
                    _errorMessage.Add(
                        $"Ошибка семантического анализатора. Ожидается точка с запятой вместо '{next.word}'");
                    return false;
                }
                int secondEllement = Expression(iterator);
                if (!VerifyTypes(firstEllement, secondEllement))
                {
                    _errorMessage.Add($"Ошибка семантического анализатора. Неправильное преобразование типа");
                    return false;
                }

                return true;
            }
        }

        private int Expression(Iterator iterator) // проверка выражения
        {
            int type;
            type = Multiplier(iterator);
            while (iterator.HasNext())
            {
                EllementSemantic next = new(iterator.Next());
                if (IsBothGO(next))
                {
                    if (!IsValidType(type))
                    {
                        _errorMessage.Add($"Ошибка семантического анализатора. Неверный тип '{type}'");
                        return -1;
                    }
                    else if (isNumGO(next))
                    {
                        if (!IsValidType(type) && type != 3)
                            _errorMessage.Add($"Ошибка семантического анализатора. " +
                                              $"Ожидаемое числовое значение (1, 2 или 4) перед числовой операцией вместо ({type})");
                        return -1;
                    }
                    else if (isBoolGO(next))
                    {
                        if (!IsValidType(type) && type == 3)
                            _errorMessage.Add($"Ошибка семантического анализатора. " +
                                              $"ожидаемое логическое значение (3) перед логической операцией вместо ({type})");
                        return -1;
                    }
                    else
                    {
                        int nextType = Multiplier(iterator);
                        if (VerifyTypes(type, nextType))
                            type = nextType;
                    }
                }
            }
            return type;
        }

        private int Multiplier(Iterator iterator) // проверка типа
        {
            int type = -1;
            if (iterator.HasNext())
            {
                EllementSemantic next = new(iterator.Next());
                if (next.tableNumber == 2) return IdType(next);
                if (next.tableNumber == 3) return NumberType(next);
                if (next.tableNumber == 0) return 3;
                if (next.indexNumber == 14)
                {
                    type = 3;
                    int rightType = Multiplier(iterator);
                    if (rightType != 3)
                    {
                        _errorMessage.Add($"Ошибка семантического анализатора. " +
                                          $"Ожидаемое логическое значение (3) после \"!\" вместо ({type})");
                        return -1;
                    }
                    return type;
                }
                if (next.indexNumber == 13)
                {
                    type = Expression(iterator);
                    if (new EllementSemantic(iterator.Previous()).indexNumber != 14)
                    {
                        _errorMessage.Add($"Ошибка семантического анализатора. " +
                                          $"ожидаемый символ \")\" вместо ({next.word})");
                        return -1;
                    }
                    return type;
                }
            }
            return type;
        }

        private bool IsBothGO(EllementSemantic ellement) // проверка на операцию с чисслом или бул
        {
            return ellement.tableNumber == 1 && ellement.indexNumber is 1 or 2;
        }

        private bool isNumGO(EllementSemantic ellement) // проверка на операцию с чисслом
        {
            return ellement.tableNumber == 1 && ellement.indexNumber is 3 or 4 or 5 or 6 or 7 or 8 or 10 or 11;
        }

        private bool isBoolGO(EllementSemantic ellement) // проверка на операцию с бул
        {
            return ellement.tableNumber == 1 && ellement.indexNumber is 9 or 12;
        }
        
        private bool IsValidType(int type) 
        {
            return type is > 0 and < 5;
        }
        
        private bool VerifyTypes(int firstEllement, int secondEllement) 
        {
            if (firstEllement == 3) return secondEllement == 3;
            if (firstEllement == 2) return secondEllement is 2 or 1;
            if (firstEllement == 1) return secondEllement == 1;
            return secondEllement is 3 or 2 or 1;
        }
        
        private bool PutExpression(String exp, int type) 
        {
            DataTokens.tableExpressionTypes.TryGetValue(exp, out int old);
            if (old != 0) 
                return old == type;
            if (DataTokens.tableExpressionTypes.ContainsKey(exp))
                DataTokens.tableExpressionTypes[exp] = type;
            else
                DataTokens.tableExpressionTypes.Add(exp, type);

            return true;
        }
        
        private bool DefineExpressionsType(String expression) 
        {
            List<String> ellements = new (expression.Replace("][", "];[").Split(';'));
            Iterator iterator = new Iterator(ellements);
            int type = Expression(iterator);
            if (!PutExpression(expression, type))
            {
                _errorMessage.Add($"Ошибка семантического анализатора. Типы данных выражений различны ({type})");
                return false;
            }
            return true;
        }
        
        private bool isEqualsSemicolon(EllementSemantic next) 
        {
            return next.tableNumber == 1 && next.indexNumber == 18;
        }
        
        private int IdType(EllementSemantic next) {
            return DataTokens.tableIdTypes[next.indexNumber];
        }

        private int NumberType(EllementSemantic next) {
            return DataTokens.tableNumberTypes[next.indexNumber];
        }
        
        public List<string> GetErrorMessage()
        {
            return _errorMessage;
        }
    }

    class Iterator
    {
        private int currentIndexEllement = -1;
        private List<string> ellements;

        public Iterator(List<string> listEllements)
        {
            ellements = listEllements;
        }
        
        public bool HasNext()
        {
            return currentIndexEllement < ellements.Count - 1;
        }
        
        public string Current()
        {
            return ellements[currentIndexEllement];
        }
        
        public string Next()
        {
            return ellements[++currentIndexEllement];
        }
        
        public string Previous()
        {
            return ellements[--currentIndexEllement];
        }
    }

    public class EllementSemantic
    {
        public int tableNumber;
        public int indexNumber;
        
        public String word;

        public EllementSemantic(String ellement)
        {
            int indexOfComma = ellement.IndexOf(',');
            tableNumber = int.Parse(ellement.Substring(1, 1));
            indexNumber = int.Parse(ellement.Substring(indexOfComma + 1, 1));
            word = Word();
        }
        
        private String Word() 
        {
            switch (tableNumber)
            {
                case 0:
                    return DataTokens.GetValueTableServiceWords(indexNumber);
                case 1:
                    return DataTokens.GetValueTableLimiters(indexNumber);
                case 2:
                    return DataTokens.GetValueTableIdentificator(indexNumber);
                case 3:
                    return DataTokens.GetValueTableNumber(indexNumber);
                default:
                    return "Неверный номер таблицы";
            }
        }
    }
}
