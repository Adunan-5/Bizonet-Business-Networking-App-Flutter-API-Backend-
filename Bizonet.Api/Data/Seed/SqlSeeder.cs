using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Bizonet.Api.Data.Seed;

public class SqlSeeder
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<SqlSeeder> _logger;

    public SqlSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<SqlSeeder> logger)
    {
        _db = db;
        _env = env;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // ✅ Increase timeout for seeding process
        _db.Database.SetCommandTimeout(300); // 5 minutes

        if (!await _db.Countries.AnyAsync())
            await RunSqlFileAsync("SeedScripts/countries.sql");

        if (!await _db.States.AnyAsync())
            await RunSqlFileAsync("SeedScripts/states.sql");

        if (!await _db.Cities.AnyAsync())
            await RunSqlFileAsync("SeedScripts/cities.sql");

        if (!await _db.BusinessCategories.AnyAsync())
            await RunSqlFileAsync("SeedScripts/business_categories.sql");

        if (!await _db.FollowupStatuses.AnyAsync())
            await RunSqlFileAsync("SeedScripts/followup_status.sql");

        if (!await _db.FollowupComments.AnyAsync())
            await RunSqlFileAsync("SeedScripts/followup_comments.sql");
    }

    private async Task RunSqlFileAsync(string relativePath)
    {
        var fullPath = Path.Combine(_env.ContentRootPath, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Seed SQL file not found: {fullPath}");

        _logger.LogInformation("Seeding from SQL file: {file}", fullPath);

        var sql = await File.ReadAllTextAsync(fullPath);

        var batches = sql.Split(new[] { "\r\nGO\r\n", "\nGO\n", "\r\nGO\n", "\nGO\r\n" },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            // ✅ automatically split huge multi-row inserts
            if (IsMultiRowInsert(trimmed))
            {
                var inserts = SplitMultiRowInsert(trimmed, 1000);

                foreach (var ins in inserts)
                {
                    await _db.Database.ExecuteSqlRawAsync(ins);
                }
            }
            else
            {
                await _db.Database.ExecuteSqlRawAsync(trimmed);
            }
        }

        _logger.LogInformation("Seeding completed for: {file}", fullPath);
    }

    private bool IsMultiRowInsert(string sql)
    {
        return sql.StartsWith("INSERT INTO", StringComparison.OrdinalIgnoreCase)
               && sql.Contains("VALUES", StringComparison.OrdinalIgnoreCase)
               && sql.Contains("),");
    }

    private List<string> SplitMultiRowInsert(string insertSql, int chunkSize)
    {
        // Match:
        // INSERT INTO TableName (columns...) VALUES (row1),(row2),...;
        var match = Regex.Match(insertSql,
            @"^(INSERT\s+INTO\s+.+?\)\s+VALUES\s*)(.+);?$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
            return new List<string> { insertSql };

        var header = match.Groups[1].Value.Trim();
        var valuesPart = match.Groups[2].Value.Trim();

        var rows = SplitValuesRows(valuesPart);

        var result = new List<string>();
        for (int i = 0; i < rows.Count; i += chunkSize)
        {
            var chunkRows = rows.Skip(i).Take(chunkSize);
            var chunkSql = $"{header}\n{string.Join(",\n", chunkRows)};";
            result.Add(chunkSql);
        }

        return result;
    }

    private List<string> SplitValuesRows(string valuesPart)
    {
        // splits (....),(....),(....) into list of "(....)"
        var rows = new List<string>();

        int depth = 0;
        int start = 0;

        for (int i = 0; i < valuesPart.Length; i++)
        {
            var c = valuesPart[i];

            if (c == '(') depth++;
            if (c == ')') depth--;

            if (depth == 0 && c == ',')
            {
                var row = valuesPart.Substring(start, i - start).Trim();
                if (!string.IsNullOrWhiteSpace(row))
                    rows.Add(row);

                start = i + 1;
            }
        }

        var last = valuesPart.Substring(start).Trim();
        if (!string.IsNullOrWhiteSpace(last))
            rows.Add(last);

        return rows;
    }
}
