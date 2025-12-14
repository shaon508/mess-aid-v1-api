using System.Linq.Expressions;

namespace MassAidVOne.Application.Interfaces
{
    public interface ICustomRepository
    {
        Task<int> UpdateRangeSelectAsyn(string tableName, Dictionary<string, object> primaryKeys, Dictionary<string, object> updateColumns);
    }
}
