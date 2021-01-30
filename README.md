# UpFlux Library API

##### Autor: `Jaison Coelho`

##### Requisitos: `.Net 3.1`, `.Net Cli`

## Observações
Não foi utilizando banco orientado a documentos, pois não possuo muita expiriência. Quanto a interface em `Angular`, conheço pouco, tenho experiência com `React.js`, portanto optei por não fazer.

Para a base de dados, foi utilizado banco relacional rodado em mémoria.

## Instalação
Nos diretórios `UpFlux.Library.API` e `UpFlux.Library.API.Tests` rode o comando abaixo
```bash
dotnet restore
```

## Rodar Aplicação
No diretório `UpFlux.Library.API` rode o comando abaixo
```bash
dotnet run
```

## Rodar Testes
No diretório `UpFlux.Library.API.Tests` rode o comando abaixo
```bash
dotnet test
```

## Endpoints
#### Listar Livros
```curl
curl --request GET \
  --url 'http://localhost:5000/books?title=Beleza&Author=Roger'
```

#### Listar Livro Específico
```curl
curl --request GET \
  --url http://localhost:5000/books/102
```

#### Criar Novo Livro
```curl
curl --request POST \
  --url http://localhost:5000/books \
  --header 'Content-Type: application/json' \
  --data '{
	"Title": "O Que Há De Errado Com O Mundo",
	"Author": "G. K. Chesterton"
}'
```

#### Editar Livro
```curl
curl --request PUT \
  --url http://localhost:5000/books/102 \
  --header 'Content-Type: application/json' \
  --data '{
	"Title": "Segundo Tratado Sobre O Governo",
	"Author": "John Locke"
}'
```

#### Excluir Livro
```curl
curl --request DELETE \
  --url http://localhost:5000/books/102
```

#### Listar Empréstimos
```curl
curl --request GET \
  --url http://localhost:5000/books/100/loans
```

#### Criar Novo Empréstimo
```curl
curl --request POST \
  --url http://localhost:5000/books/101/loans \
  --header 'Content-Type: application/json' \
  --data '{
	"user": "Jaison"
}'
```