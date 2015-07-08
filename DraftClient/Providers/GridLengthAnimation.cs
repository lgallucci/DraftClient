namespace DraftClient.Providers
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class GridLengthAnimation : AnimationTimeline
    {
        /// <summary>
        /// Returns the type of object to animate
        /// </summary>
        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        /// <summary>
        /// Creates an instance of the animation object
        /// </summary>
        /// <returns>Returns the instance of the GridLengthAnimation</returns>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        /// <summary>
        /// Dependency property for the From property
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                typeof(GridLengthAnimation));

        /// <summary>
        /// CLR Wrapper for the From depenendency property
        /// </summary>
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the To property
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength),
                typeof(GridLengthAnimation));

        /// <summary>
        /// CLR Wrapper for the To property
        /// </summary>
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        /// <summary>
        /// Animates the grid let set
        /// </summary>
        /// <param name="defaultOriginValue">The original value to animate</param>
        /// <param name="defaultDestinationValue">The final value</param>
        /// <param name="animationClock">The animation clock (timer)</param>
        /// <returns>Returns the new grid length to set</returns>
        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
            GridUnitType fromType = ((GridLength) GetValue(GridLengthAnimation.FromProperty)).GridUnitType;
            //check that from was set from the caller
            if (fromVal == 1)
                //set the from as the actual value
                fromVal = ((GridLength)defaultDestinationValue).Value;

            double toVal = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;
            GridUnitType toType = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).GridUnitType;

            if (fromVal > toVal)
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal, GridUnitType.Pixel);
            else
                return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal, GridUnitType.Pixel);
        }
    }
}
