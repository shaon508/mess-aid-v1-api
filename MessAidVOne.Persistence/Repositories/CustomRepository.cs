using System.Security;
using MassAidVOne.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

public class CustomRepository(MessManagementContext context) : ICustomRepository
{
    private readonly MessManagementContext _context = context;

    public async Task<int> UpdateRangeSelectAsyn(string tableName,
                    Dictionary<string, object> whereConditions,
                    Dictionary<string, object> updateColumns)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name is required");

        if (whereConditions == null || whereConditions.Count == 0)
            throw new InvalidOperationException(
                "WHERE clause is mandatory. Update without WHERE is forbidden.");

        if (updateColumns == null || updateColumns.Count == 0)
            throw new ArgumentException("No update columns provided");

        var setClauses = new List<string>();
        var whereClauses = new List<string>();
        var parameters = new List<MySqlParameter>();

        int setIndex = 0;
        foreach (var col in updateColumns)
        {
            var paramName = $"@set_{setIndex}";
            setClauses.Add($"{col.Key} = {paramName}");
            parameters.Add(new MySqlParameter(paramName, col.Value ?? DBNull.Value));
            setIndex++;
        }

        int whereIndex = 0;
        foreach (var condition in whereConditions)
        {
            if (condition.Value is System.Collections.IEnumerable values
                && condition.Value is not string)
            {
                var inParams = new List<string>();
                int inIndex = 0;

                foreach (var val in values)
                {
                    var paramName = $"@where_{whereIndex}_{inIndex}";
                    inParams.Add(paramName);
                    parameters.Add(new MySqlParameter(paramName, val));
                    inIndex++;
                }

                if (inParams.Count == 0)
                    throw new ArgumentException($"IN list for '{condition.Key}' is empty");

                whereClauses.Add($"{condition.Key} IN ({string.Join(", ", inParams)})");
            }
            else
            {
                var paramName = $"@where_{whereIndex}";
                whereClauses.Add($"{condition.Key} = {paramName}");
                parameters.Add(new MySqlParameter(paramName, condition.Value));
            }

            whereIndex++;
        }

        var sql = $@"
        UPDATE {tableName}
        SET {string.Join(", ", setClauses)}
        WHERE {string.Join(" AND ", whereClauses)};
    ";

        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

}
