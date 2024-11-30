namespace FieldOfWonders

open System
open System.Collections.Generic

type LetterState =
    | Open of letter: char
    | Closed

type GameResult =
    | Lost
    | Won

type GameStatus =
    | InProgress
    | Finished of GameResult

type GameState = {
    Attempts: int
    MaxAttempts: int
    WordState: LetterState list
    Status: GameStatus
}

type FieldOfWonders(wordToGuess: string, maxAttempts: int) =

    let wordToGuess = wordToGuess.ToLowerInvariant()
    let guessedLetters = HashSet<char>()

    let mutable attemptsCount = 0

    let exceededAttemptsMessage = $"Exceeded attempts count: {attemptsCount} of {maxAttempts}"

    let canGuess () =
        attemptsCount < maxAttempts

    let getResult () =
        if Seq.forall guessedLetters.Contains wordToGuess then
            Won
        else
            Lost

    member this.LetterCount with get() = wordToGuess.Length

    member this.CurrentState with get(): GameState =
        let wordState =
            wordToGuess
            |> Seq.map (fun l ->
                if guessedLetters.Contains(l) then
                    Open l
                else
                    Closed
            )
            |> List.ofSeq

        let status =
            if canGuess () then
                InProgress
            else
                Finished (getResult ())

        {
            Attempts = attemptsCount
            MaxAttempts = maxAttempts
            WordState = wordState
            Status = status
        }

    member this.GuessLetter(letter: char): bool =
        if not(canGuess ()) then
            failwith exceededAttemptsMessage

        let charInLower =
            Char.ToLowerInvariant(letter)

        let isSuccessGuess =
            wordToGuess.Contains(charInLower)

        if isSuccessGuess then
            guessedLetters.Add(charInLower) |> ignore

        attemptsCount <- attemptsCount + 1

        isSuccessGuess

    member this.GuessWord(word: string): bool =
        if not(canGuess ()) then
            failwith exceededAttemptsMessage

        let isSuccessGuess =
            wordToGuess = word.ToLowerInvariant()

        if isSuccessGuess then
            Seq.iter (guessedLetters.Add >> ignore) wordToGuess

        attemptsCount <- attemptsCount + 1

        isSuccessGuess