namespace FamilyTreeLibrary.Exceptions
{
    public class FamilyNotFoundException : SystemException
    {
        public FamilyNotFoundException(string message)
            :base(message){}
    }
}