using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    public class SessionContext
    {
        public static int SessionTimeLagSeconds = 180;

        static List<SessionModel> listSession = new List<SessionModel>();

        static SessionContext sessions;
        public static SessionContext Sessions
        {
            get
            {
                if (sessions == null) { sessions = new SessionContext(); }
                return sessions;
            }
        }

        public object this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                Add(key, value);
            }
        }
        
        public bool Add(string key, object value)
        {
            var model = listSession.FirstOrDefault(m => m.Key == key);
            if (model == null)
            {
                SessionModel session = new SessionModel();
                session.SessionID = Guid.NewGuid();
                session.Key = key;
                session.Value = value;
                session.StartTime = DateTime.Now;
                listSession.Add(session);
            }
            else
            {
                model.Value = value;
                model.StartTime = DateTime.Now;
            }
            return true;
        }

        public object GetValue(string key)
        {
            var model = listSession.FirstOrDefault(m => m.Key == key);
            if (model == null)
            {
                return null;
            }
            var rez = Security.API.IsInServerTime(model.StartTime, DateTime.Now, SessionTimeLagSeconds);
            if (!rez) { listSession.Remove(model); return null; }
            return model.Value;

        }

        public List<SessionModel> GetList()
        {
            return listSession;
        }

    }
}
