using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class PropertyList
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public List<Property> List { get; set; }
    }

    public class Property
    {
        public string PropertyID { get; set; }
        public string PropertyName { get; set; }
        public int OrderIndex { get; set; }
        

        [JsonProperty("propertySubNodes")]
        public List<Context> SubNodes { get; set; }
    }

    public class Context
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public string ParentContextId { get; set; }

        /// <summary>
        /// 资产id
        /// </summary>
        public string PropertyId { get; set; }
        public int OrderIndex { get; set; }

        [JsonProperty("children")]
        public List<Context> Children { get; set; }
    }
}
