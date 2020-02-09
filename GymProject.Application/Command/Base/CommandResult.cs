using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;

namespace GymProject.Application.Command.Base
{
    public class CommandResult
    {

        public enum ResultState
        {
            OK = 0,
            KO
        }

        public enum FailReason
        {
            DatabaseFail = 0,
            DomainInvariantViolated,
            EFCoreFailure,
            GenericException
        }


        #region Backoff Fields
        private Exception _exceptionThrown;
        #endregion

        /// <summary>
        /// <para>The Ids affected by the command:</para>
        /// <para>ADD: the DB generated IDs</para>
        /// <para>UPDATE: The IDs the command has updated</para>
        /// <para>Notice that if the Command affects a complex hierarchy of objects, the IDs returned are the root ones only</para>
        /// </summary>
        public IEnumerable<uint?> Ids { get; set; }

        /// <summary>
        /// The result state of the command
        /// </summary>
        public ResultState CommandResultState { get; set; }

        /// <summary>
        /// The reson of the Command failure, if any
        /// </summary>
        public FailReason CommandFailureReason { get; set; }

        /// <summary>
        /// The thrwon exception, should be NULL in case of successful Commands
        /// </summary>
        public Exception ExceptionThrown 
        {
            get => _exceptionThrown;
            set
            {
                _exceptionThrown = value;
                if(_exceptionThrown != null)
                {
                    CommandFailureReason = GetFailureReasonFrom(_exceptionThrown);
                    CommandResultState = ResultState.KO;
                }
            }
        }



        private FailReason GetFailureReasonFrom(Exception thrown)
        {
            if (thrown is ValueObjectInvariantViolationException ||
                thrown is GlobalDomainGenericException)
                return FailReason.DomainInvariantViolated;

            string excMessage = thrown.Message.ToLower();

            if (excMessage.Contains("connection") || excMessage.Contains("database") || excMessage.Contains("sqlite"))
                return FailReason.DatabaseFail;

            return FailReason.GenericException;
        }
    }
}
