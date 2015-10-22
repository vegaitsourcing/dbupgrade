using System.Data;
using Moq;
using Vega.DbUpgrade.Databases;
using Vega.DbUpgrade.DbProviders;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;
using Xunit;

namespace Vega.DbUpgrade.Tests
{
    public class TestMsSqlDbProvider
    {
        /// <summary>
        /// Tests script for creating change log table
        /// </summary>
        [Fact]
        public void Test_GetChangeLogTableScript()
        {
            IDbProvider provider = new MsSqlDbProvider(new MySqlDatabase());
            var actualString = provider.GetChangeLogTableScript();
            const string expectedString = Constants.ChangeLogTableScripts.MsSqlChangeLogTable;

            Assert.Equal(expectedString, actualString);
        }

        /// <summary>
        /// Tests execution of a script which contains single SQL command 
        /// </summary>
        [Fact]
        public void Test_ExecuteScripts_OneFileContent_WithoutGo()
        {
            const string fileContent = "MsSQL Script";
            const bool expectedResult = true;

            var dbConnectionMock = new Mock<IDbConnection>();
            var databaseMock = new Mock<IDatabase>();
            var dbCommandMock = new Mock<IDbCommand>();

            databaseMock.Setup(t => t.GetDbConnection()).Returns(dbConnectionMock.Object);
            databaseMock.Setup(t => t.GetDbCommand()).Returns(dbCommandMock.Object);

            dbConnectionMock.Setup(t => t.Open());
            dbCommandMock.Setup(t => t.ExecuteNonQuery());

            IDbProvider provider = new MsSqlDbProvider(databaseMock.Object);
            var actualResult = provider.ExecuteScript(fileContent);

            dbCommandMock.VerifyAll();
            databaseMock.VerifyAll();
            dbCommandMock.VerifyAll();

            Assert.Equal(expectedResult, actualResult);
        }

        /// <summary>
        /// Tests execution of a script which contains multiple SQL commands, separated by GO command
        /// </summary>
        [Fact]
        public void Test_ExecuteScripts_TwoFileContent_WithGo()
        {
            const string fileContent = @"create table BackendUser (BackendUserId int, Username nvarchar(30))
                                  go
                                  create table Company (CompanyId int, Name nvarchar(30))
                                 go";
            const bool expectedResult = true;

            var dbConnectionMock = new Mock<IDbConnection>();
            var databaseMock = new Mock<IDatabase>();
            var dbCommandMock = new Mock<IDbCommand>();

            databaseMock.Setup(t => t.GetDbConnection()).Returns(dbConnectionMock.Object);
            databaseMock.Setup(t => t.GetDbCommand()).Returns(dbCommandMock.Object);

            dbConnectionMock.Setup(t => t.Open());
            dbCommandMock.Setup(t => t.ExecuteNonQuery());

            IDbProvider provider = new MsSqlDbProvider(databaseMock.Object);
            var actualResult = provider.ExecuteScript(fileContent);

            dbCommandMock.VerifyAll();
            databaseMock.VerifyAll();
            dbCommandMock.VerifyAll();

            Assert.Equal(expectedResult, actualResult);
        }

        /// <summary>
        /// Tests execution of a script which contains multiple SQL commands, separated by GO command, also contains word "GO" within SQL command
        /// </summary>
        [Fact]
        public void Test_ExecuteScripts_TwoFileContent_WithGoAsValue()
        {
            const string fileContent = @"create table BackendUser (BackendUserId int, Username nvarchar(30))
                                  go
                                  insert into BackendUser values(1, ""I must go home.""))
                                 go";
            const bool expectedResult = true;

            var dbConnectionMock = new Mock<IDbConnection>();
            var databaseMock = new Mock<IDatabase>();
            var dbCommandMock = new Mock<IDbCommand>();

            databaseMock.Setup(t => t.GetDbConnection()).Returns(dbConnectionMock.Object);
            databaseMock.Setup(t => t.GetDbCommand()).Returns(dbCommandMock.Object);

            dbConnectionMock.Setup(t => t.Open());
            dbCommandMock.Setup(t => t.ExecuteNonQuery());

            IDbProvider provider = new MsSqlDbProvider(databaseMock.Object);
            var actualResult = provider.ExecuteScript(fileContent);

            dbCommandMock.VerifyAll();
            databaseMock.VerifyAll();
            dbCommandMock.VerifyAll();

            Assert.Equal(expectedResult, actualResult);
        }
    }
}