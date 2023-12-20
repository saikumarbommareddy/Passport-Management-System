
# Passport Management System

The PMS serves as a conduit for communication between the "applicant" and the "administration." The online passport management system is available 24 hours a day, seven days a week. To access the software or website, you must enter your email address and password. It will also include a sign-up/account creation module that will seek the user's basic information, such as full name, email, address, cellphone number, date of birth, and so on.
* This technology enables the entity in charge of passport issuance to reduce workload and process applications more quickly.
* Admin will approve/reject the applications after reviewing the documentation.
* Applicants can check the progress of their application and reapply for a passport if it is declined.


# Prerequisite Tools to run the Project
* Visual Studio 2022
* Microsoft SQL Server with SSMS
* ODBC Driver for SQL Server

# Language and Framework used
* ASP .NET MVC
* C#

# Database Connection Setup
* After installing SQL Server open SSMS and create a user. Follow the below video to create the user. (https://www.youtube.com/watch?v=11Rx35l8Khc)
* Update only your user id and password which you have setup while adding the user in "web.config" file in the "connectionString".
* Import the sql file("PMSDatabaseScript.sql") which is in repository to your sql server. (Just Update your SQL Server name in script before running the script)
    * Update Servername in Filename path in SQL Script.
    * You can find your SQL Servername in this path in your local env. (C:\Program Files\Microsoft SQL Server)

# Steps to run the Project
* Clone the given repos https://github.com/VishalReddyPachika/PMS-Project
* Open the PMS-Project with Visual Studio 2022
* Run the project is Visual Studio.

