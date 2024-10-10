using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Contants
{
    public static class LoggerConstants
    {
        public const string LogInfo = "Message: {Message}. Application: {Application}. Class: {Class}, Method: {Method}";
        public const string LogDebug = "Message: {Message}. Application: {Application}. Class: {Class}, Method: {Method}";
        public const string LogError = "Message: {Message}. Application: {Application}. Class: {Class}, Method: {Method}. Stacktrace: {Stacktrace}";
        public const string LogWarning = "Message: {Message}. Application: {Application}. Class: {Class}, Method: {Method}";
    }
}
