﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace GymProject.Domain.Base
{


    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }


        /// <summary>
        /// Object creation forbidden outside of the Class
        /// </summary>
        /// <param name="id">The id of the object, as enum value</param>
        /// <param name="name">The name of the object, as enum name</param>
        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }


        #region Methods


        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override string ToString() => Name;


        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);

        #endregion



    }

}
