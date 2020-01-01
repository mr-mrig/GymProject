using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class AthleteAggregateTest
    {


        [Fact]
        public void RegisterAthlete_DraftTransient()
        {
            AthleteRoot athlete = AthleteRoot.RegisterAthlete();

            Assert.Empty(athlete.TrainingPhases);
            Assert.Empty(athlete.TrainingProficiencies);
            Assert.Empty(athlete.TrainingPlans);
            Assert.Null(athlete.Id);
        }


        [Fact]
        public void RegisterAthlete_Draft()
        {
            uint userid = 1;
            AthleteRoot athlete = AthleteRoot.RegisterAthlete(userid);

            Assert.Empty(athlete.TrainingPhases);
            Assert.Empty(athlete.TrainingProficiencies);
            Assert.Empty(athlete.TrainingPlans);
            Assert.Equal(userid, athlete.Id);
        }

        [Fact]
        public void RegisterAthlete_Full()
        {
            uint userid = 1;
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);

            // Null lists
            List<UserTrainingPhaseRelation> phases = null;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;

            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);

            Assert.Empty(athlete.TrainingPhases);
            Assert.Empty(athlete.TrainingProficiencies);
            Assert.Empty(athlete.TrainingPlans);
            Assert.Equal(userid, athlete.Id);

            // Empty lists
            phases = new List<UserTrainingPhaseRelation>();
            proficiencies = new List<UserTrainingProficiencyRelation>();
            plans = new List<UserTrainingPlanEntity>();

            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);

            Assert.Empty(athlete.TrainingPhases);
            Assert.Empty(athlete.TrainingProficiencies);
            Assert.Empty(athlete.TrainingPlans);
            Assert.Equal(userid, athlete.Id);

            // Ordinary test
            phases = new List<UserTrainingPhaseRelation>()
            {
               UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.Date.AddDays(-200), DateTime.UtcNow.Date.AddDays(-150), PersonalNoteValue.Write("note0")),
               UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.Date.AddDays(-100), DateTime.UtcNow.Date, PersonalNoteValue.Write("note0")),
               UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.AddDays(1)),
            };
            proficiencies = new List<UserTrainingProficiencyRelation>()
            {
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(2, DateTime.UtcNow.Date.AddDays(-100)),
            };
            plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(2),
                UserTrainingPlanEntity.NewDraft(4),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(3, "plan3", true, 2, 1, new List<uint?> { 2, 1, }, new List<uint?> { 1, }, new List<uint?> { 5, }, new List<uint?> { 1, 6 } ),
            };

            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);

            Assert.True(athlete.TrainingPhases.SequenceEqual(phases));
            Assert.True(athlete.TrainingProficiencies.SequenceEqual(proficiencies));
            Assert.True(athlete.TrainingPlans.SequenceEqual(plans));
            Assert.Equal(userid, athlete.Id);
        }



        [Fact]
        public void RegisterAthlete_BusinessRuleFail_Phase()
        {
            uint userid = 1;
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);

            // 1 At most one Phase open
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;
            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100)),
                UserTrainingPhaseRelation.StartPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(10)),
            };
            Assert.Throws<TrainingDomainInvariantViolationException>(() => AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans));

            // 2 No overlapping phases
            phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100)),
                UserTrainingPhaseRelation.PlanPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.AddDays(-150), DateTime.UtcNow.AddDays(-50)),
            };
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans));
        }


        [Fact]
        public void RegisterAthlete_BusinessRuleFail_TrainingPlan()
        {
            uint userid = 1;

            // 1 Training Plan cannot be parent of itself
            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>();
            List<UserTrainingProficiencyRelation> proficiencies = new List<UserTrainingProficiencyRelation>();

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1, parentPlanId: 1));

            //// 2 No duplicate training plans
            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1),
            };
            Assert.Throws<TrainingDomainInvariantViolationException>(() => AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans));
        }



        [Fact]
        public void RegisterAthlete_BusinessRuleFail_Proficiency()
        {
            uint userid = 1;
            AthleteRoot athlete;

            // 1 At most one proficiency open
            List<UserTrainingPhaseRelation> phases = null;
            List<UserTrainingPlanEntity> plans = null;
            List<UserTrainingProficiencyRelation> proficiencies = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(1, DateTime.UtcNow.AddDays(-100)),
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(1, DateTime.UtcNow.AddDays(10)),
            };
            Assert.Throws<TrainingDomainInvariantViolationException>(() => AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans));

            // 2 No overlapping proficiencies
            proficiencies = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(1, DateTime.UtcNow.AddDays(-100)),
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-150), DateTime.UtcNow.AddDays(-50)),
            };
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans));
        }


        [Fact]
        public void AddTrainingPlanToLibrary_BusinessRuleFail()
        {
            uint userid = 1;
            AthleteRoot athlete;

            // 1 Training Plan cannot be parent of itself - One plan only
            athlete = AthleteRoot.RegisterAthlete(userid);
            athlete.AddTrainingPlanToLibrary(1);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete.MakeTrainingPlanVariantOf(1, 1));

            // 2 Training Plan cannot be parent of itself - More plans
            athlete = AthleteRoot.RegisterAthlete(userid);
            athlete.AddTrainingPlanToLibrary(3);
            athlete.AddTrainingPlanToLibrary(1);
            athlete.AddTrainingPlanToLibrary(2);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete.MakeTrainingPlanVariantOf(1, 1));
        }


        [Fact]
        public void StartTrainingPhase_BusinessRuleFail()
        {
            uint userid = 1;
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = athlete = AthleteRoot.RegisterAthlete(userid);

            List <UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;
            List<UserTrainingPhaseRelation> phases;

            // Cannot insert two phases starting on the same date
            phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(10)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<InvalidOperationException>(() => athlete.StartTrainingPhase(5, EntryStatusTypeEnum.Pending, DateTime.UtcNow));

            // Cannot insert two phases starting on the same date - More phases
            phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-10)),
                UserTrainingPhaseRelation.PlanPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(10)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<InvalidOperationException>(() => athlete.StartTrainingPhase(5, EntryStatusTypeEnum.Pending, DateTime.UtcNow));

            // 5 Start a Phase placed in the past
            phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(10)),
                UserTrainingPhaseRelation.PlanPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<InvalidOperationException>(() => athlete.StartTrainingPhase(5, EntryStatusTypeEnum.Pending, DateTime.UtcNow.AddDays(-1000)));

            // 6 Start a Phase placed in the past
            phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(10)),
                UserTrainingPhaseRelation.PlanPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<InvalidOperationException>(() => athlete.StartTrainingPhase(5, EntryStatusTypeEnum.Pending, DateTime.UtcNow.AddDays(-1000), DateTime.UtcNow.AddDays(-900)));
        }


        [Fact]
        public void AchieveTrainingProficiency_BusinessRuleFail()
        {
            uint userid = 1;
            AthleteRoot athlete;
            List<UserTrainingPhaseRelation> phases = null;
            List<UserTrainingPlanEntity> plans = null;

            // 1 No overlapping phases - Two phases only
            List<UserTrainingProficiencyRelation> proficiencies = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(10)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete.AchieveTrainingProficiency(5));

            // 2 No overlapping phases - More phases
            proficiencies = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(10)),
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete.AchieveTrainingProficiency(5));
        }


        [Fact]
        public void Athlete_AchieveTrainingProficiency()
        {
            uint userid = 1;
            uint newProficinecyId = 5;
            AthleteRoot athlete;
            List<UserTrainingPhaseRelation> phases = null;
            List<UserTrainingPlanEntity> plans = null;

            // At most one Phase open - Two phasese only
            List<UserTrainingProficiencyRelation> proficiencies = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(1, DateTime.UtcNow.AddDays(-100)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.AchieveTrainingProficiency(newProficinecyId);

            List<UserTrainingProficiencyRelation> expected = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-1)),
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(newProficinecyId, DateTime.UtcNow),
            };

            CheckProficiencies(expected, athlete.TrainingProficiencies);

            // At most one Phase open - More phases
            proficiencies = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(1, DateTime.UtcNow.AddDays(-100)),
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.AchieveTrainingProficiency(newProficinecyId);

            expected = new List<UserTrainingProficiencyRelation>
            {
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1,DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-1)),
                UserTrainingProficiencyRelation.AssignTrainingProficiency(1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300)),
                UserTrainingProficiencyRelation.AchieveTrainingProficiency(newProficinecyId, DateTime.UtcNow),
            };
            CheckProficiencies(expected, athlete.TrainingProficiencies);
        }



        [Fact]
        public void Athlete_StartTrainingPhase_Legacy()
        {
            //uint userid = 1;
            //uint newPhaseId = 5;
            //AthleteRoot athlete;
            //List<UserTrainingProficiencyRelation> proficiencies = null;
            //List<UserTrainingPlanEntity> plans = null;

            //// At most one Phase open - Two phasese only
            //List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            //{
            //    UserTrainingPhaseRelation.StartPhasePrivate(1, DateTime.UtcNow.AddDays(-100)),
            //};
            //athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            //athlete.StartTrainingPhase(newPhaseId, EntryStatusTypeEnum.Pending, null, null, PersonalNoteValue.Write("note"));

            //List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            //{
            //    UserTrainingPhaseRelation.PlanPhasePrivate(1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-1)),
            //    UserTrainingPhaseRelation.StartPhasePublic(newPhaseId, DateTime.UtcNow, PersonalNoteValue.Write("note")),
            //};

            //CheckPhases(expected, athlete.TrainingPhases);

            //// At most one Phase open - More phases
            //phases = new List<UserTrainingPhaseRelation>
            //{
            //    UserTrainingPhaseRelation.StartPhasePrivate(1, DateTime.UtcNow.AddDays(-100)),
            //    UserTrainingPhaseRelation.PlanPhasePublic(1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300),
            //        PersonalNoteValue.Write("hello")),
            //};
            //athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            //athlete.StartTrainingPhase(newPhaseId, EntryStatusTypeEnum.Pending, DateTime.UtcNow.AddDays(10));

            //expected = new List<UserTrainingPhaseRelation>
            //{
            //    UserTrainingPhaseRelation.PlanPhasePrivate(1,DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-1)),
            //    UserTrainingPhaseRelation.PlanPhasePublic(1, DateTime.UtcNow.AddDays(-500), DateTime.UtcNow.AddDays(-300)),
            //    UserTrainingPhaseRelation.StartPhasePublic(newPhaseId, DateTime.UtcNow.AddDays(10), PersonalNoteValue.Write("hello")),
            //};
            //CheckPhases(expected, athlete.TrainingPhases);
        }

        [Fact]
        public void Athlete_StartTrainingPhase()
        {
            AthleteRoot athlete;
            uint userid = 1;
            uint phaseId = 1;
            DateTime startDate1 = DateTime.UtcNow.AddDays(100).Date;
            DateTime startDate2 = startDate1.AddDays(100).Date;

            athlete = AthleteRoot.RegisterAthlete(userid);
            athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Pending, null, null, PersonalNoteValue.Write("note"));
            athlete.StartTrainingPhase(phaseId + 1, EntryStatusTypeEnum.Pending, startDate1, startDate2.AddDays(-1));
            athlete.StartTrainingPhase(phaseId + 2, EntryStatusTypeEnum.Pending, startDate2, null, PersonalNoteValue.Write("note"));

            List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePublic(athlete, phaseId, DateTime.UtcNow.Date, startDate1.AddDays(-1), PersonalNoteValue.Write("note")),
                UserTrainingPhaseRelation.PlanPhasePublic(athlete, phaseId + 1, startDate1, startDate2.AddDays(-1)),
                UserTrainingPhaseRelation.PlanPhasePublic(athlete, phaseId + 2, startDate2, null),
            };

            CheckPhases(expected, athlete.TrainingPhases);
        }

        [Fact]
        public void Athlete_StartTrainingPhase_StartNotCronologicallyOrderedPhases_Fail()
        {
            // Here the user is starting two phases, one of them in the future
            // Should the end date of the first phase be adjusted not to leave gaps
            // or should the operation fail until the user selects and end date for it - see next test case - ?
            // For now, we let the operation fail.

            AthleteRoot athlete;
            uint userid = 1;
            uint phaseId = 1;
            DateTime startDate1 = DateTime.UtcNow.AddDays(100).Date;
            DateTime startDate2 = DateTime.UtcNow.AddDays(10).Date;

            athlete = AthleteRoot.RegisterAthlete(userid);
            athlete.StartTrainingPhase(phaseId + 1, EntryStatusTypeEnum.Private, startDate1);
            Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Private, startDate2, null, PersonalNoteValue.Write("note")));

            //List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            //{
            //    UserTrainingPhaseRelation.PlanPhasePrivate(athlete, phaseId + 1, startDate1, null),
            //    UserTrainingPhaseRelation.PlanPhasePrivate(athlete, phaseId, startDate2, startDate1.AddDays(-1), PersonalNoteValue.Write("note")),
            //};

            //CheckPhases(expected, athlete.TrainingPhases);
        }

        [Fact]
        public void Athlete_StartTrainingPhase_PlanNotCronologicallyOrderedPhases_Success()
        {
            // Here the user is planning two future phases leaving a gap between them
            // The operation must be allowed and the gap should be left as plnned, 
            // since the user is taking the ownership of this operation by scheduling on specific periods

            AthleteRoot athlete;
            uint userid = 1;
            uint phaseId = 1;
            DateTime startDate1 = DateTime.UtcNow.AddDays(100).Date;
            DateTime startDate2 = DateTime.UtcNow.Date;
            DateTime endDate2 = startDate1.AddDays(-10);

            athlete = AthleteRoot.RegisterAthlete(userid);
            athlete.StartTrainingPhase(phaseId + 1, EntryStatusTypeEnum.Private, startDate1);
            athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Private, startDate2, endDate2, PersonalNoteValue.Write("note"));

            List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePrivate(athlete, phaseId + 1, startDate1, null),
                UserTrainingPhaseRelation.PlanPhasePrivate(athlete, phaseId, startDate2, endDate2, PersonalNoteValue.Write("note")),
            };

            CheckPhases(expected, athlete.TrainingPhases);
        }

        [Fact]
        public void Athlete_StartTrainingPhase_NonClosedPhases()
        {
            AthleteRoot athlete;
            uint userid = 1;
            uint phaseId = 1;
            DateTime startDate1 = DateTime.UtcNow.AddDays(100);
            DateTime endDate1 = startDate1.AddDays(100);

            athlete = AthleteRoot.RegisterAthlete(userid);
            athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Private, null, null, PersonalNoteValue.Write("this should be closed auotmatically"));
            athlete.StartTrainingPhase(phaseId + 1, EntryStatusTypeEnum.Private, startDate1, endDate1);

            List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePrivate(athlete, phaseId, DateTime.UtcNow.Date, startDate1.AddDays(-1), PersonalNoteValue.Write("this should be closed auotmatically")),
                UserTrainingPhaseRelation.PlanPhasePrivate(athlete, phaseId + 1, startDate1, endDate1),
            };

            CheckPhases(expected, athlete.TrainingPhases);
        }


        [Fact]
        public void Athlete_CloseCurrentPhase()
        {
            uint userid = 1;
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);
            List <UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;

            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.AddDays(-10)),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-50)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.CloseCurrentPhase();

            List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.PlanPhasePrivate(dummyAthlete, 1, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-1)),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-50)),
            };

            CheckPhases(expected, athlete.TrainingPhases);
        }


        [Fact]
        public void Athlete_ShiftTrainingPhaseStartDate_Fail()
        {
            uint userid = 1;
            DateTime phaseStartDate = DateTime.UtcNow.AddDays(-10);
            DateTime newStartDate = DateTime.UtcNow.AddDays(-2);
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);

            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;

            // Phase not found
            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, phaseStartDate),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-50)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<InvalidOperationException>(() => athlete.ShiftTrainingPhaseStartDate(phaseStartDate.AddDays(100), newStartDate));
        }


        [Fact]
        public void Athlete_ShiftTrainingPhaseStartDate()
        {
            uint userid = 1;
            DateTime phaseStartDate = DateTime.UtcNow.AddDays(-10);
            DateTime newStartDate = DateTime.UtcNow.AddDays(-2);
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);

            List <UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;
            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, phaseStartDate),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-50)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.ShiftTrainingPhaseStartDate(phaseStartDate, newStartDate);

            List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, newStartDate),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, DateTime.UtcNow.AddDays(-100), DateTime.UtcNow.AddDays(-50)),
            };

            CheckPhases(expected, athlete.TrainingPhases);
        }


        [Fact]
        public void Athlete_ShiftTrainingPhaseStartDate_BeforePreviousPhaseEnds()
        {
            uint userid = 1;
            DateTime phaseStartDate = DateTime.UtcNow.AddDays(-10);
            DateTime phaseStartDate2 = DateTime.UtcNow.AddDays(-100);
            DateTime newStartDate = DateTime.UtcNow.AddDays(-72);
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);

            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;
            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, phaseStartDate),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, phaseStartDate2, DateTime.UtcNow.AddDays(-50)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.ShiftTrainingPhaseStartDate(phaseStartDate, newStartDate);

            List<UserTrainingPhaseRelation> expected = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, newStartDate),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, phaseStartDate2, newStartDate.AddDays(-1)),
            };

            CheckPhases(expected, athlete.TrainingPhases);
        }


        [Fact]
        public void Athlete_ShiftTrainingPhaseStartDate_BeforeOrEqualPreviousPhaseStarts_Fail()
        {
            // This test forces one phase to be one day only, which violates the business rules

            uint userid = 1;
            DateTime phaseStartDate = DateTime.UtcNow.AddDays(-10);
            DateTime phaseStartDate2 = DateTime.UtcNow.AddDays(-100);
            DateTime newStartDate = phaseStartDate2.AddDays(1);
            AthleteRoot athlete;
            AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(userid);

            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPlanEntity> plans = null;
            List<UserTrainingPhaseRelation> phases = new List<UserTrainingPhaseRelation>
            {
                UserTrainingPhaseRelation.StartPhasePrivate(dummyAthlete, 1, phaseStartDate),
                UserTrainingPhaseRelation.PlanPhasePublic(dummyAthlete, 1, phaseStartDate2, DateTime.UtcNow.AddDays(-50)),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => athlete.ShiftTrainingPhaseStartDate(phaseStartDate, newStartDate));
        }


        [Fact]
        public void Athlete_AddTrainingPlanToLibrary()
        {
            uint userid = 1;
            uint newPlanId = 10;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            // New Draft
            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(2),
                UserTrainingPlanEntity.NewDraft(4),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(3, "plan3", true, 2, 1, new List<uint?> { 2, 1, }, new List<uint?> { 1, }, new List<uint?> { 5, }, new List<uint?> { 1, 6 } ),
            };
            UserTrainingPlanEntity newPlan = UserTrainingPlanEntity.NewDraft(newPlanId);

            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.AddTrainingPlanToLibrary(newPlan.TrainingPlanId);

            plans.Add(newPlan);
            Assert.Equal(plans, athlete.TrainingPlans, new UserTrainingPlanEqualityComparer());

            // New full relation
            plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(2),
                UserTrainingPlanEntity.NewDraft(4),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(3, "plan3", true, 2, 1, new List<uint?> { 2, 1, }, new List<uint?> { 1, }, new List<uint?> { 5, }, new List<uint?> { 1, 6 } ),
            };
            newPlan = UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(newPlanId, "Plan10", true, 1, 10,
                trainingPhaseIds: new List<uint?> { 4, 1, },
                trainingPlanProficiencyIds: new List<uint?> { 20, 3, },
                trainingMuscleFocusIds: new List<uint?> { 44 },
                hashtagIds: new List<uint?> { 100, 101 });

            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.AddTrainingPlanToLibrary(newPlan);

            plans.Add(newPlan);
            Assert.Equal(plans, athlete.TrainingPlans, new UserTrainingPlanEqualityComparer());

            // Plan already present: do nothing
            plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(1),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(newPlanId),  // Here
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(2),
                UserTrainingPlanEntity.NewDraft(4),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(3, "plan3", true, 2, 1, new List<uint?> { 2, 1, }, new List<uint?> { 1, }, new List<uint?> { 5, }, new List<uint?> { 1, 6 } ),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.AddTrainingPlanToLibrary(newPlan.TrainingPlanId);

            Assert.Equal(plans, athlete.TrainingPlans, new UserTrainingPlanEqualityComparer());
        }


        [Fact]
        public void Athlete_RemoveTrainingPlanFromLibrary()
        {
            uint userid = 1;
            uint removeId = 1;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            // Only one plan
            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(removeId),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);

            athlete.RemoveTrainingPlanFromLibrary(removeId);
            Assert.Empty(athlete.TrainingPlans);
            Assert.Single(athlete.DomainEvents);

            // More plans
            plans = new List<UserTrainingPlanEntity>
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(removeId + 1),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(removeId + 2),
                UserTrainingPlanEntity.NewDraft(removeId),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(removeId + 10, "plan3", true, 2, 1, new List<uint?> { 2, 1, }, new List<uint?> { 1, }, new List<uint?> { 5, }, new List<uint?> { 1, 6 } ),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.RemoveTrainingPlanFromLibrary(removeId);

            Assert.Equal(plans.Except(plans.Where(x => x.TrainingPlanId == removeId)), athlete.TrainingPlans, new UserTrainingPlanEqualityComparer());
            Assert.Single(athlete.DomainEvents);
        }


        [Fact]
        public void Athlete_RenameTrainingPlan()
        {
            uint userid = 1;
            uint planId = 1;
            string newName = "mew name!";
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.RenameTrainingPlan(planId, newName);

            Assert.Equal(newName, athlete.CloneTrainingPlanOrDefault(planId).Name);
        }



        [Fact]
        public void Athlete_MakeTrainingPlanBookmarked()
        {
            uint userid = 1;
            uint planId = 1;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.BookmarkTrainingPlan(planId, true);

            Assert.True(athlete.CloneTrainingPlanOrDefault(planId).IsBookmarked);

            athlete.BookmarkTrainingPlan(planId, false);
            Assert.False(athlete.CloneTrainingPlanOrDefault(planId).IsBookmarked);
        }



        [Fact]
        public void Athlete_AttachTrainingPlanNote()
        {
            uint userid = 1;
            uint planId = 1;
            uint noteId = 22;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId, personalNoteId: noteId + 10),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.AttachTrainingPlanNote(planId, noteId);

            Assert.Equal(noteId, athlete.CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId);

            // Detach
            athlete.AttachTrainingPlanNote(planId, null);
            Assert.Null(athlete.CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId);
        }


        [Fact]
        public void Athlete_CleanTrainingPlanNote()
        {
            uint userid = 1;
            uint planId = 1;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId, personalNoteId: 100),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.CleanTrainingPlanNote(planId);

            Assert.Null(athlete.CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId);
        }


        [Fact]
        public void Athlete_MakeTrainingPlanVariantOf()
        {
            uint userid = 1;
            uint planId = 1;
            uint parentId = planId + 1;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId, personalNoteId: 100),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.MakeTrainingPlanVariantOf(planId, parentId);

            Assert.Equal(parentId, athlete.CloneTrainingPlanOrDefault(planId).ParentPlanId);
        }



        [Fact]
        public void Athlete_MakeTrainingPlanNotVariantOfAny()
        {
            uint userid = 1;
            uint planId = 1;
            uint parentId = planId + 1;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId, parentPlanId: parentId),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            athlete.MakeTrainingPlanNotVariantOfAny(planId);

            Assert.Null(athlete.CloneTrainingPlanOrDefault(planId).ParentPlanId);
        }


        [Fact]
        public void Athlete_FocusTrainingPlanOnMuscle()
        {
            uint userid = 1;
            uint planId = 1;
            uint muscleId = 10;
            int prevMuscles;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    trainingMuscleFocusIds: new List<uint?> { muscleId + 100, muscleId + 5 }),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevMuscles = athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count;
            athlete.FocusTrainingPlanOnMuscle(planId, muscleId);

            Assert.Equal(prevMuscles + 1, athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count);
            Assert.Equal(muscleId, athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Last());

            // Duplicate insert -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevMuscles = athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count;
            athlete.FocusTrainingPlanOnMuscle(planId, muscleId);

            Assert.Equal(prevMuscles, athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count);
        }



        [Fact]
        public void Athlete_UnfocusTrainingPlanFromMuscle()
        {
            uint userid = 1;
            uint planId = 1;
            uint muscleId = 10;
            int prevMuscles;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    trainingMuscleFocusIds: new List<uint?> { muscleId, muscleId + 5 }),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevMuscles = athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count;
            athlete.UnfocusTrainingPlanFromMuscle(planId, muscleId);

            Assert.Equal(prevMuscles - 1, athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count);
            Assert.DoesNotContain(muscleId, athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds);

            // Duplicate deletion -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevMuscles = athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count;
            athlete.UnfocusTrainingPlanFromMuscle(planId, muscleId + 1);

            Assert.Equal(prevMuscles, athlete.CloneTrainingPlanOrDefault(planId).MuscleFocusIds.Count);
        }


        [Fact]
        public void Athlete_MarkTrainingPlanAsSuitableForProficiencyLevel()
        {
            uint userid = 1;
            uint planId = 1;
            uint profId = 10;
            int prevMuscles;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    trainingPlanProficiencyIds: new List<uint?> { profId + 1, profId + 2 }),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevMuscles = athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count;
            athlete.MarkTrainingPlanAsSuitableForProficiencyLevel(planId, profId);

            Assert.Equal(prevMuscles + 1, athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count);
            Assert.Equal(profId, athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Last());

            // Duplicate insert -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevMuscles = athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count;
            athlete.MarkTrainingPlanAsSuitableForProficiencyLevel(planId, profId);

            Assert.Equal(prevMuscles, athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count);
        }


        [Fact]
        public void Athlete_UnlinkTrainingPlanTargetProficiency()
        {
            uint userid = 1;
            uint planId = 1;
            uint profId = 10;
            int prevProficiencies;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    trainingPlanProficiencyIds: new List<uint?> { profId, profId + 1 }),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevProficiencies = athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count;
            athlete.UnlinkTrainingPlanTargetProficiency(planId, profId);

            Assert.Equal(prevProficiencies - 1, athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count);
            Assert.DoesNotContain(profId, athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds);

            // Duplicate deletion -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevProficiencies = athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count;
            athlete.UnlinkTrainingPlanTargetProficiency(planId, profId + 100);

            Assert.Equal(prevProficiencies, athlete.CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds.Count);
        }



        [Fact]
        public void Athlete_TagTrainingPlanAs()
        {
            uint userid = 1;
            uint planId = 1;
            uint hashtagId = 10;
            int prevHashtags;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    hashtagIds: new List<uint?> { hashtagId + 2, hashtagId + 1 }),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevHashtags = athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count;
            athlete.TagTrainingPlanAs(planId, hashtagId);

            Assert.Equal(prevHashtags + 1, athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count);
            Assert.Equal(hashtagId, athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Last());

            // Duplicate insert -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevHashtags = athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count;
            athlete.TagTrainingPlanAs(planId, hashtagId);

            Assert.Equal(prevHashtags, athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count);
        }


        [Fact]
        public void Athlete_UntagTrainingPlan()
        {
            uint userid = 1;
            uint planId = 1;
            uint hashtagId = 10;
            int prevHashtags;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    hashtagIds: new List<uint?> { hashtagId + 2, hashtagId }),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevHashtags = athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count;
            athlete.UntagTrainingPlan(planId, hashtagId);

            Assert.Equal(prevHashtags - 1, athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count);
            Assert.DoesNotContain(hashtagId, athlete.CloneTrainingPlanOrDefault(planId).HashtagIds);

            // Duplicate insert -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevHashtags = athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count;
            athlete.UntagTrainingPlan(planId, hashtagId);

            Assert.Equal(prevHashtags, athlete.CloneTrainingPlanOrDefault(planId).HashtagIds.Count);
        }


        [Fact]
        public void Athlete_TagTrainingPlanWithPhase()
        {
            uint userid = 1;
            uint planId = 1;
            uint phaseId = 10;
            int prevPhases;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    trainingPhaseIds: new List<uint?> { phaseId + 1, phaseId + 2}),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevPhases = athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count;
            athlete.TagTrainingPlanWithPhase(planId, phaseId);

            Assert.Equal(prevPhases + 1, athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count);
            Assert.Equal(phaseId, athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Last());

            // Duplicate insert -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevPhases = athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count;
            athlete.TagTrainingPlanWithPhase(planId, phaseId + 1);

            Assert.Equal(prevPhases, athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count);
        }


        [Fact]
        public void Athlete_UntagTrainingPlanWithPhase()
        {
            uint userid = 1;
            uint planId = 1;
            uint phaseId = 10;
            int prevPhases;
            AthleteRoot athlete;
            List<UserTrainingProficiencyRelation> proficiencies = null;
            List<UserTrainingPhaseRelation> phases = null;

            List<UserTrainingPlanEntity> plans = new List<UserTrainingPlanEntity>()
            {
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId + 10),
                UserTrainingPlanEntity.InclueTrainingPlanInUserLibrary(planId,
                    trainingPhaseIds: new List<uint?> { phaseId, phaseId + 2}),
            };
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevPhases = athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count;
            athlete.UntagTrainingPlanWithPhase(planId, phaseId);

            Assert.Equal(prevPhases - 1, athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count);
            Assert.DoesNotContain(phaseId, athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds);

            // Duplicate insert -> do nothing
            athlete = AthleteRoot.RegisterAthlete(userid, phases, proficiencies, plans);
            prevPhases = athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count;
            athlete.UntagTrainingPlanWithPhase(planId, phaseId + 100);

            Assert.Equal(prevPhases, athlete.CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.Count);
        }






        #region Service Functions

        internal void CheckProficiencies(IEnumerable<UserTrainingProficiencyRelation> left, IEnumerable<UserTrainingProficiencyRelation> right)
        {
            Assert.True(left.Select(x => x.ProficiencyId).SequenceEqual(right.Select(x => x.ProficiencyId)));

            var leftEnum = left.GetEnumerator();
            var rightEnum = left.GetEnumerator();

            while (leftEnum.MoveNext() && rightEnum.MoveNext())
            {
                StaticUtils.CheckDateTimes(leftEnum.Current.StartDate, rightEnum.Current.StartDate);
                StaticUtils.CheckDateTimes(leftEnum.Current.EndDate, rightEnum.Current.EndDate);
            }
        }



        internal void CheckPhases(IEnumerable<UserTrainingPhaseRelation> left, IEnumerable<UserTrainingPhaseRelation> right)
        {
            Assert.True(left.Select(x => x.PhaseId).SequenceEqual(right.Select(x => x.PhaseId)));

            var leftEnum = left.GetEnumerator();
            var rightEnum = left.GetEnumerator();

            while (leftEnum.MoveNext() && rightEnum.MoveNext())
            {
                StaticUtils.CheckDateTimes(leftEnum.Current.StartDate, rightEnum.Current.StartDate);
                StaticUtils.CheckDateTimes(leftEnum.Current.EndDate, rightEnum.Current.EndDate);
            }
        }
        #endregion

    }
}
