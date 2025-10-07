# 🧾 OS PROJECT

Um MVP de sistema de **Ordem de Serviço (OS)** com autenticação, checklist e upload de fotos.

---

## 🚀 Stack utilizada

- **API:** .NET 8 (ASP.NET Core, EF Core, MySQL, JWT)
- **Front:** Vue 3 + Vite + Axios + Bootstrap
- **Banco:** MySQL (via Docker)
- **Infra:** Docker Compose

---

## 📦 Requisitos

- **Docker** e **Docker Compose** instalados.

---

## ⚙️ Passo a passo para rodar

Entrar na pasta do projeto `cd os-projeto`

---

### :one: Buildar docker e subir os containers

```bash
docker compose up -d --build
```

Para acompanhar logs:

```bash
docker compose logs -f api
```

---

### :two: URLs principais

| Serviço                 | URL                                                            |
| ----------------------- | -------------------------------------------------------------- |
| **API (Swagger)**       | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
| **Frontend (Vue)**      | [http://localhost:5173](http://localhost:5173)                 |
| **Adminer (MySQL GUI)** | [http://localhost:8080](http://localhost:8080)                 |

#### :three: Credenciais do Adminer

- **System:** MySQL
- **Server:** `mysql`
- **User:** `admin`
- **Password:** `password`
- **Database:** `osdb`

---

## :four: Usando o Frontend

1. Acesse [http://localhost:5173](http://localhost:5173)
2. Registre-se e faça login
3. Crie uma nova OS
4. Abra a OS → preencha checklist → envie fotos

---

## :five: Comandos úteis

```bash
# subir containers
docker compose up -d

# ver logs
docker compose logs -f api
docker compose logs -f web

# reiniciar serviços
docker compose restart api web

# parar tudo
docker compose down

# reset total (apaga db)
docker compose down -v
```

---

**Desenvolvido para o Desafio Kodigos 2025**  
API + Front integrados em ambiente Docker pronto para rodar.
