using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_Web_API.Api.Requests.Roles
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateUserRoleRequest : BaseModel<int>
    {
        public string Name { get; set; }
    }
}