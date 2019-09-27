using GymProject.Domain.Base.Mediator;
using MediatR;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.Base
{



    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {


        // In order to not overwrite the Hash Code when it has been generated once
        int? _requestedHashCode;


        public virtual TId Id { get; protected set; }



        public Entity(TId id)
        {
            if (id == null)
                Id = default;
            else
                Id = id;
        }


        #region Domain Events Management

        private List<INotification> _domainEvents;
        /// <summary>
        /// List of domain events as MediatR notification messages
        /// </summary>
        public List<INotification> DomainEvents => _domainEvents;

        /// <summary>
        /// Add a domain event to the pipeline.
        /// The event will be actually raised before or after the transaction commit according to the UnitOfWork design
        /// </summary>
        /// <param name="eventItem">The MediatR notification message</param>
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        /// <summary>
        /// Remove a domain event from the pipeline
        /// </summary>
        /// <param name="eventItem">The MediatR notification message</param>
        public void RemoveDomainEvent(INotification eventItem)
        {
            if (_domainEvents is null)
                return;

            _domainEvents.Remove(eventItem);
        }

        /// <summary>
        /// Clear all the domain events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
        #endregion




        #region Methods
        public bool IsTransient()
        {
            //return this == default(TId);
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
            
            return Id.Equals(item.Id);
        }


        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            else
            {
                // Never change the Hash Code again
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
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
