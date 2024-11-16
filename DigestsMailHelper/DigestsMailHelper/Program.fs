open System

open System.IO
open System.Threading.Tasks

open DigestsMailHelper.EmlParser
open FSharp.Control
open Microsoft.Extensions.Configuration

let chooseDigestEmailAsync (file: FileInfo): Task<DigestInfo option> =
    task {
        use stream = file.OpenRead()
        let! digestInfoOption = tryParseDigestInfoAsync stream

        if digestInfoOption.IsNone then
            do! Console.Out.WriteLineAsync($"{file.Name}: not a digest")

        return digestInfoOption
    }

let readDigestsAsync (emlFiles: FileInfo seq): Task<DigestInfo list> =
    emlFiles
    |> TaskSeq.ofSeq
    |> TaskSeq.chooseAsync chooseDigestEmailAsync
    |> TaskSeq.toListAsync

let configuration =
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build()

let mailDirectoryPath =
    Path.Combine(
        AppContext.BaseDirectory,
        configuration
            .GetRequiredSection("MailDirectoryPath")
            .Value
    )

let directory = DirectoryInfo(mailDirectoryPath)
if not directory.Exists then
    failwith $"Unable to find directory {directory.FullName}"

printfn $"Using directory {directory.FullName}"

let emlFiles =
    directory.EnumerateFiles("*.eml", SearchOption.TopDirectoryOnly)

let csharp, react =
    readDigestsAsync emlFiles
    |> _.GetAwaiter().GetResult()
    |> List.partition (fun x -> x.Type = CSharp)

let print name digests =
    printfn name
    digests
    |> List.sortBy _.IssueNumber
    |> List.iter (fun x -> printfn $"- [ ] [#{x.IssueNumber}]({x.IssueUrl})")

csharp |> print "csharp digests:"
react |> print "react digests:"