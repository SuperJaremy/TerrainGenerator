module TerrainGenerator.SimplexNoise

let generateMap width height scale seed normalizeFunc =
    SimplexNoise.Noise.Seed <- seed

    SimplexNoise.Noise.Calc2D(width, height, scale)
    |> Array2D.map float
    |> normalizeFunc

let normalizeByMinMax min max map =
    Array2D.map (fun value -> (value - min) / (max - min)) map

let normalizeByZeroAndTwoFiftyFive map = normalizeByMinMax 0. 255. map

let normalizeBySlide map =
    let seq1 = map |> Seq.cast<float>
    let seqMin, seqMax = Seq.min seq1, Seq.max seq1
    normalizeByMinMax seqMin seqMax map
