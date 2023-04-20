module TerrainGenerator.BiomeGeneration


let generateBiomes landWaterMap (map: float[,]) =
    Array2D.mapi
        (fun x y value ->
            match value with
            | Tiles.TerrainTile.Water -> value
            | Tiles.TerrainTile.Land _ ->
                let biomeNum = map.[x, y]
                Tiles.TerrainTile.Land(Tiles.chooseTile biomeNum))
        landWaterMap

let toTableIndexMapper mapTable isRow toMap =
    let columns = (List.length mapTable)
    let rows = (List.length mapTable.[0])
    if isRow then
        toMap * (rows |> float) |> floor |> int |> min (rows - 1)
    else
        toMap * (columns |> float) |> floor |> int |> min (columns - 1)

let generateBiomesFromTable (elevationMap: float[,]) (moistureMap: float[,]) (mapTable: Tiles.TerrainTile list list) =
    let indexer = toTableIndexMapper mapTable
    let elevaTrans = Array2D.map (indexer false) elevationMap
    let moistureTrans = Array2D.map (indexer true) moistureMap
    Array2D.mapi (fun x y value ->
        let elevation = value
        let moisture = moistureTrans.[x,y]
        mapTable.[elevation].[moisture]) elevaTrans
    