using Newtonsoft.Json;
using PWMIS.DataMap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase.Model
{
    public class Tb_User : EntityBase
    {
        public Tb_User()
        {
            TableName = nameof(Tb_User);
            IdentityName = nameof(UserID);
            PrimaryKeys.Add(nameof(UserID));
        }

        [JsonIgnore]
        public override string[] PropertyNames { get => base.PropertyNames; protected set => base.PropertyNames = value; }
        [JsonIgnore]
        public override object[] PropertyValues { get => base.PropertyValues; protected set => base.PropertyValues = value; }

        [JsonIgnore]
        new private List<string> PrimaryKeys { get { return base.PrimaryKeys; } }

        public int UserID
        {
            get { return getProperty<int>(nameof(UserID)); }
            set { setProperty(nameof(UserID), value); }
        }

        public string Name
        {
            get { return getProperty<string>(nameof(Name)); }
            set { setProperty(nameof(Name), value, 50); }
        }

        public string Pwd
        {
            get { return getProperty<string>(nameof(Pwd)); }
            set { setProperty(nameof(Pwd), value, 50); }
        }

        public string JsonData
        {
            get { return getProperty<string>(nameof(JsonData)); }
            set { setProperty(nameof(JsonData), value); }
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
