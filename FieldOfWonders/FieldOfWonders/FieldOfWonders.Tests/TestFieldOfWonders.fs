module FieldOfWonders.Tests.TestFieldOfWonders

open NUnit.Framework
open FieldOfWonders

[<Test>]
let ``Letter count is correct`` () =
    let wordToGuess = "test"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.LetterCount, Is.EqualTo(wordToGuess.Length))

[<Test>]
let ``Make first guess with no matching letters`` () =
    let wordToGuess = "word"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('a'), Is.False)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 1
            MaxAttempts = 1
            WordState = [Closed; Closed; Closed; Closed]
            Status = Finished Lost
        })
    )

[<Test>]
let ``Make first guess with one matching letter`` () =
    let wordToGuess = "word"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('o'), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 1
            MaxAttempts = 1
            WordState = [Closed; Open 'o'; Closed; Closed]
            Status = Finished Lost
        })
    )

[<Test>]
let ``Make first guess with multiple matching letters`` () =
    let wordToGuess = "topo"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('o'), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 1
            MaxAttempts = 1
            WordState = [Closed; Open 'o'; Closed; Open 'o']
            Status = Finished Lost
        })
    )

[<Test>]
let ``Make two guesses with matching letters`` () =
    let wordToGuess = "space"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 2)

    Assert.That(fieldOfWonders.GuessLetter('s'), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 1
            MaxAttempts = 2
            WordState = [Open 's'; Closed; Closed; Closed; Closed]
            Status = InProgress
        })
    )

    Assert.That(fieldOfWonders.GuessLetter('p'), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 2
            MaxAttempts = 2
            WordState = [Open 's'; Open 'p'; Closed; Closed; Closed]
            Status = Finished Lost
        })
    )

[<Test>]
let ``Make multiple guesses to guess all letters`` () =
    let wordToGuess = "space"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 5)

    Assert.That(fieldOfWonders.GuessLetter('s'), Is.True)
    Assert.That(fieldOfWonders.GuessLetter('p'), Is.True)
    Assert.That(fieldOfWonders.GuessLetter('a'), Is.True)
    Assert.That(fieldOfWonders.GuessLetter('c'), Is.True)
    Assert.That(fieldOfWonders.GuessLetter('e'), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 5
            MaxAttempts = 5
            WordState = [Open 's'; Open 'p'; Open 'a'; Open 'c'; Open 'e']
            Status = Finished Won
        })
    )

[<Test>]
let ``Cannot make more guesses`` () =
    let wordToGuess = "space"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('s'), Is.True)
    Assert.Throws(fun () -> fieldOfWonders.GuessLetter('p') |> ignore) |> ignore

[<Test>]
let ``Can guess uppercase letter with a lowercase`` () =
    let wordToGuess = "tOast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('o'), Is.True)

[<Test>]
let ``Can guess lowercase letter with an uppercase`` () =
    let wordToGuess = "toast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('O'), Is.True)

[<Test>]
let ``Guess word with correct option`` () =
    let wordToGuess = "toast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessWord(wordToGuess), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 1
            MaxAttempts = 1
            WordState = [Open 't'; Open 'o'; Open 'a'; Open 's'; Open 't']
            Status = Finished Won
        })
    )

[<Test>]
let ``Guess word with incorrect option`` () =
    let wordToGuess = "toast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessWord("toase"), Is.False)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 1
            MaxAttempts = 1
            WordState = [Closed; Closed; Closed; Closed; Closed]
            Status = Finished Lost
        })
    )

[<Test>]
let ``Guess word after letter guesses`` () =
    let wordToGuess = "stop"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 4)

    Assert.That(fieldOfWonders.GuessLetter('s'), Is.True)
    Assert.That(fieldOfWonders.GuessLetter('t'), Is.True)
    Assert.That(fieldOfWonders.GuessLetter('o'), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 3
            MaxAttempts = 4
            WordState = [Open 's'; Open 't'; Open 'o'; Closed]
            Status = InProgress
        })
    )

    Assert.That(fieldOfWonders.GuessWord("stop"), Is.True)

    Assert.That(
        fieldOfWonders.CurrentState,
        Is.EqualTo({
            Attempts = 4
            MaxAttempts = 4
            WordState = [Open 's'; Open 't'; Open 'o'; Open 'p']
            Status = Finished Won
        })
    )

[<Test>]
let ``Guess word with correct option in another case`` () =
    let wordToGuess = "toast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessWord("TOAST"), Is.True)

[<Test>]
let ``Cannot guess a word when exceeded guess count`` () =
    let wordToGuess = "toast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessWord("TOASE"), Is.False)
    Assert.Throws(fun () -> fieldOfWonders.GuessWord("TOAST") |> ignore) |> ignore

[<Test>]
let ``Cannot guess a word when exceeded guess count after the letter guess`` () =
    let wordToGuess = "toast"

    let fieldOfWonders = FieldOfWonders(wordToGuess, 1)

    Assert.That(fieldOfWonders.GuessLetter('t'), Is.True)
    Assert.Throws(fun () -> fieldOfWonders.GuessWord("TOAST") |> ignore) |> ignore
