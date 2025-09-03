using System.Text;

namespace BuilderSQLQuery
{ 

	public class SQLQuery
	{
		// Атрибут
		public string Query { get; set; }
		
		public SQLQuery(string query) { 
			Query = query;
		}

		public string GetQuery() {
			return Query;
		}

		public override string ToString() { 
			return Query.ToString();
		}

	}
	
	
	public interface IQueryBuilder
	{
		IQueryBuilder Select(string columns);
		IQueryBuilder From(string table);
		IQueryBuilder Where(string condition);
		IQueryBuilder OrderBy(string columns);
		IQueryBuilder Join(string joinClause);
		SQLQuery Build();
	}

	public class StandardQueryBuilder : IQueryBuilder
	{

		private StringBuilder _query;
		private bool _hasWhere;
		private bool _hasOrderBy;

		public StandardQueryBuilder()
		{
			_query = new StringBuilder();
			_hasWhere = false;
			_hasOrderBy = false;
		}


		public IQueryBuilder Select(string columns)
		{
			_query.Append($"SELECT {columns} ");
			return this;
		}

		public IQueryBuilder From(string table)
		{
			_query.Append($"FROM {table} ");
			return this;
		}

		public IQueryBuilder Where(string condition)
		{
			if (!_hasWhere) {
				_query.Append($"WHERE {condition} ");
				_hasWhere = true;
			}  else {
				_query.Append($"AND {condition} ");
			}
			return this;
		}

		public IQueryBuilder OrderBy(string columns)
		{
			if (!_hasOrderBy)
			{
				_query.Append($"ORDER BY {columns} ");
				_hasOrderBy = true;
			}
			else
			{
				_query.Append($", {columns}");
			}
			return this;
		}

		public IQueryBuilder Join(string joinClause)
		{
			_query.AppendLine($"JOIN {joinClause}");
			return this;
		}


		public SQLQuery Build()
		{
			return new SQLQuery( _query.ToString() );
		}
	}

	public class QueryDirector
	{
		private IQueryBuilder _builder;

		public QueryDirector(IQueryBuilder queryBuilder) { _builder = queryBuilder; }

		public void ConstructUserByIdQuery(int userid)
		{
			_builder.Select("*")
					.From("users")
					.Where($"id = {userid}");
		}

		public void ConstructUserByIdQuery()
		{
			_builder.Select("u.id, u.name, u.email, p.phone")
					.From("users u")
					.Join("profiles p ON u.id = p.user_id")
					.Where("u.age > 30")
					.Where("u.active = 1")
					.OrderBy("u.name ASC");
		}

		public SQLQuery GetSQLQuery() {
			return _builder.Build();
		}
	}

	class Program
	{
		static void Main(string[] args)
		{

			var builder = new StandardQueryBuilder();
			var flexibleQuery = builder.Select("u.id, u.name, u.email, p.phone")
										.From("users u")
										.Join("profiles p ON u.id = p.user_id")
										.Where("u.age > 30")
										.Where("u.active = 1")
										.OrderBy("u.name ASC")
										.Build();


			var complexBuild = new StandardQueryBuilder();
			var director = new QueryDirector(complexBuild);

			director.ConstructUserByIdQuery();
			var q1 = director.GetSQLQuery();

			Console.WriteLine(q1.Query);



		}


	}

}
