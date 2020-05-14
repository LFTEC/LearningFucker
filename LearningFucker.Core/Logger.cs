using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace LearningFucker
{
    public static class Logger
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static NLog.Logger GetLogger { get => logger; }
    }
}
