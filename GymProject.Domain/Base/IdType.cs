namespace GymProject.Domain.Base
{

    /// <summary>
    /// To be used to globally define the type of the Ids
    /// </summary>
    public class IdType
    {

        public long Id { get; private set; }


        public IdType(long id)
        {
            Id = id;
        }


        public override string ToString()
        {
            return Id.ToString();
        }


        public static bool operator ==(IdType left, IdType right) => left.Id == right.Id;

        public static bool operator !=(IdType left, IdType right) => !(left.Id == right.Id);

    }

}
