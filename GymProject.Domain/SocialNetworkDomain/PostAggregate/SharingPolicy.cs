﻿using GymProject.Domain.Base;
using GymProject.Domain.SocialNetworkDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class SharingPolicy : Enumeration
    {


        public static SharingPolicy Public = new SharingPolicy(1, "Public");
        public static SharingPolicy Private = new SharingPolicy(2, "Private");




        #region Ctors

        public SharingPolicy(int id, string name) : base(id, name)
        {

        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<SharingPolicy> List() =>
            new[] { Public, Private };


        /// <summary>
        /// Creates a SharingPolicy object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The SharingPolicy object instance</returns>
        public static SharingPolicy FromName(string name)
        {
            SharingPolicy policy = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (policy == null)
                throw new SocialNetworkGenericException($"Possible values for SharingPolicy: {String.Join(",", List().Select(s => s.Name))}");


            return policy;
        }


        /// <summary>
        /// Creates a SharingPolicy object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The SharingPolicy object instance</returns>
        public static SharingPolicy From(int id)
        {
            SharingPolicy policy = List().SingleOrDefault(s => s.Id == id);

            if (policy == null)
                throw new SocialNetworkGenericException($"Possible values for SharingPolicy: {String.Join(",", List().Select(s => s.Name))}");
            

            return policy;
        }



    }
}
