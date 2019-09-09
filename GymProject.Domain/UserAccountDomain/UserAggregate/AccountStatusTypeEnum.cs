using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.UserAccountDomain.UserAggregate
{
    public class AccountStatusTypeEnum : Enumeration
    {



        public static AccountStatusTypeEnum Active = new AccountStatusTypeEnum(1, "Active");
        public static AccountStatusTypeEnum Inactive = new AccountStatusTypeEnum(2, "Inactive");
        public static AccountStatusTypeEnum Banned = new AccountStatusTypeEnum(3, "Banned");






        #region Ctors

        public AccountStatusTypeEnum(int id, string name) : base(id, name)
        {

        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<AccountStatusTypeEnum> List() =>
            new[] { Active, Inactive, Banned, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static AccountStatusTypeEnum FromName(string name)
        {
            AccountStatusTypeEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static AccountStatusTypeEnum From(int id)
        {
            AccountStatusTypeEnum unit = List().SingleOrDefault(s => s.Id == id);

            return unit;
        }


    }
}
