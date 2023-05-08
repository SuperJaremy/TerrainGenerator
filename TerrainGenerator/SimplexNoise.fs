module TerrainGenerator.SimplexNoise

let generateMap width height scale seed normalizeFunc =
    SimplexNoise.Noise.Seed <- seed

    SimplexNoise.Noise.Calc2D(width, height, scale)
    |> Array2D.map float
    |> normalizeFunc

let normalizeByZeroAndTwoFiftyFive map = Util.normalizeByMinMax 0. 255. map
