using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    [JsonObject]
    public class User
    {
        [JsonProperty(PropertyName = "TokenID")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "IP")]
        public string IP { get; set; }
        [JsonProperty(PropertyName = "IsSelectDevice")]
        public bool IsSelectDevice { get; set; }
        [JsonProperty(PropertyName = "Username")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "RealName")]
        public string RealName { get; set; }
        [JsonProperty(PropertyName = "CompanyId")]
        public string CompanyId { get; set; }
        [JsonProperty(PropertyName = "CompanyName")]
        public string CompanyName { get; set; }
        [JsonProperty(PropertyName = "AppMode")]
        public int? AppMode { get; set; }

        public string UserId { get; set; }
        public string Password { get; set; }

        public void Clone(User user)
        {
            this.AppMode = user.AppMode;
            this.CompanyId = user.CompanyId;
            this.CompanyName = user.CompanyName;
            this.IP = user.IP;
            this.IsSelectDevice = user.IsSelectDevice;
            this.RealName = user.RealName;
            this.Token = user.Token;
            this.UserId = user.UserId;
            this.UserName = user.UserName;
        }
    }
}
