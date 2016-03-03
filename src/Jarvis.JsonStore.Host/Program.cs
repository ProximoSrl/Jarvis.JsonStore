using System;
using System.IO;
using System.Linq;
using Jarvis.JsonStore.Host.Support;
using Topshelf;

namespace Jarvis.JsonStore.Host
{
    class Program
    { 

        static int Main(string[] args)
        {
            var lastErrorFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_lastError.txt");
            if (File.Exists(lastErrorFileName)) File.Delete(lastErrorFileName);
            try
            {
             
                Int32 executionExitCode;
                if (args.Length == 1 && (args[0] == "install" || args[0] == "uninstall"))
                {
                    executionExitCode = (Int32)StartForInstallOrUninstall();
                }
                else
                {
                    executionExitCode = (Int32)StandardStart();
                }
                return executionExitCode;
            }
            catch (Exception ex)
            {
                File.WriteAllText(lastErrorFileName, ex.ToString());
                throw;
            }

        }

        private static TopshelfExitCode StartForInstallOrUninstall()
        {
            var exitCode = HostFactory.Run(host =>
            {

                host.Service<Object>(service =>
                {

                    service.ConstructUsing(() => new Object());
                    service.WhenStarted(s => Console.WriteLine("Start fake for install"));
                    service.WhenStopped(s => Console.WriteLine("Stop fake for install"));
                });

                host.RunAsNetworkService();

                host.SetDescription("Jarvis - Json Object Store");
                host.SetDisplayName("Jarvis - Json Object Store");
                host.SetServiceName("JarvisJsonObjectStore");
            });
            return exitCode;
        }

        private static TopshelfExitCode StandardStart()
        {
            SetupColors();

            LoadConfiguration();

            var exitCode = HostFactory.Run(host =>
            {
                //var resourceDownload = ConfigurationServiceClient.Instance.DownloadResource("log4net.config", monitorForChange: true);
                //if (!resourceDownload)
                //{
                //    Console.Error.WriteLine("Unable to download log4net.config from configuration store");
                //}

                host.UseOldLog4Net("log4net.config");

                host.Service<BootStrapper>(service =>
                {
                    service.ConstructUsing(() => new BootStrapper());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                host.RunAsNetworkService();

                host.SetDescription("Jarvis - Json Object Store");
                host.SetDisplayName("Jarvis - Json Object Store");
                host.SetServiceName("JarvisJsonObjectStore");
            });

            if (Environment.UserInteractive && exitCode != TopshelfExitCode.Ok)
            {
                Console.Error.WriteLine("Abnormal exit from topshelf: {0}. Press a key to continue", exitCode);
                Console.ReadKey();
            }
            return exitCode;
        }

        static void SetupColors()
        {
            if (!Environment.UserInteractive)
                return;
            Console.Title = "JARVIS :: Json Object Service";
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Clear();
        }

        static void LoadConfiguration()
        {
           
        }

        private static string FindArgument(string[] args, string prefix)
        {
            var arg = args.FirstOrDefault(a => a.StartsWith(prefix));
            if (String.IsNullOrEmpty(arg)) return String.Empty;
            return arg.Substring(prefix.Length);
        }

        private static void Banner()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine("Jarvis Json Object Service - Proximo srl");
            Console.WriteLine("===================================================================");
            Console.WriteLine("  install                        -> install service");
            Console.WriteLine("  uninstall                      -> remove service");
            Console.WriteLine("  net start JarvisJsonObjectStore  -> start service");
            Console.WriteLine("  net stop JarvisJsonObjectStore   -> stop service");
            Console.WriteLine("===================================================================");
            Console.WriteLine();
            Console.WriteLine();
        }


    }
}
