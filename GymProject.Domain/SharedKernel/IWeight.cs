
namespace GymProject.Domain.SharedKernel
{
    public interface IWeight
    {


        /// <summary>
        /// The weight value
        /// </summary>
        float Value { get; }

        /// <summary>
        /// The measurement unit
        /// </summary>
        WeightUnitMeasureEnum Unit { get; }




        /// <summary>
        /// Creates a new ValueObject which is the conversion of the current one to the selected measure unit.
        /// If no conversion is needed return a new IWeight instance.
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new IWeight instance</returns>
        IWeight Convert(WeightUnitMeasureEnum toUnit);

    }
}
