using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class AppInfo
    {
        public int customerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAppName { get; set; }
        public string CustomerAppVersion { get; set; }

    }
}
