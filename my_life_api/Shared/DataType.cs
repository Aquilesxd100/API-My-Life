using System.Collections.Immutable;

namespace my_life_api.Shared;

public enum DataTypesEnum {
    Bool = 1,
    String = 2,
    Int = 3,
    Float = 4,
    File = 5,
    IntEnumerable = 6,
    StringEnumerable = 7,
    FileEnumerable = 8
}

static public class DataType {
    public class MatchedDataType {
        public DataTypesEnum fieldTypeEnum { get; set; }
        public Type type { get; set; }
        public MatchedDataType(
            DataTypesEnum _fieldTypeEnum, 
            Type _type
        ) {
            fieldTypeEnum = _fieldTypeEnum;
            type = _type;
        }
    }

    private static readonly ImmutableArray<MatchedDataType> matchedTypesArray = ImmutableArray.Create(
        new MatchedDataType(DataTypesEnum.String, typeof(string)),
        new MatchedDataType(DataTypesEnum.Bool, typeof(bool)),
        new MatchedDataType(DataTypesEnum.Int, typeof(int)),
        new MatchedDataType(DataTypesEnum.Int, typeof(int?)),
        new MatchedDataType(DataTypesEnum.Float, typeof(float)),
        new MatchedDataType(DataTypesEnum.Float, typeof(float?)),
        new MatchedDataType(DataTypesEnum.File, typeof(IFormFile)),
        new MatchedDataType(DataTypesEnum.IntEnumerable, typeof(IEnumerable<int>)),
        new MatchedDataType(DataTypesEnum.StringEnumerable, typeof(IEnumerable<string>)),
        new MatchedDataType(DataTypesEnum.FileEnumerable, typeof(IEnumerable<IFormFile>))
    );

    public static MatchedDataType? GetMatchedTypeByType(Type type) {
        return matchedTypesArray
            .FirstOrDefault(ft =>
                ft.type == type
        );
    }
}