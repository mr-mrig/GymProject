
# PRELIMINARY STEP:
# Before Code-first DB
# Move to the project to be built
cd .\GymProject.Infrastructure


# To Add custom SQL script to migrations:

1. dotnet ef migrations add Test

2. Edit the migration file Up function:

// Append to the end of the Up function
            string scriptDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory
    .Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin"))
    , "MigrationSQLScripts");

            var sqlFile = System.IO.Path.Combine(scriptDirectory, @"ExcerciseDataSeeding.sql");
            migrationBuilder.Sql(System.IO.File.ReadAllText(sqlFile));
 
3. dotnet ef database update