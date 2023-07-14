using BussinessUnitFileProcAPI.Entities;
using BussinessUnitFileProcAPI.Services;
using Microsoft.WindowsAzure.Storage.Table;

namespace BussinessUnitFileProcAPI.Repositories;
public class TableStorageRepo : ITableStorageService
{
    private const string TableName = "BussinessUnit";
    private readonly IConfiguration _configuration;
    private readonly ILogger<TableStorageRepo> _logger;
    private readonly CloudTable _cloudTable;

    public TableStorageRepo(IConfiguration configuration, ILogger<TableStorageRepo> logger,CloudTable cloudTable)
    {
        _configuration = configuration;
        _logger = logger;
        _cloudTable = cloudTable;
    }
    public async Task<List<BussinessUnitEntity>> GetEntityAsync(string BatchId)
    {
        //var table = await GetTableClient();
        string condition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, BatchId);
        var query = new TableQuery<BussinessUnitEntity>().Where(condition);
        var result = default(TableQuerySegment<BussinessUnitEntity>);
        try
        {
            result = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);
            _logger.LogInformation("Successfully retrived Record from Azure Table storage" + TableName);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured inside{nameof(TableStorageRepo)} in a Method {nameof(GetEntityAsync)}");
        }

        return result.Results;

    }

    public async Task<string> InsertEntityAsync(BussinessUnitEntity entity)
    {
        TableOperation insertOperation = TableOperation.Insert(entity);
        var result = default(TableResult);
        string generatedBatchId = string.Empty;
        try
        {
            result = await _cloudTable.ExecuteAsync(insertOperation);
            generatedBatchId = ((BussinessUnitEntity)result.Result).BatchID;
            _logger.LogInformation("Successfully Inserted record in Azure Table storage" + TableName + " with BatchId:" + generatedBatchId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured inside{nameof(TableStorageRepo)} in a Method {nameof(InsertEntityAsync)}");
        }

        return generatedBatchId;
    }
}