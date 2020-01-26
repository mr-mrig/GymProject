using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    public abstract class DatabaseSeed
    {

        public GymContext Context { get; }

        public DatabaseSeedService SeedingService { get; private set; }



        public DatabaseSeed(GymContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SeedingService = new DatabaseSeedService(context);
        }



        /// <summary>
        /// Check whether the Unit Test Database has the test cases loaded or it must be seeded in order to start the tests.
        /// This method just checks a sample query, hence the developer must ensure that all the queries have been seeded.
        /// </summary>
        /// <returns>True if Db ready, false if seeding is needed</returns>
        public virtual bool IsDbReadyForUnitTesting()

            => Context.TrainingPlans.Count() > 0
                && Context.WorkoutTemplates.Count() > 0;
        // Insert other checks here....


        public virtual async Task SeedTrainingDomain()
        {
            try
            {
                await SeedHashtagsPhasesProficiencies().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedUser().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedAthlete().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedExcercise().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedNotes().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedIntensityTechnique().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedTrainingPlan().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }
            try
            {
                await SeedUserTrainingPlanLibrary().ConfigureAwait(false);
            }
            catch (Exception exc) { System.Diagnostics.Debugger.Break(); }

            //await SeedMuscle();
            await SeedTrainingSchedule().ConfigureAwait(false);
        }




        #region Abstract Methods
        public abstract Task SeedUserTrainingPlanLibrary();
        public abstract Task SeedTrainingPlan();
        public abstract Task SeedIntensityTechnique();
        public abstract Task SeedNotes();
        public abstract Task SeedExcercise();
        public abstract Task SeedAthlete();
        public abstract Task SeedUser();
        public abstract Task SeedHashtagsPhasesProficiencies();
        public abstract Task SeedMuscle();
        public abstract Task SeedTrainingSchedule();
        #endregion
    }
}
