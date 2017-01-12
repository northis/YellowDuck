using System;
using System.Collections.Generic;

namespace YellowDuck.Common.Logging
{
    public interface ILogService
    {
        void Write(string message, Exception ex, Dictionary<string,object> parameters);
    }
}
