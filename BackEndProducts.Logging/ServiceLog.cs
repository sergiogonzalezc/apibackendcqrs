using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using NLog.Config;
using static BackEndProducts.Common.Enum;

namespace BackEndProducts.Common
{
    /// <summary>
    /// Clase que maneja el Log
    /// </summary>
    public class ServiceLog
    {
        /// <summary>
        /// Weite a local file log using Nlog
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="ex"></param>
        /// <param name="category"></param>
        /// <param name="message"></param>
        public static void Write(LogType logType, Exception ex, string category, string message)
        {
            NLog.Logger l = null;

            NLog.LogManager.Configuration.Variables["category"] = category;

            switch (logType)
            {
                case LogType.WebSite:
                    {
                        //l = LogManager.GetLogger("WebSite");
                        l = MyLogManager.Instance.GetLogger("WebSite");                        

                        break;
                    }
                case LogType.ConsoleTask:
                    {
                        l = MyLogManager.Instance.GetLogger("ConsoleTask");

                        break;
                    }
                default:
                    {
                        l = MyLogManager.Instance.GetLogger("WebSite");

                        break;
                    }
            }

            message += " - MSG --> " + ex.Message;

            message = string.Format("[{0}] {1}", category, message);

            if (ex.InnerException != null)
            {
                message += " - INNER --> " + ex.InnerException.ToString();
            }

            if (ex.StackTrace != null)
            {
                message += " - STACK --> " + ex.StackTrace.ToString();
            }

            l.Error(message);
        }

        public static void Write(LogType logType, TraceLevel traceLevel, string category, string message)
        {
            NLog.Logger l = null;

            switch (logType)
            {
                case LogType.WebSite:
                    {
                        l = MyLogManager.Instance.GetLogger("WebSite");

                        break;
                    }
                case LogType.ConsoleTask:
                    {
                        l = MyLogManager.Instance.GetLogger("ConsoleTask");

                        break;
                    }
                default:
                    {
                        l = MyLogManager.Instance.GetLogger("WebSite");

                        break;
                    }
            }

            message = string.Format("[{0}] {1}", category, message);

            switch (traceLevel)
            {
                case TraceLevel.Info:
                    {
                        l.Info(message);

                        break;
                    }
                case TraceLevel.Verbose:
                    {
                        l.Info(message);

                        break;
                    }
                case TraceLevel.Warning:
                    {
                        l.Warn(message);

                        break;
                    }
                case TraceLevel.Error:
                    {
                        l.Error(message);

                        break;
                    }
                default:
                    {
                        l.Info(message);

                        break;
                    }
            }
        }
    }

    internal class MyLogManager
    {
        // A Logger dispenser for the current assembly (Remember to call Flush on application exit)
        public static LogFactory Instance { get { return _instance.Value; } }
        private static Lazy<LogFactory> _instance = new Lazy<LogFactory>(BuildLogFactory);

        // 
        // Use a config file located next to our current assembly dll 
        // eg, if the running assembly is c:\path\to\MyComponent.dll 
        // the config filepath will be c:\path\to\MyComponent.nlog 
        // 
        // WARNING: This will not be appropriate for assemblies in the GAC 
        // 
        private static LogFactory BuildLogFactory()
        {
            // Use name of current assembly to construct NLog config filename 
           
            string configFilePath = Path.Combine(Path.GetFullPath(AssemblyDirectory), "NLog.config");

            LogFactory logFactory = new LogFactory();
            logFactory.Configuration = new XmlLoggingConfiguration(configFilePath, true, logFactory);
            return logFactory;
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
