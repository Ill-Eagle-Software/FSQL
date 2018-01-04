using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FSQL.ExecCtx;
using FSQL.Interfaces;
using FSQL.ProgramParts;
using FSQL.ProgramParts.Functions;

namespace FSQL {
    public class FSQLSession {

        protected const string LineContinuationCharacter = "_";
        protected const string DefaultPrompt = "\nFSQL:> ";
        protected const string ContinuationPrompt = "\n++++:> ";

        public TextReader In { get; protected set; }
        public TextWriter Out { get; protected set; }

        protected IScriptEngine Engine { get; set; }
        protected IExecutionContext Ctx { get; set; }

        public FSQLSession() : this (Console.In, Console.Out){ 
        }

        public  FSQLSession(TextReader input, TextWriter output) {
            In = input;
            Out = output;
        }

        public void Execute() { 

            var cmd = string.Empty;
            Out.WriteLine(Logo());
            Initialize();
            while (cmd != "exit") {                
                if (!string.IsNullOrWhiteSpace(cmd))
                    Out.WriteLine(Execute(cmd));
                Out.Write(Prompt);
                cmd = In.ReadLine();
            }
        }

        protected void Initialize() {
            Engine = new FSQLEngine();
            Ctx = ExecutionContext.Create();
            LoadUserProfileScript();
            Environment.CurrentDirectory = ProfileFolder;
        }

        public string ProfileFolder => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string ProfileScript => Path.Combine(ProfileFolder, ".fsql");

        protected void LoadUserProfileScript() {
            var script = ProfileScript;
            if (File.Exists(script)) {
                Out.WriteLine($"Loading Profile Script: {script}");
                Load(script);
            } else {
                Out.WriteLine($"Creating Empty Profile Script: {script}");
                File.WriteAllText(script, "{}");
            }
        }

        protected string CommandBuffer = string.Empty;
        protected Stack<string> PromptStack = new Stack<string>();
        protected bool IsInContinuation = false;
        protected string Execute(string source) {
            source = source.Trim();
            
            // Shell Commmand?
            if (source.StartsWith("!")) {
                return Shell(source.Substring(1));
            }

            // Internal Commmand?
            if (source.StartsWith(":"))
            {
                return Internal(source.Substring(1));
            }

            // FSQL Command

            //Continuation?
            if (IsInContinuation) {
                if (source.EndsWith(LineContinuationCharacter)) {
                    // Continuation continues
                    source = source.TrimEnd(LineContinuationCharacter.ToCharArray());                    
                    CommandBuffer += source + "\n";
                } else {
                    //Exit Continuation
                    IsInContinuation = false;
                    Prompt = PromptStack.Pop();
                    source = CommandBuffer + source;
                }
            } else {
                if (source.EndsWith(LineContinuationCharacter))
                {
                    source = source.TrimEnd(LineContinuationCharacter.ToCharArray());
                    // Continuation begins
                    IsInContinuation = true;
                    PromptStack.Push(Prompt);
                    Prompt = ContinuationPrompt;
                    CommandBuffer = source + "\n";
                }
            }

            if (!IsInContinuation) {
                var stmt = Engine.Parse(source);
                string results;
                try {
                    results = stmt?.Execute(Ctx).ToString();
                } catch (Exception ex) {
                    results = ex.Message + "\n";
                }
                return results;
            } else {
                return string.Empty;
            }
        }

        protected string Internal(string command) {
            var parsed = command.Split(' ');
            var cmd = parsed[0].ToLowerInvariant();
            var args = command.Substring(cmd.Length);
            string results = string.Empty;
            switch (cmd) {
                case "cls":
                case "clear":
                    Console.Clear();
                    break;
                case "load":
                    results = Load(args);
                    break;
                case "list":
                    results = List(args);
                    break;
                default:
                    results = $"Unrecognized internal command: '{cmd}'.";
                    break;
            }
            return results;
        }

        protected string Shell(string command) {
            return ShellCommand.Run(command);
        }

        #region Internal Commands

        protected string Load(string filename) {
            if (string.IsNullOrWhiteSpace(filename)) {
                filename = ProfileScript;
                Out.WriteLine($"(Reloading '{filename}')");
            }
            if (!File.Exists(filename)) return $"File not found: {filename}";
            var script = File.ReadAllText(filename);
            if (string.IsNullOrWhiteSpace(script)) return $"Script is empty.";
            var prg = Engine.Build(script);
            return prg.InvokeGeneric(Ctx).ToString();
        }

        protected string List(string functionName) {
            var fnDef = Ctx["#" + functionName.Trim().ToLowerInvariant()] as Function;
            return "\n" + fnDef?.GetListing();
        }
        #endregion

        protected string Logo() {
            var sb = new StringBuilder();
            sb.AppendLine($"FSQL Interactive");
            sb.AppendLine($"Version {Assembly.GetExecutingAssembly().GetName().Version}");
            sb.AppendLine($"(C) 2017 Ill Eagle Software");
            
            return sb.ToString();
        }

        public string Prompt { get; set; } = DefaultPrompt;

    }
}