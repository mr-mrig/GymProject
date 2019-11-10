using System;
using System.Diagnostics.CodeAnalysis;

namespace GymProject.Application.Queries.Base
{
    public class BaseIdentifiedDto : IEquatable<BaseIdentifiedDto>, IEquatable<uint>
    {


        public uint Id { get; set; }



        public bool Equals([AllowNull] BaseIdentifiedDto other)
        {
            return other != null && other.Id.Equals(Id);
        }


        public bool Equals([AllowNull] uint otherId)
        {
            return Id.Equals(otherId);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseIdentifiedDto))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            BaseIdentifiedDto item = (BaseIdentifiedDto)obj;

            return Id.Equals(item.Id);
        }


        public override int GetHashCode()
        {
            if (Id == 0)
                return base.GetHashCode();

            else
                return Id.GetHashCode() ^ 31;
        }

    }
}
