using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace REITS.Infrastructure.Behaviors
{
    public class WholeTextBoxRegExBehaviour : Behavior<TextBox>
    {
        public static readonly DependencyProperty RegExpressionProperty =
            DependencyProperty.Register("RegExpression", typeof(String), typeof(WholeTextBoxRegExBehaviour),
            new FrameworkPropertyMetadata("*"));

        public static readonly DependencyProperty AllowWhitespaceProperty =
        DependencyProperty.Register("AllowWhitespace", typeof(bool), typeof(WholeTextBoxRegExBehaviour),
        new FrameworkPropertyMetadata(false));

        public string RegExpression
        {
            get { return (string)GetValue(RegExpressionProperty); }
            set { SetValue(RegExpressionProperty, value); }
        }

        public bool AllowWhitespace
        {
            get { return (bool)GetValue(AllowWhitespaceProperty); }
            set { SetValue(AllowWhitespaceProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                TextBox textbox = (TextBox)sender;
                string inputText = Convert.ToString(e.DataObject.GetData(DataFormats.Text));
                string textResult = GetResultOfInput(textbox, inputText);

                if (!IsValid(textResult))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                string inputText = e.Text;
                string textResult = GetResultOfInput(textbox, inputText);

                e.Handled = !IsValid(textResult);
            }
            else
                e.Handled = true;
        }

        private string GetResultOfInput(TextBox textbox, string inputText)
        {
            string text = textbox.Text;
            int position = textbox.SelectionStart;
            int length = textbox.SelectionLength;

            if (length > 0)
                text = textbox.Text.Remove(position, length);

            text = text.Insert(position, inputText);

            if (AllowWhitespace)
                text = text.Replace(" ", "");

            return text;
        }

        private bool IsValid(string newText)
        {
            return Regex.IsMatch(newText, RegExpression);
        }
    }
}