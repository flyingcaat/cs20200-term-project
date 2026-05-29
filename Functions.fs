namespace Minesweeper
open Minesweeper.Domain
module Functions = 
    let flag (pos:Position) (board: Board) : Board =
        if board.status = Playing then 
            match Map.tryFind pos board.cell with
            | Some cell -> 
                let ncell = 
                    match cell.state with
                    | Closed -> {cell with state = Flag}
                    | Flag -> {cell with state = Closed}
                    | Opened -> cell
                let ncells = Map.add pos ncell board.cell
                {board with cell = ncells}
            | None -> board
        else board
    
    let rec openCell (pos: Position) (board: Board) : Board = 
        match Map.tryFind pos board.cell with
        | None -> board
        | Some cell when cell.state = Opened || cell.state = Flag -> board
        | Some cell ->
            let ucell = {cell with state = Opened}
            let uboard = {board with cell = (Map.add pos ucell board.cell)}
            match cell.content with
            | Mine -> 
                Finish.openMines {uboard with status = Gameover}
            | Number n -> uboard
            | Empty -> 
                let neighbor = Board.neighbor pos
                neighbor |> List.fold(fun b p -> openCell p b) uboard  

    let chording (pos: Position) (board: Board): Board = 
        if board.status = Playing then
            match Map.tryFind pos board.cell with
            | Some cell when cell.state = Opened -> 
                match cell.content with 
                | Number n -> 
                    let neighbor = Board.neighbor pos
                    let fcnt = 
                        neighbor
                        |> List.choose (fun p -> Map.tryFind p board.cell)
                        |> List.filter (fun c -> c.state = Flag)
                        |> List.length
                    if fcnt = n then 
                        let cboard = neighbor |> List.fold (fun b p -> openCell p b) board
                        Finish.win cboard
                    else board
                | _ -> board
            | _ -> board
        else board

    let rec solver (board: Board) : bool = 
        let mutable change = false
        let easy (curboard: Board) (pos: Position) : Board = 
            match Map.tryFind pos curboard.cell with
            | Some cell when cell.state = Opened -> 
                match cell.content with
                | Number n ->
                    let ncells = 
                        List.choose (fun p -> Map.tryFind p curboard.cell |> Option.map (fun c -> p, c)) (Board.neighbor pos)
                    let f = ncells |> List.filter (fun (p, c) -> c.state = Flag)
                    let close = ncells |> List.filter (fun (p, c) -> c.state = Closed)
                    if close.IsEmpty then curboard
                    else 
                        if f.Length = n then
                            change <- true
                            close |> List.fold (fun b (p, c) -> openCell p b) curboard
                        elif f.Length + close.Length = n then 
                            change <- true
                            close |> List.fold (fun b (p, c) -> flag p b) curboard
                        else curboard
                | _ -> curboard
            | _ -> curboard
        let nboard = board.cell.Keys |> Seq.fold easy board
        let ifwin = Finish.win nboard
        if ifwin.status = Win then true
        elif ifwin.status = Gameover then false
        elif change then solver nboard
        else
            let err (b: Board) = 
                b.status = Gameover || (b.cell |> Map.exists (fun pos c ->
                    if c.state = Opened then
                        match c.content with
                        | Number n -> 
                            let neighbor = Board.neighbor pos |> List.choose (fun p -> Map.tryFind p b.cell)
                            let fcnt = neighbor |> List.filter (fun cc -> cc.state = Flag) |> List.length
                            let ccnt = neighbor |> List.filter (fun cc -> cc.state = Closed) |> List.length
                            fcnt > n || (fcnt + ccnt) < n
                        | _ -> false
                    else false
                ))
            let test = 
                nboard.cell |> Map.tryPick (fun pos c -> 
                    if c.state = Closed then
                        let good = 
                            Board.neighbor pos |> List.exists (fun p -> 
                                match Map.tryFind p nboard.cell with
                                | Some cc -> cc.state = Opened
                                | None -> false
                            )
                        if good then Some pos else None
                    else None
                )
            match test with
            | Some pos -> 
                let testboard = flag pos nboard
                let result = testboard.cell.Keys |> Seq.fold easy testboard
                if err result then solver (openCell pos nboard)
                else
                    let testboard = openCell pos nboard
                    let result = testboard.cell.Keys |> Seq.fold easy testboard
                    if err result then solver (flag pos nboard)
                    else false
            | None -> false

    let space (pos: Position) (board: Board) : Board = 
            match board.status with
            | Begin -> 
                let rec noguess (b: Board) = 
                    let nboard = 
                        b
                        |> Board.mines pos
                        |> Board.updateNum
                        |> openCell pos
                    if solver nboard then nboard |> Finish.win
                    else noguess b
                match board.mode with
                | Basic _ -> noguess board
                | Harder _ -> board |> Board.mines pos |> Board.updateNum |> openCell pos |> Finish.win
            | Playing -> 
                match Map.tryFind pos board.cell with
                | Some cell when cell.state = Opened -> 
                    match cell.content with
                    | Number _ -> chording pos board
                    | _ -> board
                | Some cell when cell.state = Closed ->
                    board
                    |> openCell pos
                    |> Finish.win
                | _ -> board
            | _ -> board  
            