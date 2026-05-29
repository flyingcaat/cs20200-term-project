namespace Minesweeper.Domain

type Content = 
    | Empty
    | Mine
    | Number of int 

type State = 
    | Closed
    | Opened
    | Flag

type Cell = {
    content: Content
    state: State
}

type Difficulty = 
    | Easy
    | Hard

type GameMode = 
    | Basic of Difficulty
    | Harder of width: int * height: int * cnt: int

type Status = 
    | Begin
    | Playing
    | Win
    | Gameover

type Position = {
    x: int
    y: int
}

type Board = {
    width: int
    height: int
    cnt: int
    cell: Map<Position, Cell>
    curpos: Position
    status: Status
    mode: GameMode
}