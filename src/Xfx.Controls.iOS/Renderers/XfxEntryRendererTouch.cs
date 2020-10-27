﻿using System;
using System.ComponentModel;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xfx;
using Xfx.Controls.iOS.Controls;
using Xfx.Controls.iOS.Extensions;
using Xfx.Controls.iOS.Renderers;
using Xfx.Extensions;
using static Xamarin.Forms.Entry;
using ColorExtensions = Xfx.Controls.iOS.Extensions.ColorExtensions;

[assembly: ExportRenderer(typeof(XfxEntry), typeof(XfxEntryRendererTouch))]

namespace Xfx.Controls.iOS.Renderers
{
   public class XfxEntryRendererTouch : ViewRenderer<XfxEntry, FloatLabeledTextField>
   {
      private bool _hasError;
      private bool _hasFocus;
      private UIColor _defaultTextColor;

      protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
      {
         base.OnElementChanged(e);

         // unsubscribe
         if (e.OldElement != null)
         {
            Control.EditingDidBegin -= OnEditingDidBegin;
            Control.EditingDidEnd -= OnEditingDidEnd;
            Control.EditingChanged -= ViewOnEditingChanged;
         }

         if (e.NewElement != null)
         {
            var ctrl = CreateNativeControl();
            SetNativeControl(ctrl);

            if (!string.IsNullOrWhiteSpace(Element.AutomationId))
               SetAutomationId(Element.AutomationId);

            _defaultTextColor = Control.FloatingLabelTextColor;

            SetIsPassword();
            SetText();
            SetHintText();
            SetTextColor();
            SetBackgroundColor();
            SetPlaceholderColor();
            SetKeyboard();
            SetHorizontalTextAlignment();
            SetErrorText();
            SetFont();
            SetFloatingHintEnabled();
            SetErrorDisplay();
            SetFocusedColor();
            SetUnfocusedColor();
            SetReturnType();

            Control.UnderlineErrorTextIsVisible = Element.ErrorDisplay == ErrorDisplay.Underline;

            // subscribe
            Control.EditingDidBegin += OnEditingDidBegin;
            Control.EditingDidEnd += OnEditingDidEnd;
            Control.EditingChanged += ViewOnEditingChanged;
         }
      }

      protected override FloatLabeledTextField CreateNativeControl()
      {
         return new FloatLabeledTextField();
      }

      protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         base.OnElementPropertyChanged(sender, e);

         if (Element == null || Control == null)
            return;

         if (e.PropertyName == PlaceholderProperty.PropertyName)
            SetHintText();
         else if (e.PropertyName == XfxEntry.ErrorTextProperty.PropertyName)
            SetErrorText();
         else if (e.PropertyName == TextColorProperty.PropertyName)
            SetTextColor();
         else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            SetBackgroundColor();
         else if (e.PropertyName == IsPasswordProperty.PropertyName)
            SetIsPassword();
         else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
         {
            SetIsPassword();
            SetTextColor();
         }
         else if (e.PropertyName == TextProperty.PropertyName)
            SetText();
         else if (e.PropertyName == PlaceholderColorProperty.PropertyName)
         {
            SetUnfocusedColor();
            SetPlaceholderColor();
         }
         else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
            SetKeyboard();
         else if (e.PropertyName == HorizontalTextAlignmentProperty.PropertyName)
            SetHorizontalTextAlignment();
         else if (e.PropertyName == XfxEntry.FloatingHintEnabledProperty.PropertyName)
            SetFloatingHintEnabled();
         else if ((e.PropertyName == FontAttributesProperty.PropertyName) ||
                  (e.PropertyName == FontFamilyProperty.PropertyName) ||
                  (e.PropertyName == FontSizeProperty.PropertyName))
            SetFont();
         else if (e.PropertyName == XfxEntry.ActivePlaceholderColorProperty.PropertyName)
            SetFocusedColor();
         else if (e.PropertyName == ReturnTypeProperty.PropertyName)
            SetReturnType();
      }

      private void OnEditingDidEnd(object sender, EventArgs eventArgs)
      {
         // Method is called via event subscription which could result to a call to a disposed object.
         if (Element == null || Control == null)
            return;

         var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
         Element.SetValueFromRenderer(isFocusedPropertyKey, false);
         _hasFocus = false;
         Control.UnderlineColor = GetUnderlineColorForState();
      }

      private void OnEditingDidBegin(object sender, EventArgs eventArgs)
      {
         // Method is called via event subscription which could result to a call to a disposed object.
         if (Element == null || Control == null)
            return;

         var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
         Element.SetValueFromRenderer(isFocusedPropertyKey, true);
         _hasFocus = true;
         Control.UnderlineColor = GetUnderlineColorForState();
      }

      private void ViewOnEditingChanged(object sender, EventArgs eventArgs)
      {
         // Method is called via event subscription which could result to a call to a disposed object.
         if (Element == null || Control == null)
            return;

         Element.SetValueFromRenderer(TextProperty, Control.Text);
      }

      private void SetUnfocusedColor()
      {
         Control.FloatingLabelTextColor = Element.PlaceholderColor == Color.Default
             ? Control.TextColor
             : Element.PlaceholderColor.ToUIColor();
      }

      private void SetFocusedColor()
      {
         Control.FloatingLabelActiveTextColor = Element.ActivePlaceholderColor == Color.Accent
             ? Control.TintColor
             : Element.ActivePlaceholderColor.ToUIColor();
      }

