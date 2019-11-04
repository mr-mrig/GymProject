using GymProject.Application.Command;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.MediatorBehavior;
using GymProject.Application.Test.Utils;
using GymProject.Application.Validator.TrainingDomain;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS
{
    public class TrainingDomainQueriesTest
    {


        [Fact]
        public async Task GetTraininPlansSummariesTest()
        {
            GymContext context = StaticUtilities.InitQueryTest(MethodBase.GetCurrentMethod().DeclaringType.Name);

            //MyDbContext.Database.Connection.ConnectionString

            
        }


    }
}
