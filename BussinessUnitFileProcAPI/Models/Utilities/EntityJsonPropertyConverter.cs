using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BussinessUnitFileProcAPI.Models.Utilities;
public class EntityJsonPropertyConverter
{
    public static void Serialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> results)
    {
        entity.GetType().GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EntityJsonPropertyConverterAttribute), false).Count() > 0)
            .ToList()
            .ForEach(x => results.Add(x.Name, new EntityProperty(JsonConvert.SerializeObject(x.GetValue(entity)))));
    }

    public static void Deserialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
    {
        entity.GetType().GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EntityJsonPropertyConverterAttribute), false).Count() > 0)
            .ToList()
            .ForEach(x => x.SetValue(entity, JsonConvert.DeserializeObject(properties[x.Name].StringValue, x.PropertyType)));
    }
}
[AttributeUsage(AttributeTargets.Property)]
public class EntityJsonPropertyConverterAttribute : Attribute
{
    public EntityJsonPropertyConverterAttribute()
    {
    }
}
