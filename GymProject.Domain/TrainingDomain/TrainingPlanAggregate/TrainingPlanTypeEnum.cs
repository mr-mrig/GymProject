﻿using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingPlanTypeEnum : Enumeration
    {


        public static TrainingPlanTypeEnum NotSet = new TrainingPlanTypeEnum(0, "NotSet", "Not specified");
        //public static TrainingPlanTypeEnum Template = new TrainingPlanTypeEnum(1, "Template", "Root for other Variant plans");
        public static TrainingPlanTypeEnum Variant = new TrainingPlanTypeEnum(2, "Variant", "Variant of another plan");
        public static TrainingPlanTypeEnum Inherited = new TrainingPlanTypeEnum(3, "Inherited", "Received by another user");




        public string Description { get; private set; } = string.Empty;




        #region Ctors

        public TrainingPlanTypeEnum(int id, string name, string description) : base(id, name)
        {
            Description = description;
        }
        #endregion




        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<TrainingPlanTypeEnum> List() =>
            new[] { NotSet, /*Template,*/ Variant, Inherited, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static TrainingPlanTypeEnum FromName(string name)
        {
            TrainingPlanTypeEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }

    }
}