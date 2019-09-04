﻿using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingPlanRelation : ValueObject
    {



        /// <summary>
        /// The Root Plan of the relation
        /// </summary>
        public TrainingPlanRoot ParentPlan { get; private set; }


        /// <summary>
        /// The ID of the Parent Training Plan
        /// </summary>
        public IdTypeValue ParentPlanId { get; private set; } = null;


        /// <summary>
        /// The ID of the Child Training Plan - 
        /// Must be considered as another Aggregate Root, since deleting the Parent must not entail the deletion of the child
        /// </summary>
        public IdTypeValue ChildPlanId { get; private set; } = null;


        /// <summary>
        /// FK to the Training Plan Message, when sent to another user -> Only Inherited Plans can have it
        /// </summary>
        public IdTypeValue MessageId { get; private set; } = null;

        /// <summary>
        /// The type of the Child Plan
        /// </summary>
        public TrainingPlanTypeEnum ChildPlanType { get; private set; } = null;



        #region Ctors

        private TrainingPlanRelation()
        {

        }

        private TrainingPlanRelation(IdTypeValue parentPlanId, IdTypeValue childPlanId, TrainingPlanTypeEnum relationType, IdTypeValue messageId = null)
        {
            ParentPlanId = parentPlanId;
            ChildPlanId = childPlanId;
            //ChildPlanType = TrainingPlanTypeEnum.From((int)relationTypeId.Id);
            ChildPlanType = relationType;
            MessageId = MessageId;

            TestBusinessRules();
        }

        #endregion


        #region Ctors

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="parentPlanId">The ID of the parent Training Plan</param>
        /// <param name="childPlanId">The ID of the child Training Plan</param>
        /// <param name="relationType">The relation that exists between the parent and the child plan</param>
        /// <param name="messageId">The ID of the message attached</param>
        /// <returns>The TrainingPlanRelationRoot instance</returns>
        public static TrainingPlanRelation EnstablishRelation(IdTypeValue parentPlanId, IdTypeValue childPlanId, TrainingPlanTypeEnum relationType, IdTypeValue messageId = null)

            => new TrainingPlanRelation(parentPlanId, childPlanId, relationType, messageId);

        #endregion



        #region Business Validation Logic

        /// <summary>
        /// A Training Plan Relation requires the plans to not be the same one.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool PlanIsNotBothParentAndChild() => ParentPlanId != ChildPlanId;


        /// <summary>
        /// Non-Inherited Training Plans must have no message attached.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NonInheritedPlansHaveNoMessage() => ChildPlanType == TrainingPlanTypeEnum.Inherited || MessageId == null;



        private void TestBusinessRules()
        {
            //if (!NoNullChildPlans())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Child Training Plans.");

            if (!NonInheritedPlansHaveNoMessage())
                throw new TrainingDomainInvariantViolationException($"Non-Inherited Training Plans must have no message attached.");

            if (!PlanIsNotBothParentAndChild())
                throw new TrainingDomainInvariantViolationException($"A Training Plan Relation requires the plans not to be the same.");
        }

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ParentPlanId;
            yield return ChildPlanId;
            // Other properties should not identify the Relation
        }

    }
}