using System.Data;
using Moq;
using Vega.DbUpgrade.Databases;
using Vega.DbUpgrade.DbProviders;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;
using Xunit;

namespace Vega.DbUpgrade.Tests
{
    public class TestMySqlDbProvider
    {
        /// <summary>
        /// Tests script for creating change log table
        /// </summary>
        [Fact]
        public void MySql_Test_GetChangeLogTableScript()
        {
            IDbProvider provider = new MySqlDbProvider(new MySqlDatabase());
            var actualString = provider.GetChangeLogTableScript();
            const string expectedString = Constants.ChangeLogTableScripts.MySqlChangeLogTable;

            Assert.Equal(expectedString, actualString);
        }

        /// <summary>
        /// Tests execution of a script which contains single SQL command 
        /// </summary>
        [Fact]
        public void MySql_Test_ExecuteScripts_FileContentExecuted()
        {
            const string fileContent = "MySQL Script";
            const bool expectedResult = true;

            var dbConnectionMock = new Mock<IDbConnection>();
            var databaseMock = new Mock<IDatabase>();
            var dbCommandMock = new Mock<IDbCommand>();

            databaseMock.Setup(t => t.GetDbConnection()).Returns(dbConnectionMock.Object);
            databaseMock.Setup(t => t.GetDbCommand(fileContent, dbConnectionMock.Object)).Returns(dbCommandMock.Object);
            
            dbConnectionMock.Setup(t => t.Open());
            dbCommandMock.Setup(t => t.ExecuteNonQuery());

            IDbProvider provider = new MySqlDbProvider(databaseMock.Object);
            var actualResult = provider.ExecuteScript(fileContent);

            dbCommandMock.VerifyAll();
            databaseMock.VerifyAll();
            dbCommandMock.VerifyAll();
            
            Assert.Equal(expectedResult, actualResult);
        }

        /// <summary>
        /// Tests execution of a script which contains multiple SQL commands
        /// </summary>
        [Fact]
        public void MySql_Test_ExecuteScripts_TwoFileContent()
        {
            const string fileContent = @"create table BackendUser (BackendUserId int, Username nvarchar(30)); insert into BackendUser values(1, ""I must go home.""));";
            const bool expectedResult = true;

            var dbConnectionMock = new Mock<IDbConnection>();
            var databaseMock = new Mock<IDatabase>();
            var dbCommandMock = new Mock<IDbCommand>();

            databaseMock.Setup(t => t.GetDbConnection()).Returns(dbConnectionMock.Object);
            databaseMock.Setup(t => t.GetDbCommand(fileContent, dbConnectionMock.Object)).Returns(dbCommandMock.Object);

            dbConnectionMock.Setup(t => t.Open());
            dbCommandMock.Setup(t => t.ExecuteNonQuery());

            IDbProvider provider = new MySqlDbProvider(databaseMock.Object);
            var actualResult = provider.ExecuteScript(fileContent);

            dbCommandMock.VerifyAll();
            databaseMock.VerifyAll();
            dbCommandMock.VerifyAll();

            Assert.Equal(expectedResult, actualResult);
        }
    }
}