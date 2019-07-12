using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.Base
{


    /// <summary>
    ///     Business Entity abstract root class.
    /// </summary>
    public abstract class Entity
    {
        int? _requestedHashCode;


        public virtual long Id { get; protected set; }



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
        #endregion




        #region Methods
        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
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
        #endregion


        #region Operators

        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null));
            else
                return left.Equals(right);
        }


        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
        #endregion
    }

}
