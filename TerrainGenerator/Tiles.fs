module TerrainGenerator.Tiles

open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core

type LandWaterTile =
    | Land
    | Water

type LandTile =
    | Sand
    | Rock
    | Tundra
    | Forest
    | Field
    | Snow

    member c.weight =
        match c with
        | Sand -> LandTileWeights.Sand
        | Rock -> LandTileWeights.Rock
        | Tundra -> LandTileWeights.Tundra
        | Forest -> LandTileWeights.Forest
        | Field -> LandTileWeights.Field
        | Snow -> LandTileWeights.Snow


and LandTileWeights =
    | Sand = 2
    | Rock = 1
    | Tundra = 2
    | Snow = 1
    | Forest = 3
    | Field = 4

let sumWeight = Array.sum (Enum.GetValues typeof<LandTileWeights> :?> int[])

let private landTileProbability (tile: LandTile) =
    let unit = 1.0 / (sumWeight |> float)
    (tile.weight |> float) * unit

let tileMeasure =
    [ Snow, landTileProbability Snow ]
    @ [ Rock, landTileProbability Rock ]
    @ [ Tundra, landTileProbability Tundra ]
    @ [ Forest, landTileProbability Forest ]
    @ [ Field, landTileProbability Field ]
    @ [ Sand, landTileProbability Sand ]

let private randomSelect =
    let _, b =
        List.fold
            (fun state value ->
                let sum, l = state
                let tile, (num: float) = value
                let newSum = sum + num
                newSum, (l @ [ tile, newSum ]))
            (0, [])
            tileMeasure

    b

let chooseTile num =
    let _, tile =
        List.fold
            (fun state value ->
                let success, t = state
                let tile, prob = value
                if success then state else ((num < prob), tile))
            (false, LandTile.Sand)
            randomSelect

    tile

type WaterTile = | Water

type TerrainTile =
    | Water
    | Land of tile: LandTile
