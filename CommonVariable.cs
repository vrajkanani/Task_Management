namespace Task_Management
{
    public class CommonVariable
    {
        private static IHttpContextAccessor _HiddenContextAccessor;

        static CommonVariable()
        {
            _HiddenContextAccessor = new HttpContextAccessor();
        }

        public static int? UserID()
        {
            if (_HiddenContextAccessor.HttpContext.Session.GetString("Random_UserKey") == null)
            {
                return null;
            }

            return Convert.ToInt32(_HiddenContextAccessor.HttpContext.Session.GetString("Random_UserKey"));
        }

        public static string UserName()
        {
            if (_HiddenContextAccessor.HttpContext.Session.GetString("Random_NameKey") == null)
            {
                return null;
            }

            return _HiddenContextAccessor.HttpContext.Session.GetString("Random_NameKey");
        }

        public static bool? Role()
        {
            if (_HiddenContextAccessor.HttpContext.Session.GetString("Random_RoleKey") == null)
            {
                return null;
            }

            return Convert.ToBoolean(_HiddenContextAccessor.HttpContext.Session.GetString("Random_RoleKey"));
        }
    }
}
