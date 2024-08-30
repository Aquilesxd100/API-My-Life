Use patholo1_my_life;

-- Postagens e seus recursos

Create Table Registries (
  id Integer Auto_Increment Primary Key,
  created_at Timestamp,
  year Year Not Null,
  content Text Not Null,
  main_image_url Varchar(255)
);

Create Table SecondaryImages (
  id Integer Auto_Increment Primary Key,
  imageUrl Varchar(255) Not Null,
  registry_id Integer Not Null,
  Foreign Key (registry_id) References Registries(id)
);

-- Registros de conteudos vistos e seus recursos

Create Table ContentTypes (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null
);

Create Table Authors (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null,
  imageUrl Varchar(255),
  content_type_id Integer Not Null,
  Foreign Key (content_type_id) References ContentTypes(id)
);

Create Table Categories (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null,
  iconBase64 Text,
  content_type_id Integer Not Null,
  Foreign Key (content_type_id) References ContentTypes(id)
);

Create Table Movies (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null,
  imageUrl Varchar(255),
  rating Decimal(3,1) Not Null,
  dubbed Tinyint(1) Default 0, 
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Movie_x_Category (
  id Integer Auto_Increment Primary Key,
  movie_id Integer Not Null,
  Foreign Key (movie_id) References Movies(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Musics (
  id Integer Auto_Increment Primary Key,
  name Varchar(70) Not Null,
  rating Decimal(3,1) Not Null,
  soul_fragment Tinyint(1) Default 0,
  link Varchar(255),
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Music_x_Category (
  id Integer Auto_Increment Primary Key,
  music_id Integer Not Null,
  Foreign Key (music_id) References Musics(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Phrases (
  id Integer Auto_Increment Primary Key,
  phrase Tinytext Not Null,
  rating Decimal(3,1) Not Null, 
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Phrase_x_Category (
  id Integer Auto_Increment Primary Key,
  phrase_id Integer Not Null,
  Foreign Key (phrase_id) References Phrases(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Series (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null,
  imageUrl Varchar(255),
  rating Decimal(3,1), 
  completed Tinyint(1) Default 1,
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Serie_x_Category (
  id Integer Auto_Increment Primary Key,
  serie_id Integer Not Null,
  Foreign Key (serie_id) References Series(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Mangas (
  id Integer Auto_Increment Primary Key,
  name Varchar(70) Not Null,
  imageUrl Varchar(255),
  rating Decimal(3,1), 
  completed Tinyint(1) Default 1,
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Manga_x_Category (
  id Integer Auto_Increment Primary Key,
  manga_id Integer Not Null,
  Foreign Key (manga_id) References Mangas(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Animes (
  id Integer Auto_Increment Primary Key,
  name Varchar(70) Not Null,
  imageUrl Varchar(255),
  rating Decimal(3,1), 
  completed Tinyint(1) Default 1,
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Anime_x_Category (
  id Integer Auto_Increment Primary Key,
  anime_id Integer Not Null,
  Foreign Key (anime_id) References Animes(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Games (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null,
  imageUrl Varchar(255),
  rating Decimal(3,1), 
  completed Tinyint(1) Default 1,
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Game_x_Category (
  id Integer Auto_Increment Primary Key,
  game_id Integer Not Null,
  Foreign Key (game_id) References Games(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);

Create Table Books (
  id Integer Auto_Increment Primary Key,
  name Varchar(50) Not Null,
  imageUrl Varchar(255),
  rating Decimal(3,1), 
  completed Tinyint(1) Default 1,
  soul_fragment Tinyint(1) Default 0,
  author_id Integer Not Null,
  Foreign Key (author_id) References Authors(id)
);

Create Table Book_x_Category (
  id Integer Auto_Increment Primary Key,
  book_id Integer Not Null,
  Foreign Key (book_id) References Books(id),
  category_id Integer Not Null,
  Foreign Key (category_id) References Categories(id)
);




