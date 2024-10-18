using System.Text.RegularExpressions;

namespace MauiSample.Presentation.CustomViews
{
    public class RichLabel : Label
    {
        public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(
            nameof(Markdown),
            typeof(string),
            typeof(RichLabel),
            default(string),
            propertyChanged: OnMarkdownPropertyChanged);

        public static readonly BindableProperty BoldFontFamilyProperty = BindableProperty.Create(
            nameof(BoldFontFamily),
            typeof(string),
            typeof(RichLabel),
            "FontBold",
            propertyChanged: OnBoldFontFamilyPropertyChanged);

        public static readonly BindableProperty BoldColorProperty = BindableProperty.Create(
            nameof(BoldColor),
            typeof(Color),
            typeof(RichLabel),
            defaultValue: null);

        private static readonly Regex FindBoldRegex = new Regex(
            @"^(?<before>[^\\*]+)?([\\*]{2}(?<bold>[^\\*]+)[\\*]{2})(?<remainingText>.+)?",
            RegexOptions.Compiled|RegexOptions.Singleline);

        public string Markdown
        {
            get => (string)GetValue(MarkdownProperty);
            set => SetValue(MarkdownProperty, value);
        }

        public string BoldFontFamily
        {
            get => (string)GetValue(BoldFontFamilyProperty);
            set => SetValue(BoldFontFamilyProperty, value);
        }

        public Color BoldColor
        {
            get => (Color)GetValue(BoldColorProperty);
            set => SetValue(BoldColorProperty, value);
        }

        private static void OnMarkdownPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((RichLabel)bindable).ProcessMarkdown((string)newValue);
        }

        private static void OnBoldFontFamilyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }

        private void ProcessMarkdown(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return;
            }

            string remainingText = markdown;
            var formattedString = new FormattedString();
            var match = FindBoldRegex.Match(remainingText);

            while (match.Success)
            {
                string before = match.Groups[nameof(before)]
                    .Value;

                AddSpan(formattedString, FontFamily, TextColor, before);

                string bold = match.Groups[nameof(bold)]
                    .Value;

                AddSpan(formattedString, BoldFontFamily, BoldColor ?? TextColor, bold);

                remainingText = match.Groups[nameof(remainingText)]
                    .Value;

                match = FindBoldRegex.Match(remainingText);
            }

            AddSpan(formattedString, FontFamily, TextColor, remainingText);

            FormattedText = formattedString;
        }

        private void AddSpan(FormattedString formattedString, string fontFamily, Color color, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            formattedString.Spans.Add(new Span
            {
                FontFamily = fontFamily,
                Text = text,
                LineHeight = LineHeight,
                CharacterSpacing = CharacterSpacing,
                FontSize = FontSize,
                TextColor = color,
            });
        }
    }
}