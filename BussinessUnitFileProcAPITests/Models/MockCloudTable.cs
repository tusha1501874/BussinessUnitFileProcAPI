using Microsoft.WindowsAzure.Storage.Table;

namespace BussinessUnitFileProcAPITests.Models;
    public class CloudTableMock : CloudTable
    {
        public CloudTableMock() : base(new Uri("https://tableassesment.table.core.windows.net/BussinessUnit"))
        {
        }

        public async override Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                Result ="xyz",
                HttpStatusCode = 200
            });
        }
    }
