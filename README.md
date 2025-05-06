# NASA Meteorite Explorer 🌠

This project visualizes NASA's meteorite landing data with filtering, sorting, and optional grouping by year. Built with **ASP.NET Core**, **Vue 3**, and **PostgreSQL**, and runs via **Docker Compose**.

## 🔧 Quick Start (via Docker)

1. Clone the repository:
   ```bash
   git clone https://github.com/res0lut1on/NasaLanding.git
   cd NasaLanding
2. Build and run the services:
   ```bash
   docker-compose up --build -d
3. Open your browser:

Frontend: http://localhost:8080

Backend API (Swagger): http://localhost:5000/swagger

Hangfire Dashboard: http://localhost:5000/hangfire

🐞 Troubleshooting
  Make sure Docker is running.
  
  If issues persist, feel free to reach out:
  
  Email: navimuxacek@gmail.com
  
  Telegram: https://t.me/gr1ndms

🚀 Future Improvements
🔹 Backend
  Redis caching — to speed up repeated queries.
  
  AutoMapper — for clean DTO ↔ entity mapping.
  
  Polly — to handle HTTP retries and resilience in HttpClient.
  
  Custom response wrappers — for standardized API outputs.
  
  Unit and integration tests — using xUnit, Testcontainers, etc.
  
  Rate limiting & request logging — to monitor API usage.

🔹 Frontend
  Vuex / Pinia store — for better state management of filters and results.
  
  Component-level caching — to avoid refetching unchanged data.
  
  Pagination support — instead of infinite scroll (optional).
  
  Dark/light theme toggle — for better UX.
  
  Skeleton loading UI — for smoother perception of loading states.
  
  Form validation — for more robust filters.

📦 Technologies Used
  Backend: ASP.NET Core 6, Entity Framework Core, Hangfire, PostgreSQL
  
  Frontend: Vue 3, Vite, Axios
  
  DevOps: Docker, Docker Compose


