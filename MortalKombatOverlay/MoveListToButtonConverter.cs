using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MortalKombatOverlay
{
    public class MoveListToButtonConverter : IValueConverter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _useNumberNotation = true;
        public bool UseNumberNotation
        {
            get { return _useNumberNotation; }
            set
            {
                if (_useNumberNotation != value)
                {
                    _useNumberNotation = value;
                    OnPropertyChanged("UseNumberNotation");
                }
            }
        }

        private bool _reverseDirectionsForP2 = true;
        public bool ReverseDirectionsForP2
        {
            get { return _reverseDirectionsForP2; }
            set
            {
                if (_reverseDirectionsForP2 != value)
                {
                    _reverseDirectionsForP2 = value;
                    OnPropertyChanged("ReverseDirectionsForP2");
                }
            }
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

                var moveNameTextBlock = new TextBlock
                {
                    Text = moveName + ": ",
                    VerticalAlignment = VerticalAlignment.Center
                };
                panel.Children.Add(moveNameTextBlock);

                var shadow = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 5,
                    Opacity = 0.7
                };

                var shouldReverse = (string)parameter == "Player2" && ReverseDirectionsForP2;

                foreach (var movePart in moveParts)
                {
                    string content = "";
                    SolidColorBrush buttonColor = new SolidColorBrush(Colors.Transparent);
                    SolidColorBrush textColor = new SolidColorBrush(Colors.White);

                    switch (movePart.Value)
                    {
                        case "F":
                            content = shouldReverse ? "←" : "→";
                            break;
                        case "B":
                            content = shouldReverse ? "→" : "←";
                            break;
                        case "U":
                            content = "↑";
                            break;
                        case "D":
                            content = "↓";
                            break;
                        case "S":
                            content = "S";
                            buttonColor = new SolidColorBrush(Colors.Aqua);
                            break;
                        case "G":
                            content = "G";
                            buttonColor = new SolidColorBrush(Colors.Gray);
                            break;
                        case "BP":
                            content = UseNumberNotation ? "2" : "BP";
                            buttonColor = new SolidColorBrush(Colors.Green);
                            break;
                        case "BK":
                            content = UseNumberNotation ? "4" : "BK";
                            buttonColor = new SolidColorBrush(Colors.Blue);
                            break;
                        case "FP":
                            content = UseNumberNotation ? "1" : "FP";
                            buttonColor = new SolidColorBrush(Colors.DeepPink);
                            break;
                        case "FK":
                            content = UseNumberNotation ? "3" : "FK";
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
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Foreground = textColor
                        }
                    };

                    if (movePart.Equals(moveParts[moveParts.Count - 1]))
                    {
                        border.Margin = new Thickness(2, 2, 6, 2);
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