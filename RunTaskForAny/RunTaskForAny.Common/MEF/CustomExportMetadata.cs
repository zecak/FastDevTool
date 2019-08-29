using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.MEF
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomExportMetadata : ExportAttribute, IMetadata
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }

        public CustomExportMetadata() : base(typeof(IMetadata))
        {
        }

        public CustomExportMetadata(string name)
        {
            this.Name = name;
        }

        public CustomExportMetadata(string name, string description) : this(name)
        {
            this.Description = description;
        }

        public CustomExportMetadata(string name, string description, string author) : this(name, description)
        {
            this.Author = author;
        }

        public CustomExportMetadata(string name, string description, string author, string version) : this(name, description, author)
        {
            this.Version = version;
        }
    }
}
