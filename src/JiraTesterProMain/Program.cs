// See https://aka.ms/new-console-template for more information

using System.Collections;
using CommandLine;
using JiraTesterProData;

Console.WriteLine("Hello, World!");
Parser.Default.ParseArguments<JiraTesterCommandLineOptions>(args)
    .WithParsed(opts => DoSomeWork(opts))
    .WithNotParsed((errs) => HandleParseError(errs));


void DoSomeWork(JiraTesterCommandLineOptions opts)
{
    Console.WriteLine("Command Line parameters provided were valid :)");
}


void HandleParseError(IEnumerable errs)
{
    Console.WriteLine("Command Line parameters provided were not valid!");
}
