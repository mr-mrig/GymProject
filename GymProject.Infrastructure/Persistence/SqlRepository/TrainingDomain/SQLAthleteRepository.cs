using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLAthleteRepository : IAthleteRepository
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

        public SQLAthleteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public AthleteRoot Add(AthleteRoot athlete)
        {
            return _context.Add(athlete).Entity;
        }


        public AthleteRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();
            Dictionary<uint?, AthleteRoot> lookup = new Dictionary<uint?, AthleteRoot>();

            AthleteRoot res = db.Query(
                " SELECT U.Id, U.CurrentTrainingPlanId, U.CurrentTrainingWeekId," +
                " UPha.StartDate, UPha.EndDate, UPha.TrainingPhaseId as PhaseId, UPha.UserId, UPha.EntryStatusId, UPha.OwnerNote as NoteBody, " +
                " UProf.StartDate, UProf.EndDate, UProf.TrainingProficiencyId as ProficiencyId, UProf.UserId," +
                " UPlan.Id, UPlan.Name, UPlan.IsBookmarked, UPlan.TrainingPlanNoteId, UPlan.ParentPlanId, UPlan.TrainingPlanId," +
                " TPha.TrainingPhaseId, TProf.TrainingProficiencyId, TMus.MuscleGroupId, THash.HashtagId" +
                " FROM User U" +
                " LEFT JOIN UserTrainingPhase UPha" +
                " ON UPha.UserId = U.Id" +
                " LEFT JOIN EntryStatusType EST" +
                " ON UPha.EntryStatusId = EST.Id" +
                " LEFT JOIN UserTrainingProficiency UProf" +
                " ON UProf.UserId = U.Id" +
                " LEFT JOIN UserTrainingPlan UPlan" +
                " ON UPlan.UserId = U.Id" +
                " LEFT JOIN TrainingPlanPhase TPha" +
                " ON TPha.UserTrainingPlanId = UPlan.Id" +
                " LEFT JOIN TrainingPlanProficiency TProf" +
                " ON TProf.UserTrainingPlanId = UPlan.Id" +
                " LEFT JOIN TrainingPlanMuscleFocus TMus" +
                " ON TMus.UserTrainingPlanId = UPlan.Id" +
                " LEFT JOIN TrainingPlanHashtag THash" +
                " ON THash.UserTrainingPlanId = UPlan.Id" +
                " WHERE U.Id = @id",
               types: new[]
               {
                    typeof(AthleteRoot),
                    typeof(UserTrainingPhaseRelation),
                    typeof(long?),
                    typeof(string),
                    typeof(UserTrainingProficiencyRelation),
                    typeof(UserTrainingPlanEntity),
                    typeof(long?),
                    typeof(long?),
                    typeof(long?),
                    typeof(long?),
               },
               map: objects =>
               {
                   AthleteRoot athlete;

                   AthleteRoot ath = objects[0] as AthleteRoot;
                   UserTrainingPhaseRelation userPhase = objects[1] as UserTrainingPhaseRelation;
                   long? phaseStatusId = objects[2] as long?;
                   string phaseNote = objects[3] as string;
                   UserTrainingProficiencyRelation userProf = objects[4] as UserTrainingProficiencyRelation;
                   UserTrainingPlanEntity userPlan = objects[5] as UserTrainingPlanEntity;
                   long? planPhaseId = objects[6] as long?;
                   long? planProfId = objects[7] as long?;
                   long? planMuscleId = objects[8] as long?;
                   long? planHashtagId = objects[9] as long?;

                   if (!lookup.TryGetValue(ath.Id, out athlete))
                       lookup.Add(ath.Id, athlete = ath);

                   // User Phases
                   if (userPhase?.PhaseId != null && !athlete.TrainingPhases.Contains(userPhase))
                   {
                       EntryStatusTypeEnum entryStatus = phaseStatusId.HasValue ? EntryStatusTypeEnum.From((int)phaseStatusId.Value) : null;

                       athlete.AssignTrainingPhase(userPhase.PhaseId.Value, 
                           entryStatus, 
                           userPhase.StartDate, 
                           userPhase.EndDate, 
                           phaseNote == null ? null : PersonalNoteValue.Write(phaseNote));
                   }

                   // User Proficiencies
                   if (userProf?.ProficiencyId != null && !athlete.TrainingProficiencies.Contains(userProf))
                   {
                       if (userProf.EndDate.HasValue)
                           athlete.AssignTrainingProficiency(userProf.ProficiencyId.Value, userProf.StartDate, userProf.EndDate.Value);
                       else
                           athlete.AssignTrainingProficiency(userProf.ProficiencyId.Value, userProf.StartDate);
                   }

                   // User Plan
                   if (userPlan?.Id != null && athlete.TrainingPlans.All(x => x.Id != userPlan.Id))
                       athlete.AddTrainingPlanToLibrary(userPlan);

                   var plan = athlete.CloneTrainingPlanOrDefault(userPlan.TrainingPlanId);

                   if (planPhaseId.HasValue && !plan.HasTargetPhase((uint)planPhaseId.Value))
                       athlete.TagTrainingPlanWithPhase(userPlan.TrainingPlanId, (uint)planPhaseId.Value);

                   if (planProfId.HasValue && !plan.HasTargetProficiency((uint)planProfId.Value))
                       athlete.MarkTrainingPlanAsSuitableForProficiencyLevel(userPlan.TrainingPlanId, (uint)planProfId.Value);

                   if (planMuscleId.HasValue && !plan.DoesFocusOn((uint)planMuscleId.Value))
                       athlete.FocusTrainingPlanOnMuscle(userPlan.TrainingPlanId, (uint)planMuscleId.Value);

                   if (planHashtagId.HasValue && !plan.IsTaggedAs((uint)planHashtagId.Value))
                       athlete.TagTrainingPlanAs(userPlan.TrainingPlanId, (uint)planHashtagId.Value);

                   return athlete;
               },
               param: new { id },
               splitOn: "StartDate, EntryStatusId, NoteBody, StartDate, Id, TrainingPhaseId, TrainingProficiencyId, MuscleGroupId, HashtagId")
           .FirstOrDefault();

            //RIGM: the following really does not work... so all the commands that involve multiple
            // fetches of the same entity in the same context will fail.
            // However, the context should have a per request lifetime, hence this SHOULD not cause practical issues
            // Though, need to take an eye of this...
            //
            // Do not attach to EF context again if already up-to-date
            //if (res != null)
            //{
            //    //var trackedEntry = _context.ChangeTracker.Entries<AthleteRoot>().Any(x => x.Entity.Id == id);
            //    var trackedEntry = _context.ChangeTracker.Entries<AthleteRoot>().SingleOrDefault(x => x.Entity.Id == id);
            //    if (trackedEntry != null)
            //    {
            //        if (trackedEntry.State == EntityState.Unchanged)
            //        {
            //            trackedEntry.State = EntityState.Detached;
            //            _context.Attach(res);
            //        }
            //        else
            //            throw new NotImplementedException("How do we implement this?");
            //    }
            //    else
            //        _context.Attach(res);       // Unfortunately we cannot just _context.Find(id)
            //}

            if (res != null)
                _context.Attach(res);

            return res;
        }

        public AthleteRoot Modify(AthleteRoot athlete)
        {
            return _context.Update(athlete).Entity;
        }


        public void Remove(AthleteRoot athlete)
        {
            _context.Remove(athlete);
        }

        #endregion


        #region IAthleteRepository Implementation

        public int CountAthletesWithTrainingPlanInLibrary(uint trainingPlanId)
        
            => _context.Athletes.SelectMany(x => x.TrainingPlans).Where(x => x.TrainingPlanId == trainingPlanId).Count();

        #endregion


        public AthleteRoot FindWithEF(uint id)
        {
            var res = _context.Find<AthleteRoot>(id);

            if (res != null)
            {
                _context.Entry(res).Collection(x => x.TrainingPhases).Query().Include(x => x.EntryStatus).Load();
                _context.Entry(res).Collection(x => x.TrainingProficiencies).Load();
                _context.Entry(res).Collection(x => x.TrainingPlans).Query()
                    .Include("_trainingPlanPhases")
                    .Include("_trainingPlanProficiencies")
                    .Include("_trainingPlanMuscleFocusIds")
                    .Include("_trainingPlanHashtags")
                    .Load();

            }
            return res;
        }
    }
}
