namespace GymProject.Domain.Base
{

    /// <summary>
    /// It's just a label to identify Enities or Value Objects storing many-to-many relations
    /// This is needed to ensure EF Core consitence
    /// </summary>
    public class RelationEntity : Entity<uint?>
    {


        public RelationEntity() : base(null)
        {

        }
    }
}
