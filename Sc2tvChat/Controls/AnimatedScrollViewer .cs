using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace RatChat.Controls {
    public class AnimatedScrollViewer: ScrollViewer {
        public AnimatedScrollViewer(): base() {
            this.ScrollChanged += AnimatedScrollViewer_ScrollChanged;
        }

        void AnimatedScrollViewer_ScrollChanged( object sender, ScrollChangedEventArgs e ) {
            if( e.ExtentHeightChange > 0.0 || e.ViewportHeightChange > 0.0 )
                AnimatedScrollDown();
        }

        public static readonly DependencyProperty MyOffsetProperty = DependencyProperty.Register(
            "MyOffset", typeof(double), typeof(AnimatedScrollViewer),
                new PropertyMetadata(new PropertyChangedCallback(onChanged)));

        public double MyOffset {
            get { return (double)this.GetValue(ScrollViewer.VerticalOffsetProperty); }
            set { this.ScrollToVerticalOffset(value); }
        }

        private static void onChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
            ((AnimatedScrollViewer)d).MyOffset = (double)e.NewValue;
        }

        Storyboard scrDown;

        public void AnimatedScrollDown() {
            if (scrDown != null) {
                scrDown.Stop();
                scrDown = null;
            }

            if (ViewportHeight < ExtentHeight) {
                DoubleAnimation goDown = new DoubleAnimation(
                   ExtentHeight - ViewportHeight,
                   new Duration(TimeSpan.FromMilliseconds(200)));

                //goDown.EasingFunction = new BounceEase() { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut };

                scrDown = new Storyboard();
                scrDown.Children.Add(goDown);

                Storyboard.SetTarget(goDown, this);
                Storyboard.SetTargetProperty(goDown, new PropertyPath("MyOffset"));
                scrDown.Completed += ( a, b ) => { scrDown = null; };
                scrDown.Begin();
            }
        }
    }
}

