using Ckode.Dapper.Repository.Attributes;

namespace Ckode.Dapper.Repository.IntegrationTests.Entities
{
	public record ProductListViewEntity : DapperEntity
	{
		[Column]
		public int ProductID { get; }

		[Column]
		public string ProductName { get; } = default!;
	}
}
