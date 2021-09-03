using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace WLED.Effects
{
    public class ChangeColorSwitchEffect : RoutingEffect
    {

        public static readonly BindableProperty FalseColorProperty = BindableProperty.CreateAttached("FalseColor", typeof(Color), typeof(ChangeColorSwitchEffect),
            Color.Transparent, propertyChanged: OnColorChanged);

        public static readonly BindableProperty TrueColorProperty = BindableProperty.CreateAttached("TrueColor", typeof(Color), typeof(ChangeColorSwitchEffect),
            Color.Transparent, propertyChanged: OnColorChanged);

        /// <summary>
        /// The colour of the "thumb", the thing you tap or drag to toggle the switch
        /// </summary>
        public static readonly BindableProperty ThumbColorProperty = BindableProperty.CreateAttached("ThumbColor", typeof(Color), typeof(ChangeColorSwitchEffect),
            Color.Silver, propertyChanged: OnColorChanged);

        /// <summary>
        /// [assembly: ResolutionGroupName("xamformsdemo")] moved to the AssemblyInfo
        /// classes in the platform projects
        /// </summary>
        public ChangeColorSwitchEffect() : base("xamformsdemo.ChangeColorSwitchEffect")
        {
        }

        private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as Switch;
            if (control == null)
                return;

            var color = (Color)newValue;

            var attachedEffect = control.Effects.FirstOrDefault(e => e is ChangeColorSwitchEffect);
            if (color != Color.Transparent && attachedEffect == null)
                control.Effects.Add(new ChangeColorSwitchEffect());
            else if (color == Color.Transparent && attachedEffect != null)
                control.Effects.Remove(attachedEffect);
        }

        public static Color GetFalseColor(BindableObject view)
        {
            return (Color)view.GetValue(FalseColorProperty);
        }

        public static void SetFalseColor(BindableObject view, string color)
        {
            view.SetValue(FalseColorProperty, color);
        }

        public static Color GetTrueColor(BindableObject view)
        {
            return (Color)view.GetValue(TrueColorProperty);
        }

        public static void SetTrueColor(BindableObject view, Color color)
        {
            view.SetValue(TrueColorProperty, color);
        }

        public static Color GetThumbColor(BindableObject view)
        {
            return (Color)view.GetValue(ThumbColorProperty);
        }

        public static void SetThumbColor(BindableObject view, Color color)
        {
            view.SetValue(ThumbColorProperty, color);
        }
    }
}