module TerrainGenerator.Util

open System

/// <summary>
/// Shuffles list using given seed value
/// </summary>
/// <param name="list">List to shuffle.</param>
/// <param name="seed">Seed value used in shuffling</param>
let shuffleList seed list =
    let rng = Random(seed)
    let size = List.length list
    List.sortBy (fun _ -> rng.Next(1, size)) list

/// <summary>
/// Generates square Array2D of 1s and 0s
/// </summary>
/// <param name="ratio">0s to 1s ratio</param>
/// <param name="sideSize">Side length of generated Array2D</param>
/// <param name="seed">Seed used to shuffle values in generated Array2D</param>
let generateDualWhiteNoise ratio sideSize seed =
    let size = pown sideSize 2
    let zeroes = floor (ratio * (size |> float)) |> int

    let values =
        List.init size (fun x -> if x < zeroes then 0 else 1) |> shuffleList seed

    Array2D.init sideSize sideSize (fun x y -> List.item (x * sideSize + y) values)

/// <summary>
/// Generates square Array2D of random values in range [0,1)
/// </summary>
/// <param name="sideSize">Side length of generated Array2D</param>
/// <param name="seed">Seed used to generate random values in generated Array2D</param>
let generateWhiteNoise sideSize seed =
    let rng = Random(seed)
    let size = pown sideSize 2
    let values = List.init size (fun _ -> rng.NextDouble())

    Array2D.init sideSize sideSize (fun x y -> List.item (x * sideSize + y) values)

/// <summary>
/// Helper function to normalize Array2D values. Do not use directly
/// </summary>
/// <param name="min">Leftmost point of segment</param>
/// <param name="max">Rightmost point of segment</param>
/// <param name="map">Array2D which values to normalize</param>
let normalizeByMinMax min max map =
    Array2D.map (fun value -> (value - min) / (max - min)) map

/// <summary>
/// Normalize Array2D to [0,1] segment
/// </summary>
/// <param name="map">Array2D which values to normalize</param>
let normalizeBySlide map =
    let seq1 = map |> Seq.cast<float>
    let seqMin, seqMax = Seq.min seq1, Seq.max seq1
    normalizeByMinMax seqMin seqMax map

/// <summary>
/// Normalize Array2D to [0,1] segment using absolute values
/// </summary>
/// <param name="map">Array2D which values to normalize</param>
let normalizeByAbs map =
    Array2D.map (fun (value: float) -> Math.Abs(value)) map |> normalizeBySlide

/// <summary>
/// Blank normalization function
/// </summary>
/// <param name="map">Array2D which values to normalize</param>
let doNotNormalize map = map

/// <summary>
/// Transform value in range [0,1] to row/line index in table
/// </summary>
/// <param name="mapTable">Table to which indexes to transform</param>
/// <param name="isRow">Transform to row/line index</param>
/// <param name="toMap">Value to transform</param>
let to2DTableIndexMapper mapTable isRow toMap =
    let columns = (List.length mapTable)
    let rows = (List.length mapTable.[0])

    if isRow then
        toMap * (rows |> float) |> floor |> int |> min (rows - 1)
    else
        toMap * (columns |> float) |> floor |> int |> min (columns - 1)

/// <summary>
/// Applies a function to each element of the collection, threading an accumulator argument through the computation.
/// The integer indexes passed to the function indicates the indexes of element being applied, starting at zero.
/// </summary>
/// <param name="folder">The function to update the state given the input elements.</param>
/// <param name="state">The initial state.</param>
/// <param name="array">The input array2d.</param>
/// <returns>The final state value.</returns>
let foldiArray2D (folder: int -> int -> 'S -> 'T -> 'S) (state: 'S) (array: 'T[,]) =
    seq {
        for x in 0 .. Array2D.length1 array - 1 do
            for y in 0 .. Array2D.length2 array - 1 do
                yield (x, y, array.[x, y])
    }
    |> Seq.fold (fun acc (x, y, e) -> folder x y acc e) state
