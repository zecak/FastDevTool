using Newtonsoft.Json;
using PWMIS.DataMap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase.Model
{
    public class sys_enum : EntityBase
    {
        [JsonIgnore]
        public override string[] PropertyNames { get => base.PropertyNames; protected set => base.PropertyNames = value; }
        [JsonIgnore]
        public override object[] PropertyValues { get => base.PropertyValues; protected set => base.PropertyValues = value; }

        [JsonIgnore]
        new private List<string> PrimaryKeys { get { return base.PrimaryKeys; } }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public sys_enum()
        {
            TableName = nameof(sys_enum);
            IdentityName = nameof(ID);
            PrimaryKeys.Add(nameof(ID));
        }

        /// <summary>
        /// 编号
        /// </summary>
        public int ID
        {
            get { return getProperty<int>(nameof(ID)); }
            set { setProperty(nameof(ID), value); }
        }

        /// <summary>
        /// 全局编号
        /// </summary>
        public Guid GID
        {
            get { return getProperty<Guid>(nameof(GID)); }
            set { setProperty(nameof(GID), value); }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return getProperty<string>(nameof(Name)); }
            set { setProperty(nameof(Name), value, 50); }
        }

        /// <summary>
        /// 枚举数据
        /// </summary>
        public string JsonData
        {
            get { return getProperty<string>(nameof(JsonData)); }
            set { setProperty(nameof(JsonData), value); }
        }

        /// <summary>
        /// 枚举数据类型:0普通json数据,1表数据
        /// </summary>
        public int DataType
        {
            get { return getProperty<int>(nameof(DataType)); }
            set { setProperty(nameof(DataType), value); }
        }

        /// <summary>
        /// 系统标志:0普通,1系统
        /// </summary>
        public int SystemMark
        {
            get { return getProperty<int>(nameof(SystemMark)); }
            set { setProperty(nameof(SystemMark), value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return getProperty<DateTime>(nameof(CreateTime)); }
            set { setProperty(nameof(CreateTime), value); }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return getProperty<DateTime>(nameof(UpdateTime)); }
            set { setProperty(nameof(UpdateTime), value); }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNo
        {
            get { return getProperty<int>(nameof(SortNo)); }
            set { setProperty(nameof(SortNo), value); }
        }

        /// <summary>
        /// 状态:0禁用,1正常
        /// </summary>
        public int Status
        {
            get { return getProperty<int>(nameof(Status)); }
            set { setProperty(nameof(Status), value); }
        }
    }
}
