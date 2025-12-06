namespace Urban.AI.Application.Geography.Common;

#region Usings
using Urban.AI.Domain.Geography;
using System.Globalization;
#endregion

internal sealed class GeographyDataParser : IGeographyDataParser
{
    #region Constants
    private const string DepartmentsFileName = "data_departments.sql";
    private const string MunicipalitiesFileName = "data_municipalities.sql";
    private const string TownshipsFileName = "data_rural_districts.sql";
    private const string InfrastructureFolder = "infrastructure";
    private const string ResourcesFolder = "resources";
    private const string LineBreak = "\n";
    private const string InitialCharacter = "(";
    private const string FinalCharacter = ")";
    private const int DepartmentFieldCount = 4;
    private const int MunicipalityFieldCount = 5;
    private const int TownshipFieldCount = 3;
    private const int NumberOfHeaderLines = 1;
    #endregion

    public GeographyDataParser()
    {
    }

    public async Task<IEnumerable<Department>> ParseDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var departments = new List<Department>();

        var filePath = GetDataFilePath(DepartmentsFileName);
        if (!File.Exists(filePath))
        {
            return departments;
        }

        var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
        var content = RemoveHeaderFromContent(fileContent);
        var values = ParseToStringList(content);

        foreach (var value in values)
        {
            var row = ParseInsertValues(value);
            if (row.Count >= DepartmentFieldCount)
            {
                var id = CleanValue(row[0]);
                var name = CleanValue(row[1]);
                var latitude = ParseDecimal(CleanValue(row[2]));
                var longitude = ParseDecimal(CleanValue(row[3]));

                departments.Add(Department.Create(id, name, latitude, longitude));
            }
        }

        return departments;
    }

    public async Task<IEnumerable<Municipality>> ParseMunicipalitiesAsync(CancellationToken cancellationToken = default)
    {
        var municipalities = new List<Municipality>();

        var filePath = GetDataFilePath(MunicipalitiesFileName);
        if (!File.Exists(filePath))
        {
            return municipalities;
        }

        var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
        var content = RemoveHeaderFromContent(fileContent);
        var values = ParseToStringList(content);

        foreach (var value in values)
        {
            var row = ParseInsertValues(value);
            if (row.Count >= MunicipalityFieldCount)
            {
                var id = CleanValue(row[0]);
                var name = CleanValue(row[1]);
                var departmentDaneCode = CleanValue(row[2]);
                var latitude = ParseDecimal(CleanValue(row[3]));
                var longitude = ParseDecimal(CleanValue(row[4]));

                municipalities.Add(Municipality.Create(id, name, departmentDaneCode, latitude, longitude));
            }
        }

        return municipalities;
    }

    public async Task<IEnumerable<Township>> ParseTownshipsAsync(CancellationToken cancellationToken = default)
    {
        var townships = new List<Township>();

        var filePath = GetDataFilePath(TownshipsFileName);
        if (!File.Exists(filePath))
        {
            return townships;
        }

        var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
        var content = RemoveHeaderFromContent(fileContent);
        var values = ParseToStringList(content);

        foreach (var value in values)
        {
            var row = ParseInsertValues(value);
            if (row.Count >= TownshipFieldCount)
            {
                var id = CleanValue(row[0]);
                var name = CleanValue(row[1]);
                var municipalityDaneCode = CleanValue(row[2]);

                townships.Add(Township.Create(id, name, municipalityDaneCode));
            }
        }

        return townships;
    }

    #region Private Methods
    private static string RemoveHeaderFromContent(string fileContent)
    {
        var lines = fileContent.Split(LineBreak).Skip(NumberOfHeaderLines).ToArray();
        var valuesSection = string.Join(LineBreak, lines);
        return valuesSection.Trim().TrimEnd(';');
    }

    private static string[] ParseToStringList(string content)
    {
        return [.. content
                .Split([LineBreak], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.TrimEnd(',', ' ', '\t', '\r'))];
    }

    private string GetDataFilePath(string fileName)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var projectRoot = FindProjectRoot(currentDirectory);
        var DataFolder = Path.Combine(InfrastructureFolder, ResourcesFolder);

        if (projectRoot is not null)
        {
            var dataFolderPath = Path.Combine(projectRoot, DataFolder);
            var filePath = Path.Combine(dataFolderPath, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }
        }

        var possiblePaths = new[]
        {
            Path.Combine(currentDirectory, "..", "..", "..", "..", DataFolder, fileName),
            Path.Combine(currentDirectory, "..", "..", "..", DataFolder, fileName),
            Path.Combine(currentDirectory, "..", "..", DataFolder, fileName),
            Path.Combine(currentDirectory, DataFolder, fileName)
        };

        foreach (var path in possiblePaths)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        return Path.Combine(currentDirectory, fileName);
    }

    private static string? FindProjectRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            if (IsItTheRootProjectDirectory(directory))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static bool IsItTheRootProjectDirectory(DirectoryInfo directory)
    {
        return directory.GetFiles("*.sln").Any() ||
               directory.GetDirectories("infrastructure").Any();
    }

    private static List<string> ParseInsertValues(string valuesString)
    {
        if (!string.IsNullOrWhiteSpace(valuesString) &&
            valuesString.StartsWith(InitialCharacter) && valuesString.EndsWith(FinalCharacter))
        {
            valuesString = valuesString.Substring(1, valuesString.Length - 2);
        }

        var values = new List<string>();
        var currentValue = string.Empty;
        var inQuotes = false;
        var quoteChar = '\0';

        for (var i = 0; i < valuesString.Length; i++)
        {
            var currentChar = valuesString[i];

            if (!inQuotes && currentChar is '\'' or '"')
            {
                inQuotes = true;
                quoteChar = currentChar;
                currentValue += currentChar;
            }
            else if (inQuotes && currentChar == quoteChar)
            {
                if (i + 1 < valuesString.Length && valuesString[i + 1] == quoteChar)
                {
                    currentValue += currentChar;
                    currentValue += valuesString[++i];
                }
                else
                {
                    inQuotes = false;
                    currentValue += currentChar;
                }
            }
            else if (!inQuotes && currentChar == ',')
            {
                values.Add(currentValue.Trim());
                currentValue = string.Empty;
            }
            else
            {
                currentValue += currentChar;
            }
        }

        if (!string.IsNullOrWhiteSpace(currentValue))
        {
            values.Add(currentValue.Trim());
        }

        return values;
    }


    private static string CleanValue(string value)
    {
        return value.Trim('\'', '"', ' ');
    }

    private static decimal ParseDecimal(string value)
    {
        if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        return 0m;
    }
    #endregion
}