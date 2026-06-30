# Fintrack

Sistema em C# para a disciplina projeto integrador III-B.

### Tecnologias

- ASP.NET Core
- Entity Framework Core
- MySQL
- JWT Authentication
- React

## API

### Configuração

Editar o arquivo:

`appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=NOMEDB;user=USUARIO;password=SENHA"
  },

  "Jwt": {
    "Key": "CHAVE_JWT_GRANDE_AQUI",
    "Issuer": "fintrack-api",
    "Audience": "fintrack-client"
  }
}
```

### Instalação

Restaurar dependências:

```bash
dotnet restore
```

Aplicar migrations:

```bash
dotnet ef database update
```

---

### Executar projeto

```bash
dotnet run
```

API disponível em:

```txt
http://localhost:5018
```

Swagger:

```txt
http://localhost:5018/swagger
```

## Frontend

### Executar

```bash
npm start
```

Front disponível em:

```txt
http://localhost:3000
```
