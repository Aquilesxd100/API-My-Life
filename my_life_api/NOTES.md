- Updates de todos os itens ir�o esperar somente os campos que ser�o realmente ATUALIZADOS (exceto listas)
- idTipoConteudo de um conteudo N�O pode ser mudado, � definido somente em sua cria��o
- Autores s� podem ser deletados DEPOIS de suas obras
- Exclus�o de imagens de recursos ser�o feitos por uma rota separada, update segue pela mesma
- Lista de categorias no update s�o SEMPRE esperadas, caso n�o informadas considera que o conteudo n�o
deva ter categorias

Tela de Registros

- Na tela de cria��o de registro ser� possivel enviar multiplas imagens secundarias
- No Update de registro a adi��o e exclusao de imagens secundarias ir� acontecer de uma a uma,
sendo deletadas ao clicar em um "X" no canto delas e adicionadas em um "+" lado direito, 
tudo com requisicao separada


- IMPORTANTE: Andar da coisa pelo visto os DTOs de conteudos que ainda n�o tem 
dever�o ter um m�todo para criar o DTO baseado em object


- NOTAS REFATORACAO

- Verificar maneiras de padronizar o m�ximo possivel a manipulacao de conteudos
- Talvez criar uma s� validacao para todos os tipos de conteudos,
recebendo um object com os dados, o type do conteudo para se basear e um par�metro opcional
indicando se � cria��o ou update
- seguindo a ideia de cima tamb�m poderia ser possivel montar, criar e atualizar os dados em si no banco
de forma padronizada

SISTEMA DE AUTENTICACAO POR COOKIE
Seria interessante atualizar o sistema de autenticacao para ser feito usando cookies ao inves
de local storage

- Criar Rota para cria��o de tabelas do sistema, inclus�o de types conforme necessario, 
pastas no storage FTP e o que mais faltar para configuracao de tudo em um contexto inicial


(PRIORIDADE FINAL) 
- Documentar todos os m�todos padronizados da aplicacao, par�metros e etc










- OBRIGATORIO AS ENTITY E DTOs DOS CONTEUDOS TEREM SUAS PROPRIEDADES EM ORDEM
SENDO CATEGORIAS A ULTIMA A SER REGISTRADA
CONTEUDOS TAMB�M PRECISAM TER OBRIGATORIAMENTE SEMPRE 'id' & 'categorias'


