module TerrainGenerator.Tiles

open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core

type LandWaterTile =
    | Land
    | Water

type LandTile =
    | Snow
    | Tundra
    | Bare
    | Scorched
    | Taiga
    | Shrubland
    | Temperate_Desert
    | Temperate_Rain_Forest
    | Temperate_Deciduous_Forest
    | Grassland
    | Tropical_Rain_Forest
    | Tropical_Seasonal_Forest
    | Subtropical_Desert
    

    member c.weight =
        match c with
        | Snow -> LandTileWeights.Snow
        | Tundra -> LandTileWeights.Tundra
        | Bare -> LandTileWeights.Bare
        | Scorched -> LandTileWeights.Scorched
        | Taiga -> LandTileWeights.Taiga
        | Shrubland -> LandTileWeights.Shrubland
        | Temperate_Desert -> LandTileWeights.Temperate_Desert
        | Temperate_Rain_Forest -> LandTileWeights.Temperate_Rain_Forest
        | Temperate_Deciduous_Forest -> LandTileWeights.Temperate_Deciduous_Forest
        | Grassland -> LandTileWeights.Grassland
        | Tropical_Rain_Forest -> LandTileWeights.Tropical_Rain_Forest
        | Tropical_Seasonal_Forest -> LandTileWeights.Tropical_Seasonal_Forest
        | Subtropical_Desert -> LandTileWeights.Subtropical_Desert


and LandTileWeights =
    | Snow = 3
    | Tundra = 1
    | Bare = 1
    | Scorched = 1
    | Taiga = 2
    | Shrubland = 2
    | Temperate_Desert = 3
    | Temperate_Rain_Forest = 1
    | Temperate_Deciduous_Forest = 2
    | Grassland = 3
    | Tropical_Rain_Forest = 2
    | Tropical_Seasonal_Forest = 2
    | Subtropical_Desert = 1

let sumWeight = Array.sum (Enum.GetValues typeof<LandTileWeights> :?> int[])

let private landTileProbability (tile: LandTile) =
    let unit = 1.0 / (sumWeight |> float)
    (tile.weight |> float) * unit

let tileMeasure =
    [ Snow, landTileProbability Snow ]
    @ [ Tundra, landTileProbability Tundra ]
    @ [ Bare, landTileProbability Bare ]
    @ [ Scorched, landTileProbability Scorched ]
    @ [ Taiga, landTileProbability Taiga ]
    @ [ Shrubland, landTileProbability Shrubland ]
    @ [ Temperate_Desert, landTileProbability Temperate_Desert ]
    @ [ Temperate_Rain_Forest, landTileProbability Temperate_Rain_Forest ]
    @ [ Grassland, landTileProbability Grassland ]
    @ [ Tropical_Rain_Forest, landTileProbability Tropical_Rain_Forest ]
    @ [ Tropical_Seasonal_Forest, landTileProbability Tropical_Seasonal_Forest ]
    @ [ Subtropical_Desert, landTileProbability Subtropical_Desert ]

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
            (false, LandTile.Snow)
            randomSelect

    tile

type WaterTile = | Water

type TerrainTile =
    | Water
    | Land of tile: LandTile

let TerrainTable =
    [ [ Land(Subtropical_Desert); Land(Grassland); Water; Water; Water; Water ]
      (List.map Land [ Subtropical_Desert; Grassland; Tropical_Seasonal_Forest; Tropical_Seasonal_Forest; Tropical_Rain_Forest; Tropical_Rain_Forest ])
      (List.map Land [ Temperate_Desert; Grassland; Grassland; Temperate_Deciduous_Forest; Temperate_Deciduous_Forest; Temperate_Rain_Forest ])
      (List.map Land [ Temperate_Desert; Temperate_Desert; Shrubland; Shrubland; Taiga; Taiga])
      (List.map Land [ Scorched; Bare; Tundra; Snow; Snow; Snow ]) ]