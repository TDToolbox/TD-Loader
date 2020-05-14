using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    public delegate void ProcFinished_BoolEventHandler(object source, ProcFinished_BoolEventArgs e);
    public delegate void ProcFinished_StringEventHandler(object source, ProcFinished_StringEventArgs e);
    class ProcessFinishedEvent
    {
        public event ProcFinished_BoolEventHandler GotResult;
        public event ProcFinished_StringEventHandler GotString;
    }

    public class ProcFinished_StringEventArgs : EventArgs
    {
        private string EventInfo;
        public ProcFinished_StringEventArgs(string Text)
        {
            EventInfo = Text;
        }
        public string GetInfo()
        {
            return EventInfo;
        }
    }

    public class ProcFinished_BoolEventArgs : EventArgs
    {
        private bool EventInfo;
        public ProcFinished_BoolEventArgs(bool result)
        {
            EventInfo = result;
        }
        public bool GetInfo()
        {
            return EventInfo;
        }
    }
}
