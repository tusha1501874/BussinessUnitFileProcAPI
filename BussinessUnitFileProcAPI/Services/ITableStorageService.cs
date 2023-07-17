using BussinessUnitFileProcAPI.Entities;

namespace BussinessUnitFileProcAPI.Services;
    public interface ITableStorageService
    {
        Task<List<BussinessUnitEntity>?> GetEntityAsync(string BatchId);
        Task<string> InsertEntityAsync(BussinessUnitEntity entity);
    }