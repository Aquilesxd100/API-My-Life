using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Immutable;
using MySql.Data.MySqlClient;
using my_life_api.Shared.ContentResources;
using my_life_api.Models;
using my_life_api.Shared;
using my_life_api.Models.Content;

namespace my_life_api.Database.Managers;

public class ColumnData {
    public string name { get; set; }
    public string value { get; set; }

    public ColumnData(string name, string value){
        this.name = name;
        this.value = value;
    }
}

public class ContentDBManager {
    public async Task<IEnumerable<ContentDTO>> GetItems(
        ContentTypeData contentTypeData,
        ContentFilters filters
    ) {
        List<ContentDTO> items = new List<ContentDTO>();

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        string tableName = contentTypeData.dbTableName;

        string querySelectLine = 
            "Select " +
                String.Join(", ", contentTypeData.GetDbColumnsNames(
                    tableName + "."
                )) +
                ", Authors.id As author_id, " + 
                "Authors.name As author_name, " +
                "Authors.imageUrl As author_imageUrl " +
            $"From {tableName} ";

        myCommand.CommandText = querySelectLine +
            "Left Join Authors On " +
                $"{tableName}.authorId = Authors.id " +
            MountConditionalQueryPartByFilters(
                contentTypeData,
                filters
            ) + 
            ";";

        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            ContentDTO item = await GetItemDataFromReaderByContentType(
                contentTypeData,
                myReader,
                true
            );
            items.Add(item);
        }

        await DataBase.CloseConnection();

        foreach (ContentDTO item in items) {
            IEnumerable<CategoryDTO> categories = await GetCategoriesByContentId(
                contentTypeData.contentType, 
                (int)item.id,
                filters.categoriesIds
            );

            item.categorias = categories;
        }

