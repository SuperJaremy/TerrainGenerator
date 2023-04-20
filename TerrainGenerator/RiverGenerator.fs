module TerrainGenerator.RiverGenerator

open System

type Direction =
    | Up
    | Down
    | Left
    | Right

let directionToCoordinate x y direction =
    match direction with
    | Up -> (x - 1, y)
    | Down -> (x + 1, y)
    | Left -> (x, y - 1)
    | Right -> (x, y + 1)

let logProbability (mapWidth: int) (mapHeight: int) (velocity: int) =
    let side = max mapHeight mapWidth
    ((log ((velocity + 1) |> float)) / (log ((side + 1) |> float))) * 0.15 + 0.7

let rec private step (x, y) probabilityFunc velocity direction (rng: Random) (map: Tiles.TerrainTile[,]) =
    let mapHeight = Array2D.length1 map
    let mapWidth = Array2D.length2 map

    if
        not (
            x > 0
            && x < mapHeight
            && y > 0
            && y < mapWidth
            && map.[x, y] <> Tiles.TerrainTile.Water
        )
    then
        map
    else
        map.[x, y] <- Tiles.TerrainTile.Water

        let sideOne, sideTwo =
            match direction with
            | Up -> Left, Right
            | Down -> Left, Right
            | Left -> Up, Down
            | Right -> Up, Down

        let directProbability = probabilityFunc mapWidth mapHeight velocity
        let sideProbability = (1.0 - directProbability) / 2.0
        let rand = rng.NextDouble()

        match rand with
        | _ when rand < directProbability ->
            step (directionToCoordinate x y direction) probabilityFunc (velocity + 1) direction rng map
        | _ when rand < (directProbability + sideProbability) ->
            step (directionToCoordinate x y sideOne) probabilityFunc 1 sideOne rng map
        | _ -> step (directionToCoordinate x y sideTwo) probabilityFunc 1 sideTwo rng map


let generateRiver x y probabilityFunc seed (map: Tiles.TerrainTile[,]) =
    let rng = Random(seed)
    let rand = rng.NextDouble()

    map.[x, y] <- Tiles.TerrainTile.Water
    
    let direction =
        match rand with
        | _ when rand < 0.25 -> Up
        | _ when rand < 0.5 -> Down
        | _ when rand < 0.75 -> Left
        | _ -> Right

    step (directionToCoordinate x y direction) probabilityFunc 1 direction rng map
