module TerrainGenerator.SimplexNoise

let generateMap width height scale seed =
    SimplexNoise.Noise.Seed <- seed
    Array2D.map (fun value -> (value |> float) / 255.0) (SimplexNoise.Noise.Calc2D(width, height, scale))
