module TerrainGenerator.SimplexNoise

let generateMap width height scale seed normalizeFunc =
    SimplexNoise.Noise.Seed <- seed
    SimplexNoise.Noise.Calc2D(width, height, scale)
    |> Array2D.map float
    |> normalizeFunc

let normalizeByMinMax min max map =
    Array2D.map (fun value -> (value - min) / (max - min)) map
    
let normalizeByZeroAndTwoFiftyFive map =
    normalizeByMinMax 0. 255. map