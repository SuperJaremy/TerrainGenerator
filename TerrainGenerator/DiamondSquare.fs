module TerrainGenerator.DiamondSquare

open System
open Microsoft.FSharp.Collections

let private calculateEdges x y side =
    let side = (side - 1) / 2
    x - side, x + side, y - side, y + side

let private squareStep centreCords sideSize (rng: Random) variance map =
    let squareStepDot (map: float[,]) centerCord =
        let x, y = centerCord
        let rand = rng.NextDouble() * variance - variance / 2.
        let leftX, rightX, upY, downY = calculateEdges x y sideSize

        let value =
            (map.[leftX, upY] + map.[rightX, upY] + map.[leftX, downY] + map.[rightX, downY])
            / 4.
            + rand

        Array2D.set map x y value
        map

    List.fold squareStepDot map centreCords

let private diamondStep diamondCords sideSize (rng: Random) variance map =
    let diamondStepDot (map: float[,]) diamondCord =
        let rand = rng.NextDouble() * variance - variance / 2.
        let x, y = diamondCord
        let leftX, rightX, upY, downY = calculateEdges x y sideSize
        let sum, count = 0.0, 0

        let sum, count =
            if leftX > 0 then
                sum + map.[leftX, y], count + 1
            else
                sum, count

        let sum, count =
            if upY > 0 then
                sum + map.[x, upY], count + 1
            else
                sum, count

        let sum, count =
            if rightX < sideSize then
                sum + map.[rightX, y], count + 1
            else
                sum, count

        let sum, count =
            if downY < sideSize then
                sum + map.[x, downY], count + 1
            else
                sum, count

        let value = sum / (count |> float) + rand
        Array2D.set map x y value
        map

    List.fold diamondStepDot map diamondCords

let rec private proc map centreCords sideSize rng variance =
    if sideSize = 1 then
        map
    else
        let diamondCords =
            centreCords
            |> List.collect (fun (x, y) ->
                let leftX, rightX, upY, downY = calculateEdges x y sideSize
                [ (leftX, y); (x, upY); (rightX, y); (x, downY) ])
            |> List.distinct

        let map =
            map
            |> squareStep centreCords sideSize rng variance
            |> diamondStep diamondCords sideSize rng variance

        let centreCords =
            centreCords
            |> List.collect (fun (x, y) ->
                let leftX, rightX, upY, downY = calculateEdges x y ((sideSize + 1) / 2)
                [ (leftX, upY); (rightX, upY); (leftX, downY); (rightX, downY) ])
            |> List.distinct

        proc map centreCords ((sideSize + 1) / 2) rng (variance / 2.)

let normalizeByAbs map =
    let seq1 = map |> Seq.cast<float> |> Seq.map Math.Abs
    let seqMin, seqMax = Seq.min seq1, Seq.max seq1
    let seqDiff = seqMax - seqMin
    Array2D.map (fun (value: float) -> (Math.Abs(value) - seqMin) / seqDiff) map

let normalizeBySlide map =
    let seq1 = map |> Seq.cast<float>
    let seqMin, seqMax = Seq.min seq1, Seq.max seq1
    let seqDiff = seqMax - seqMin
    Array2D.map (fun (value: float) -> (value - seqMin) / seqDiff) map

let generateMap mapSideN leftUp leftDown rightUp rightDown variance seed normalizationFunc =
    let sideSize = (pown 2 mapSideN) + 1
    let map = Array2D.create sideSize sideSize 0.0
    map.[0, 0] <- leftUp
    map.[0, (sideSize - 1)] <- rightUp
    map.[(sideSize - 1), 0] <- leftDown
    map.[(sideSize - 1), (sideSize - 1)] <- rightDown
    let rng = Random(seed)
    let centerX = ((sideSize - 1) / 2)
    let centerCords = [ (centerX, centerX) ]
    let map = proc map centerCords sideSize rng variance
    normalizationFunc map
