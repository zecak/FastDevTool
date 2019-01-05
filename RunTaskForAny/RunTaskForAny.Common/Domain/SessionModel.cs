using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    public class SessionModel
    {
        public Guid SessionID { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime StartTime { get; set; }
    }
}
