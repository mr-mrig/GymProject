using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingHashtagRepository : ITrainingHashtagRepository
    {


        private readonly GymContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }


        #region Ctors

        public SQLTrainingHashtagRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingHashtagRoot Add(TrainingHashtagRoot hashtag)
        {
            return _context.Add(hashtag).Entity;
        }


        public TrainingHashtagRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();

            TrainingHashtagRoot res = db.Query<TrainingHashtagRoot, string, long?, TrainingHashtagRoot>(
                "SELECT Id, Body, EntryStatusId " +
                " FROM TrainingHashtag " +
                " WHERE Id = @id",
               (hashtag, body, entryStatusId) =>
               {
                   return TrainingHashtagRoot.TagWith(hashtag.Id,
                       GenericHashtagValue.TagWith(body),
                       entryStatusId.HasValue ? EntryStatusTypeEnum.From((int)entryStatusId.Value) : null);
               },
               param: new { id },
               splitOn: "Body, EntryStatusId")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

            return res;
        }


        public TrainingHashtagRoot Modify(TrainingHashtagRoot hashtag)
        {
            return _context.Update(hashtag).Entity;
        }


        public void Remove(TrainingHashtagRoot hashtag)
        {
            _context.Remove(hashtag);
        }
        #endregion

    }
}
