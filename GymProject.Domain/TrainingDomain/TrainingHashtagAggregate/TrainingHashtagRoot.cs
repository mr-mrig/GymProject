using System;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.TrainingDomain.TrainingHashtagAggregate
{
    public class TrainingHashtagRoot : StatusTrackingEntity<uint?>, IAggregateRoot, ICloneable
    {



        /// <summary>
        /// The Hashtag content
        /// </summary>
        public GenericHashtagValue Hashtag { get; private set; }



        #region Ctors

        private TrainingHashtagRoot() : base(null, null)
        {

        }

        private TrainingHashtagRoot(uint? id, GenericHashtagValue hashtag, EntryStatusTypeEnum status = null) : base(id, status)
        {
            Hashtag = hashtag;
            TestBusinessRules();
        }
        #endregion


        #region Factory

        /// <summary>
        /// Factory method for transient entities
        /// </summary>
        /// <param name="hashtag">The hashtag content</param>
        /// <returns>The TrainingHashtagRoot instance</returns>
        public static TrainingHashtagRoot TagWithTransient(GenericHashtagValue hashtag)

            => TagWith(null, hashtag, null);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the entity</param>
        /// <param name="hashtag">The hashtag content</param>
        /// <returns>The TrainingHashtagRoot instance</returns>
        public static TrainingHashtagRoot TagWith(uint? id, GenericHashtagValue hashtag)

            => TagWith(id, hashtag, null);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the entity</param>
        /// <param name="hashtag">The hashtag content</param>
        /// <param name="status">The Status of the entry</param>
        /// <returns>The TrainingHashtagRoot instance</returns>
        public static TrainingHashtagRoot TagWith(uint? id, GenericHashtagValue hashtag, EntryStatusTypeEnum status)

            => new TrainingHashtagRoot(id, hashtag, status);

        #endregion




        #region Business Rules Validation

        private void TestBusinessRules()
        {
            
        }

        #endregion


        #region ICloneable Implementation

        public object Clone()

            => TagWith(Id, Hashtag, EntryStatus);


        #endregion
    }
}

