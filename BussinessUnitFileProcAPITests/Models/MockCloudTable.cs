using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessUnitFileProcAPI.Entities;

namespace BussinessUnitFileProcAPITests.Models
{
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
}
