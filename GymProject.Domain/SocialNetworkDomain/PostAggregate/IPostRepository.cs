using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class IPostRepository : IRepository<Post>
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public void Add(Post entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> FindAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(Post entity)
        {
            throw new NotImplementedException();
        }
    }
}
