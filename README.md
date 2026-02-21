# User Service


## 🔑 Autenticação

Os endpoints protegidos requerem um **JWT Bearer Token**:
Authorization: Bearer <seu_token_jwt>

---

## 📚 Endpoints Disponíveis

1. 👤 Criar Usuário

POST /
Cria um novo usuário no sistema e dispara automaticamente a Azure Function Email Notification, que envia um e-mail de boas-vindas.

POST https://localhost:port/
Content-Type: application/json
```json
{
  "nome": "Lucas Losano",
  "dataDeNascimento": "2000-08-01T03:40:10.735Z",
  "email": "lucas.losano@fiap.com.br",
  "senha": "Senhaforte!1" // Deve ter pelo menos 8 digitos, com números e caracteres especiais 
}
```

2. 🔐 Login

POST /login
Valida as credenciais do usuário e retorna um JWT token de autenticação.

POST https://localhost:port/login
Content-Type: application/json
```json
{
  "email": "lucas.losano@fiap.com.br",
  "senha": "SenhaForte123!"
}
```

3. ♻️ Reativar Usuário

PATCH /reativar/{id}
Reativa uma conta de usuário previamente desativada.

PATCH https://localhost:port/reativar/123
Authorization: Bearer <token_de_administrador>

---


## 📚 Como implementar

Instale o pacote no seu projeto: [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)

Inclua no seu appsettings.json o seguinte trecho:
```json
{
  "Jwt": {
    "SecretKey": "GrandeChaveSegura123!GrandeChaveSegura123!",
    "Issuer": "UsuarioService"
  }
}
```

Depois de instalado adicione o seguinte trecho de código no seu builder.

```C#
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});
builder.Services.AddAuthorization();
```

Adicione .RequireAuthorization() ao final de seus mappings como o exemplo a seguir:

```C#
app.MapGet("/weatherforecast", () =>
{
    var forecast = "Examplo"
    return forecast;
})
.RequireAuthorization() // Está linha
.WithName("GetWeatherForecast")
```


Se estiver usando Swagger, substitua builder.Services.AddSwaggerGen() pelo trecho abaixo para facilitar os testes:

```C#
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```
