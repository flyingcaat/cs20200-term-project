namespace Minesweeper
open Minesweeper.Domain

module Finish = 
    let openMines (board: Board) : Board = 
        let opencell = 
            board.cell
            |> Map.map (fun p c ->
                if c.content = Mine then {c with state = Opened}
                else c)
        {board with cell = opencell}
    
    let win (board: Board) : Board = 
        if board.status = Playing then 
            let ocnt = board.cell.Values |> Seq.filter (fun c->c.state = Opened) |> Seq.length
            if ocnt = (board.width * board.height - board.cnt) then 
                let leftflag = 
                    board.cell
                        |> Map.map (fun _ cell ->
                            if cell.state = Closed then { cell with state = Flag }
                            else cell
                        )
                {board with status = Win; cell = leftflag}
            else board
        else board