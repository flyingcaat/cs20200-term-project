namespace Minesweeper

open System
open Minesweeper.Domain
open Minesweeper.Board

module Console = 
    let moveCursor (key: ConsoleKey) (board: Board) : Board = 
        let cur = board.curpos
        let newpos = 
            match key with
            | ConsoleKey.UpArrow -> {cur with y = max 0 (cur.y-1)}
            | ConsoleKey.DownArrow -> {cur with y = min (board.height-1) (cur.y+1)}
            | ConsoleKey.LeftArrow -> {cur with x = max 0 (cur.x-1)}
            | ConsoleKey.RightArrow -> {cur with x = min (board.width-1) (cur.x+1)}
            | _ -> cur
        {board with curpos = newpos}
    
    let draw (board: Board) = 
        //Console.Clear()
        Console.SetCursorPosition(0, 0)
        let fcnt = board.cell.Values |> Seq.filter (fun cell -> cell.state = Flag) |> Seq.length
        Console.ForegroundColor <- ConsoleColor.Yellow
        Console.WriteLine("=====================================================")
        Console.WriteLine($"   Flags Placed : {fcnt},  {board.cnt - fcnt} left")
        Console.WriteLine("=====================================================")
        Console.ResetColor()
        match board.status with
        | Begin | Playing -> 
            Console.WriteLine(" Tip: Move(Arrow) | Open/Chording(Space) | Flag(F) | Back to Menu(Backspace) ")
        | Win -> 
            Console.ForegroundColor <- ConsoleColor.Green
            Console.WriteLine("\n YOU WIN! EVERY MINES FOUND! ")
            Console.WriteLine("AGAIN? (Y: yes / N: exit)")
            Console.ResetColor()
        | Gameover -> 
            Console.ForegroundColor <- ConsoleColor.Red
            Console.WriteLine("\n GAME OVER! ")
            Console.WriteLine("AGAIN? (Y: yes / N: exit)")
            Console.ResetColor()
        Console.WriteLine()
        Console.WriteLine()
        for y in 0 .. board.height-1 do
            for x in 0 .. board.width-1 do
                let pos = {x=x;y=y}
                if pos = board.curpos then
                    Console.BackgroundColor <- ConsoleColor.Gray
                    Console.ForegroundColor <- ConsoleColor.Black
                else Console.ResetColor()
                match Map.tryFind pos board.cell with
                | Some cell ->
                    match cell.state with
                    | Flag -> 
                        if pos <> board.curpos then Console.ForegroundColor <- ConsoleColor.Red
                        Console.Write("🚩")  
                        Console.ResetColor()
                    | Closed -> Console.Write(" -")
                    | Opened -> 
                        match cell.content with
                            | Mine -> 
                                if pos <> board.curpos then Console.ForegroundColor <- ConsoleColor.Red
                                Console.Write("💥")
                                Console.ResetColor()
                            | Number n -> Console.Write($" {n}")
                            | Empty -> Console.Write(" .")
                | None -> ()
                Console.ResetColor()
            Console.WriteLine()
    
    let rec stage (board: Board) = 
        draw board
        let key = Console.ReadKey(true)
        if board.status = Gameover || board.status = Win then
            match key.Key with
            | ConsoleKey.Y -> true
            | ConsoleKey.N -> false
            | _ -> stage board
        else
            match key.Key with
            | ConsoleKey.Escape -> false 
            | ConsoleKey.Backspace -> true
            | ConsoleKey.Spacebar -> 
                let uboard = Functions.space board.curpos board
                stage uboard
            | ConsoleKey.F ->
                let uboard = Functions.flag board.curpos board
                stage uboard
            | k when k = ConsoleKey.UpArrow || k = ConsoleKey.DownArrow || k = ConsoleKey.LeftArrow || k = ConsoleKey.RightArrow ->
                let updateBoard = moveCursor k board
                stage updateBoard
            | _ ->
                stage board