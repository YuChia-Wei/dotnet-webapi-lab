using System.Data;
using dotnetLab.Repository.Enums;

namespace dotnetLab.Repository.Factories;

/// <summary>
/// db connection factory
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// 建立連線物件
    /// </summary>
    /// <param name="dbInstanceEnum">資料庫實體資訊</param>
    /// <returns></returns>
    IDbConnection CreateConnection(DbInstanceEnum dbInstanceEnum);
}