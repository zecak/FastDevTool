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
    public class sys_table_column : EntityBase
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

        public sys_table_column()
        {
            TableName = nameof(sys_table_column);
            IdentityName = nameof(ID);
            PrimaryKeys.Add(nameof(ID));
            //SetForeignKey<sys_field_type>(nameof(FieldTypeID));
            //SetForeignKey<sys_table>(nameof(TableID));
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
        /// 字段类型编号
        /// </summary>
        public int FieldTypeID
        {
            get { return getProperty<int>(nameof(FieldTypeID)); }
            set { setProperty(nameof(FieldTypeID), value); }
        }

        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength
        {
            get { return getProperty<int>(nameof(MaxLength)); }
            set { setProperty(nameof(MaxLength), value); }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        {
            get { return getProperty<string>(nameof(DefaultValue)); }
            set { setProperty(nameof(DefaultValue), value, 50); }
        }

        /// <summary>
        /// 枚举类型编号
        /// </summary>
        public int EnumID
        {
            get { return getProperty<int>(nameof(EnumID)); }
            set { setProperty(nameof(EnumID), value); }
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
        /// 表编号
        /// </summary>
        public int TableID
        {
            get { return getProperty<int>(nameof(TableID)); }
            set { setProperty(nameof(TableID), value); }
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
