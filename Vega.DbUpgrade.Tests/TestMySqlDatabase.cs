using Vega.DbUpgrade.Databases;
using Xunit;

namespace Vega.DbUpgrade.Tests
{
    public class TestMySqlDatabase
    {
        [Fact]
        public void MySqlDatabase_ReadConnectionString()
        {
            MySqlDatabase database = new MySqlDatabase();
            Assert.NotNull(database.DbConnectionString);
        }
    }
}
