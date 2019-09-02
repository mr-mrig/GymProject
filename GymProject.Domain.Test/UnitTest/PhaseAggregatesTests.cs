using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using Xunit;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.TrainingDomain.UserPhaseAggregate;

namespace GymProject.Domain.Test.UnitTest
{
    public class PhaseAggregatesTests
    {



        [Fact]
        public void CreateTrainingPhaseFail()
        {
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhaseRoot.CreatePublicTrainingPhase(""));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhaseRoot.CreatePrivateTrainingPhase(""));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhaseRoot.CreateNativeTrainingPhase(""));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhaseRoot.CreatePublicTrainingPhase(null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhaseRoot.CreatePrivateTrainingPhase(null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhaseRoot.CreateNativeTrainingPhase(null));
        }


        [Fact]
        public void PrivateTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhaseRoot phase = TrainingPhaseRoot.CreatePrivateTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Private, phase.EntryStatusType);
        }


        [Fact]
        public void PublicTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhaseRoot phase = TrainingPhaseRoot.CreatePublicTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Pending, phase.EntryStatusType);
        }


        [Fact]
        public void NativeTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhaseRoot phase = TrainingPhaseRoot.CreateNativeTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Native, phase.EntryStatusType);
        }


        [Fact]
        public void UserPhasePhaseIdFail()
        {
            int days = 10;
            DateTime startDate = DateTime.Today;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.PlanPhasePrivate(null, owner, period));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.PlanPhasePublic(null, owner, period));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.StartPhasePrivate(null, owner, startDate));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.StartPhasePublic(null, owner, startDate));
        }


        [Fact]
        public void UserPhaseOwnerFail()
        {
            int days = 10;
            IdTypeValue phaseId = IdTypeValue.Create(1);
            DateTime startDate = DateTime.Today;
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.PlanPhasePrivate(phaseId, null, period));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.PlanPhasePublic(phaseId, null, period));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.StartPhasePrivate(phaseId, null, startDate));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.StartPhasePublic(phaseId, null, startDate));
        }


        [Fact]
        public void UserPhasePeriodFail()
        {
            IdTypeValue phaseId = IdTypeValue.Create(1);
            DateTime startDate = DateTime.MinValue;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.PlanPhasePrivate(phaseId, owner, null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhaseRoot.PlanPhasePublic(phaseId, owner, null));

            Assert.Throws<ValueObjectInvariantViolationException>(() => UserPhaseRoot.StartPhasePrivate(phaseId, owner, startDate));
            Assert.Throws<ValueObjectInvariantViolationException>(() => UserPhaseRoot.StartPhasePublic(phaseId, owner, startDate));
        }


        [Fact]
        public void PlanUserPhase()
        {
            int days = 10;
            IdTypeValue phaseId = IdTypeValue.Create(1);
            DateTime startDate = DateTime.Today;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));
            PersonalNoteValue note = PersonalNoteValue.Write("Little-note.");

            UserPhaseRoot phasePrivate = UserPhaseRoot.PlanPhasePrivate(phaseId, owner, period);

            Assert.NotNull(phasePrivate);
            Assert.Equal(owner, phasePrivate.Owner);
            Assert.Equal(phaseId, phasePrivate.PhaseId);
            Assert.Equal(period, phasePrivate.Period);
            Assert.Equal(EntryStatusTypeEnum.Private, phasePrivate.EntryStatusType);
            Assert.Null(phasePrivate.OwnerNote);

            phasePrivate.WriteNote(note.Body);
            Assert.Equal(note, phasePrivate.OwnerNote);

            UserPhaseRoot phasePublic = UserPhaseRoot.PlanPhasePublic(phaseId, owner, period, note);

            Assert.NotNull(phasePublic);
            Assert.Equal(owner, phasePublic.Owner);
            Assert.Equal(phaseId, phasePublic.PhaseId);
            Assert.Equal(period, phasePublic.Period);
            Assert.Equal(EntryStatusTypeEnum.Pending, phasePublic.EntryStatusType);
            Assert.Equal(note, phasePublic.OwnerNote);
        }



        [Fact]
        public void StartUserPhase()
        {
            IdTypeValue phaseId = IdTypeValue.Create(1);
            DateTime startDate = DateTime.Today;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");
            PersonalNoteValue note = PersonalNoteValue.Write("Little-note.");

            UserPhaseRoot phasePrivate = UserPhaseRoot.StartPhasePrivate(phaseId, owner, startDate);

            Assert.NotNull(phasePrivate);
            Assert.Equal(owner, phasePrivate.Owner);
            Assert.Equal(phaseId, phasePrivate.PhaseId);
            Assert.Equal(startDate, phasePrivate.Period.Start);
            Assert.False(phasePrivate.Period.IsRightBounded());
            Assert.Equal(EntryStatusTypeEnum.Private, phasePrivate.EntryStatusType);
            Assert.Null(phasePrivate.OwnerNote);

            phasePrivate.WriteNote(note.Body);
            Assert.Equal(note, phasePrivate.OwnerNote);

            UserPhaseRoot phasePublic = UserPhaseRoot.StartPhasePublic(phaseId, owner, startDate, note);

            Assert.NotNull(phasePublic);
            Assert.Equal(owner, phasePublic.Owner);
            Assert.Equal(phaseId, phasePublic.PhaseId);
            Assert.Equal(startDate, phasePublic.Period.Start);
            Assert.False(phasePublic.Period.IsRightBounded());
            Assert.Equal(EntryStatusTypeEnum.Pending, phasePublic.EntryStatusType);
            Assert.Equal(note, phasePublic.OwnerNote);
        }



    }
}
