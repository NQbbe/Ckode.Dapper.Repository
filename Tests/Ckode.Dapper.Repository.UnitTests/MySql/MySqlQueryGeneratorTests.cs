using System;
using Ckode.Dapper.Repository.MySql;
using Ckode.Dapper.Repository.UnitTests.Entities;
using Xunit;

namespace Ckode.Dapper.Repository.UnitTests.MySql
{
	public class QueryGeneratorTests
	{
		#region Constructor

		[Fact]
		public void Constructor_TableNameIsNull_Throws()
		{
			// Arrange, Act && Assert
			Assert.Throws<ArgumentNullException>(() => new MySqlQueryGenerator(null!));
		}


		[Fact]
		public void Constructor_TableNameIsWhiteSpace_Throws()
		{
			// Arrange, Act && Assert
			Assert.Throws<ArgumentException>(() => new MySqlQueryGenerator(" "));
		}
		#endregion

		#region Delete

		[Fact]
		public void GenerateDeleteQuery_OnePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery<SinglePrimaryKeyEntity>();

			// Assert
			Assert.Equal($@"SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;
DELETE FROM Users WHERE Users.Id = @Id;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery<CompositePrimaryKeyEntity>();

			// Assert
			Assert.Equal($@"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;
DELETE FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Orders");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery<CustomColumnNamesEntity>();

			// Assert
			Assert.Equal($@"SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = @Id;
DELETE FROM Orders WHERE Orders.OrderId = @Id;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery<HeapEntity>();

			// Assert
			Assert.Equal($@"SELECT Users.Username, Users.Password FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;
DELETE FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", deleteQuery);
		}
		#endregion

		#region GetAll
		[Fact]
		public void GenerateGetAllQuery_ProperTableName_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var selectQuery = generator.GenerateGetAllQuery<HeapEntity>();

			// Assert
			Assert.Equal($"SELECT Users.Username, Users.Password FROM Users;", selectQuery);
		}

		[Fact]
		public void GenerateGetAllQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Orders");

			// Act
			var selectQuery = generator.GenerateGetAllQuery<CustomColumnNamesEntity>();

			// Assert
			Assert.Equal($"SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders;", selectQuery);
		}
		#endregion

		#region Get
		[Fact]
		public void GenerateGetQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var selectQuery = generator.GenerateGetQuery<SinglePrimaryKeyEntity>();

			// Assert
			Assert.Equal($"SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var selectQuery = generator.GenerateGetQuery<CompositePrimaryKeyEntity>();

			// Assert
			Assert.Equal($"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Orders");

			// Act
			var selectQuery = generator.GenerateGetQuery<CustomColumnNamesEntity>();

			// Assert
			Assert.Equal($"SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var query = generator.GenerateGetQuery<HeapEntity>();

			// Assert
			Assert.Equal("SELECT Users.Username, Users.Password FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", query);
		}
		#endregion

		#region Insert
		[Fact]
		public void GenerateInsertQuery_ColumnHasDefaultConstraintAndDefaultValue_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Actj
			var query = generator.GenerateInsertQuery(new HasDefaultConstraintEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Id) VALUES (@Id);
SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
		}

		[Fact]
		public void GenerateInsertQuery_ColumnHasDefaultConstraintAndNonDefaultValue_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");
			var record = new HasDefaultConstraintEntity
			{
				Id = 42,
				DateCreated = DateTime.Now
			};

			// Act
			var query = generator.GenerateInsertQuery(record);

			// Assert
			Assert.Equal(@"INSERT INTO Users (Id, DateCreated) VALUES (@Id, @DateCreated);
SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
		}

		[Fact]
		public void GenerateInsertQuery_IdentityValuePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password) VALUES (@Username, @Password);
SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = LAST_INSERT_ID();", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_MissingColumnValue_ContainsColumn()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password =  @Password;", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password =  @Password;", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Orders");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CustomColumnNamesEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Orders (DateCreated) VALUES (@Date);
SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = LAST_INSERT_ID();", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new HeapEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password) OUTPUT inserted.Username, inserted.Password VALUES (@Username, @Password);
SELECT Users.Username, Users.Password FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", insertQuery);
		}
		#endregion

		#region Update

		[Fact]
		public void GenerateUpdateQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery<SinglePrimaryKeyEntity>();

			// Assert
			Assert.Equal($"UPDATE Users SET Users.Username = @Username, Users.Password = @Password OUTPUT inserted.Id, inserted.Username, inserted.Password WHERE Users.Id = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery<CompositePrimaryKeyEntity>();

			// Assert
			Assert.Equal($"UPDATE Users SET Users.DateCreated = @DateCreated OUTPUT inserted.Username, inserted.Password, inserted.DateCreated WHERE Users.Username = @Username AND Users.Password = @Password;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Orders");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery<CustomColumnNamesEntity>();

			// Assert
			Assert.Equal($"UPDATE Orders SET Orders.DateCreated = @Date OUTPUT inserted.OrderId AS Id, inserted.DateCreated AS Date WHERE Orders.OrderId = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_NoPrimaryKey_Throws()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery<HeapEntity>());
		}

		[Fact]
		public void GenerateUpdateQuery_AllColumnsHasNoSetter_Throws()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery<AllColumnsHasMissingSetterEntity>());
		}

		[Fact]
		public void GenerateUpdateQuery_ColumnHasNoSetter_ColumnIsExcluded()
		{
			// Arrange
			var generator = new MySqlQueryGenerator("Users");

			// Act
			var query = generator.GenerateUpdateQuery<ColumnHasMissingSetterEntity>();

			// Assert
			Assert.Equal("UPDATE Users SET Users.Age = @Age OUTPUT inserted.Id, inserted.Age, inserted.DateCreated WHERE Users.Id = @Id;", query);
		}
		#endregion
	}
}
