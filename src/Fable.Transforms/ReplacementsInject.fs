/// AUTOMATICALLY GENERATED - DO NOT TOUCH!
module Fable.Transforms.ReplacementsInject

let fableReplacementsModules =
  Map [
    "Seq", Map [
      "maxBy", [(Types.comparer, 1)]
      "max", [(Types.comparer, 0)]
      "minBy", [(Types.comparer, 1)]
      "min", [(Types.comparer, 0)]
      "sumBy", [(Types.adder, 1)]
      "sum", [(Types.adder, 0)]
      "averageBy", [(Types.averager, 1)]
      "average", [(Types.averager, 0)]
    ]
    "Array", Map [
      "append", [(Types.arrayCons, 0)]
      "mapIndexed", [(Types.arrayCons, 1)]
      "map", [(Types.arrayCons, 1)]
      "mapIndexed2", [(Types.arrayCons, 2)]
      "map2", [(Types.arrayCons, 2)]
      "mapIndexed3", [(Types.arrayCons, 3)]
      "map3", [(Types.arrayCons, 3)]
      "mapFold", [(Types.arrayCons, 2)]
      "mapFoldBack", [(Types.arrayCons, 2)]
      "concat", [(Types.arrayCons, 0)]
      "collect", [(Types.arrayCons, 1)]
      "countBy", [(Types.equalityComparer, 1)]
      "distinctBy", [(Types.equalityComparer, 1)]
      "distinct", [(Types.equalityComparer, 0)]
      "contains", [(Types.equalityComparer, 0)]
      "except", [(Types.equalityComparer, 0)]
      "groupBy", [(Types.arrayCons, 0); (Types.equalityComparer, 1)]
      "singleton", [(Types.arrayCons, 0)]
      "initialize", [(Types.arrayCons, 0)]
      "replicate", [(Types.arrayCons, 0)]
      "copy", [(Types.arrayCons, 0)]
      "reverse", [(Types.arrayCons, 0)]
      "scan", [(Types.arrayCons, 1)]
      "scanBack", [(Types.arrayCons, 1)]
      "skip", [(Types.arrayCons, 0)]
      "skipWhile", [(Types.arrayCons, 0)]
      "take", [(Types.arrayCons, 0)]
      "takeWhile", [(Types.arrayCons, 0)]
      "partition", [(Types.arrayCons, 0)]
      "choose", [(Types.arrayCons, 1)]
      "sortInPlaceBy", [(Types.comparer, 1)]
      "sortInPlace", [(Types.comparer, 0)]
      "sort", [(Types.comparer, 0)]
      "sortBy", [(Types.comparer, 1)]
      "sortDescending", [(Types.comparer, 0)]
      "sortByDescending", [(Types.comparer, 1)]
      "sum", [("Fable.Core.IGenericAdder`1", 0)]
      "sumBy", [("Fable.Core.IGenericAdder`1", 1)]
      "maxBy", [(Types.comparer, 1)]
      "max", [(Types.comparer, 0)]
      "minBy", [(Types.comparer, 1)]
      "min", [(Types.comparer, 0)]
      "average", [("Fable.Core.IGenericAverager`1", 0)]
      "averageBy", [("Fable.Core.IGenericAverager`1", 1)]
      "ofSeq", [(Types.arrayCons, 0)]
      "ofList", [(Types.arrayCons, 0)]
      "transpose", [(Types.arrayCons, 0)]
    ]
    "List", Map [
      "contains", [(Types.equalityComparer, 0)]
      "except", [(Types.equalityComparer, 0)]
      "sort", [(Types.comparer, 0)]
      "sortBy", [(Types.comparer, 1)]
      "sortDescending", [(Types.comparer, 0)]
      "sortByDescending", [(Types.comparer, 1)]
      "sum", [("Fable.Core.IGenericAdder`1", 0)]
      "sumBy", [("Fable.Core.IGenericAdder`1", 1)]
      "maxBy", [(Types.comparer, 1)]
      "max", [(Types.comparer, 0)]
      "minBy", [(Types.comparer, 1)]
      "min", [(Types.comparer, 0)]
      "average", [("Fable.Core.IGenericAverager`1", 0)]
      "averageBy", [("Fable.Core.IGenericAverager`1", 1)]
      "distinctBy", [(Types.equalityComparer, 1)]
      "distinct", [(Types.equalityComparer, 0)]
      "groupBy", [(Types.equalityComparer, 1)]
      "countBy", [(Types.equalityComparer, 1)]
    ]
    "Set", Map [
      "FSharpSet$$Map$$7597B8F7", [(Types.comparer, 1)]
      "singleton", [(Types.comparer, 0)]
      "unionMany", [(Types.comparer, 0)]
      "empty", [(Types.comparer, 0)]
      "map", [(Types.comparer, 1)]
      "ofList", [(Types.comparer, 0)]
      "ofArray", [(Types.comparer, 0)]
      "toArray", [(Types.arrayCons, 0)]
      "ofSeq", [(Types.comparer, 0)]
      "createMutable", [(Types.equalityComparer, 0)]
      "distinct", [(Types.equalityComparer, 0)]
      "distinctBy", [(Types.equalityComparer, 1)]
      "intersectWith", [(Types.comparer, 0)]
      "isSubsetOf", [(Types.comparer, 0)]
      "isSupersetOf", [(Types.comparer, 0)]
      "isProperSubsetOf", [(Types.comparer, 0)]
      "isProperSupersetOf", [(Types.comparer, 0)]
    ]
    "Map", Map [
      "ofList", [(Types.comparer, 0)]
      "ofSeq", [(Types.comparer, 0)]
      "ofArray", [(Types.comparer, 0)]
      "empty", [(Types.comparer, 0)]
      "createMutable", [(Types.equalityComparer, 0)]
      "groupBy", [(Types.equalityComparer, 1)]
      "countBy", [(Types.equalityComparer, 1)]
    ]
  ]

