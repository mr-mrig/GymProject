using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    public class TestServiceLayer
    {



        private bool _enableEagerLoading;


        private GymContext _context;




        public TestServiceLayer(GymContext context, bool enableEagerLoading = true)
        {
            _context = context;
            _enableEagerLoading = enableEagerLoading;
        }


        public void AddWorkingSet(uint workoutId, uint wunitProgressiveNumber, WSRepetitionsValue repetitions = null,
            RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IEnumerable<uint?> intensityTechniques = null)
        {
            using(GymContext _context = new GymContext())
            {
                //WorkoutTemplateRoot wo = context.WorkoutTemplates.Where(x => x.Id == workoutId)
                //    .Include(x => x.WorkUnits)
                //    .Single();

                WorkoutTemplateRoot wo = WorkoutRepositoryFind(workoutId);

                wo.AddTransientWorkingSet(wunitProgressiveNumber, repetitions, rest, effort, tempo, intensityTechniques);
                _context.Update(wo);
                _context.SaveChanges();
            }
        }


        /// <summary>
        /// This is the simulation of what the Repository will do
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public WorkoutTemplateRoot WorkoutRepositoryFind(uint workoutId)
        {
            WorkoutTemplateRoot workout = null;

            if (_enableEagerLoading)
            {
                // On query for all objects
                workout = _context.WorkoutTemplates.Where(x => x.Id == workoutId)
                    .Include(wo => wo.WorkUnits)
                        .ThenInclude(wu => wu.WorkingSets)
                    .SingleOrDefault();
            }
            else
            {
                // No query if object already tracked
                workout = _context.WorkoutTemplates.Find(workoutId);

                if (workout != null)
                {
                    // One query for each Load -> wasteful
                    _context.Entry(workout).Collection(x => x.WorkUnits).Load();
                    _context.Entry(workout).Reference(x => x.n).Load();
                }
            }


            return workout;
        }
    }
}
