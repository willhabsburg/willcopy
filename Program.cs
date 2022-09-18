/*
Will Habsburg
willcopy
Program.cs - entry point for this program
This file is self-explanitory, using System.CommandLine to parse the 
command line and call WillCopy.start()
*/
using willcopy;

var fromDir = new Argument<string>("from", "Source Directory");
var toDir = new Argument<string>("to", "Destination Directory");
var commandOption = new Option<string>(new[] { "-c", "--command" }, description: "One of: m (move - will move all files (including hidden) regardless of mode option)\nc (default) (copy)\nl (list files that would be moved/copied)");
var modeOption = new Option<string>(new[] { "-m", "--mode" }, description: "One of: a (always overwrite)\nn (never overwrite)\nu (default) (update if mod date is different AND/OR the file size is different)\nc (clone - like update, but deletes files in destination if they are not in the source.  Always includes hidden files)");
var logOption = new Option<string>(new[] { "-l", "--logfile" }, description: "log file to append information");
var recurOption = new Option<bool>(new[] { "-r", "--recursive" }, description: "Copy directories recursively");
var quietOption = new Option<bool>(new[] { "-q", "--quiet" }, description: "Keep the console quiet except for fatal errors");
var hiddenOption = new Option<bool>(new[] { "--hidden", "-i" }, description: "Include hidden and system files");

var rootCommand = new RootCommand
{
    commandOption,
    modeOption,
    logOption,
    recurOption,
    quietOption,
    hiddenOption,
    fromDir,
    toDir
};
rootCommand.Description = "A directory copy app";
rootCommand.SetHandler((string fromDir, string toDir, string command, string mode, string log, bool recur, bool quiet, bool hidden) =>
{
    WillCopy wc = new WillCopy(fromDir, toDir, command, mode, log, recur, quiet, hidden);
    wc.start();
}, fromDir, toDir, commandOption, modeOption, logOption, recurOption, quietOption, hiddenOption);
return rootCommand.Invoke(args);



