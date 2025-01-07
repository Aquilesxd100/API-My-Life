using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using my_life_api.Models.Content;
using my_life_api.Models.Content.Entities;

namespace my_life_api.Shared.ContentResources;

public enum ContentTypesEnum {
    Musical = 1,
    Cinema = 2,
    Mangas = 3,
    Jogos = 4,
    Frases = 5,
    Livros = 6,
    Seriado  = 7,
    Animes = 8
}

public class ContentTypeData {
    public readonly ContentTypesEnum contentType;
    public readonly string name;
    public readonly string nameInPtBr;
    public readonly string nameInPtBrWithNoAccent;
    public readonly Type dtoType;
    public readonly Type entityType;
    public readonly string dbTableName;
    public readonly string storageFolder;
    public readonly string prefixFileName;

    public ContentTypeData(
        ContentTypesEnum _contentType,
        Type _entityType,
        Type _dtoType,
        string _name,
        string _nameInPtBr,
        string? _dbTableName = null,
        string? _storageFolder = null,
        string? _prefixFileName = null
    ) {
        this.contentType = _contentType;
        this.entityType = _entityType;
        this.dtoType = _dtoType;
        this.name = _name;
        this.nameInPtBr = _nameInPtBr;
        // Guarda uma versao do nome sem acentos
        this.nameInPtBrWithNoAccent = 
            Regex.Replace(
                _nameInPtBr.Normalize(NormalizationForm.FormD),
                @"[\p{Mn}]",
                ""
            );

        // Se nao informado, define o nome da tabela do item
        // seguindo o padrao de inicial maiuscula e plural com 's' no final, caso se aplique
        this.dbTableName = 
            _dbTableName ?? Format.GetWordWithUpperCaseInitial(_name) + "s";

        // Caso guarde arquivos pelo FTP
        // EX: caso tenha imagens
        this.storageFolder = _storageFolder ?? $"{_name}_pictures";
        this.prefixFileName = _prefixFileName ?? $"{_name}-";
    }

    // Se ao terminar todas as chamadas mandarem prefixo deixa-lo obrigatório
    public IEnumerable<string> GetDbColumnsNames(string? prefix = null) { 
        PropertyInfo[] properties = this.entityType.GetProperties();

        return properties.Select(prop => 
            (!String.IsNullOrEmpty(prefix) ? prefix : "") + prop.Name
        );
    }

    public string GetRelationTableName() { 
        return $"{Format.GetWordWithUpperCaseInitial(this.name)}_x_Category";
    }
}

public static class ContentUtils { 
    public static ImmutableArray<ContentTypeData> contentTypesData = ImmutableArray.Create(
        new ContentTypeData(
            ContentTypesEnum.Cinema, typeof(MovieEntity), typeof(MovieDTO), "movie", "filme"
        )
    );

    public static ContentTypeData GetContentTypeData(ContentTypesEnum contentType) {
        return ContentUtils.contentTypesData.FirstOrDefault(
            (ct => ct.contentType == contentType)
        );
    }

    public static ContentTypeData GetContentTypeDataByPath(string path) {

        ImmutableArray<KeyValuePair<string, ContentTypesEnum>> contentTypesPairs = 
            ImmutableArray.ToImmutableArray(
                contentTypesData.Select(ctd => 
                    new KeyValuePair<string, ContentTypesEnum>(
                        ctd.nameInPtBrWithNoAccent, 
                        ctd.contentType
                    )
                )
            );

       var matchedContentPair = contentTypesPairs.FirstOrDefault(
           (ctp) => path.Contains(ctp.Key)
       );

        return GetContentTypeData(matchedContentPair.Value);
    }
}


