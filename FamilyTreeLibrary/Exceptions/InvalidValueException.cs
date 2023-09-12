using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Exceptions
{
    public class InvalidValueException : ArgumentOutOfRangeException
    {
        public InvalidValueException(string value)
        :base()
        {
            ActualValue = value;
        }

        public override object ActualValue
        {
            get;
        }

        public override string Message
        {
            get
            {
                return $"{ActualValue} is in an invalid Format";
            }
        }

        public override string ParamName
        {
            get
            {
                return "Value";
            }
        }

    }
}