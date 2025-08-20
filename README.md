# Linkify

Linkify is a modern web application built with ASP.NET Core Razor Pages (.NET 9) designed to simplify and manage user authentication, registration, and profile management. The project follows a clean architecture, separating concerns across the Presentation Layer (PLL), Business Logic Layer (BLL), and Data Access Layer (DAL).

## Features

-   **User Registration & Login:** Secure user registration and authentication with validation.
-   **Profile Management:** Update user details, change passwords, and manage account status.
-   **Error Handling:** Friendly error pages and robust exception management.
-   **Layered Architecture:**
    -   **PLL:** Razor Pages for fast, maintainable UI.
    -   **BLL:** Business logic and validation.
    -   **DAL:** Data access and repository pattern.

## Technologies

-   **.NET 9**
-   **ASP.NET Core Razor Pages**
-   **C# 13**
-   **Entity Framework Core** (recommended for data access)
-   **jQuery Validation** (client-side validation)
-   **MVC Patterns** (where appropriate)

## Getting Started

1. **Clone the repository:**
   git clone https://github.com/mnbil0680/linkify.git
2. **Open in Visual Studio 2022.**
3. **Restore NuGet packages.**
4. **Update database connection strings as needed.**
5. **Run the project (F5 or Ctrl+F5).**

## Project Structure

-   `LinkifyPLL/` – Presentation Layer (Razor Pages, static files)
-   `LinkifyBLL/` – Business Logic Layer (services, model views)
-   `LinkifyDAL/` – Data Access Layer (entities, repositories)

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License. See the [LICENSE](LinkifyPLL/wwwroot/lib/jquery-validation/LICENSE.md) file for details.

---

**Linkify** aims to be a robust starting point for secure, scalable web applications using the latest .NET technologies.