      private void SetReturnType()
      {
         switch (Element.ReturnType)
         {
            case ReturnType.Done:
               Control.ReturnKeyType = UIReturnKeyType.Done;
               break;
            case ReturnType.Default:
               Control.ReturnKeyType = UIReturnKeyType.Default;
               break;
            case ReturnType.Go:
               Control.ReturnKeyType = UIReturnKeyType.Go;
               break;
            case ReturnType.Next:
               Control.ReturnKeyType = UIReturnKeyType.Next;
               break;
            case ReturnType.Search:
               Control.ReturnKeyType = UIReturnKeyType.Search;
               break;
            case ReturnType.Send:
               Control.ReturnKeyType = UIReturnKeyType.Send;
               break;
            default:
               Control.ReturnKeyType = UIReturnKeyType.Done;
               break;
         }
      }

      private void SetFloatingHintEnabled()
      {
         Control.FloatingLabelEnabled = Element.FloatingHintEnabled;
      }

      public void SetErrorDisplay()
      {
         switch (Element.ErrorDisplay)
         {
            case ErrorDisplay.None:
               Control.UnderlineErrorSpaceEnabled = false;
               Control.UnderlineErrorTextIsVisible = false;
               break;
            case ErrorDisplay.Underline:
               Control.UnderlineErrorSpaceEnabled = true;
               _hasError = !string.IsNullOrEmpty(Element.ErrorText);
               Control.UnderlineErrorTextIsVisible = _hasError;
               break;
         }
      }

      private void SetErrorText()
      {
         _hasError = !string.IsNullOrEmpty(Element.ErrorText);
         Control.UnderlineColor = GetUnderlineColorForState();
         Control.UnderlineErrorTextIsVisible = _hasError && (Element.ErrorDisplay == ErrorDisplay.Underline);
         Control.ErrorText = Element.ErrorText;
      }

      private CGColor GetUnderlineColorForState()
      {
         if (_hasError) return UIColor.Red.CGColor;
         return _hasFocus
             ? (Element.ActivePlaceholderColor == Color.Accent
                 ? Control.TintColor.CGColor
                 : Element.ActivePlaceholderColor.ToCGColor())
             : (Element.PlaceholderColor == Color.Default
                 ? Control.TextColor.CGColor
                 : Element.PlaceholderColor.ToCGColor());
      }

      private void SetBackgroundColor()
      {
         NativeView.BackgroundColor = Element.BackgroundColor.ToUIColor();
         Control.UnderlineColor = Element.PlaceholderColor.ToCGColor();
      }

      private void SetText()
      {
         if (Control.Text != Element.Text)
            Control.Text = Element.Text;
      }

      private void SetIsPassword()
      {
         if (Element.IsPassword && Control.IsFirstResponder)
         {
            Control.Enabled = false;
            Control.SecureTextEntry = true;
            Control.Enabled = Element.IsEnabled;
            Control.BecomeFirstResponder();
         }
         else
         {
            Control.SecureTextEntry = Element.IsPassword;
         }
      }

      private void SetHintText()
      {
         Control.Placeholder = Element.Placeholder;
      }

      // taken from Xamarin.Forms codebase
      private void SetPlaceholderColor()
      {
         var formatted = (FormattedString)Element.Placeholder;

         if (formatted == null)
            return;

         var targetColor = Element.PlaceholderColor;

         // Placeholder default color is 70% gray
         // https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UITextField_Class/index.html#//apple_ref/occ/instp/UITextField/placeholder

         var color = Element.IsEnabled && !targetColor.IsDefault ? targetColor : ColorExtensions.SeventyPercentGrey.ToColor();
         Control.AttributedPlaceholder = formatted.ToAttributed(Element, color);
      }

      private void SetTextColor()
      {
         if ((Element.TextColor == Color.Default) || !Element.IsEnabled)
            Control.TextColor = _defaultTextColor;
         else
            Control.TextColor = Element.TextColor.ToUIColor();
      }

      private void SetFont()
      {
         Control.Font = Element.ToUIFont();
      }

      private void SetHorizontalTextAlignment()
      {
         switch (Element.HorizontalTextAlignment)
         {
            case TextAlignment.Center:
               Control.TextAlignment = UITextAlignment.Center;
               break;
            case TextAlignment.End:
               Control.TextAlignment = UITextAlignment.Right;
               break;
            default:
               Control.TextAlignment = UITextAlignment.Left;
               break;
         }
      }

      private void SetKeyboard()
      {
         var kbd = Element.Keyboard.ToNative();
         Control.KeyboardType = kbd;
         Control.InputAccessoryView = kbd == UIKeyboardType.NumberPad ? NumberpadAccessoryView() : null;
         Control.ShouldReturn = InvokeCompleted;
      }

      private UIToolbar NumberpadAccessoryView()
      {
         return new UIToolbar(new System.Drawing.RectangleF(0.0f, 0.0f, (float)Control.Frame.Size.Width, 44.0f))
         {
            Items = new[]
             {
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { InvokeCompleted(null); })
                }
         };
      }

      private bool InvokeCompleted(UITextField textField)
      {
         Control.ResignFirstResponder();
         ((IEntryController)Element).SendCompleted();
         return true;
      }
   }
}