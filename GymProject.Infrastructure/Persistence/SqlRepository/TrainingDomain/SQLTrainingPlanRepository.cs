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

        public TrainingPlan Add(TrainingPlan aggregateRoot)
        {
            _gymContext.Add(aggregateRoot);


            _gymContext.Add(TrainingPlan.CreateTrainingPlan(IdTypeValue.Create(1), "", true, true, null));

            foreach (TrainingWeekTemplate week in aggregateRoot.TrainingWeeks)
                _gymContext.Add(week);

            throw new NotImplementedException();
        }


        public TrainingPlan Modify(TrainingPlan aggregateRoot)
        {

            foreach (TrainingWeekTemplate week in aggregateRoot.TrainingWeeks)
            {
                if(week.State == Modified)
                    // Change it
            }

            throw new NotImplementedException();
        }


        public void Remove(TrainingPlan aggregateRoot)
        {
            throw new NotImplementedException();
        }


        public TrainingPlan WithId(IdTypeValue id)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
