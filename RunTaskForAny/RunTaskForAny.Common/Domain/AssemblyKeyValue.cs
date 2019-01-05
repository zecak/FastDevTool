using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    [Serializable]
    public class AssemblyKeyValue
    {
        public string Key { get; set; }
        public Assembly Value { get; set; }
        
    }
}
