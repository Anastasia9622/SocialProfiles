using System.Text.RegularExpressions;
using SocialProfiles.Core.Domain.Exceptions;

namespace SocialProfiles.Core.Domain.Profiles;

public class Theme
{
    // Шаблон: "#" і рівно 6 символів 0-9, a-f, A-F. Наприклад, #1b1033.
    private static readonly Regex HexColor = new(@"^#[0-9a-fA-F]{6}$");

    public string BackgroundColor { get; private set; }
    public string CardColor { get; private set; }
    public string TextColor { get; private set; }
    public string AccentColor { get; private set; }

    public Theme(string backgroundColor, string cardColor, string textColor, string accentColor)
    {
        BackgroundColor = backgroundColor;
        CardColor = cardColor;
        TextColor = textColor;
        AccentColor = accentColor;

        if (!Validate())
            throw new DomainException("Тема містить некоректний колір. Використовуйте формат #RRGGBB.");
    }

    public bool Validate()
    {
        var colors = new[] { BackgroundColor, CardColor, TextColor, AccentColor };
        if (ContrastRatio() < 4.5)
            throw new DomainException("Занадто низький контраст між текстом і фоном.");
        return colors.All(IsValidColor);
    }

    private static bool IsValidColor(string? color) =>
        !string.IsNullOrEmpty(color) && HexColor.IsMatch(color);

    // Рядок для вставки в style="..." у Blazor-компоненті.
    public string ToCssVariables() =>
        $"--bg-color:{BackgroundColor};--card-color:{CardColor};" +
        $"--text-color:{TextColor};--accent-color:{AccentColor}";

    public double ContrastRatio() //ratio = (Lсвітліше + 0.05) / (Lтемніше + 0.05).
    {
        double textLuminance = RelativeLuminance(TextColor);
        double backgroundLuminance = RelativeLuminance(BackgroundColor);

        double lighter = Math.Max(textLuminance, backgroundLuminance);
        double darker = Math.Min(textLuminance, backgroundLuminance);

        return (lighter + 0.05) / (darker + 0.05);

    }
    private static double RelativeLuminance(string hex)
    {
        int redChannel = Convert.ToInt32(hex.Substring(1, 2), 16);
        int greenChannel = Convert.ToInt32(hex.Substring(3, 2), 16);
        int blueChannel = Convert.ToInt32(hex.Substring(5, 2), 16);

        double r = ToLinear(redChannel);
        double g = ToLinear(greenChannel);
        double b = ToLinear(blueChannel);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static double ToLinear(int channel)
    {
        double c = channel / 255.0;
        double result;
        if (c <= 0.03928)
        {
            result = c / 12.92;
        }
        else
        {
            result = Math.Pow((c + 0.055) / 1.055, 2.4);
        }
        return result;
    }
}


