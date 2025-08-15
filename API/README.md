# Student Grades API

Uma API REST desenvolvida em .NET 9 para gerenciar notas de estudantes com cálculo automático de médias.

## Características

- **Framework**: .NET 9
- **Banco de Dados**: Entity Framework Core com banco de dados em memória
- **Funcionalidades**: CRUD completo para estudantes e notas
- **Cálculo Automático**: Média das notas calculada automaticamente
- **CORS**: Habilitado para permitir requisições de qualquer origem

## Endpoints

### Estudantes

#### GET /api/students
Retorna todos os estudantes com suas notas e médias.

**Resposta:**
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "createdAt": "2025-08-11T16:10:18.737532Z",
    "averageGrade": 8.75,
    "grades": [
      {
        "id": 1,
        "value": 8.5,
        "subject": "Mathematics",
        "createdAt": "2025-08-11T16:10:18.738266Z",
        "studentId": 1
      }
    ]
  }
]
```

#### GET /api/students/{id}
Retorna um estudante específico com suas notas e média.

#### POST /api/students
Cria um novo estudante.

**Corpo da requisição:**
```json
{
  "name": "Nome do Estudante",
  "email": "email@exemplo.com"
}
```

#### PUT /api/students/{id}
Atualiza um estudante existente.

**Corpo da requisição:**
```json
{
  "name": "Novo Nome",
  "email": "novoemail@exemplo.com"
}
```

#### DELETE /api/students/{id}
Remove um estudante e todas suas notas.

### Notas

#### GET /api/grades
Retorna todas as notas.

#### GET /api/grades/{id}
Retorna uma nota específica.

#### GET /api/grades/student/{studentId}
Retorna todas as notas de um estudante específico com a média calculada.

**Resposta:**
```json
{
  "studentId": 1,
  "studentName": "John Doe",
  "grades": [...],
  "averageGrade": 8.75,
  "totalGrades": 2
}
```

#### POST /api/grades
Adiciona uma nova nota e retorna a média atualizada.

**Corpo da requisição:**
```json
{
  "value": 9.5,
  "subject": "Physics",
  "studentId": 1
}
```

**Resposta:**
```json
{
  "grade": {...},
  "studentInfo": {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "newAverageGrade": 9.0,
    "totalGrades": 3
  },
  "message": "Grade added successfully. New average: 9.0"
}
```

#### PUT /api/grades/{id}
Atualiza uma nota existente e retorna a nova média.

#### DELETE /api/grades/{id}
Remove uma nota e retorna a nova média.

## Como Executar

1. Certifique-se de ter o .NET 9 SDK instalado
2. Clone o repositório
3. Execute o comando:
   ```bash
   dotnet run
   ```
4. A API estará disponível em `http://localhost:5000`

## Validações

- **Notas**: Devem estar entre 0 e 10
- **Email**: Deve ser um email válido e único
- **Nome**: Obrigatório, máximo 100 caracteres
- **Matéria**: Obrigatória, máximo 100 caracteres

## Dados Iniciais

A API vem com dados de exemplo:
- 2 estudantes (John Doe e Jane Smith)
- 3 notas distribuídas entre eles

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Banco de dados em memória
- System.ComponentModel.DataAnnotations para validação

