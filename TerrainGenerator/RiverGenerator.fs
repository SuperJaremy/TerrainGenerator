﻿module TerrainGenerator.RiverGenerator

open System

type Direction =
    | Up
    | Down
    | Left
    | Right

let private directionToCoordinate x y direction =
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
            && map.[x, y] <> Tiles.TerrainTile.Water(Tiles.WaterTile.Water)
        )
    then
        map
    else
        map.[x, y] <- Tiles.TerrainTile.Water(Tiles.WaterTile.Water)

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


let probabilityRiverGenerator probabilityFunc (rng: Random) x y (map: Tiles.TerrainTile[,]) =
    let rand = rng.NextDouble()

    map.[x, y] <- Tiles.TerrainTile.Water(Tiles.WaterTile.Water)

    let direction =
        match rand with
        | _ when rand < 0.25 -> Up
        | _ when rand < 0.5 -> Down
        | _ when rand < 0.75 -> Left
        | _ -> Right

    step (directionToCoordinate x y direction) probabilityFunc 1 direction rng map

let rec private elevationMapStep x y visited (elevationMap: float[,]) flowChooser (map: Tiles.TerrainTile[,]) =
    let mapWidth = Array2D.length2 map
    let mapHeight = Array2D.length1 map
    let left = x, y - 1
    let right = x, y + 1
    let up = x - 1, y
    let down = x + 1, y

    let leftHeight =
        (Left,
         (if y > 0 && not (List.contains left visited) then
              Some map.[fst left, snd left], Some elevationMap.[fst left, snd left]
          else
              None, None))
        |> fun (a, (b, c)) -> (a, b, c)

    let rightHeight =
        (Right,
         if y < mapWidth - 1 && not (List.contains right visited) then
             Some map.[fst right, snd right], Some elevationMap.[fst right, snd right]
         else
             None, None)
        |> fun (a, (b, c)) -> (a, b, c)

    let upHeight =
        (Up,
         (if x > 0 && not (List.contains up visited) then
              Some map.[fst up, snd up], Some elevationMap.[fst up, snd up]
          else
              None, None))
        |> fun (a, (b, c)) -> (a, b, c)

    let downHeight =
        (Down,
         if x < mapHeight - 1 && not (List.contains down visited) then
             Some map.[fst down, snd down], Some elevationMap.[fst down, snd down]
         else
             None, None)
        |> fun (a, (b, c)) -> (a, b, c)

    let res =
        match (flowChooser leftHeight rightHeight upHeight downHeight) with
        | Some Left -> Some left
        | Some Right -> Some right
        | Some Up -> Some up
        | Some Down -> Some down
        | _ -> None

    match res with
    | None -> map
    | Some value ->
        let newX, newY = value
        map.[newX, newY] <- Tiles.TerrainTile.Water(Tiles.WaterTile.Water)
        elevationMapStep newX newY (value :: visited) elevationMap flowChooser map

let elevationMapRiverGenerator (elevationMap: float[,]) flowChooser initX initY (map: Tiles.TerrainTile[,]) =
    map.[initX, initY] <- Tiles.TerrainTile.Water(Tiles.WaterTile.Water)
    elevationMapStep initX initY [] elevationMap flowChooser map

let maxFlowChooser endCondition left right up down =

    let dir, tile, value =
        Seq.ofList [ left; right; up; down ]
        |> Seq.filter (fun (_, _, z) -> Option.isSome z)
        |> Seq.maxBy (fun (_, _, z) -> Option.get z)

    if not (endCondition (dir, tile, value)) then
        Some dir
    else
        None

let minFlowChooser endCondition left right up down =

    try
        let dir, tile, value =
            Seq.ofList [ left; right; up; down ]
            |> Seq.filter (fun (_, _, z) -> Option.isSome z)
            |> Seq.minBy (fun (_, _, z) -> Option.get z)

        if not (endCondition (dir, tile, value)) then
            Some dir
        else
            None
    with :? ArgumentException ->
        None

let isWater (_, tile, _) =
    Option.get tile = Tiles.TerrainTile.Water(Tiles.WaterTile.Water)

let isSnow (_, tile, _) =
    Option.get tile = Tiles.TerrainTile.Land(Tiles.LandTile.Snow)

let blankRiverGenerator _ _ map = map

let rec private generateNRivers generator (rng: Random) rivers points map =
    if rivers = 0 || (List.length points) = 0 then
        map
    else
        let rand = rng.Next(0, List.length points)
        let x, y = List.item rand points

        generator x y map
        |> generateNRivers generator rng (rivers - 1) (List.removeAt rand points)

let generateRivers
    (generator: int -> int -> Tiles.TerrainTile[,] -> Tiles.TerrainTile[,])
    count
    srcCond
    seed
    (map: Tiles.TerrainTile[,])
    =
    let folder x y state elem =
        if srcCond x y elem then (x, y) :: state else state

    let possibleSources = Util.foldiArray2D folder [] map
    let rng = Random(seed)
    generateNRivers generator rng count possibleSources map

let generateNoRivers map = map
