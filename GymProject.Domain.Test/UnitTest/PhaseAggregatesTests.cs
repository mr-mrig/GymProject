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
            Assert.Equal(EntryStatusTypeEnum.Private, phase.EntryStatus);
        }


        [Fact]
        public void PublicTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhaseRoot phase = TrainingPhaseRoot.CreatePublicTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Pending, phase.EntryStatus);
        }


        [Fact]
        public void NativeTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhaseRoot phase = TrainingPhaseRoot.CreateNativeTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Native, phase.EntryStatus);
        }


        [Fact]
        public void UserPhasePhaseIdFail()
        {
            int days = 10;
            DateTime startDate = DateTime.Today;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.PlanPhasePrivate(null, owner, period));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.PlanPhasePublic(null, owner, period));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.StartPhasePrivate(null, owner, startDate));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.StartPhasePublic(null, owner, startDate));
        }


        [Fact]
        public void UserPhaseOwnerFail()
        {
            int days = 10;
            uint? phaseId = 1;
            DateTime startDate = DateTime.Today;
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.PlanPhasePrivate(phaseId, null, period));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.PlanPhasePublic(phaseId, null, period));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.StartPhasePrivate(phaseId, null, startDate));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.StartPhasePublic(phaseId, null, startDate));
        }


        [Fact]
        public void UserPhasePeriodFail()
        {
            uint? phaseId = 1;
            DateTime startDate = DateTime.MinValue;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.PlanPhasePrivate(phaseId, owner, null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserTrainingPhaseRoot.PlanPhasePublic(phaseId, owner, null));

            Assert.Throws<ValueObjectInvariantViolationException>(() => UserTrainingPhaseRoot.StartPhasePrivate(phaseId, owner, startDate));
            Assert.Throws<ValueObjectInvariantViolationException>(() => UserTrainingPhaseRoot.StartPhasePublic(phaseId, owner, startDate));
        }


        [Fact]
        public void PlanUserPhase()
        {
            int days = 10;
            uint? phaseId = 1;
            DateTime startDate = DateTime.Today;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));
            PersonalNoteValue note = PersonalNoteValue.Write("Little-note.");

            UserTrainingPhaseRoot phasePrivate = UserTrainingPhaseRoot.PlanPhasePrivate(phaseId, owner, period);

            Assert.NotNull(phasePrivate);
            Assert.Equal(owner, phasePrivate.Owner);
            Assert.Equal(phaseId, phasePrivate.PhaseId);
            Assert.Equal(period, phasePrivate.Period);
            Assert.Equal(EntryStatusTypeEnum.Private, phasePrivate.EntryStatus);
            Assert.Null(phasePrivate.OwnerNote);

            phasePrivate.WriteNote(note.Body);
            Assert.Equal(note, phasePrivate.OwnerNote);

            UserTrainingPhaseRoot phasePublic = UserTrainingPhaseRoot.PlanPhasePublic(phaseId, owner, period, note);

            Assert.NotNull(phasePublic);
            Assert.Equal(owner, phasePublic.Owner);
            Assert.Equal(phaseId, phasePublic.PhaseId);
            Assert.Equal(period, phasePublic.Period);
            Assert.Equal(EntryStatusTypeEnum.Pending, phasePublic.EntryStatus);
            Assert.Equal(note, phasePublic.OwnerNote);
        }



        [Fact]
        public void StartUserPhase()
        {
            uint? phaseId = 1;
            DateTime startDate = DateTime.Today;
            OwnerEntity owner = OwnerEntity.Register("user", "mypic");
            PersonalNoteValue note = PersonalNoteValue.Write("Little-note.");

            UserTrainingPhaseRoot phasePrivate = UserTrainingPhaseRoot.StartPhasePrivate(phaseId, owner, startDate);

            Assert.NotNull(phasePrivate);
            Assert.Equal(owner, phasePrivate.Owner);
            Assert.Equal(phaseId, phasePrivate.PhaseId);
            Assert.Equal(startDate, phasePrivate.Period.Start);
            Assert.False(phasePrivate.Period.IsRightBounded());
            Assert.Equal(EntryStatusTypeEnum.Private, phasePrivate.EntryStatus);
            Assert.Null(phasePrivate.OwnerNote);

            phasePrivate.WriteNote(note.Body);
            Assert.Equal(note, phasePrivate.OwnerNote);

            UserTrainingPhaseRoot phasePublic = UserTrainingPhaseRoot.StartPhasePublic(phaseId, owner, startDate, note);

            Assert.NotNull(phasePublic);
            Assert.Equal(owner, phasePublic.Owner);
            Assert.Equal(phaseId, phasePublic.PhaseId);
            Assert.Equal(startDate, phasePublic.Period.Start);
            Assert.False(phasePublic.Period.IsRightBounded());
            Assert.Equal(EntryStatusTypeEnum.Pending, phasePublic.EntryStatus);
            Assert.Equal(note, phasePublic.OwnerNote);
        }



    }
}
