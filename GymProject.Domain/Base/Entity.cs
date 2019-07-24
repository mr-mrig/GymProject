using GymProject.Domain.Base.Mediator;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.Base
{



    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {


        int? _requestedHashCode;


        public virtual TId Id { get; protected set; }



        #region Domain Events Management

        private List<IMediatorNotification> _domainEvents;
        /// <summary>
        /// List of domain events as MediatR notification messages
        /// </summary>
        public List<IMediatorNotification> DomainEvents => _domainEvents;

        /// <summary>
        /// Add a domain event to the pipeline.
        /// The event will be actually raised before or after the transaction commit according to the UnitOfWork design
        /// </summary>
        /// <param name="eventItem">The MediatR notification message</param>
        public void AddDomainEvent(IMediatorNotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IMediatorNotification>();
            _domainEvents.Add(eventItem);
        }

        /// <summary>
        /// Remove a domain event from the pipeline
        /// </summary>
        /// <param name="eventItem">The MediatR notification message</param>
        public void RemoveDomainEvent(IMediatorNotification eventItem)
        {
            if (_domainEvents is null)
                return;

            _domainEvents.Remove(eventItem);
        }
        #endregion




        #region Methods
        public bool IsTransient()
        {
            //return this.Id == default(TId);
            return Equals(Id, default(TId));
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TId>))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            Entity<TId> item = (Entity<TId>)obj;

            if (item.IsTransient() || IsTransient())
                return false;
            else
                return Id.Equals(item.Id);
        }


        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }


        public bool Equals(Entity<TId> other)
        {
            if (other == null)
                return false;


            if (other.IsTransient() && IsTransient())
                return ReferenceEquals(other, this);

            return other.Id.Equals(Id);
        }
        #endregion


        #region Operators

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null));
            else
                return left.Equals(right);
        }


        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !(left == right);
        }
        #endregion


    }


}
