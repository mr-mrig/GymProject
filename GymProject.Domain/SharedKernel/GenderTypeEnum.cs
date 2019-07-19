using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class GenderTypeEnum : Enumeration
    {



        #region Class Enums
        public static GenderTypeEnum NotSet = new GenderTypeEnum(0, "NotSet");
        public static GenderTypeEnum Male = new GenderTypeEnum(1, "Male");
        public static GenderTypeEnum Female = new GenderTypeEnum(2, "Female");
        #endregion




        public GenderTypeEnum(int id, string name) : base(id, name)
        {
        }



        /// <summary>
        /// Checks if the gender is Femal
        /// </summary>
        /// <returns>True if Female</returns>
        public bool IsFemale() => Equals(Female);


        /// <summary>
        /// Checks if the gender is Male
        /// </summary>
        /// <returns>True if Male</returns>
        public bool IsMale() => Equals(Male);


        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<GenderTypeEnum> List() =>
            new[] { NotSet, Male, Female, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static GenderTypeEnum FromName(string name)
        {
            GenderTypeEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


    }
}
