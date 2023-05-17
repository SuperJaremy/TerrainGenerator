module TerrainGenerator.LandWaterSplice

let landWater (ratio: float) factorCalculator map =
    let factor = factorCalculator ratio map

    map
    |> Array2D.map (fun num ->
        if num < factor then
            Tiles.TerrainTile.Water Tiles.WaterTile.Water
        else
            Tiles.TerrainTile.Land Tiles.LandTile.Grassland)

let calcFactorMean (ratio: float) map =
    let mean = map |> Seq.cast |> Seq.average
    mean * ratio

let calcFactorMed (ratio: float) map =
    let seq1 = map |> Seq.cast
    let sorted = Seq.sort<float> seq1
    let med = Seq.item ((Seq.length sorted) / 2) sorted |> float
    med * ratio
