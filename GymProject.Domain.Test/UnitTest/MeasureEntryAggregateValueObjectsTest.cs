using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using System;
using Xunit;
using GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate;

namespace GymProject.Domain.Test.UnitTest
{
    public class MeasureEntryAggregateValueObjectsTest
    {



        [Fact]
        public void CheckCaliperSkinfoldValue()
        {
            CaliperSkinfoldValue millimeters = CaliperSkinfoldValue.Measure(6.7f);
            CaliperSkinfoldValue millimeters2 = CaliperSkinfoldValue.MeasureMillimeters(6.91f);
            CaliperSkinfoldValue inches = CaliperSkinfoldValue.Measure(0.118f, LengthMeasureUnitEnum.Inches);
            CaliperSkinfoldValue inches2 = CaliperSkinfoldValue.MeasureInches(0.121f);

            Assert.NotNull(millimeters);
            Assert.Equal(millimeters, millimeters2);
            Assert.Equal(7f, millimeters.Value);
            Assert.NotNull(inches2);
            Assert.Equal(inches, inches2);
            Assert.Equal(0.12f, inches.Value);


            #region Not yet supported conversions
            //// Metric 2 Metric Conversions
            //CaliperSkinfoldValue sourceMetric = CaliperSkinfoldValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            //CaliperSkinfoldValue metersConv = sourceMetric.ConvertMetricVsImperial(LengthMeasureUnitEnum.Meters);
            //CaliperSkinfoldValue centimetersConv = sourceMetric.ConvertMetricVsImperial(LengthMeasureUnitEnum.Centimeters);
            //CaliperSkinfoldValue millimetersConv = sourceMetric.ConvertMetricVsImperial(LengthMeasureUnitEnum.Millimeters);

            //Assert.Equal(sourceMetric.Value * 1000f, metersConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Meters, metersConv.Unit);
            //Assert.Equal(sourceMetric.Value * 100000f, centimetersConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Centimeters, centimetersConv.Unit);
            //Assert.Equal(sourceMetric.Value * 1000000f, millimetersConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Millimeters, millimetersConv.Unit);

            //// Imperial 2 Imperial Conversions
            //CaliperSkinfoldValue sourceImperial = CaliperSkinfoldValue.Measure(0.3f, LengthMeasureUnitEnum.Miles);
            //CaliperSkinfoldValue yardsConv = sourceMetric.ConvertMetricVsImperial(LengthMeasureUnitEnum.Meters);
            //CaliperSkinfoldValue inchesConv = sourceMetric.ConvertMetricVsImperial(LengthMeasureUnitEnum.Centimeters);


            //Assert.Equal(sourceImperial.Value * 1760, yardsConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Yards, yardsConv.Unit);
            //Assert.Equal(sourceImperial.Value * 63360f, inchesConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Inches, inchesConv.Unit);
            #endregion


            // Metric 2 Imperial Conversions
            CaliperSkinfoldValue metricSrc = CaliperSkinfoldValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            CaliperSkinfoldValue milesConv = metricSrc.ConvertMetricVsImperial(LengthMeasureUnitEnum.Miles);
            CaliperSkinfoldValue sourceMixed2 = CaliperSkinfoldValue.Measure(101.67f, LengthMeasureUnitEnum.Centimeters);
            CaliperSkinfoldValue cmToInches = sourceMixed2.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches);
            CaliperSkinfoldValue sourceMixed3 = CaliperSkinfoldValue.Measure(101.67f, LengthMeasureUnitEnum.Millimeters);
            CaliperSkinfoldValue mmToInches = sourceMixed3.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches);

            // km to miles
            Assert.Equal((float)Math.Round(metricSrc.Value / 1.609f, 1), milesConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Miles, milesConv.Unit);
            // cm to inches
            Assert.Equal((float)Math.Round(sourceMixed2.Value / 2.54f, 2), cmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, cmToInches.Unit);
            // mm to inches
            Assert.Equal((float)Math.Round(sourceMixed3.Value / 25.4f, 2), mmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, mmToInches.Unit);

