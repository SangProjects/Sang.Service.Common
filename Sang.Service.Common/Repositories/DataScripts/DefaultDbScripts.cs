namespace Sang.Service.Common.Repositories.DataScripts
{
    public static class DefaultDbScripts
    {
        public static string GetConnectionStringSql() => "select sConnection from [dbo].[t_s_Database] where sDatabase=@sDatabase";
        public static string GetDatabaseSql() => "select iId as Id,sDatabase as Company from [dbo].[t_s_Database]";
        public static string GetDatabaseProcedure() => "[xsp_s_GetDatabase]";
    }
}
