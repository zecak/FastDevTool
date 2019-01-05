using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    public class ModuleBase: MarshalByRefObject
    {
        public InfoModel InfoModel { get; private set; }
        public ModuleBase(InfoModel info)
        {
            InfoModel = info;
        }

        
    }
}
