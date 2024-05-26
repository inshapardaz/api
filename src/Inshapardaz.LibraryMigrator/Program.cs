using Inshapardaz.Domain.Models.Library;
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

        var sourceTypeOption = new Option<string>(
                name: "--source-type",
                description: "Databse Type of source. Options are mysql and sqlserver")
        { IsRequired = true };
        sourceTypeOption.AddAlias("-st");

        var destinationOption = new Option<string>(
                        name: "--destination",
                        description: "Connection String for the destination database")
        { IsRequired = true };
        destinationOption.AddAlias("-d");
        destinationOption.AddAlias("--dest");

        var destinationTypeOption = new Option<string>(
                name: "--destination-type",
                description: "Databse Type of destination. Options are mysql and sqlserver")
        { IsRequired = true };
        destinationTypeOption.AddAlias("-dt");

        var libraryIdOption = new Option<int>(
                        name: "--libraryId",
                        description: "Id of library to migrate")
        { IsRequired = true };
        libraryIdOption.AddAlias("-l");

        var correctionOption= new Option<bool>(
                        name: "--corrections",
                        description: "Set if want to migrate corrections only")
        { IsRequired = false };
        correctionOption.AddAlias("-c");

        var rootCommand = new RootCommand("Nawishta library migration tool.");
        rootCommand.AddOption(sourceOption);
        rootCommand.AddOption(sourceTypeOption);
        rootCommand.AddOption(destinationOption);
        rootCommand.AddOption(destinationTypeOption);
        rootCommand.AddOption(libraryIdOption);
        rootCommand.AddOption(correctionOption);

        rootCommand.SetHandler(async (context) =>
        {
            var source = context.ParseResult.GetValueForOption(sourceOption);
            var sourceType = Enum.Parse<DatabaseTypes>(context.ParseResult.GetValueForOption(sourceTypeOption));
            var destination = context.ParseResult.GetValueForOption(destinationOption);
            var destinationType = Enum.Parse<DatabaseTypes>(context.ParseResult.GetValueForOption(destinationTypeOption));
            int libraryId = context.ParseResult.GetValueForOption(libraryIdOption);
            bool corrections = context.ParseResult.GetValueForOption(correctionOption);

            var token = context.GetCancellationToken();

            await new Migrator(source, sourceType, destination, destinationType).Migrate(libraryId, corrections, token);
        });

        await rootCommand.InvokeAsync(args);

        return 0;
    }
}
