using System.CommandLine;

namespace Inshapardaz.LibraryMigrator;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var sourceOption = new Option<string>(
                name: "--source",
                description: "Connection String for the source")
        { IsRequired = true };
        sourceOption.AddAlias("-s");

        var destinationOption = new Option<string>(
                        name: "--destination",
                        description: "Connection String for the destination database")
        { IsRequired = true };
        destinationOption.AddAlias("-d");
        destinationOption.AddAlias("--dest");

        var libraryIdOption = new Option<int>(
                        name: "--libraryId",
                        description: "Id of library to migrate")
        { IsRequired = true };
        libraryIdOption.AddAlias("-l");

        var rootCommand = new RootCommand("Nawishta library migration tool.");
        rootCommand.AddOption(sourceOption);
        rootCommand.AddOption(destinationOption);
        rootCommand.AddOption(libraryIdOption);

        rootCommand.SetHandler(async (context) =>
        {
            string source = context.ParseResult.GetValueForOption(sourceOption);
            string destination = context.ParseResult.GetValueForOption(destinationOption);
            int libraryId = context.ParseResult.GetValueForOption(libraryIdOption);

            var token = context.GetCancellationToken();

            await new Migrator(source, destination).Migrate(libraryId, token);
        });

        await rootCommand.InvokeAsync(args);

        return 0;
    }
}
