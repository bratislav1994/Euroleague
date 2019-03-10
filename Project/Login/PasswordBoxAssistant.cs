using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project.Login
{
    public static class PasswordBoxAssistant
    {
        /// <summary>
        /// dependency property for bound password
        /// </summary>
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxAssistant), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        /// dependency property for bind password
        /// </summary>
        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false, OnBindPasswordChanged));

        /// <summary>
        /// dependency property for update password
        /// </summary>
        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false));

        /// <summary>
        /// Set bind password
        /// </summary>
        /// <param name="dp">password box</param>
        /// <param name="value">new value</param>
        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        /// <summary>
        /// Get bind password
        /// </summary>
        /// <param name="dp">password box</param>
        /// <returns>is password bind</returns>
        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        /// <summary>
        /// Get bound password
        /// </summary>
        /// <param name="dp">password box</param>
        /// <returns>bound password</returns>
        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        /// <summary>
        /// Set bound password
        /// </summary>
        /// <param name="dp">password box</param>
        /// <param name="value">value of password</param>
        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        /// <summary>
        /// Get updating password
        /// </summary>
        /// <param name="dp">password box</param>
        /// <returns>is password updated</returns>
        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        /// <summary>
        /// Set updating password
        /// </summary>
        /// <param name="dp">password box</param>
        /// <param name="value">is changed</param>
        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }

        /// <summary>
        /// only handle this event when the property is attached to a PasswordBox and when the BindPassword attached property has been set to true
        /// </summary>
        /// <param name="d">password box</param>
        /// <param name="e">event args</param>
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        /// <summary>
        /// when the BindPassword attached property is set on a PasswordBox, start listening to its PasswordChanged event
        /// </summary>
        /// <param name="dp">name of password box</param>
        /// <param name="e">event args</param>
        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)e.OldValue;
            bool needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        /// <summary>
        /// Handle when password box is changed
        /// </summary>
        /// <param name="sender">password box</param>
        /// <param name="e">event args</param>
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            //// set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);
            //// push the new password into the BoundPassword property
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }
    }
}