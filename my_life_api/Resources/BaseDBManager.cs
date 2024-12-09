using System.Data;
using MySql.Data.MySqlClient;
using my_life_api.Database;
using my_life_api.Models;

namespace my_life_api.Resources
{
    public abstract class BaseDBManager
    {
        public string GetTreatedRatingValue(float rating)
        {
            float treatedFloatValue = MathF.Floor(rating * 100) / 100;

            return $"'{treatedFloatValue.ToString("F2").Replace(',', '.')}'";
        }

        public string MountConditionalQueryPartByFilters(
            ContentFilters filters, 
            string? categoryRelationTableName = "",
            string? contentTableName = ""
        ) {
            string conditionalQueryPart = "";
            List<string> conditionals = new List<string>();

            if (filters.authorId != null)
                conditionals.Add($"authorId = {filters.authorId}");

            if (
                filters.categoriesIds.Count() > 0
                && !string.IsNullOrEmpty(categoryRelationTableName)
            ) {
                string formatedCategoriesIds = String.Join(", ", filters.categoriesIds);
                conditionals.Add($"{categoryRelationTableName}.categoryId In ({formatedCategoriesIds})");
            }

            if (filters.soulFragment != null) {
                int soulFragmentInByte = filters.soulFragment == true ? 1 : 0;
                conditionals.Add($"soulFragment = {soulFragmentInByte}");
            }

            if (filters.dubbed != null) {
                int dubbedInByte = filters.dubbed == true ? 1 : 0;
                conditionals.Add($"dubbed = {dubbedInByte}");
            }

            if (filters.finished != null) {
                int completedInByte = filters.finished == true ? 1 : 0;
                conditionals.Add($"completed = {completedInByte}");
            }

            if (filters.ratingGreaterEqualTo != null)
                conditionals.Add($"rating >= '{filters.ratingGreaterEqualTo}'");

            if (filters.ratingLesserEqualTo != null)
                conditionals.Add($"rating <= '{filters.ratingLesserEqualTo}'");

            if (!string.IsNullOrEmpty(filters.name)) {
                string nameColumn = string.IsNullOrEmpty(contentTableName)
                    ? "name"
                    : $"{contentTableName}.name";

                conditionals.Add($"{nameColumn} Like '%{filters.name}%'");
            }

            if (conditionals.Count > 0) {
                conditionalQueryPart = "Where " + String.Join(" And ", conditionals);
            }

            return conditionalQueryPart;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByContentId(
            int contentId,
            ContentTypesEnum categoryType
        ) {
            await DataBase.OpenConnectionIfClosed();

            string relationTableName = GetContentNameByContentType(categoryType) + "_x_Category";
            string contentIdFieldName = GetContentNameByContentType(categoryType).ToLower() + "Id";

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

            while (myReader.Read())
            {
                CategoryDTO categoryToAdd = new CategoryDTO()
                {
                    id = myReader.GetInt32("category_id"),
                    nome = myReader.GetString("category_name"),
                    iconeBase64 = myReader.IsDBNull("iconBase64") ? null : myReader.GetString("iconBase64"),
                    idTipoConteudo = (ContentTypesEnum)myReader.GetInt32("contentTypeId"),
                };
                categories.Add(categoryToAdd);
            }

            await DataBase.CloseConnection();

            return categories;
        }

        public string GetTableNameByContentType(ContentTypesEnum contentType)
        {
            string tableName = "";

            switch(contentType) {
                case ContentTypesEnum.Animes:
                    tableName = "Animes";
                break;
                case ContentTypesEnum.Mangas:
                    tableName = "Mangas";
                break;
                case ContentTypesEnum.Seriado:
                    tableName = "Series";
                break;
                case ContentTypesEnum.Livros:
                    tableName = "Books";
                break;
                case ContentTypesEnum.Jogos:
                    tableName = "Games";
                break;
                case ContentTypesEnum.Cinema:
                    tableName = "Movies";
                break;
                case ContentTypesEnum.Musical:
                    tableName = "Musics";
                break;
                case ContentTypesEnum.Frases:
                    tableName = "Phrases";
                break;
            }

            return tableName;
        }
        public string GetContentNameByContentType(ContentTypesEnum contentType)
        {
            string contentName = GetTableNameByContentType(contentType);

            // Caso nao seja serie remove o 'S' do final para indicar singular
            if (contentType != ContentTypesEnum.Seriado) {
                contentName = contentName.Remove(contentName.Length - 1);
            }

            return contentName;
        }

    }
}
