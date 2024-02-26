namespace FamilyTreeLibrary.Exceptions
{
    public class PersonNotFoundException : SystemException
    {
        public PersonNotFoundException(string message)
            :base(message){}
    }
}