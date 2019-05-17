using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TimeShift
{
    class Program
    {
        static void Main(string[] args)
        {
            var match = Regex.Match(args[1], @"([+-]{0,1})(\d{1,2}):(\d{1,2}):(\d{1,2})", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                Usage();
                return;
            }

            int signum = (match.Groups[1].Value == "-") ? -1 : 1;
            int shiftHours = int.Parse(match.Groups[2].Value);
            int shiftMinutes = int.Parse(match.Groups[3].Value);
            int shiftSeconds = int.Parse(match.Groups[4].Value);
            var files = Directory.GetFiles(args[0], "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {

                match = Regex.Match(file, @".*\\((.*?)(UTC\s){0,1}(\d{4}).(\d{2}).(\d{2}).(\d{2}).(\d{2}).(\d{2})(.*))", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var dt = new DateTime(
                        int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value),
                        int.Parse(match.Groups[7].Value), int.Parse(match.Groups[8].Value), int.Parse(match.Groups[9].Value));
                    dt = dt.AddHours(signum * shiftHours);
                    dt = dt.AddMinutes(signum * shiftMinutes);
                    dt = dt.AddSeconds(signum * shiftSeconds);
                    Console.WriteLine("ren \"{0}\" \"{1}\"", file, match.Groups[2].Value + dt.ToString("yyyy-MM-dd_HH.mm.ss") + match.Groups[10].Value);
                }
            }

            Console.WriteLine(@"");
            Console.WriteLine(@"");
            //Console.WriteLine(@"Press Enter.");
            //Console.ReadLine();
        }

        static void Usage()
        {
            var cmdLineArgs = Environment.GetCommandLineArgs();
            var exe = Path.GetFileName(cmdLineArgs[0]);
            Console.WriteLine(@"Usage: {0} <sourceDirectory> <offset>", exe);
            Console.WriteLine(@"");
            Console.WriteLine(@"<offset> => time offset in [-]HH:mm:ss format");
            Console.WriteLine(@"");
            Console.WriteLine(@"");

            //Console.ReadLine();
        }
    }
}
