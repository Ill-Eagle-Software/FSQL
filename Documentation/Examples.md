## Examples

**Example 1 -- Hello World!**

The traditional "Hello, World" application is about as simple as it can get, but it's still a good place to start.

All FSQL scripts must be contained in curly braces.

```
{
  WriteLine("Hello, World!");
}
```

**Example 2 -- Find Backup Files**

SELECT is actually a function, so you can store a query's results in a variable. Variables, as in SQL Server, are preceded by the '@' symbol. FSQL is a typeless language, and variables are created on an as-needed basis.

```
{
  // MAP assigns an alias to a folder.
  MAP Source TO "D:\\Data\\Source" WITH FILTER "*.bak";

  // Find all backup files.
  @Results = SELECT Source.FileName FROM Source;
  return @results; // FSQL is NOT case sensitive.
}
```

**Example 3 -- Find Modified Files**

Simple joins are supported. Notice that the equality operator is '=='. Other Boolean operators are taken from SQL. Math and string operators are modeled after C#. Additionally, the '^' operator allows you to raise a number to a power.

The FSQL-specific clause "FORMAT AS" allows you to deliver the output of the query in either JSON, CSV, or TEXT format. FORMAT AS SCALAR returns the value in column one, row one of the result set.

Additionally, the OUTPUT TO clause allows you to redirect the output to a file, to the Windows CLIPBOARD, or to the SCREEN.

```
{
  MAP Source TO "D:\\Data\\Source";
  MAP Backup TO "E:\\Data\\Backup";

  // Find Files that have been modified.
  SELECT Source.FileName
    FROM Source
    JOIN Backup ON Source.Name == Backup.Name
                AND NOT (Source.CheckSum == Backup.Checksum)
    FORMAT AS Json
    OUTPUT TO FILE "C:\\Temp\\SourceBackup.Diff.json";
}
```