        return items;
    }

    public async Task<object> GetItemByIdAndTypeData(
        ContentTypeData contentTypeData,
        int itemId,
        bool shouldGetCategories = false
    ) {
        object item = null;

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        string tableName = contentTypeData.dbTableName;

        string querySelectLine = 
            "Select " +
                String.Join(", ", contentTypeData.GetDbColumnsNames(
                    tableName + "."
                )) +
                ", Authors.id As author_id, " + 
                "Authors.name As author_name, " +
                "Authors.imageUrl As author_imageUrl ";

        myCommand.CommandText = querySelectLine +
            $"From {tableName} " +
            "Inner Join Authors On " +
                $"{tableName}.authorId = Authors.id " +
            $"Where {tableName}.id = {itemId};";

        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            item = await GetItemDataFromReaderByContentType(
                contentTypeData,
                myReader,
                shouldGetCategories
            );
        }

        await DataBase.CloseConnection();

        return item;
    }

    public async Task<int> CreateItem<T>(
        T item,
        string tableName
    ) {
        IEnumerable<ColumnData> columnsData = GetItemColumnsData(item);     

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText = 
            $"Insert Into {tableName} " +
                $"({String.Join(',', columnsData.Select(cd => cd.name))})" +
                "Values " +
                    $"({String.Join(',', columnsData.Select(cd => $"'{cd.value}'"))});" +
            "Select Last_Insert_Id();";

        var result = await myCommand.ExecuteScalarAsync();
        int itemId = int.Parse(result.ToString());

        await DataBase.CloseConnection();

        return itemId;
    }

    public async Task CreateItemCategoryRelations(
        ContentTypeData contentTypeData,
        int itemId,
        IEnumerable<int> categoriesIdsToAdd
    ) {
        string contentColumnIdName = $"{contentTypeData.name}Id";

        string valuesToAddQueryPart = "";
        foreach (int categoryId in categoriesIdsToAdd) {
            if (valuesToAddQueryPart.Length > 0)
                valuesToAddQueryPart += ", ";

            valuesToAddQueryPart += $"({itemId}, {categoryId})";
        }
        valuesToAddQueryPart += ";";

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText =
            $"Insert Into {contentTypeData.GetRelationTableName()}" +
                $"({contentColumnIdName}, categoryId)" +
                "Values" +
                    valuesToAddQueryPart;

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task UpdateItem<T>(
        int itemId,
        T item,
        string tableName
    ) {
        IEnumerable<ColumnData> columnsData = GetItemColumnsData(item, true);

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText = 
            $"Update {tableName} " +
                "Set " +
                    String.Join(
                        ',', 
                        columnsData.Select(cd => 
                            $"{cd.name} = '{cd.value}'"
                        )
                    ) +
            $" Where id = {itemId};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task UpdateItemCategoryRelations(
        ContentTypeData contentTypeData,
        int contentId,
        IEnumerable<int> idsCategorias
    ) {
            await DeleteItemCategoryRelations(
                contentTypeData,
                contentId
            );

            if (idsCategorias.Count() > 0) {
                await CreateItemCategoryRelations(
                    contentTypeData,
                    contentId,
                    idsCategorias
                );
            }
    }

    public async Task UpdateItemImageUrlByIdAndTableName(
        int contentId,
        string tableName,
        string? imageUrl
    ) {
        await DataBase.OpenConnectionIfClosed();

        string newImageUrl = String.IsNullOrEmpty(imageUrl) ? "NULL" : $"'{imageUrl}'";

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            $"Update {tableName} " +
                "Set " +
                    $"imageUrl = {newImageUrl} " +
                $"Where id = {contentId};";

        await myCommand.ExecuteReaderAsync();        

        await DataBase.CloseConnection();
    }

    public async Task DeleteItemByIdAndTableName(
        int itemId,
        string tableName
    ) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            $"Delete From {tableName} " +
                $"Where id = {itemId};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task DeleteItemCategoryRelations(
        ContentTypeData contentTypeData,
        int contentId
    ) {
        string contentColumnIdName = $"{contentTypeData.name}Id";

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText =
            $"Delete From {contentTypeData.GetRelationTableName()} " +
                $"Where {contentColumnIdName} = {contentId};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    private async Task<ContentDTO> GetItemDataFromReaderByContentType(
       ContentTypeData contentTypeData,
       DbDataReader reader,
       bool shouldGetCategories = false
    ) {
        // Cria uma instancia da classe do DTO para retorno
        object item = Activator.CreateInstance(contentTypeData.dtoType);

        PropertyInfo[] dtoProperties = contentTypeData.dtoType.GetProperties();
        PropertyInfo[] entityProperties = contentTypeData.entityType.GetProperties();
        
        for (int idx = 0; idx < entityProperties.Length; idx++) {
            PropertyInfo dtoProperty = dtoProperties[idx];
            PropertyInfo entityProperty = entityProperties[idx];

            // Preenche o autor manualmente
            if (dtoProperty.Name == "autor") { 
                dtoProperty.SetValue(
                    item, 
                    new AuthorDTO() {
                        id = reader.GetInt32("author_id"),
                        nome = reader.GetString("author_name"),
                        urlImagem = 
                            reader.IsDBNull("author_imageUrl") 
                                ? null 
                                : reader.GetString("author_imageUrl")
                    }
                );
                continue;
            }

            // Quando chega em categorias as preenche manualmente, entao,
            // para de preencher os dados do item
            if (dtoProperty.Name == "categorias") { 
                int itemId = reader.GetInt32("id");

                if (shouldGetCategories) {
                    await DataBase.CloseConnection();
                }

                IEnumerable<CategoryDTO> categories = 
                    shouldGetCategories
                    ? 
                        await this.GetCategoriesByContentId(
                            contentTypeData.contentType,
                            itemId
                        )
                    : ImmutableArray.Create<CategoryDTO>();

                dtoProperty.SetValue(
                    item, 
                    categories
                );

                await DataBase.OpenConnectionIfClosed();
                break;
            }

            object valueToSet = null;
            DataTypesEnum? dataType = 
                DataType.GetMatchedTypeByType(dtoProperty.PropertyType)?.fieldTypeEnum;

            bool isReceivedValueNull = reader.IsDBNull(entityProperty.Name);
            if (!isReceivedValueNull) {
                switch (dataType) {
                    case (DataTypesEnum.Int):
                        valueToSet = reader.GetInt32(entityProperty.Name);
                    break;
                    case (DataTypesEnum.Bool):
                        valueToSet = reader.GetBoolean(entityProperty.Name);
                    break;
                    case (DataTypesEnum.Float):
                        valueToSet = reader.GetFloat(entityProperty.Name);
                    break;
                    default:
                    case (DataTypesEnum.String):
                        valueToSet = reader.GetString(entityProperty.Name);
                    break;
                }
            }

            dtoProperty.SetValue(
                item, 
                valueToSet
            );
        }

        return (ContentDTO)item;
    }

    private IEnumerable<ColumnData> GetItemColumnsData<T>(T item, bool isUpdate = false) {
        IEnumerable<string> ignoredFieldNamesOnUpdate = new List<string>() {
            "id", "authorId"
        };

        List<ColumnData> columnsData = new List<ColumnData>();
        PropertyInfo[] itemProperties = item.GetType().GetProperties();
        
        for (int idx = 0; idx < itemProperties.Length; idx++) {
            PropertyInfo entityProperty = itemProperties[idx];

            if (
                isUpdate 
                && ignoredFieldNamesOnUpdate.Any(fn => fn == entityProperty.Name)
            ) {
                continue;
            }

            object? propertyValue = entityProperty.GetValue(item);

            if (propertyValue != null) {
                string treatedValue = $"{propertyValue}";

                // Caso seja float troca virgula por ponto,
                // evitando problemas de arredondamento
                if (
                    treatedValue.Contains(',') 
                    && float.TryParse(treatedValue, out _)
                ) {
                    treatedValue = treatedValue.Replace(',', '.');
                }

                columnsData.Add(
                    new ColumnData(entityProperty.Name, treatedValue)
                );
            }
        }

        return columnsData;
    }

    protected async Task<IEnumerable<CategoryDTO>> GetCategoriesByContentId(
        ContentTypesEnum categoryType,
        int contentId,
        IEnumerable<int>? categoriesIdsToFilter = null
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(categoryType);

        await DataBase.OpenConnectionIfClosed();

        string relationTableName = contentTypeData.GetRelationTableName();

        string contentIdFieldName = contentTypeData.name + "Id";

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText = 
            @"Select " + 
                "Categories.id As category_id, Categories.name As category_name, " + 
                "iconBase64, contentTypeId " +
            "From Categories " +
            $"Inner Join {relationTableName} " +
                $"On {relationTableName}.{contentIdFieldName} = {contentId} " +
            $"Where {relationTableName}.categoryId = Categories.id;";

        List<CategoryDTO> categories = new List<CategoryDTO>();
        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            CategoryDTO categoryToAdd = new CategoryDTO()
            {
                id = myReader.GetInt32("category_id"),
                nome = myReader.GetString("category_name"),
                iconeBase64 = 
                    myReader.IsDBNull("iconBase64") 
                        ? null 
                        : myReader.GetString("iconBase64"),
                idTipoConteudo = (ContentTypesEnum)myReader.GetInt32("contentTypeId"),
            };
            categories.Add(categoryToAdd);
        }

        await DataBase.CloseConnection();

        return categories;
    }

    protected string MountConditionalQueryPartByFilters(
        ContentTypeData contentTypeData,
        ContentFilters filters
    ) {
        string conditionalQueryPart = "";
        List<string> conditionals = new List<string>();

        if (filters.authorId != null)
            conditionals.Add($"authorId = {filters.authorId}");

        if (filters.soulFragment != null) {
            int soulFragmentInByte = filters.soulFragment == true ? 1 : 0;
            conditionals.Add($"soulFragment = {soulFragmentInByte}");
        }

        if (filters.dubbed != null) {
            int dubbedInByte = filters.dubbed == true ? 1 : 0;
            conditionals.Add($"dubbed = {dubbedInByte}");
        }

        if (filters.finished != null) {
            int finishedInByte = filters.finished == true ? 1 : 0;
            conditionals.Add($"finished = {finishedInByte}");
        }

        if (filters.ratingGreaterEqualTo != null)
            conditionals.Add($"rating >= '{filters.ratingGreaterEqualTo}'");

        if (filters.ratingLesserEqualTo != null)
            conditionals.Add($"rating <= '{filters.ratingLesserEqualTo}'");

        if (!string.IsNullOrEmpty(filters.search)) {
            IEnumerable<string> fieldsToFilter = GetStringDbColumnsNamesByEntity(
                contentTypeData.entityType,
                $"{contentTypeData.dbTableName}."
            // Ignora campos de url ao filtrar
            ).Where(name => !name.Contains("url", StringComparison.OrdinalIgnoreCase))
            // Garante que o filtro ainda sera aplicado mesmo com o valor do campo nulo
            .Select(name => $"IfNull({name}, '')");

            conditionals.Add(
                $"concat({String.Join(",", fieldsToFilter)}) Like '%{filters.search}%'"
            );
        }

        if (conditionals.Count > 0) {
            conditionalQueryPart = "Where " + String.Join(" And ", conditionals);
        }

        return conditionalQueryPart;
    }

    private IEnumerable<string> GetStringDbColumnsNamesByEntity(Type entity, string prefix) { 
        PropertyInfo[] properties = entity.GetProperties();
        List<string> dbColumnNames = properties
            .Where(prop =>
                DataType.GetMatchedTypeByType(prop.PropertyType)
                    ?.fieldTypeEnum == DataTypesEnum.String
            )
            .Select(prop => 
                prefix + prop.Name
            ).ToList();

        // Nome de Autor fixo para filtragem
        dbColumnNames.Add("Authors.name");

        return dbColumnNames;
    }
}
