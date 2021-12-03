using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.ServiceResources
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public string RoleName { get; set; }
    }
}
