using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pvms_lb1_ClassLibrary_Chervenko
{
    public class Class1 : MarshalByRefObject
    {
        // функція знаходження та взяття в лапки шістнадцяткових чисел 
        public string CalcEquation(string s)
        {
            // регулярний вираз для знаходження шістнадцяткових чисел (0x або # перед цифрами A-F, a-f, 0-9)
            //string pattern = @"(0x[0-9A-Fa-f]+|#[0-9A-Fa-f]+)";
            //string pattern = @"(?<=\W|^)(0x[0-9A-Fa-f]+|#[0-9A-Fa-f]+)(?=\W|$)";
            string pattern = @"(0x[0-9A-Fa-f]+|#[0-9A-Fa-f]{3,})";

            // додаємо лапки у знайдені числа
            string result = Regex.Replace(s, pattern, "\"$1\"");

            return result;
        }
    }
}
