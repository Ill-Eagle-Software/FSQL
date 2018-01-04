using System.Collections.Generic;
using System.Threading.Tasks;
using FSQL.ProgramParts.FSQLStructs;

namespace FSQL.Interfaces {
    public interface ITable : IActiveRecord
    {
    }

    public interface ITable<T> : ITable {
        IEnumerable<T> Rows { get; }
        Task<IEnumerable<T>> GetRowsAsync();
        T CurrentRecord { get; }        
    }
}