using System.Data.Linq.Mapping;
using SPCore.BusinessData;

namespace SPCore.Examples
{
    public class CustomerRepository: BaseRepository<Customer, System.Data.Linq.DataContext>
    {
        public CustomerRepository(string stringConnection, MappingSource mapping, bool readOnly) : base(stringConnection, mapping, readOnly)
        {
        }
    }
}
