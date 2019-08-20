﻿using GymProject.Domain.Base;
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
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhase.CreatePublicTrainingPhase(""));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhase.CreatePrivateTrainingPhase(""));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhase.CreateNativeTrainingPhase(""));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhase.CreatePublicTrainingPhase(null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhase.CreatePrivateTrainingPhase(null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPhase.CreateNativeTrainingPhase(null));
        }


        [Fact]
        public void PrivateTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhase phase = TrainingPhase.CreatePrivateTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Private, phase.EntryStatusType);
        }


        [Fact]
        public void PublicTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhase phase = TrainingPhase.CreatePublicTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Pending, phase.EntryStatusType);
        }


        [Fact]
        public void NativeTrainingPhase()
        {
            string phaseName = "My Phase";

            TrainingPhase phase = TrainingPhase.CreateNativeTrainingPhase(phaseName);

            Assert.NotNull(phase);
            Assert.Equal(phaseName, phase.Name);
            Assert.Equal(EntryStatusTypeEnum.Native, phase.EntryStatusType);
        }


        [Fact]
        public void UserPhasePhaseIdFail()
        {
            int days = 10;
            DateTime startDate = DateTime.Today;
            Owner owner = Owner.Register("user", "mypic");
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.PlanPhasePrivate(null, owner, period));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.PlanPhasePublic(null, owner, period));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.StartPhasePrivate(null, owner, startDate));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.StartPhasePublic(null, owner, startDate));
        }


        [Fact]
        public void UserPhaseOwnerFail()
        {
            int days = 10;
            IdType phaseId = new IdType(1);
            DateTime startDate = DateTime.Today;
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.PlanPhasePrivate(phaseId, null, period));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.PlanPhasePublic(phaseId, null, period));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.StartPhasePrivate(phaseId, null, startDate));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.StartPhasePublic(phaseId, null, startDate));
        }


        [Fact]
        public void UserPhasePeriodFail()
        {
            IdType phaseId = new IdType(1);
            DateTime startDate = DateTime.MinValue;
            Owner owner = Owner.Register("user", "mypic");

            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.PlanPhasePrivate(phaseId, owner, null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => UserPhase.PlanPhasePublic(phaseId, owner, null));

            Assert.Throws<ValueObjectInvariantViolationException>(() => UserPhase.StartPhasePrivate(phaseId, owner, startDate));
            Assert.Throws<ValueObjectInvariantViolationException>(() => UserPhase.StartPhasePublic(phaseId, owner, startDate));
        }


        [Fact]
        public void PlanUserPhase()
        {
            int days = 10;
            IdType phaseId = new IdType(1);
            DateTime startDate = DateTime.Today;
            Owner owner = Owner.Register("user", "mypic");
            DateRangeValue period = DateRangeValue.RangeBetween(startDate, startDate.AddDays(days));
            PersonalNoteValue note = PersonalNoteValue.Write("Little-note.");

            UserPhase phasePrivate = UserPhase.PlanPhasePrivate(phaseId, owner, period);

            Assert.NotNull(phasePrivate);
            Assert.Equal(owner, phasePrivate.Owner);
            Assert.Equal(phaseId, phasePrivate.PhaseId);
            Assert.Equal(period, phasePrivate.Period);
            Assert.Equal(EntryStatusTypeEnum.Private, phasePrivate.EntryStatusType);
            Assert.Null(phasePrivate.OwnerNote);

            phasePrivate.WriteNote(note.Body);
            Assert.Equal(note, phasePrivate.OwnerNote);

            UserPhase phasePublic = UserPhase.PlanPhasePublic(phaseId, owner, period, note);

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
            IdType phaseId = new IdType(1);
            DateTime startDate = DateTime.Today;
            Owner owner = Owner.Register("user", "mypic");
            PersonalNoteValue note = PersonalNoteValue.Write("Little-note.");

            UserPhase phasePrivate = UserPhase.StartPhasePrivate(phaseId, owner, startDate);

            Assert.NotNull(phasePrivate);
            Assert.Equal(owner, phasePrivate.Owner);
            Assert.Equal(phaseId, phasePrivate.PhaseId);
            Assert.Equal(startDate, phasePrivate.Period.Start);
            Assert.False(phasePrivate.Period.IsRightBounded());
            Assert.Equal(EntryStatusTypeEnum.Private, phasePrivate.EntryStatusType);
            Assert.Null(phasePrivate.OwnerNote);

            phasePrivate.WriteNote(note.Body);
            Assert.Equal(note, phasePrivate.OwnerNote);

            UserPhase phasePublic = UserPhase.StartPhasePublic(phaseId, owner, startDate, note);

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