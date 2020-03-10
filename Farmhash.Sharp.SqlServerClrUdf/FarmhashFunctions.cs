using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using static Farmhash.Sharp.FarmhashSafe;

public class FarmhashFunctions
{
    [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
    public static SqlBinary Farmhash32(object input)
    {
        if (input is DBNull)
            return SqlBinary.Null;

        switch (input)
        {
            case SqlBytes bytes:
                var array = bytes.Buffer;
                var byteHash = Hash32(array, array.Length);
                return BitConverter.GetBytes(byteHash);

            case SqlString str:
                var stringHash = Hash32(str.Value);
                return BitConverter.GetBytes(stringHash);

            default:
                throw new ArgumentException($"Parameter must be either byte[] (VARBINARY) or string (NVARCHAR). Type '{input.GetType()}' is not supported.", nameof(input));
        }
    }

    [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
    public static SqlBinary StringFarmhash32(SqlString input)
    {
        if (input.IsNull)
            return SqlBinary.Null;

        var stringHash = Hash32(input.Value);

        return BitConverter.GetBytes(stringHash);
    }

    [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
    public static SqlBinary BinaryFarmhash32(SqlBytes input)
    {
        if (input.IsNull)
            return SqlBinary.Null;

        var array = input.Buffer;
        var byteHash = Hash32(array, array.Length);

        return BitConverter.GetBytes(byteHash);
    }

    [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
    public static SqlBinary Farmhash64(object input)
    {
        if (input is DBNull)
            return SqlBinary.Null;

        switch (input)
        {
            case SqlBytes bytes:
                var array = bytes.Buffer;
                var byteHash = Hash64(array, array.Length);
                return BitConverter.GetBytes(byteHash);

            case SqlString str:
                var stringHash = Hash64(str.Value);
                return BitConverter.GetBytes(stringHash);

            default:
                throw new ArgumentException($"Parameter must be either SqlBytes (VARBINARY) or SqlString (NVARCHAR). Type '{input.GetType()}' is not supported.", nameof(input));
        }
    }
    
    [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
    public static SqlBinary StringFarmhash64(SqlString input)
    {
        if (input.IsNull)
            return SqlBinary.Null;

        var stringHash = Hash64(input.Value);

        return BitConverter.GetBytes(stringHash);
    }

    [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
    public static SqlBinary BinaryFarmhash64(SqlBytes input)
    {
        if (input.IsNull)
            return SqlBinary.Null;

        var array = input.Buffer;
        var byteHash = Hash64(array, array.Length);

        return BitConverter.GetBytes(byteHash);
    }
}