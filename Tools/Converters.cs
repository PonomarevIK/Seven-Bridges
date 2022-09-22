using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;

namespace Seven_Bridges.Converters
{
    // Returns an average of all bound values
    public class AverageValue : IMultiValueConverter
    {
        private static Func<object, double> propertyConverter = (value) => value == DependencyProperty.UnsetValue ? 0 : System.Convert.ToDouble(value);

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Average(propertyConverter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Shifts a control using margins so that it's top-left corner becomes it's center
    public class CenteredMargin : IMultiValueConverter
    {
        private static Func<object, bool> isSet = (value) => value != DependencyProperty.UnsetValue;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.All(isSet))
            {
                double leftMargin = -System.Convert.ToDouble(values[0]) / 2;
                double topMargin = -System.Convert.ToDouble(values[1]) / 2;
                return new Thickness(leftMargin, topMargin, 0, 0);
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Calculates points for drawing a triangle to make the edge look like an arrow
    public class DirectedEdgeArrowPosition : IMultiValueConverter
    {
        private static Func<object, bool> isSet = (value) => value != DependencyProperty.UnsetValue;

        private static (Vector, Vector) GetPerpendicularVectors(Vector vector, double length = 1)
        {
            Vector vector1 = new Vector(vector.Y, -vector.X);
            vector1.Normalize();
            Vector vector2 = new Vector(-vector.Y, vector.X);
            vector2.Normalize();
            return (vector1 * length, vector2 * length);
        }
        private enum BoundValues : int
        {
            IsDirected,
            V1_X,
            V1_Y,
            V2_X,
            V2_Y,
            V2_diameter,
            LineThickness,
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object Unpack(BoundValues value) => values[(int)value];

            if (!(bool)Unpack(BoundValues.IsDirected)) return null;

            PointCollection result = new PointCollection();
            if (values.All(isSet))
            {
                Point origin = new Point((double)Unpack(BoundValues.V1_X), (double)Unpack(BoundValues.V1_Y));
                Point destination = new Point((double)Unpack(BoundValues.V2_X), (double)Unpack(BoundValues.V2_Y));
                Vector edgeVector = destination - origin;
                double vertexRadius = (double)Unpack(BoundValues.V2_diameter) / 2;
                double vectorLength = edgeVector.Length;
                double ratio = vertexRadius / vectorLength;
                double lineThickness = (double)Unpack(BoundValues.LineThickness);

                Point triangleTopPoint = destination - edgeVector * ratio;
                Point triangleBaseCenter = triangleTopPoint - edgeVector * (2 * lineThickness / vectorLength);
                (Vector leftOffset, Vector rightOffset) = GetPerpendicularVectors(edgeVector, lineThickness);
                result.Add(triangleTopPoint);
                result.Add(triangleBaseCenter + leftOffset);
                result.Add(triangleBaseCenter + rightOffset);
                return result;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}