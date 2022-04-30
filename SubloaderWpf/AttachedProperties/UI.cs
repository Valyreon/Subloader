using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SubloaderWpf.AttachedProperties
{
    public static class UI
    {
        public static readonly DependencyProperty ElevationProperty = DependencyProperty.RegisterAttached("Elevation",
            typeof(double), typeof(UI),
            new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, null,
                OnElevationChanged));

        public static double GetElevation(this UIElement element)
        {
            return element.GetValue(ElevationProperty) as double? ?? default;
        }

        public static void SetElevation(this UIElement element, double elevation)
        {
            element.SetValue(ElevationProperty, elevation);
        }

        private static Effect CreateElevation(double elevation, Effect source)
        {
            static void MixShadows(DropShadowEffect nearest, DropShadowEffect matched, double balance)
            {
                matched.BlurRadius = (matched.BlurRadius * (1 - balance)) + (nearest.BlurRadius * balance);
                matched.ShadowDepth = (matched.ShadowDepth * (1 - balance)) + (nearest.ShadowDepth * balance);
            }

            DropShadowEffect[] shadows =
            {
                new() { BlurRadius = 5, ShadowDepth = 1 },
                new() { BlurRadius = 8, ShadowDepth = 1.5 },
                new() { BlurRadius = 14, ShadowDepth = 4.5 },
                new() { BlurRadius = 25, ShadowDepth = 8 },
                new() { BlurRadius = 35, ShadowDepth = 13 }
            };
            elevation = Math.Max(0, (elevation / 12 * shadows.Length) - 1);
            int prevIndex = (int)Math.Floor(elevation),
                index = (int)elevation,
                nextIndex = (int)Math.Ceiling(elevation);
            var approx = elevation - index;
            var shadow = shadows[index];
            if (approx != 0)
            {
                MixShadows(approx < 0 ? shadows[prevIndex] : shadows[nextIndex], shadow, Math.Abs(approx));
            }

            var modify = false;
            if (source is DropShadowEffect sourceShadow)
            {
                sourceShadow.BlurRadius = shadow.BlurRadius;
                sourceShadow.ShadowDepth = shadow.ShadowDepth;
                shadow = sourceShadow;
                modify = true;
            }

            shadow.Direction = 270;
            shadow.Color = Color.FromArgb(0xAA, 0, 0, 0);
            shadow.Opacity = .42;
            shadow.RenderingBias = RenderingBias.Performance;
            return modify ? null : shadow;
        }

        private static object OnElevationChanged(DependencyObject d, object value)
        {
            if (d is not UIElement element || value is not double elevation)
            {
                return value;
            }

            if (elevation == 0)
            {
                element.Effect = null;
            }
            else
            {
                var e = CreateElevation(elevation, element.Effect);
                if (e != null)
                {
                    element.Effect = e;
                }
            }

            return value;
        }
    }
}
