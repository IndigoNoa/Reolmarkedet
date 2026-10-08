using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Repositories
{
	// Samlet sted for forbindelsesstrengen, så den kun skal rettes ét sted
	public static class DatabaseConfig
	{
		public const string ConnectionString =
			"Server=localhost;Database=Reolmarkedet;Trusted_Connection=True;TrustServerCertificate=True;";
	}
}
