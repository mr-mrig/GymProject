using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    public class DatabaseSeedService
    {

        private GymContext _context;



        public DatabaseSeedService(GymContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }






        public async Task AddTrainingPlanToUserLibrary(AthleteRoot athlete, uint planId, string planName, bool isBookmarked, uint? parentPlanId, uint? planNoteId = null,
            IEnumerable<uint> hashtags = null, IEnumerable<uint> proficiencies = null, IEnumerable<uint> phases = null, IEnumerable<uint> muscleFocuses = null)
        {
            athlete.AddTrainingPlanToLibrary(planId);

            _context.Update(athlete);
            await _context.SaveAsync();

            athlete.RenameTrainingPlan(planId, planName);
            athlete.MakeTrainingPlanBookmarked(planId, isBookmarked);
            athlete.AttachTrainingPlanNote(planId, planNoteId);

            if (parentPlanId != null)
                athlete.MakeTrainingPlanVariantOf(planId, parentPlanId.Value);

            hashtags = hashtags ?? new List<uint>();
            phases = phases ?? new List<uint>();
            proficiencies = proficiencies ?? new List<uint>();
            muscleFocuses = muscleFocuses ?? new List<uint>();

            foreach (uint id in hashtags)
                athlete.TagTrainingPlanAs(planId, id);

            foreach (uint id in phases)
                athlete.TagTrainingPlanWithPhase(planId, id);

            foreach (uint id in proficiencies)
                athlete.MarkTrainingPlanAsSuitableForProficiencyLevel(planId, id);

            foreach (uint id in muscleFocuses)
                athlete.FocusTrainingPlanOnMuscle(planId, id);

            _context.Update(athlete);
            await _context.SaveAsync();
        }

        
        public async Task<TrainingScheduleRoot> ScheduleTrainingPlan(AthleteRoot athlete, uint userPlanId, DateTime startDate, DateTime? endDate = null,
            IEnumerable<TrainingScheduleFeedbackEntity> feedbacks = null)
        {
            DateRangeValue schedulePeriod = endDate.HasValue 
                ? DateRangeValue.RangeBetween(startDate, endDate.Value)
                : DateRangeValue.RangeStartingFrom(startDate);

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(null, userPlanId, schedulePeriod);
            //TrainingScheduleFeedbackEntity feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(4), null);

            foreach(TrainingScheduleFeedbackEntity feedback in feedbacks ?? new List<TrainingScheduleFeedbackEntity>())
                schedule.ProvideFeedback(feedback);

            _context.Add(schedule);
            await _context.SaveAsync();

            athlete.ScheduleTraining(userPlanId, schedule.Id.Value);

            return schedule;
        }

       

    }
}
