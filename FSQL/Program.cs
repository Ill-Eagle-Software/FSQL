using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSQL
{
    class Program
    {
        static void Main(string[] args) {
            string script;

            if (!args.Any()) {

                var repl = new FSQLSession();
                repl.Execute();

            } else {

                script = GetScript(args);
                IEnumerable<string> parms = new string[] {};
                if (args.Any()) {
                    parms = args.Skip(1).Take(args.Length - 1);
                }
                if (!string.IsNullOrWhiteSpace(script)) {
                    Execute(script, parms.Cast<object>().ToArray());
                } else {
                    Console.WriteLine("Could not locate FSQL script.");
                }
            }
        }

        private static string GetScript(string[] args) {

            var script = "{}";
            var scriptFile = args.First();

            if (args.Contains("/r")) {
                script = ReadPipedScript();
            } else if (File.Exists(scriptFile)) {
                script = File.ReadAllText(scriptFile);
            } else {
                scriptFile = scriptFile + ".fsql";
                if (File.Exists(scriptFile))
                    script = File.ReadAllText(scriptFile);
            }
            script = script.Trim();
            if (!(script.StartsWith("{") && script.EndsWith("}"))) {
                //Trim batch code
                var scriptStart = script.IndexOf("{", StringComparison.Ordinal);
                script = script.Substring(scriptStart);

                var scriptend = script.LastIndexOf("}");
                script = script.Substring(0, scriptend + 1);
            }
            return script;
        }

        private static string ReadPipedScript() {
            var script = string.Empty;
            string line;
            while ((line = Console.ReadLine()) != null) {
                script += line;
            }
            return script;
        }

        private static object Execute(string script, object[] args) {            
            var eng = FSQLEngine.Create();
            return eng.Execute(script, args);
        }
    }
}
