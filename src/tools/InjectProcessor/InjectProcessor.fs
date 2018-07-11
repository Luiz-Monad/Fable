module InjectProcessor

open System
open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.BasicPatterns
open Fable

let typeAliases =
    Map [
        "System.Collections.Generic.IComparer`1", "comparer"
        "System.Collections.Generic.IEqualityComparer`1", "equalityComparer"
        "Array.IArrayCons`1", "arrayCons"
    ]

let parse (checker: FSharpChecker) projFile =
    let projFile = Path.GetFullPath(projFile)
    let options =
        match Path.GetExtension(projFile) with
        | ".fsx" ->
            let projCode = File.ReadAllText projFile
            checker.GetProjectOptionsFromScript(projFile, projCode)
            |> Async.RunSynchronously
            |> fst
        | ".fsproj" ->
            let opts, _, _ = Fable.CLI.ProjectCoreCracker.GetProjectOptionsFromProjectFile(projFile)
            opts
        | ext -> failwithf "Unexpected extension: %s" ext
    // for f in options.OtherOptions do
    //     printfn "%s" f
    options
    |> checker.ParseAndCheckProject
    |> Async.RunSynchronously


let (|InjectAttribute|_|) (arg: FSharpParameter) =
    arg.Attributes |> Seq.tryPick (fun att ->
        match att.AttributeType.TryFullName with
        | Some "Fable.Core.InjectAttribute" when arg.Type.HasTypeDefinition ->
            match arg.Type.TypeDefinition.TryFullName, Seq.toList arg.Type.GenericArguments with
            | Some typeArgName, [genArg] ->
                Some(typeArgName, genArg.GenericParameter.Name)
            | _ -> None
        | _ -> None)

let rec getInjects initialized decls =
    let processInfo (memb: FSharpMemberOrFunctionOrValue) (typeArgName) (genArg) =
        let genArgIndex = memb.GenericParameters |> Seq.findIndex (fun p -> p.Name = genArg)
        typeArgName, genArgIndex

    seq {
        for decl in decls do
            match decl with
            | FSharpImplementationFileDeclaration.Entity(_, sub) ->
                // If the entity contains multiple declarations it must be the root module
                if not initialized then
                    yield! getInjects (List.isMultiple sub) sub
            | FSharpImplementationFileDeclaration.InitAction _ -> ()
            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(memb, _, _) ->
                let _, injections =
                    (Seq.concat memb.CurriedParameterGroups, (false, []))
                    ||> Seq.foldBack (fun arg (finished, acc) ->
                        match finished, arg with
                        | false, InjectAttribute(typeArg, genArg) ->
                            false, (processInfo memb typeArg genArg)::acc
                        | _ -> true, acc)
                match injections with
                | [] -> ()
                | injections ->
                    let membName =
                        match memb.DeclaringEntity with
                        | Some ent when not ent.IsFSharpModule ->
                            let suffix = Fable.Transforms.OverloadSuffix.getIndex ent memb
                            Naming.buildNameWithoutSanitationFrom
                                ent.CompiledName (not memb.IsInstanceMember) memb.CompiledName suffix
                        | _ -> memb.CompiledName
                    yield membName, injections
        }

[<EntryPoint>]
let main _argv =
    printfn "Checking methods in Fable.Core.JS with last argument decorated with Inject..."
    let checker = FSharpChecker.Create(keepAssemblyContents=true)
    let proj = parse checker (IO.Path.Combine(__SOURCE_DIRECTORY__,"../../js/fable-core/Fable.Core.JS.fsproj"))
    let lines =
        seq {
            yield """/// AUTOMATICALLY GENERATED - DO NOT TOUCH!
module Fable.Transforms.ReplacementsInject

let fableCoreModules =
  Map [
    "Seq", Map [
      "maxBy", [(Types.comparer, 1)]
      "max", [(Types.comparer, 0)]
      "minBy", [(Types.comparer, 1)]
      "min", [(Types.comparer, 0)]
    ]"""
            for file in proj.AssemblyContents.ImplementationFiles do
                let fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName)
                // Apparently FCS generates the AssemblyInfo file automatically
                if fileName.Contains("AssemblyInfo") |> not then
                    let moduleInjects =
                        getInjects false file.Declarations
                        |> Seq.map (fun (membName, infos) ->
                            infos |> List.map (fun (typeArgName, genArgIndex) ->
                                let typeArgName =
                                    match Map.tryFind typeArgName typeAliases with
                                    | Some alias -> "Types." + alias
                                    | None -> "\"" + typeArgName + "\""
                                sprintf "(%s, %i)" typeArgName genArgIndex)
                            |> String.concat "; "
                            |> sprintf "      \"%s\", [%s]" membName)
                        |> Seq.toArray
                    if moduleInjects.Length > 0 then
                        yield sprintf "    \"%s\", Map [" fileName
                        yield! moduleInjects
                        yield "    ]"
            yield "  ]\n"
        }
    File.WriteAllLines(IO.Path.Combine(__SOURCE_DIRECTORY__,"../../dotnet/Fable.Compiler/Transforms/ReplacementsInject.fs"), lines)
    printfn "Finished!"
    0
