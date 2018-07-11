[<RequireQualifiedAccess>]
module Fable.Transforms.OverloadSuffix

open System.Collections.Generic
open Microsoft.FSharp.Compiler.SourceCodeServices

// TODO: These two first functions are duplicateds from FSharp2Fable.Util
let rec private nonAbbreviatedType (t: FSharpType) =
    if t.IsAbbreviation then nonAbbreviatedType t.AbbreviatedType else t

let private isUnit (typ: FSharpType) =
    let typ = nonAbbreviatedType typ
    if typ.HasTypeDefinition
    then typ.TypeDefinition.TryFullName = Some "Microsoft.FSharp.Core.Unit"
    else false

// Attention: we need to keep this similar to FSharp2Fable.TypeHelpers.makeType
let rec private getTypeFastFullName (t: FSharpType) =
    let t = nonAbbreviatedType t
    if t.IsGenericParameter
    then t.GenericParameter.Name
    elif t.IsTupleType
    then t.GenericArguments |> Seq.map getTypeFastFullName |> String.concat " * "
    elif t.IsFunctionType
    then t.GenericArguments |> Seq.map getTypeFastFullName |> String.concat " -> "
    elif t.HasTypeDefinition
    then
        let tdef = t.TypeDefinition
        let genArgs = t.GenericArguments |> Seq.map getTypeFastFullName |> String.concat ","
        match tdef.IsArrayType, genArgs with
        | true, _ -> genArgs + "[]"
        | false, "" -> t.TypeDefinition.FullName
        | false, genArgs -> t.TypeDefinition.FullName + "[" + genArgs + "]"
    else "System.Object"

// From https://stackoverflow.com/a/37449594
let private combineHashCodes (hashes: int seq) =
    let hashes = Seq.toArray hashes
    if hashes.Length = 0
    then 0
    else hashes |> Array.reduce (fun h1 h2 -> ((h1 <<< 5) + h1) ^^^ h2)

// F# hash function gives different results in different runs
// Taken from fable-core/Util.ts. Possible variant in https://stackoverflow.com/a/1660613
let stringHash (s: string) =
    let mutable h = 5381
    for i = 0 to s.Length - 1 do
        h <- (h * 33) ^^^ (int s.[i])
    h

let private hashToString (i: int) =
    if i < 0
    then "Z" + (abs i).ToString("X")
    else i.ToString("X")

let getHash (m: FSharpMemberOrFunctionOrValue) =
    let curriedParams = m.CurriedParameterGroups
        // Overrides and interface implementations don't have override suffix in Fable
    if m.IsOverrideOrExplicitInterfaceImplementation
        // Members with curried params cannot be overloaded in F#
        || curriedParams.Count <> 1
        // Don't use overload suffix for members without arguments
        || curriedParams.[0].Count = 0
        || (curriedParams.[0].Count = 1 && isUnit curriedParams.[0].[0].Type)
    then ""
    else
        curriedParams.[0]
        |> Seq.map (fun p -> getTypeFastFullName p.Type |> stringHash)
        |> combineHashCodes
        |> hashToString

/// Simple overload resolution enumerating overloads within an entity (used for fable-core F# types)
let getIndex (entity: FSharpEntity) (m: FSharpMemberOrFunctionOrValue) =
    let argsEqual (args1: IList<FSharpParameter>) (args2: IList<FSharpParameter>) =
        if args1.Count = args2.Count
        // We're using the overload index mainly for types in fable-core to replace BCL classes,
        // just checking the param name should be enough to disambiguate interfaces
        then (args1, args2) ||> Seq.forall2 (fun a1 a2 -> a1.Name = a2.Name)
        else false
    // Check that m.CurriedParameterGroups.Count <= 1 before using this
    let getOverloadableParams (m: FSharpMemberOrFunctionOrValue): IList<_> =
        if m.CurriedParameterGroups.Count = 0
        then upcast [||]
        else m.CurriedParameterGroups.[0]
    // Overrides and interface implementations don't have override suffix in Fable
    // Members with curried params cannot be overloaded in F#
    if m.IsOverrideOrExplicitInterfaceImplementation || m.CurriedParameterGroups.Count > 1
    then ""
    else
        // m.Overloads(false) doesn't work
        let name = m.CompiledName
        let isInstance = m.IsInstanceMember
        let params1 = getOverloadableParams m
        let index, _found =
            ((0, false), entity.MembersFunctionsAndValues)
            ||> Seq.fold (fun (i, found) m2 ->
                if not found && m2.IsInstanceMember = isInstance && m2.CompiledName = name && m2.CurriedParameterGroups.Count <= 1 then
                    // .Equals() doesn't work.
                    // .IsEffectivelySameAs() doesn't work for constructors
                    if argsEqual params1 (getOverloadableParams m2)
                    then i, true
                    else i + 1, false
                else i, found)
        // TODO: Log error if not found?
        if index = 0 then "" else string index
