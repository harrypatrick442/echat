using Core.DTOs;

namespace Authentication.DAL
{
    public interface IAuthenticationRelatedUserInfoSource
    {
        public string GetInfoForEmailing(long userId);
    }
}