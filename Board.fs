namespace Minesweeper

open System
open Minesweeper.Domain

module Board = 
    
    let Size (mode: GameMode) = 
        match mode with
        | Basic Easy -> (10, 10, 10)
        | Basic Hard -> (16, 16, 40)
        | Harder(width, height, cnt) -> (width, height, cnt)
    
    let createBoard (mode: GameMode) = 
        let width, height, cnt = Size mode
        let emptylist = 
            [for x in 0..width-1 do
                for y in 0..height-1 do 
                    let pos = {x = x; y = y}
                    let cell = {content = Empty; state = Closed}
                    yield (pos, cell)]
        let emptycell = Map.ofList emptylist
        {
            width = width
            height = height
            cnt = cnt
            cell = emptycell
            curpos = {x = 0; y = 0}
            status = Begin
            mode = mode
        }

    let mines (start: Position) (board: Board) : Board =
        let sx = start.x
        let sy = start.y
        let exceptstart = 
            [for x in 0..board.width-1 do 
                for y in 0..board.height-1 do 
                    let pos = {x = x; y = y}
                    if not (x>=sx-2 && x<=sx+2 && y>=sy-2 && y<=sy+2) then yield pos]
        let r = Random()
        let minepos = 
            exceptstart |> List.sortBy(fun _ -> r.Next()) |> List.take board.cnt
        let createMine = 
            List.fold (fun (cells: Map<Position, Cell>) pos ->
                match Map.tryFind pos cells with
                | Some cell -> 
                    let mine = {cell with content = Mine}
                    Map.add pos mine cells
                | None -> cells
            ) board.cell minepos
        {board with cell = createMine; status = Playing}
    
    let neighbor (pos: Position) = 
        [for i in -1..1 do 
            for j in -1..1 do 
                if i<>0 || j<>0 then yield {x = pos.x + i; y = pos.y+j}]
    
    let countCell (pos: Position) (cells: Map<Position, Cell>) : int =
        neighbor pos
        |> List.choose (fun p -> Map.tryFind p cells)
        |> List.filter (fun cell -> cell.content = Mine)
        |> List.length
    
    let updateNum (board: Board) : Board = 
        let update = 
            board.cell
            |> Map.map (fun pos cell ->
                if cell.content = Mine then cell
                else 
                    let cnt = countCell pos board.cell
                    if cnt = 0 then cell
                    else {cell with content = Number cnt}
            )
        {board with cell = update}
