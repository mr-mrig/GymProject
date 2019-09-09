using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanRepository : ITrainingPlanRepository
    {


        private readonly GymContext _gymContext;


        #region Ctors

        public SQLTrainingPlanRepository(GymContext context)
        {
            _gymContext = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion


        #region IRepository Implementation

        public TrainingPlanRoot Add(TrainingPlanRoot aggregateRoot)
        {
            //_gymContext.Add(aggregateRoot);


            //_gymContext.Add(TrainingPlanRoot.CreateTrainingPlan(IdTypeValue.Create(1), "", true, null));

            //foreach (TrainingWeekEntity week in aggregateRoot.TrainingWeeks)
            //    _gymContext.Add(week);

            throw new NotImplementedException();
        }


        public TrainingPlanRoot Modify(TrainingPlanRoot aggregateRoot)
        {
            _gymContext.Update(aggregateRoot);

            throw new NotImplementedException();
        }


        public void Remove(TrainingPlanRoot aggregateRoot)
        {
            throw new NotImplementedException();
        }


        public TrainingPlanRoot WithId(uint? id)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
