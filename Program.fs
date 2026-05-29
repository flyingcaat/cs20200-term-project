namespace Minesweeper

open System
open System.Text
open Minesweeper.Domain


module Program = 
    [<EntryPoint>]
    let main argv = 
        Console.OutputEncoding <- Encoding.UTF8
        let rec mainloop () = 
            let initialBoard = Start.startMenu 0
            let again = Console.stage initialBoard
            if again then mainloop ()
            else 
                Console.WriteLine("\nEXIT")
                0
        mainloop ()