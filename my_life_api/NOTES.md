- Updates de todos os itens irão esperar somente os campos que serão realmente ATUALIZADOS (exceto listas)
- idTipoConteudo de um conteudo NÃO pode ser mudado, é definido somente em sua criação
- Autores só podem ser deletados DEPOIS de suas obras
- Exclusão de imagens de recursos serão feitos por uma rota separada, update segue pela mesma
- Lista de categorias no update são SEMPRE esperadas, caso não informadas considera que o conteudo não
deva ter categorias

Tela de Registros

- Na tela de criação de registro será possivel enviar multiplas imagens secundarias
- No Update de registro a adição e exclusao de imagens secundarias irá acontecer de uma a uma,
sendo deletadas ao clicar em um "X" no canto delas e adicionadas em um "+" lado direito, 
tudo com requisicao separada


- IMPORTANTE: Andar da coisa pelo visto os DTOs de conteudos que ainda não tem 
deverão ter um método para criar o DTO baseado em object


- NOTAS REFATORACAO

- Verificar maneiras de padronizar o máximo possivel a manipulacao de conteudos
- Talvez criar uma só validacao para todos os tipos de conteudos,
recebendo um object com os dados, o type do conteudo para se basear e um parâmetro opcional
indicando se é criação ou update
- seguindo a ideia de cima também poderia ser possivel montar, criar e atualizar os dados em si no banco
de forma padronizada

SISTEMA DE AUTENTICACAO POR COOKIE
Seria interessante atualizar o sistema de autenticacao para ser feito usando cookies ao inves
de local storage

- Criar Rota para criação de tabelas do sistema, inclusão de types conforme necessario, 
pastas no storage FTP e o que mais faltar para configuracao de tudo em um contexto inicial


(PRIORIDADE FINAL) 
- Documentar todos os métodos padronizados da aplicacao, parâmetros e etc










- OBRIGATORIO AS ENTITY E DTOs DOS CONTEUDOS TEREM SUAS PROPRIEDADES EM ORDEM
SENDO CATEGORIAS A ULTIMA A SER REGISTRADA
CONTEUDOS TAMBÉM PRECISAM TER OBRIGATORIAMENTE SEMPRE 'id' & 'categorias'


