namespace Urban.AI.Application.Common.Abstractions.Data;

#region Usings
using System.Data; 
#endregion

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}