            // Imperial 2 Metric Conversions
            CaliperSkinfoldValue imperialSrc = CaliperSkinfoldValue.Measure(4.5f, LengthMeasureUnitEnum.Miles);
            CaliperSkinfoldValue kmConv = imperialSrc.ConvertMetricVsImperial(LengthMeasureUnitEnum.Kilometers);
            CaliperSkinfoldValue imperialSrc2 = CaliperSkinfoldValue.Measure(101.67f, LengthMeasureUnitEnum.Inches);
            CaliperSkinfoldValue inchesToCm = imperialSrc2.ConvertMetricVsImperial(LengthMeasureUnitEnum.Centimeters);
            CaliperSkinfoldValue imperialSrc3 = CaliperSkinfoldValue.Measure(101.67f, LengthMeasureUnitEnum.Inches);
            CaliperSkinfoldValue inchesToMm = imperialSrc3.ConvertMetricVsImperial(LengthMeasureUnitEnum.Millimeters);

            // km to miles
            Assert.Equal((float)Math.Round(imperialSrc.Value * 1.609f, 1), kmConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Kilometers, kmConv.Unit);
            // cm to inches
            Assert.Equal((float)Math.Round(imperialSrc2.Value * 2.54f, 1), inchesToCm.Value);
            Assert.Equal(LengthMeasureUnitEnum.Centimeters, inchesToCm.Unit);
            // mm to inches
            Assert.Equal((float)Math.Round(imperialSrc3.Value * 25.4f, 0), inchesToMm.Value);
            Assert.Equal(LengthMeasureUnitEnum.Millimeters, inchesToMm.Unit);
        }


        [Fact]
        public void CheckCircumferenceSkinfoldValue()
        {
            BodyMeasureValue cm = BodyMeasureValue.Measure(66.72f);
            BodyMeasureValue cm2 = BodyMeasureValue.MeasureCentimeters(66.68f);
            BodyMeasureValue inches = BodyMeasureValue.Measure(0.118f, LengthMeasureUnitEnum.Inches);
            BodyMeasureValue inches2 = BodyMeasureValue.MeasureInches(0.121f);

            Assert.NotNull(cm);
            Assert.Equal(cm, cm2);
            Assert.Equal(66.7f, cm.Value);
            Assert.NotNull(inches2);
            Assert.Equal(inches, inches2);
            Assert.Equal(0.1f, inches.Value);

            #region Not yet supported conversions
            //// Metric 2 Metric Conversions
            //BodyCircumferenceValue sourceMetric = BodyCircumferenceValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            //BodyCircumferenceValue metersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Meters);
            //BodyCircumferenceValue centimetersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Centimeters);
            //BodyCircumferenceValue millimetersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Millimeters);

            //Assert.Equal(sourceMetric.Value * 1000f, metersConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Meters, metersConv.Unit);
            //Assert.Equal(sourceMetric.Value * 100000f, centimetersConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Centimeters, centimetersConv.Unit);
            //Assert.Equal(sourceMetric.Value * 1000000f, millimetersConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Millimeters, millimetersConv.Unit);

            //// Imperial 2 Imperial Conversions
            //BodyCircumferenceValue sourceImperial = BodyCircumferenceValue.Measure(0.3f, LengthMeasureUnitEnum.Miles);
            //BodyCircumferenceValue yardsConv = sourceMetric.Convert(LengthMeasureUnitEnum.Meters);
            //BodyCircumferenceValue inchesConv = sourceMetric.Convert(LengthMeasureUnitEnum.Centimeters);

            //Assert.Equal(sourceImperial.Value * 1760, yardsConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Yards, yardsConv.Unit);
            //Assert.Equal(sourceImperial.Value * 63360f, inchesConv.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Inches, inchesConv.Unit);
            #endregion


            // Metric 2 Imperial Conversions
            //BodyMeasureValue metricSrc = BodyMeasureValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            //BodyMeasureValue kmToMiles = metricSrc.Convert(LengthMeasureUnitEnum.Miles);
            BodyMeasureValue metricSrc2 = BodyMeasureValue.Measure(101.67f, LengthMeasureUnitEnum.Centimeters);
            BodyMeasureValue cmToInches = metricSrc2.Convert(LengthMeasureUnitEnum.Inches);
            //BodyMeasureValue metricSrc3 = BodyMeasureValue.Measure(101.67f, LengthMeasureUnitEnum.Millimeters);
            //BodyMeasureValue mmToInches = metricSrc3.Convert(LengthMeasureUnitEnum.Inches);

            // km to miles
            //Assert.Equal((float)Math.Round(metricSrc.Value / 1.609f, 1), kmToMiles.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Miles, kmToMiles.Unit);
            // cm to inches
            Assert.Equal((float)Math.Round(metricSrc2.Value / 2.54f, 1), cmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, cmToInches.Unit);
            // mm to inches
            //Assert.Equal((float)Math.Round(metricSrc3.Value / 25.4f, 1), mmToInches.Value);
            //Assert.Equal(LengthMeasureUnitEnum.Inches, mmToInches.Unit);
        }


        [Fact]
        public void CheckPercentageValue()
        {
            float percNum = 70.26f;
            PercentageValue perc = PercentageValue.MeasurePercentage(percNum);
            PercentageValue perc2 = PercentageValue.MeasurePercentage(70.3f);
            PercentageValue perc3 = PercentageValue.MeasurePercentage(percNum, 2);
            PercentageValue perc4 = PercentageValue.MeasurePercentage(50.1f, 0);

            Assert.NotNull(perc);
            Assert.Equal(perc, perc2);
            Assert.NotEqual(perc, perc3);
            Assert.Equal(70.3f, perc.Value);
            Assert.Equal(PercentageMeasureUnitEnum.Percentage, perc.Unit);
            Assert.Equal(percNum, perc3.Value);
            Assert.Equal(50f, perc4.Value);

            PercentageValue ratio = PercentageValue.MeasureRatio(perc.Value / 100);
            PercentageValue ratio2 = PercentageValue.MeasureRatio(perc.Value / 100 + 0.001f);
            PercentageValue ratio3 = PercentageValue.MeasureRatio(percNum / 100, 4);
            PercentageValue ratio4 = PercentageValue.MeasureRatio(perc4.Value / 100, 4);

            Assert.NotNull(perc);
            Assert.Equal(ratio, ratio2);
            Assert.NotEqual(ratio, ratio3);
            Assert.Equal((float)Math.Round(70.3 / 100, 1), ratio.Value);
            Assert.Equal(PercentageMeasureUnitEnum.Ratio, ratio.Unit);
            Assert.Equal((float)Math.Round(percNum / 100, 4), ratio3.Value);
            Assert.Equal((float)Math.Round(50f / 100, 4), ratio4.Value);

            Assert.Equal(perc4.Value / 100f, ratio4.Value);

            PercentageValue test = PercentageValue.MeasurePercentage(90f);
            PercentageValue percToRatio = test.Convert(PercentageMeasureUnitEnum.Ratio);

            Assert.Equal(test.Value, percToRatio.Convert(PercentageMeasureUnitEnum.Percentage).Value);
        }


        [Fact]
        public void CheckBodyFat()
        {
            BodyFatValue bf = BodyFatValue.MeasureBodyFat(7.811f);
            BodyFatValue bf2 = BodyFatValue.MeasureBodyFat(7.809f);

            Assert.NotNull(bf);
            Assert.Equal(bf, bf2);
            Assert.Equal(7.81f, bf.Value);
        }
    }
}
