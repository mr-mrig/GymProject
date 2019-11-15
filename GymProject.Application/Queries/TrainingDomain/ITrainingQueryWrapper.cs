using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymProject.Application.Queries.TrainingDomain
{
    public interface ITrainingQueryWrapper
    {

        /// <summary>
        /// Get the summaries of all the Training Plans belonging to the specified user
        /// </summary>
        /// <param name="userId">The ID of the user to be searched</param>
        /// <returns>The Training Plan Summary DTO list</returns>
        Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummaries(uint ownerId);


        /// <summary>
        /// Get the plan over the training weeks for the specified workout day.
        /// In order to query for a specific training plan, the caller should already have all its training weeks and workouts names fetched, if not another query is needed.
        /// Furthermore, notice that the weeks IDs should be ordered according to the progressive number, otherwise further post-processing is needed.
        /// </summary>
        /// <param name="trainingWeekIds">The IDs of the Training Weeks belonging to the Training Plan to be queried, ordered by their progressive number.</param>
        /// <param name="workoutName">The name of the workout to be queried, which is unique with respect to the training week</param>
        /// <returns>The WorkoutFullPlanDto list</returns>
        Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutDays(List<uint> trainingWeekIds, string workoutName);


        /// <summary>
        /// Get all the Feedbacks - full detailed - of the Training Plans which are direct childs of the specified Training Plan.
        /// A training plan is considered direct child if:
        ///    1. it is the parent plan of the query
        ///    2. it is a direct variant of the parent plan
        ///    3. it is directly inherited from the parent plan
        ///    4. it is directly inherited of the 2. plans
        /// Direct childs of the selected Plan which have never been scheduled are still returned
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Root Training Plan</param>
        /// <param name="userId">The ID of the user who is querying - This is needed n order to skip a JOIN</param>
        /// <returns>The FullFeedbackDetailDto list</returns>
        Task<IEnumerable<FullFeedbackDetailDto>> GetFullFeedbacksDetails(uint trainingPlanId, uint userId);


        /// <summary>
        /// Get the main details of the selected Training Plan.
        /// These include all the fields not fethced in the <see cref="TrainingPlanSummaryDto"/> excluding:
        ///
        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
        ///
        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
        ///
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        Task<IEnumerable<TrainingPlanDetailDto>> GetTrainingPlanDetails(uint trainingPlanId);


        /// <summary>
        /// Get the all the details of the selected Training Plan, when no pre-fetch data is available, namely no  <see cref="TrainingPlanSummaryDto"/> in memory.
        /// No data about Workouts and Feedbaks/Variants are loaded, please refer to:
        ///
        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
        ///
        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
        ///
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        Task<TrainingPlanFullDetailDto> GetTrainingPlan(uint trainingPlanId);


    }
}
