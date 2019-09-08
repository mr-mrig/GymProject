using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingNoteTest
    {


        public const int ntests = 200;


        [Fact]
        public void WorkingSetNoteRootTest()
        {
            float fakeCaseProbability = 0.3f;

            WorkingSetNoteRoot note;
            PersonalNoteValue body;
            uint? id;

            for (int itest = 0; itest < ntests; itest++)
            {
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.25f);
                bool isFake = RandomFieldGenerator.RollEventWithProbability(fakeCaseProbability);

                if (isFake)
                {

                    // NULL Body
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = null;

                    Assert.Throws<TrainingDomainInvariantViolationException>(()
                        => isTransient ? WorkingSetNoteRoot.WriteTransient(body) : WorkingSetNoteRoot.Write(id, body));
                }
                else
                {
                    // Valid fields
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

                    if (isTransient)
                    {
                        note = WorkingSetNoteRoot.WriteTransient(body);
                        Assert.Null(note.Id);
                    }
                    else
                    {
                        note = WorkingSetNoteRoot.Write(id, body);
                        Assert.Equal(id, note.Id);
                    }

                    Assert.Equal(body, note.Body);
                }
            }
        }


        [Fact]
        public void WorkUnitTemplateNoteRootTest()
        {
            float fakeCaseProbability = 0.3f;

            WorkUnitTemplateNoteRoot note;
            PersonalNoteValue body;
            uint? id;

            for (int itest = 0; itest < ntests; itest++)
            {
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.25f);
                bool isFake = RandomFieldGenerator.RollEventWithProbability(fakeCaseProbability);

                if (isFake)
                {

                    // NULL Body
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = null;

                    Assert.Throws<TrainingDomainInvariantViolationException>(()
                        => isTransient ? WorkingSetNoteRoot.WriteTransient(body) : WorkingSetNoteRoot.Write(id, body));
                }
                else
                {
                    // Valid fields
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

                    if (isTransient)
                    {
                        note = WorkUnitTemplateNoteRoot.WriteTransient(body);
                        Assert.Null(note.Id);
                    }
                    else
                    {
                        note = WorkUnitTemplateNoteRoot.Write(id, body);
                        Assert.Equal(id, note.Id);
                    }

                    Assert.Equal(body, note.Body);
                }
            }
        }


        [Fact]
        public void TrainingPlanMessageRootTest()
        {
            float fakeCaseProbability = 0.3f;

            TrainingPlanMessageRoot note;
            PersonalNoteValue body;
            uint? id;

            for (int itest = 0; itest < ntests; itest++)
            {
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.25f);
                bool isFake = RandomFieldGenerator.RollEventWithProbability(fakeCaseProbability);

                if (isFake)
                {

                    // NULL Body
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = null;

                    Assert.Throws<TrainingDomainInvariantViolationException>(()
                        => isTransient ? WorkingSetNoteRoot.WriteTransient(body) : WorkingSetNoteRoot.Write(id, body));
                }
                else
                {
                    // Valid fields
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

                    if (isTransient)
                    {
                        note = TrainingPlanMessageRoot.WriteTransient(body);
                        Assert.Null(note.Id);
                    }
                    else
                    {
                        note = TrainingPlanMessageRoot.Write(id, body);
                        Assert.Equal(id, note.Id);
                    }

                    Assert.Equal(body, note.Body);
                }
            }
        }


        [Fact]
        public void TrainingPlanNoteRootTest()
        {
            float fakeCaseProbability = 0.3f;

            TrainingPlanNoteRoot note;
            PersonalNoteValue body;
            uint? id;

            for (int itest = 0; itest < ntests; itest++)
            {
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.25f);
                bool isFake = RandomFieldGenerator.RollEventWithProbability(fakeCaseProbability);

                if (isFake)
                {

                    // NULL Body
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = null;

                    Assert.Throws<TrainingDomainInvariantViolationException>(()
                        => isTransient ? WorkingSetNoteRoot.WriteTransient(body) : WorkingSetNoteRoot.Write(id, body));
                }
                else
                {
                    // Valid fields
                    id = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
                    body = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

                    if (isTransient)
                    {
                        note = TrainingPlanNoteRoot.WriteTransient(body);
                        Assert.Null(note.Id);
                    }
                    else
                    {
                        note = TrainingPlanNoteRoot.Write(id, body);
                        Assert.Equal(id, note.Id);
                    }

                    Assert.Equal(body, note.Body);
                }
            }
        }



    }
}
