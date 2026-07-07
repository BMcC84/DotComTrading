# Dot-Com Trading

Dot Com Trading is a full-stack web-based application that simulates a stock market where users trade assets represented as websites. These websites are given value by an algorithm which takes in real-world website data such as backlinks, daily visits and domain age to effectively represent a websites popularity and influence as a monetary value.

&#x20;

The decision to have users trade with "Website Stocks" rather than normal stocks on the market is so that they gain a greater understanding of why their asset's value is increasing or decreasing, as the average player would be more knowledgeable in popular websites rather than private companies.



It was built as my final-year dissertation for the BEng Software Engineering Degree at Queen's University Belfast.



## Demo



!\[Dot-Com Trading Demo](docs/demo.gif)





## Technology Stack

Backend:

* ASP.NET Core Web API
* C#
* .NET 8
* Entity Framework Core



Frontend:

* Blazor
* Radzen Components



Database:

* SQL Server



External APIs:

* Google OAuth 2.0
* Moz API
* Cloudflare Radar API
* Verisign RDAP API



## Key Features

* Live Market Simulation With Automated Price Updates
* Portfolio Management and Transaction History
* Website Valuation Engine using Real-World Metrics
* Add Websites to the System via URL
* Google OAuth 2.0 Authentication
* Historical Price Charts and Data Visualisation
* Progression System and Leaderboard



## Website Valuation Algorithm

Website prices are calculated using a combination of real-world website data and simulated metrics. This data is listed below:

* Backlink Count (Moz API)
* Number of Backlinks From Other Market Stocks (Simulated)
* Daily Visits (Estimate)
* Lifetime Visits (Estimate)
* Domain Age (Verisign RDAP API)



## Architecture

This Application follows a layered architecture separating Presentation, Business Logic and Data Accessing.

##### Blazor UI -> ASP.NET Core Web API - Controllers, Services, Repositories, Entity Framework Core -> SQL Server



## Screenshots

##### Home



!\[Home](docs/screenshots/home.png)





##### Login



!\[Market](docs/screenshots/login.png)





##### Market



!\[Market](docs/screenshots/market.png)





##### Websites



!\[Website](docs/screenshots/website.png)





##### Website Information



!\[Website](docs/screenshots/websiteinfo.png)





##### Portfolio



!\[Portfolio](docs/screenshots/portfolio.png)







## Running the Project

#### Prerequisites

Before running the application, ensure you have the following:

* .NET 8 SDK
* Visual Studio 2022 (Or Later) With ASP.NET and Web Development Workload
* SQL Server LocalDB or SQL Server Express



#### Clone the Repository

```bash

git clone https://github.com/BMcC84/DotComTrading.git

cd DotComTrading

```



#### Configuration

The application uses external services for authentication and website valuation. Before running the project, configure the following credentials using \*\*.NET User Secrets\*\*:

* Google OAuth 2.0 Client ID and Client Secret
* Cloudflare API Token
* Moz Access ID and Secret Key

Example:

```bash

dotnet user-secrets set "Authentication:Google:ClientId" "<your-client-id>"

dotnet user-secrets set "Authentication:Google:ClientSecret" "<your-client-secret>"

dotnet user-secrets set "Cloudflare:ApiToken" "<your-api-token>"

dotnet user-secrets set "Moz:AccessId" "<your-access-id>"

dotnet user-secrets set "Moz:SecretKey" "<your-secret-key>"

```



#### Database

Create the database by applying the Entity Framework Core Migrations:

```bash

dotnet ef database update

```



#### Running the Application

1. Open the Solution in Visual Studio
2. Configure \*\*Multiple Startup Projects\*\*.
3. Set both \*\*DotComTrading\*\* and \*\*DotComTrading.UI\*\* to \*\*Start\*\*.
4. Run the Application.

The Blazor Frontend will launch in your browser and communicate with the ASP.NET Core Web API.

>\*\*Note:\*\* Google OAuth 2.0 Credentials are required for User Authentication. This application will not support login until valid credentials have been configured.





## Future Improvements

* Further refinement of Valuation Algorithm
* Expansion of Gamification Elements
* More detailed Trading Graphs
* Improved Accessibility



## Author

Ben McCurry

BEng Software Engineering Graduate

Queen's University Belfast

