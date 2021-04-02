using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LearningFucker.Console
{
    public class UserSection : ConfigurationSection
    {
        public UserSection()
        {
        }

        [ConfigurationProperty("users", IsDefaultCollection = true)]
        public UserElementCollection Users
        {
            get
            {
                return (UserElementCollection)base["users"];
            }
        }
    }

    public class UserElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        protected override ConfigurationElement CreateNewElement()
        {
            return new UserElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserElement)element).User;
        }

        public void Add(string user, string password)
        {
            var userElement = CreateNewElement() as UserElement;
            userElement.User = user;
            userElement.Password = password;
            base.BaseAdd(userElement);
        }

        public bool Contain(string user)
        {
            var keys = base.BaseGetAllKeys();
            return keys.Contains(user);
        }

        public string[] AllKeys
        {
            get
            {
                var keys = base.BaseGetAllKeys();
                string[] allKeys = new string[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    allKeys[i] = keys[i].ToString();
                }
                return allKeys;
            }
        }

        public UserElement GetUser(string userId)
        {
            return base.BaseGet(userId) as UserElement;
        }
    }

    public class UserElement : ConfigurationElement
    {
        [ConfigurationProperty("user", IsRequired = true)]
        public string User
        {
            get { return (string)base["user"]; }
            set { base["user"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }
    }
}
