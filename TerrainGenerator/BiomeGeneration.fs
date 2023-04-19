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
