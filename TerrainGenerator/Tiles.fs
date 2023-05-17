module TerrainGenerator.Tiles

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

type WaterTile = | Water

type TerrainTile =
    | Water of tile: WaterTile
    | Land of tile: LandTile

let private segmentBuilder segment =
    let _, b =
        List.fold
            (fun state value ->
                let sum, l = state
                let tile, (num: float) = value
                let newSum = sum + num
                newSum, (l @ [ tile, newSum ]))
            (0, [])
            segment

    b

let private toProbabilityList segment =
    let biomeSum = List.sumBy snd segment
    List.map (fun (x, y) -> x, (y / (biomeSum |> float))) segment
    |> segmentBuilder
    
let chooseTile segment num =
    let _, tile =
        List.fold
            (fun state value ->
                let success, _ = state
                let tile, prob = value
                if success then state else ((num < prob), tile))
            (false, TerrainTile.Land(LandTile.Snow))
            (toProbabilityList segment)

    tile
    
let biomeSegment =
    [ TerrainTile.Water(WaterTile.Water), 2.
      TerrainTile.Land(LandTile.Grassland), 3.
      TerrainTile.Land(LandTile.Tropical_Seasonal_Forest), 3.
      TerrainTile.Land(LandTile.Tundra), 2.
      TerrainTile.Land(LandTile.Snow), 1. ]

let landSegment =
    [TerrainTile.Land(LandTile.Subtropical_Desert), 2.
     TerrainTile.Land(LandTile.Grassland), 3.
     TerrainTile.Land(LandTile.Tropical_Seasonal_Forest), 3.
     TerrainTile.Land(LandTile.Tundra), 2.
     TerrainTile.Land(LandTile.Snow), 1.]

let TerrainTable =
    [ [ Land(Subtropical_Desert); Land(Grassland); Water(WaterTile.Water); Water(WaterTile.Water); Water(WaterTile.Water); Water(WaterTile.Water) ]
      (List.map
          Land
          [ Subtropical_Desert
            Grassland
            Tropical_Seasonal_Forest
            Tropical_Seasonal_Forest
            Tropical_Rain_Forest
            Tropical_Rain_Forest ])
      (List.map
          Land
          [ Temperate_Desert
            Grassland
            Grassland
            Temperate_Deciduous_Forest
            Temperate_Deciduous_Forest
            Temperate_Rain_Forest ])
      (List.map Land [ Temperate_Desert; Temperate_Desert; Shrubland; Shrubland; Taiga; Taiga ])
      (List.map Land [ Scorched; Bare; Tundra; Snow; Snow; Snow ]) ]
