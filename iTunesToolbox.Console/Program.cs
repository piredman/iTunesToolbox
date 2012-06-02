using System.Linq;
using System.Reflection;
using iTunesToolbox.Logic;
using System;
using iTunesToolbox.Logic.Entity;
using iTunesToolbox.Logic.Common;

namespace iTunesToolbox
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayHeader();

            try
            {
                var options = new ProcessingOptions();
                var proceed = ProcessArguments(args, options);

                if (!proceed) {
                    DisplayHelp();
                    return;
                }

                proceed = !(options.UpdateWindowsFiles && options.UpdateiTunesFiles);
                if (!proceed) {
                    DisplayUpdatingRestrictions();
                }

                if (options.RemoveRatingOneTracks)
                    proceed = ConfirmDelete();

                if (proceed) {
                    var processManager = new ProcessManager(options);
                    processManager.Execute();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine(exception.Message);
            }
        }

        #region Private Methods

        private static bool ProcessArguments(string[] argumentList, ProcessingOptions options)
        {
            if (argumentList.Count() <= 0)
                return false;

            var result = true;
            foreach (var argument in argumentList)
            {
                if (!result)
                    break;

                switch (argument.ToUpper())
                {
                    case "/D":
                        options.DisplayInformation = true;
                        break;

                    case "/W":
                        options.UpdateWindowsFiles = true;
                        break;
                    case "/I":
                        options.UpdateiTunesFiles = true;
                        break;
                    case "/REN":
                        options.RenameFiles = true;
                        break;
                    case "/DEL":
                        options.RemoveRatingOneTracks = true;
                        break;

                    case "/L":
                        options.LogAllMessages = true;
                        break;
                    case "/LI":
                        options.LogInformationMessages = true;
                        break;
                    case "/LW":
                        options.LogWarningMessages = true;
                        break;
                    case "/LE":
                        options.LogErrorMessages = true;
                        break;

                    default:
                        System.Console.WriteLine(string.Format(Literals.UnknownArgument, argument));
                        result = false;
                        break;
                }
            }

            return result;
        }

        private static void DisplayHeader()
        {
            var version = "[unknown]";
            var assembly = Assembly.GetExecutingAssembly();

            AssemblyName assemblyName = null;
            if (null != assembly)
                assemblyName = assembly.GetName();
            if (null != assemblyName)
                version = string.Format("[v{0}]", assemblyName.Version);

            var separator = new System.Text.StringBuilder();
            separator.Append('-', 80);

            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(separator);
            System.Console.WriteLine(string.Format("iTunesToolbox {0}", version));
            System.Console.WriteLine(separator);
            System.Console.WriteLine(string.Empty);
        }

        private static void DisplayHelp()
        {
            var headerFormat = "  {0}";
            var argumentFormat = "  {0}\t|\t{1}";
            var wrapTextFormat = "\t\t{0}";

            System.Console.WriteLine(string.Format(argumentFormat, "Usage", "Console [/options]"));
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(string.Empty);

            System.Console.WriteLine(string.Format(headerFormat, "[Informational Options]"));
            System.Console.WriteLine(string.Format(argumentFormat, "/D", "Display track information"));
            System.Console.WriteLine(string.Empty);

            System.Console.WriteLine(string.Format(headerFormat, "[Updating Options]"));
            System.Console.WriteLine(string.Format(argumentFormat, "/W", "Update ratings in Windows files (iTunes => File)"));
            System.Console.WriteLine(string.Format(argumentFormat, "/I", "Update ratings in iTunes Tracks (File => iTunes)"));
            System.Console.WriteLine(string.Format(argumentFormat, "/REN", "Rename Windows files. Format: (Artist [Album] - Title)"));
            System.Console.WriteLine(string.Format(argumentFormat, "/DEL", "Delete & Remove tracks with a rating of one (1)"));
            System.Console.WriteLine(string.Empty);

            System.Console.WriteLine(string.Format(headerFormat, "[Logging Options]"));
            System.Console.WriteLine(string.Format(argumentFormat, "/L", "Log all messages to the console (equivalent to /LI /LW /LE)"));
            System.Console.WriteLine(string.Format(argumentFormat, "/LI", "Log informational messages to the console"));
            System.Console.WriteLine(string.Format(argumentFormat, "/LW", "Log warning messages to the console"));
            System.Console.WriteLine(string.Format(argumentFormat, "/LE", "Log error messages to the console"));
            System.Console.WriteLine(string.Empty);
        }

        private static void DisplayUpdatingRestrictions()
        {
            Console.WriteLine("You may only update files or tracks in one direction at a time.  You cannot");
            Console.WriteLine("use both the /W and /I options together.");
            Console.WriteLine(string.Empty);

            DisplayHelp();
        }

        private static bool ConfirmDelete()
        {
            Console.WriteLine("Tracks with a rating of one (1) will be REMOVED from iTunes and DELETED from");
            Console.WriteLine("the file system.  This action CANNOT be undone.");

            Console.WriteLine(string.Empty);
            Console.Write("Are you sure you want to continue? [Y/N]: ");
            var answer = Console.ReadLine();

            var result = (answer.ToUpper().Equals("Y"));
            Console.WriteLine(string.Empty);

            return result;
        }

        #endregion Private Methods
    }
}
