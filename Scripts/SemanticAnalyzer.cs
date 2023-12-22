using System;
using System.Collections.Generic;
using System.Linq;

namespace Kursach_AK_47
{
    internal class SemanticAnalyzer
    {
        private bool _endAnalysis = false;
        private bool _errorAnalysis = false;
        private List<string> _errorMessage;
        
       
        
        public List<string> GetErrorMessage()
        {
            return _errorMessage;
        }
    }
}
