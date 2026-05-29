namespace Minesweeper

open System
open Minesweeper.Domain
open Minesweeper.Board

module Start = 
    let rec selectdiff (idx : int) : Board = 
        Console.Clear()
        Console.ForegroundColor <- ConsoleColor.Cyan
        Console.WriteLine("=======================================")
        Console.WriteLine("              DIFFICULTY               ")
        Console.WriteLine("=======================================\n")
        Console.ResetColor()  
        let menu = [
            "Easy : (10x10, 10 Mines)"
            "Hard : (16x16, 40 Mines)"
            "Harder! : (Custom Size & Mines)"
            "Back to Menu"
        ]   
        for i in 0..menu.Length-1 do
            if i = idx then
                Console.BackgroundColor <- ConsoleColor.White
                Console.ForegroundColor <- ConsoleColor.Black
                Console.WriteLine($"> {menu.[i]}")
                Console.ResetColor()
            else Console.WriteLine($" {menu.[i]}")
        Console.WriteLine("\nChoose with up/down arrows. Press Space to select.")
        let key = Console.ReadKey(true)
        match key.Key with
        | ConsoleKey.UpArrow -> selectdiff ((idx + menu.Length-1)%menu.Length)
        | ConsoleKey.DownArrow -> selectdiff ((idx + 1)%menu.Length)
        | ConsoleKey.Spacebar -> 
            match idx with
            | 0 -> 
                Console.Clear()
                Board.createBoard (Basic Easy)
            | 1 -> 
                Console.Clear()
                Board.createBoard (Basic Hard)
            | 2 ->
                Console.Clear()
                Console.WriteLine("=== Harder! (Custom Mode) ===")
                Console.Write("Width (Max: 30): ")
                let wIn = Console.ReadLine()
                let w = match Int32.TryParse(wIn) with | (true, v) -> min (max v 5) 30 | _ -> 20
                Console.Write("Height (Max: 24): ")
                let hIn = Console.ReadLine()
                let h = match Int32.TryParse(hIn) with | (true, v) -> min (max v 5) 24 | _ -> 15
                Console.Write("Number of Mines (ex: 50): ")
                let cntIn = Console.ReadLine()
                let maxMines = (w * h) - 10
                let cnt = match Int32.TryParse(cntIn) with | (true, v) -> min (max v 1) maxMines | _ -> 50
                Board.createBoard (Harder (w, h, cnt))
            | _ -> startMenu 0
        | _ -> selectdiff idx
    and startMenu (idx: int) : Board = 
        Console.Clear()
        Console.ForegroundColor <- ConsoleColor.Cyan
        Console.WriteLine("=======================================")
        Console.WriteLine("          F# CONSOLE MINESWEEPER       ")
        Console.WriteLine("=======================================\n")
        Console.ResetColor()
        let menu = [
            "Start Game"
            "Exit"
        ]
        for i in 0..menu.Length-1 do 
            if i = idx then 
                Console.BackgroundColor <- ConsoleColor.White
                Console.ForegroundColor <- ConsoleColor.Black
                Console.WriteLine($"> {menu.[i]}")
                Console.ResetColor()
            else Console.WriteLine($"  {menu.[i]}")
        Console.WriteLine("\nChoose with up/down arrows. Press Space to select.")
        let key = Console.ReadKey(true)
        match key.Key with
        | ConsoleKey.UpArrow -> startMenu ((idx + menu.Length-1)%menu.Length)
        | ConsoleKey.DownArrow -> startMenu ((idx + 1)%menu.Length)
        | ConsoleKey.Spacebar -> 
            match idx with
            | 0 -> selectdiff 0
            | _ -> 
                Console.WriteLine("Exiting..")
                Environment.Exit(0)
                Board.createBoard (Basic Easy)
        | _ -> startMenu idx
