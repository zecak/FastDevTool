using Newtonsoft.Json;
using PWMIS.DataMap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase.Model
{
    [PropertyChanged.DoNotNotify]
    public class sys_table : EntityBase
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



        public sys_table()
        {
            TableName = nameof(sys_table);
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
        /// 标题
        /// </summary>
        public string Title
        {
            get { return getProperty<string>(nameof(Title)); }
            set { setProperty(nameof(Title), value, 50); }
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description
        {
            get { return getProperty<string>(nameof(Description)); }
            set { setProperty(nameof(Description), value, 500); }
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
        /// 名称前缀
        /// </summary>
        public string Prefix
        {
            get { return getProperty<string>(nameof(Prefix)); }
            set { setProperty(nameof(Prefix), value, 50); }
        }

        /// <summary>
        /// 名称后缀
        /// </summary>
        public string RestName
        {
            get { return getProperty<string>(nameof(RestName)); }
            set { setProperty(nameof(RestName), value, 50); }
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
