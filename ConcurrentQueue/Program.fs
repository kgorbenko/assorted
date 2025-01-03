open System
open System.Threading.Tasks
open System.Collections.Concurrent
open System.Linq

open FSharp.Control

let tryDequeue (queue: ConcurrentQueue<'a>) : 'a option =
    match queue.TryDequeue() with
    | true, value -> Some value
    | false, _ -> None

let loop (action: unit -> 'a option) (delaySeconds: int) : TaskSeq<'a> =
    taskSeq {
        let mutable item = action ()

        while item.IsSome do
            yield item.Value
            do! Task.Delay(TimeSpan.FromSeconds(delaySeconds |> int64))
            item <- action ()
    }

let runThread threadNumber delaySeconds queueAction : Task =
    task {
        do!
            loop queueAction delaySeconds
            |> TaskSeq.iterAsync (fun x ->
                task { do! Console.Out.WriteLineAsync($"Thread {threadNumber}: item {x}") }
            )
    }

let main () =
    task {
        let queue = ConcurrentQueue(Enumerable.Range(1, 30))

        let queueAction () =
            tryDequeue queue

        let thread1Task = queueAction |> runThread 1 1
        let thread2Task = queueAction |> runThread 2 4

        do! Task.WhenAll([thread1Task; thread2Task])

        ()
    }

(main ()).GetAwaiter().GetResult()