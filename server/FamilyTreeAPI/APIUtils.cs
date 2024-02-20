using FamilyTreeAPI.Models;

namespace FamilyTreeAPI
{
    public static class APIUtils
    {
        public static ExceptionResponse SerializeErrorResponse(Exception ex)
        {
            return new ExceptionResponse
            {
                Name = ex.GetType().Name,
                Message = ex.Message
            };
        }
    }
}