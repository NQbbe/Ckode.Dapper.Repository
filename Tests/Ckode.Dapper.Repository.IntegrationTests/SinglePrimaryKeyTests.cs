using System;
using System.Data.SqlClient;
using System.Linq;
using Ckode.Dapper.Repository.IntegrationTests.Entities;
using Ckode.Dapper.Repository.IntegrationTests.Repositories;
using Xunit;

namespace Ckode.Dapper.Repository.IntegrationTests
{
	public class SinglePrimaryKeyTests
	{
		#region Delete
		[Fact]
		public void Delete_InputIsNull_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => repository.Delete(null!));
		}

		[Fact]
		public void Delete_PrimaryKeyNotEntered_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act && Assert
			Assert.Throws<ArgumentException>(() => repository.Delete(entity));
		}

		[Fact]
		public void Delete_UseMissingPrimaryKeyValue_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<NoEntityFoundException>(() => repository.Delete(new CategoryPrimaryKeyEntity { CategoryId = int.MaxValue }));
		}

		[Fact]
		public void Delete_UsePrimaryKey_Valid()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};
			var insertedEntity = repository.Insert(entity);

			// Act
			var deleted = repository.Delete(new CategoryPrimaryKeyEntity { CategoryId = insertedEntity.CategoryId });

			// Assert
			Assert.Equal(insertedEntity.CategoryId, deleted.CategoryId);
			Assert.Equal(entity.Description, deleted.Description);
			Assert.Equal(entity.Name, deleted.Name);
			Assert.Equal(entity.Picture, deleted.Picture);
		}

		[Fact]
		public void Delete_UseEntity_Valid()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};
			var insertedEntity = repository.Insert(entity);

			// Act
			var deleted = repository.Delete(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity.CategoryId, deleted.CategoryId);
			Assert.Equal(entity.Description, deleted.Description);
			Assert.Equal(entity.Name, deleted.Name);
			Assert.Equal(entity.Picture, deleted.Picture);
		}
		#endregion

		#region Get
		[Fact]
		public void Get_InputIsNull_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => repository.Get(null!));
		}

		[Fact]
		public void Get_UsePrimaryKey_Valid()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};
			var insertedEntity = repository.Insert(entity);

			// Act
			var fetchedEntity = repository.Get(new CategoryPrimaryKeyEntity { CategoryId = insertedEntity.CategoryId });

			// Assert
			Assert.Equal(insertedEntity.Name, fetchedEntity.Name);
			Assert.Equal(insertedEntity.Description, fetchedEntity.Description);
			Assert.Equal(insertedEntity.Picture, fetchedEntity.Picture);

			repository.Delete(insertedEntity);
		}

		[Fact]
		public void Get_UseFullEntity_Valid()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};
			var insertedEntity = repository.Insert(entity);

			// Act
			var fetchedEntity = repository.Get(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity.Description, fetchedEntity.Description);
			Assert.Equal(insertedEntity.Name, fetchedEntity.Name);
			Assert.Equal(insertedEntity.Picture, fetchedEntity.Picture);

			repository.Delete(insertedEntity);
		}

		[Fact]
		public void Get_PrimaryKeyNotEntered_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<ArgumentException>(() => repository.Get(new CategoryPrimaryKeyEntity { }));
		}

		[Fact]
		public void Get_UseMissingPrimaryKey_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<NoEntityFoundException>(() => repository.Get(new CategoryPrimaryKeyEntity { CategoryId = int.MaxValue }));
		}
		#endregion

		#region GetAll
		[Fact]
		public void GetAll_NoInput_Valid()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act
			var fetchedEntities = repository.GetAll();

			// Assert
			Assert.True(fetchedEntities.Count() > 0);
		}
		#endregion

		#region Insert
		[Fact]
		public void Insert_InputIsNull_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => repository.Insert(null!));
		}

		[Fact]
		public void Insert_HasIdentityKeyWithValue_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				CategoryId = 42,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};

			// Act && Assert
			Assert.Throws<ArgumentException>(() => repository.Insert(entity));
		}

		[Fact]
		public void Insert_HasIdentityKeyWithoutValue_IsInserted()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};

			// Act
			var insertedEntity = repository.Insert(entity);
			try
			{
				// Assert
				Assert.NotEqual(default, insertedEntity.CategoryId);
				Assert.Equal(entity.Description, insertedEntity.Description);
				Assert.Equal(entity.Name, insertedEntity.Name);
				Assert.Equal(entity.Picture, insertedEntity.Picture);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}

		[Fact]
		public void Insert_NonNullPropertyMissing_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = null!,
				Picture = null
			};

			// Act && Assert
			Assert.Throws<SqlException>(() => repository.Insert(entity));
		}
		#endregion

		#region Update
		[Fact]
		public void Update_InputIsNull_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();

			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => repository.Update(null!));
		}

		[Fact]
		public void Update_UseEntity_Valid()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};
			var insertedEntity = repository.Insert(entity);

			var update = insertedEntity with { Description = "Something else" };

			// Act
			var updatedEntity = repository.Update(update);

			// Assert
			Assert.Equal("Something else", updatedEntity.Description);

			repository.Delete(insertedEntity);
		}

		[Fact]
		public void Update_PrimaryKeyNotEntered_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act && Assert
			Assert.Throws<ArgumentException>(() => repository.Update(entity));
		}

		[Fact]
		public void Update_UseMissingPrimaryKeyValue_Throws()
		{
			// Arrange
			var repository = new CategoryRepository();
			var entity = new CategoryEntity
			{
				CategoryId = int.MaxValue,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act && Assert
			Assert.Throws<NoEntityFoundException>(() => repository.Update(entity));
		}
		#endregion
	}
}
