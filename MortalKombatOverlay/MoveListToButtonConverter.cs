using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MortalKombatOverlay
{
    public class MoveListToButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is KeyValuePair<string, List<MovePart>> moveData)
            {
                var moveName = moveData.Key;
                var moveParts = moveData.Value;
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                // Add move name
                var moveNameTextBlock = new TextBlock
                {
                    Text = moveName + ": ",
                    VerticalAlignment = VerticalAlignment.Center
                };
                panel.Children.Add(moveNameTextBlock);

                // Create a drop shadow effect
                var shadow = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 5,
                    Opacity = 0.7
                };

                foreach (var movePart in moveParts)
                {
                    string content = "";
                    SolidColorBrush buttonColor = new SolidColorBrush(Colors.Transparent);
                    SolidColorBrush textColor = new SolidColorBrush(Colors.White);

                    switch (movePart.Value)
                    {
                        case "F":
                            content = "→";
                            break;
                        case "B":
                            content = "←";
                            break;
                        case "U":
                            content = "↑";
                            break;
                        case "D":
                            content = "↓";
                            break;
                        case "S":
                            content = movePart.Value;
                            buttonColor = new SolidColorBrush(Colors.Aqua);
                            break;
                        case "G":
                            content = movePart.Value;
                            buttonColor = new SolidColorBrush(Colors.Gray);
                            break;
                        case "BP":
                            content = movePart.Value;
                            buttonColor = new SolidColorBrush(Colors.Green);
                            break;
                        case "BK":
                            content = movePart.Value;
                            buttonColor = new SolidColorBrush(Colors.Blue);
                            break;
                        case "FP":
                            content = movePart.Value;
                            buttonColor = new SolidColorBrush(Colors.DeepPink);
                            break;
                        case "FK":
                            content = movePart.Value;
                            buttonColor = new SolidColorBrush(Colors.Red);
                            break;
                    }

                    var border = new Border
                    {
                        Width = 28,
                        Height = 28,
                        CornerRadius = new CornerRadius(14),
                        Background = buttonColor,
                        BorderBrush = buttonColor,
                        Effect = shadow,
                        Child = new TextBlock
                        {
                            Text = content,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(2),  // Apply margin to the last button
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Foreground = textColor
                        }
                    };

                    // Check if this is the last item in the list
                    if (movePart.Equals(moveParts[moveParts.Count - 1]))
                    {
                        border.Margin = new Thickness(2, 2, 6, 2);  // Apply margin to the last button
                    }

                    panel.Children.Add(border);
                }

                return panel;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
