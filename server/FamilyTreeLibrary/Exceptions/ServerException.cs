
using System.Collections;
using System.Net;

namespace FamilyTreeLibrary.Exceptions
{
    public class ServerException : Exception
    {
        public ServerException(Exception cause)
            :base() 
        {
            Cause = cause;
        }

        public override IDictionary Data
        {
            get
            {
                return new Dictionary<string,object>()
                {
                    {"Name", $"{Cause.GetType().Name}"},
                    {"Message", Cause.Message},
                    {"Status Code", HttpStatusCode.InternalServerError}
                };
            }
        }

        public override string Message
        {
            get
            {
                return $"{Data["Name"]}: {Data["Message"]}";
            }
        }

        private Exception Cause
        {
            get;
        }

        public override Exception GetBaseException()
        {
            return Cause;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}