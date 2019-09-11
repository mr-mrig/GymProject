using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Domain.UserAccountDomain.UserAggregate
{
    public class UserRoot : Entity<uint?>, IAggregateRoot
    {





        /// <summary>
        /// The Account Email
        /// </summary>
        public string Email { get; private set; } = string.Empty;


        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; private set; } = string.Empty;


        /// <summary>
        /// The Salt
        /// </summary>
        public string Salt { get; private set; } = string.Empty;


        /// <summary>
        /// The Username
        /// </summary>
        public string UserName { get; private set; } = string.Empty;


        /// <summary>
        /// The Date the account has been created
        /// </summary>
        public DateTime SubscriptionDate { get; private set; }


        /// <summary>
        /// The Account status
        /// </summary>
        public AccountStatusTypeEnum AccountStatusType { get; private set; } = null;





        #region Ctors

        private UserRoot() : base(null) { }


        private UserRoot(uint? id, string email, string username, string password, string salt, DateTime subscriptionDate, AccountStatusTypeEnum accountStatusType) : base(id)
        {
            Email = email;
            UserName = username;
            Password = password;
            Salt = salt;
            SubscriptionDate = subscriptionDate;
            AccountStatusType = accountStatusType;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <param name="username">The username</param>
        /// <param name="email">The registration email</param>
        /// <param name="subscriptionDate">The registration date</param>
        /// <param name="password">The password</param>
        /// <param name="salt">The salt</param>
        /// <param name="accountStatusType">The account status</param>
        /// <returns>The UserRoot instance</returns>
        public static UserRoot RegisterUser
        (
            uint? id, 
            string email, 
            string username, 
            string password, 
            string salt, 
            DateTime subscriptionDate, 
            AccountStatusTypeEnum accountStatusType
        )
            => new UserRoot(id, email, username, password, salt, subscriptionDate, accountStatusType);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="email">The registration email</param>
        /// <param name="subscriptionDate">The registration date</param>
        /// <param name="password">The password</param>
        /// <param name="salt">The salt</param>
        /// <param name="accountStatusType">The account status</param>
        /// <returns>The UserRoot instance</returns>
        public static UserRoot RegisterUser
        (
            string email,
            string username,
            string password,
            string salt,
            DateTime subscriptionDate,
            AccountStatusTypeEnum accountStatusType
        )
            => RegisterUser(null, email, username, password, salt, subscriptionDate, accountStatusType);

        #endregion



        #region Public Methods


        #endregion




        #region Business Rules Specifications

        /// <summary>
        /// Test the Diet Plan business rules and manages invalid states
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestBusinessRules()
        {

        }

        #endregion

    }
}
