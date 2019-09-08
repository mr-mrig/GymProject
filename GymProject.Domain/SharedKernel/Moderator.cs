using GymProject.Domain.Base;


namespace GymProject.Domain.SharedKernel
{
    public class Moderator : Entity<uint?>
    {


        public UsernameValue Username { get; private set; }



        private Moderator(string username) : base(null)
        {
            Username = UsernameValue.Register(username);
        }


        public static Moderator Register(string username)
        {
            return new Moderator(username);
        }

    }
}
