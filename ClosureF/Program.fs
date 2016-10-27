open Closure
open Argu

type Args =
    | [<Mandatory>] File of string
with
    interface IArgParserTemplate with
        member s.Usage = 
            match s with
            | File _ -> "File to process"

let parser = ArgumentParser.Create<Args>()
let usage = parser.Usage()

[<EntryPoint>]
let main argv = 
    let results = parser.Parse()
    let fileName = results.GetResults <@ File @>
    let proc = Processor()
    proc.ProcessFile(fileName.Head)
    0
