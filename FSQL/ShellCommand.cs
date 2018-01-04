using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FSQL {
    public static  class ShellCommand {

        private static string CmdSpec { get; }
        public static IEnumerable<string> InternalCommands { get; }
        public static IEnumerable<string> RestrictedCommands { get; }

        static ShellCommand() {
            CmdSpec = GetFullPath("cmd.exe");
            InternalCommands = InternalCommandString.ToList();
            RestrictedCommands = RestrictedCommandString.ToList();
        }

        public static string Run(string command) {
            string output = "Sorry... I can't figure out what you want me to do!";
            var parsed = command.Split(' ');
            var cmd = parsed.First().Trim().ToLowerInvariant();
            var args = command.Substring(cmd.Length);
            if (RestrictedCommands.Contains(cmd)) {
                return $"The '{cmd}' command is restricted.";
            }
            if (InternalCommands.Contains(cmd)) {
                output = RunInternalCommand(cmd, args);
            } else {
                output = RunExternalCommand(cmd, args) ?? output;
            }

            return output;
        }

        private static string RunInternalCommand(string cmd, string args) {
            string output = null;
            using (var p = new Process())
            {
                p.StartInfo.FileName = $"{CmdSpec}";
                p.StartInfo.Arguments = $"/c {cmd} {args}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            return output;
        }


        private static string RunExternalCommand(string cmd, string args) {
            
            string output = null;
            if (!cmd.ToLowerInvariant().EndsWith(".exe")) cmd += ".exe";
            cmd = GetFullPath(cmd);
            if (cmd == null) return output;
            using (var p = new Process())
            {
                p.StartInfo.FileName = $"{cmd}";
                p.StartInfo.Arguments = $"{args}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            return output;
        }

        public static string GetFullPath(string filename)
        {
            return new[] { Environment.CurrentDirectory }
                .Concat(Environment.GetEnvironmentVariable("PATH").Split(';'))
                .Select(dir => Path.Combine(dir, filename))
                .FirstOrDefault(path => File.Exists(path));
        }

        private static IEnumerable<string> ToList(this string data) =>
            data.ToLowerInvariant()
                .Trim()
                .Split(Environment.NewLine.ToCharArray())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())                
                .OrderBy(s => s);        

        #region Command Lists
        private const string InternalCommandString = @"
            assoc	
            at
            attrib
            bootcfg
            cd
            chdir
            chkdsk
            cls
            copy
            del
            dir
            echo
            exit
            fc
            find
            findstr
            for
            getmac
            if
            ipconfig
            md
            mkdir
            more
            move
            net
            netsh
            netstat
            path
            pathping
            ping
            popd
            pushd	
            rd
            rmdir
            ren
            rename
            sc
            schtasks
            set
            sort
            start
            subst
            systeminfo
            taskkill
            tasklist
            tree
            type
            xcopy";

        private const string RestrictedCommandString = @"
            diskpart
            driverquery
            fsutil
            ftype
            goto
            pause
            powercfg
            reg
            sfc
            shutdown
            vssadmin";
        #endregion
    }
}