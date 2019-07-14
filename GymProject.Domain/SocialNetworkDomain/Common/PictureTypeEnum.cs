using GymProject.Domain.Base;
using GymProject.Domain.SocialNetworkDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.Common
{
    public class PictureTypeEnum : Enumeration
    {


        public static PictureTypeEnum Standard = new PictureTypeEnum(0, "Standard");
        public static PictureTypeEnum ProgressPic = new PictureTypeEnum(1, "ProgressPic");




        #region Ctors

        public PictureTypeEnum(int id, string name) : base(id, name)
        {

        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<PictureTypeEnum> List() =>
            new[] { Standard, ProgressPic };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static PictureTypeEnum FromName(string name)
        {
            PictureTypeEnum policy = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (policy == null)
                throw new SocialNetworkGenericException($"Possible values for PictureType: {String.Join(",", List().Select(s => s.Name))}");


            return policy;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static PictureTypeEnum From(int id)
        {
            PictureTypeEnum policy = List().SingleOrDefault(s => s.Id == id);

            if (policy == null)
                throw new SocialNetworkGenericException($"Possible values for PictureType: {String.Join(",", List().Select(s => s.Name))}");


            return policy;
        }

    }
}
