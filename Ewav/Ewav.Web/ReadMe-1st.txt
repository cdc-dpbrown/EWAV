First ReadMe for Simple MVVM with WCF RIA Services

You should be able to just press Ctrl+F5 to start the app, then click the
Load, Edit, Add and Remove buttons to use the app with mock data. You can also
set the Test project as the Startup and press Ctrl+F5 to run the tests.

Start by following the steps below in this ReadMe file. Then follow steps in
the second ReadMe file in the client Silverlight project.

After building the solution you should be able to run both the Silverlight client
project and also the Silverlight Test client.

1. Right-click on the the Web project and add a new item.
   - Under Data select ADO.NET Entity Data Model and give it a name
   - Follow the wizard steps to select tables from a database to add to a model
   - Build the Web project
   - To support validation the edmx file must be in the Web project

2. Right-click on the Services folder in the Web project and add a new item.
   - Search for and select a Domain Service Class and give it a name
   - In the Add New Domain Service Class dialog, make sure to select Enable Client Access
   - Then check the tables and editing options
   - You can also elect to generate associated metadata classes for validation

3. Add methods to the domain service class as desired
   - Insert methods returning IQueryable<T> to retrieve items
   - Insert methods performing operations by placing [Invoke] attribute on them

4. Build the Web project
   - This will project code to the client project, which you can see by
     showing all files and expanding the Generated_Code folder on the client.
   - There will be a class extending DomainClient that manages entities and changes
   - There will be entity classes based on the edmx file you added earlier.

Next open ReadMe-2nd.txt in the Silverlight client project and follow the steps.
