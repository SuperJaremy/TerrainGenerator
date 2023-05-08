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

let generateBiomesFrom2DTable (elevationMap: float[,]) (moistureMap: float[,]) indexMapper (mapTable: Tiles.TerrainTile list list) =
    let indexer = indexMapper mapTable
    let elevaTrans = Array2D.map (indexer false) elevationMap
    let moistureTrans = Array2D.map (indexer true) moistureMap
    Array2D.mapi (fun x y value ->
        let elevation = value
        let moisture = moistureTrans.[x,y]
        mapTable.[elevation].[moisture]) elevaTrans
    