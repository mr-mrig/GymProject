
# PRELIMINARY STEP:
# Before Code-first DB
# Move to the project to be built
cd .\GymProject.Infrastructure


# To Add custom SQL script to migrations:

0.0 Ensure OnConfiguring method in the DbContext contains connection string, to avoid
	"No database provider has been configured for this DbContext" error

0.1. Clear existing migrations - needed if targeting SQLite

# Package Manager Console:

1. dotnet ef migrations add Test

2. Edit the migration file Up function:

// Append to the end of the Up function
            string scriptDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory
    .Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin"))
    , "MigrationSQLScripts");

            var sqlFile = System.IO.Path.Combine(scriptDirectory, @"ExcerciseDataSeeding.sql");
            migrationBuilder.Sql(System.IO.File.ReadAllText(sqlFile));
 
3. dotnet ef database update