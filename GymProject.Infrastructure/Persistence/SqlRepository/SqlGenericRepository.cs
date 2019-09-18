using GymProject.Domain.Base;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository
{
    public class SqlGenericRepository : IRepository<IAggregateRoot>
    {


        private readonly GymContext _context;



        public SqlGenericRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }




        public IAggregateRoot Add(IAggregateRoot aggregateRoot)
        {
            _context.Add(aggregateRoot);
            return aggregateRoot;
        }


        public IAggregateRoot Modify(IAggregateRoot aggregateRoot)
        {
            _context.Update(aggregateRoot);
            return aggregateRoot;
        }

        public void Remove(IAggregateRoot aggregateRoot)
        {
            _context.Remove(aggregateRoot);
        }

    }
}
