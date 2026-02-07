# MiniAuthMvcVS26

Create and apply migrations (this is what creates tables)

Run these in the project root (same folder as .csproj):

dotnet ef migrations add InitialCreate
dotnet ef database update

After this, you should see a Migrations/ folder created.

If you get dotnet-ef errors (EF 10)

Install the matching tool version:

dotnet tool install --global dotnet-ef --version 10.0.2

Then run migration commands again.

