#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

// credit: http://msdn.microsoft.com/de-de/library/system.diagnostics.tracing.eventsource.aspx

namespace EtwProfiling
{

    [EventSource(Name = "Bnaya ETW Sample", 
        Guid = "F952D9A1-123B-4FE4-A9CB-B355A604178C"
        /* , LocalizationResources = "BnayaEtwTracing.SRResources" */)]
    public class CustomEtw: EventSource
    {
        public class Keywords
        {
            public const EventKeywords Diagnostic = (EventKeywords)1;
            public const EventKeywords Perf = (EventKeywords)2;
            public const EventKeywords Other = (EventKeywords)4;
            public const EventKeywords Page = (EventKeywords)4;
        }

        public class Tasks
        {
            public const EventTask IO = (EventTask)1;
            public const EventTask Debug = (EventTask)2;
            public const EventTask Page = (EventTask)2;
        }

        public static CustomEtw Log = new CustomEtw();

        [Event(1, Message = "Starting up.", Keywords = Keywords.Diagnostic, Level = EventLevel.Informational)]
        public void Startup() { if (IsEnabled()) WriteEvent(1); }


        [Event(3, Task = Tasks.IO, Opcode = EventOpcode.Start, Message = "Start IO: {0}, [{1}]", Keywords = Keywords.Diagnostic, Level = EventLevel.Informational)]
        public void IOStart(int ID, string FileName) { if (IsEnabled()) WriteEvent(3, ID, FileName); }

        [Event(4, Task = Tasks.IO, Opcode = EventOpcode.Stop, Keywords = Keywords.Diagnostic, Level = EventLevel.Informational)]
        public void IOStop(int ID) { if (IsEnabled()) WriteEvent(4, ID); }

        [Event(5, Message = "Failure: {0}", Level = EventLevel.Error, Keywords = Keywords.Diagnostic)]
        public void Failure(string message) { if (IsEnabled())WriteEvent(5, message); }


        [Event(7, Message = "loading page {1} activityID={0}", Opcode = EventOpcode.Start,
      Task = Tasks.Page, Keywords = Keywords.Page, Level = EventLevel.Informational)]
        public void PageStart(int ID, string url) { if (IsEnabled()) WriteEvent(7, ID, url); }

        [Event(8, Opcode = EventOpcode.Stop, Task = Tasks.Page, Keywords = Keywords.Page, Level = EventLevel.Informational)]
        public void PageStop(int ID) { if (IsEnabled()) WriteEvent(8, ID); }

    }
}
