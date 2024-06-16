namespace Proyectonext.Models
{
    public class Users
    {

        public int IdUser { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
        
        public string Mail { get; set; }

        public bool Active { get; set; }
    }
}
