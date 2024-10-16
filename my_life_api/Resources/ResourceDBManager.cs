using System.Data;
using MySql.Data.MySqlClient;
using my_life_api.Database;
using my_life_api.Models;
using my_life_api.Models.Requests;

namespace my_life_api.Resources
{
    public abstract class ResourceDBManager
    {
        public string GetTreatedRatingValue(float rating)
        {
            float treatedFloatValue = MathF.Floor(rating * 100) / 100;

            return $"'{treatedFloatValue.ToString("F2").Replace(',', '.')}'";
        }

        public string MountConditionalQueryPart(
            ResourceFilters filters, 
            string? categoryRelationTableName = "",
            string? resourceTableName = ""
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
                string nameColumn = string.IsNullOrEmpty(resourceTableName)
                    ? "name"
                    : $"{resourceTableName}.name";

                conditionals.Add($"{nameColumn} Like '%{filters.name}%'");
            }

            if (conditionals.Count > 0) {
                conditionalQueryPart = "Where " + String.Join(" And ", conditionals);
            }

            return conditionalQueryPart;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByResourceId(
            int resourceId,
            ContentTypesEnum categoryType
        ) {
            await DataBase.OpenConnectionIfClosed();

            string relationTableName = "";
            string resourceIdFieldName = "";

            switch (categoryType) {
                case ContentTypesEnum.Animes:
                    relationTableName = "Anime";
                    resourceIdFieldName = "animeId";
                break;
                case ContentTypesEnum.Mangas:
                    relationTableName = "Manga";
                    resourceIdFieldName = "mangaId";
                break;
                case ContentTypesEnum.Seriado:
                    relationTableName = "Series";
                    resourceIdFieldName = "serieId";
                break;
                case ContentTypesEnum.Livros:
                    relationTableName = "Book";
                    resourceIdFieldName = "bookId";
                break;
                case ContentTypesEnum.Jogos:
                    relationTableName = "Game";
                    resourceIdFieldName = "gameId";
                break;
                case ContentTypesEnum.Cinema:
                    relationTableName = "Movie";
                    resourceIdFieldName = "movieId";
                break;
            }
            relationTableName += "_x_Category";

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText = 
                @"Select " + 
                    "Categories.id As category_id, Categories.name As category_name, " + 
                    "iconBase64, contentTypeId " +
                "From Categories " +
                $"Inner Join {relationTableName} " +
                    $"On {relationTableName}.{resourceIdFieldName} = {resourceId} " +
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
    }
}
