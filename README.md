# SharpIgnite

SharpIgnite is a lightweight and simple application framework inspired by CodeIgniter, designed to make it easy to build applications in C#.

## Features

* [TODO] MVC (Model-View-Controller) architecture for organizing your code
* [TODO] Easy routing for defining clean and user-friendly URLs
* Database support for interacting with popular database systems such as MySQL, SQL Server, SQLite, etc.
* [TODO] Templating engine for separating your HTML markup from your application logic
* [TODO] Built-in form validation to ensure data integrity and security
* [TODO] Security features such as XSS (Cross-Site Scripting) and CSRF (Cross-Site Request Forgery) protection
* Helper functions to simplify common tasks such as working with arrays, strings, URLs, etc.
* [TODO] Session management for handling user sessions and authentication
* [TODO] Error handling and logging for debugging and monitoring your application

## Getting Started

To get started with SharpIgnite, simply install the package via NuGet:

```
Install-Package SharpIgnite
```

Once installed, you can start using SharpIgnite in your project by importing the necessary namespaces and leveraging its features.

For more detailed instructions on how to use SharpIgnite, please refer to the documentation provided with the NuGet package or visit the SharpIgnite GitHub repository for additional information.

## Contributing

We welcome contributions from the community to help improve SharpIgnite. If you encounter any bugs, issues, or have suggestions for new features, please submit a pull request or open an issue on the GitHub repository.

## Publish

We pack and push SharpIgnite via NuGet. Use the following commands:

```
> nuget pack SharpIgnite.csproj
> nuget push SharpIgnite.0.2.2.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey %NUGET_API_KEY%
```

## License

SharpIgnite is licensed under the MIT License. See the LICENSE file for details.