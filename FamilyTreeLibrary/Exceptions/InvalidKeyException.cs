using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Exceptions
{
    public class InvalidKeyException : ArgumentOutOfRangeException
    {
        public InvalidKeyException(int key)
        :base()
        {
            ActualValue = key;
        }

        public override object ActualValue
        {
            get;
        }

        public override string Message 
        {
            get
            {
                return $"{ActualValue} doesn't have a conversion.";
            }
        }

        public override string ParamName
        {
            get
            {
                return "Key";
            }
        }
    }
}