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
        Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummariesAsync(uint ownerId);


        /// <summary>
        /// Get the plan over the training weeks for the specified workout day.
        /// In order to query for a specific training plan, the caller should already have all its training weeks and workouts names fetched, if not another query is needed.
        /// Furthermore, notice that the weeks IDs should be ordered according to the progressive number, otherwise further post-processing is needed.
        /// </summary>
        /// <param name="trainingWeekIds">The IDs of the Training Weeks belonging to the Training Plan to be queried, ordered by their progressive number.</param>
        /// <param name="workoutName">The name of the workout to be queried, which is unique with respect to the training week</param>
        /// <returns>The WorkoutFullPlanDto list</returns>
        Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutAsync(IList<uint> trainingWeekIds, string workoutName);


        /// <summary>
        /// Get the plan over the training weeks for the specified workout day, assuming the Workout IDs are already known, which should be the case.
        /// In order to query for a specific training plan, the caller should already have all its Workout IDs fetched, IE: <cref="GetWorkoutsWeeklyScheduleAsync"> should hav been called previously.
        /// Furthermore, notice that the Workouts IDs should be ordered according to the Week progressive number, otherwise further post-processing is needed.
        /// </summary>
        /// <param name="workoutsIds">The IDs of the Workouts to be queried</param>
        /// <returns>The WorkoutFullPlanDto list</returns>
        Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutAsync(IEnumerable<uint> workoutsIds);


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
        Task<IEnumerable<FullFeedbackDetailDto>> GetFullFeedbacksDetailsAsync(uint trainingPlanId, uint userId);


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
        Task<IEnumerable<TrainingPlanDetailDto>> GetTrainingPlanDetailsAsync(uint trainingPlanId);


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
        Task<TrainingPlanFullDetailDto> GetTrainingPlanFullDetailsAsync(uint trainingPlanId);


        /// <summary>
        /// Get the Workouts schedule over the planned Training Weeks.
        /// This query is redundant if all the <cref="GetTraininPlanPlannedWorkoutAsync"> or the single <cref="GetTraininPlanAllPlannedWorkoutsAsync"> query has been executed for each Workout Day
        ///
        /// </summary>
        /// <param name="trainingWeekIds">The IDs of the planned Training Weeks</param>
        /// <returns>The Training Plan Details DTO</returns>
        Task<TrainingPlanWorkoutsScheduleDto> GetWorkoutsWeeklyScheduleAsync(IEnumerable<uint> trainingWeekIds);


        /// <summary>
        /// Get the plan over the training weeks for all the Workouts belonging to the specified Training Weeks. No info about the Training Weeks is fetched.
        /// Please notice that in order to use this query to search for the Training Plan Workouts, the Training Weeks should have been fetched before - however if additional week info is required a more specific query is suggested.
        /// <param name="trainingWeeksIds">The IDs of the Training Weeks to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        Task<IEnumerable<WorkoutFullPlanDto>> GetTrainingPlanAllPlannedWorkoutsAsync(IEnumerable<uint> trainingWeeksIds);


        /// <summary>
        /// Get the Workouts scheduled for the specified Training Week fetching all the WOs data, not only the main info.
        /// This query might be useful when loading the schedule for the current Training Week. Three cases:
        ///   - SessionStarted, SessionEnded are null -> Workout still to be done
        ///   - SessionStarted, SessionEnded not null -> Workout completed
        ///   - SessionStarted not null, SessionEnded is null -> Workout not completed
        /// <param name="trainingWeekId">The IDs of the Training Week to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        Task<TrainingWeekWorkoutsDetailsDto> GetTrainingWeekPlannedWorkoutsDetailsAsync(uint trainingWeekId);



        /// <summary>
        /// Get the details for the specified Workout Session - IE: the performed excercises
        /// <param name="workoutSessionId">The ID of the Workout Session to be searched</param>
        /// <returns>The WorkUnitSessionDetailsDto</returns>
        Task<WorkUnitSessionDetailsDto> GetWorkoutSessionDetailsAsync(uint workoutSessionId);



        /// <summary>
        /// Get the Working Sets of the specified Excercise performed by the specified user, including additional WorkUnit info.
        /// IMPORTANT: This query does not work for on-the-fly workouts. Two solutions are available:
        ///
        /// 1. Go through the Post table looking for on-the-fly WOs
        /// 2. Create a dummy TrainingPlan for collecting all on-the-fly WOs
        /// 3. Add them to the Schedule
        /// <param name="excerciseId">The ID of the Excercise to be searched</param>
        /// <param name="ownerId">The ID of the user to be searched</param>
        /// <param name="offset">The offset of the results to be fetched -> unlimited if value < 0 or left default</param>
        /// <param name="limit">The limit of the results to be fetched -> unlimited if value <= 0 or left default</param>
        /// <returns>The ExcerciseHistoryDto</returns>
        //Task<ExcerciseHistoryDto> GetExcerciseHistoryAsync(uint ownerId, uint excerciseId, int offset = -1, int limit = -1);

    }
}
