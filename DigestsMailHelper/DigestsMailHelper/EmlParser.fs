module DigestsMailHelper.EmlParser

open System
open System.IO
open System.Threading.Tasks
open MimeKit

type DigestType =
    | CSharp
    | React

type DigestInfo = {
    Type: DigestType
    IssueNumber: int
    Headline: string
    IssueUrl: string
}

module FromAddressParser =

    let private csharpDigestFrom = MailboxAddress.Parse("jakub@csharpdigest.net")
    let private reactDigestFrom = MailboxAddress.Parse("jakub@reactdigest.net")

    let getDigestType (from: MailboxAddress): DigestType option =
        match from with
        | from when from.Address.Equals(csharpDigestFrom.Address) -> Some DigestType.CSharp
        | from when from.Address.Equals(reactDigestFrom.Address) -> Some DigestType.React
        | _ -> None

module SubjectParser =

    open System.Text.RegularExpressions

    type SubjectParsingResult = {
        IssueNumber: int
        Headline: string
    }

    let private numberGroupName = "number"
    let private headlineGroupName = "headline"
    let private csharpSubjectRegex = Regex("^C\#(?<number>\d+)\s(?<headline>.*)$")
    let private reactSubjectRegex = Regex("^RD\#(?<number>\d+)\s(?<headline>.*)$")

    let parseSubject (digestType: DigestType) (subject: string): SubjectParsingResult option =
        let regex =
            match digestType with
            | CSharp -> csharpSubjectRegex
            | React -> reactSubjectRegex

        let parseMatch (match': Match): SubjectParsingResult =
            { IssueNumber = match'.Groups[numberGroupName].Value |> Int32.Parse
              Headline = match'.Groups[headlineGroupName].Value }

        regex.Matches(subject)
        |> Seq.tryExactlyOne
        |> Option.bind (parseMatch >> Some)

open SubjectParser

let private xNewsletterHeader = "x-newsletter"

let tryParseDigestInfoAsync (stream: Stream) : Option<DigestInfo> Task =
    task {
        use! message = MimeMessage.LoadAsync(stream)

        let tryParseDigestType (): DigestType option =
            Seq.exactlyOne message.From.Mailboxes |> FromAddressParser.getDigestType

        let tryParseSubject (digestType: DigestType): SubjectParsingResult option =
            parseSubject digestType message.Subject

        let tryGetIssueUrl (): string option =
            message.Headers
            |> Seq.tryFind (fun x -> x.Field = xNewsletterHeader)
            |> Option.map _.Value

        return
            tryParseDigestType ()
            |> Option.bind (fun digestType ->
                tryParseSubject digestType |> Option.map (fun x -> digestType, x)
            )
            |> Option.bind (fun (digestType, subject) ->
                tryGetIssueUrl () |> Option.map (fun x -> digestType, subject, x)
            )
            |> Option.map (fun (digestType, subject, issueUrl) ->
                { Type = digestType
                  IssueNumber = subject.IssueNumber
                  Headline = subject.Headline
                  IssueUrl = issueUrl }
            )
    }