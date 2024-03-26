namespace Sang.Service.Common.Repositories.DataScripts
{
    public static class UserScripts
    {
        public static string GetUserByNameSql() => "SELECT * FROM t_s_Users WHERE sUsername=@sUsername";  
    }
}
