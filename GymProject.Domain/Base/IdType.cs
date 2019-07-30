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




        /// <summary>
        /// Overridden method, prints the Id as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(object obj) => obj is IdType type && Id == type.Id;

        public override int GetHashCode() => base.GetHashCode();



        public static bool operator ==(IdType left, IdType right) => left?.Id == right?.Id;

        public static bool operator !=(IdType left, IdType right) => !(left?.Id == right?.Id);

    }

}
