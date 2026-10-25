using OpenSteps.Core.Models;

namespace OpenSteps.Core.Services;

public sealed class StepTitleGenerator
{
    public string Generate(RecordedStep step)
    {
        return GenerateWithReason(step).Title;
    }

    public StepTitleResult GenerateWithReason(RecordedStep step)
    {
        if (step.ActionType == StepActionType.Shortcut && !string.IsNullOrWhiteSpace(step.ShortcutName))
        {
            return new StepTitleResult(Loc.Format("Title_Press", step.ShortcutName), "keyboard shortcut");
        }

        if (step.ActionType == StepActionType.SpecialKey && !string.IsNullOrWhiteSpace(step.SpecialKeyName))
        {
            return new StepTitleResult(Loc.Format("Title_Press", step.SpecialKeyName), "special key");
        }

        if (step.ActionType == StepActionType.TextEntry)
        {
            if (step.IsSensitiveInput)
            {
                return new StepTitleResult(Loc.Get("Title_EnterHiddenText"), "sensitive text entry");
            }

            var target = Clean(step.InputTargetName);
            if (!string.IsNullOrWhiteSpace(target))
            {
                return new StepTitleResult(Loc.Format("Title_TypeIntoField", Quote(target)), "named text input target");
            }

            if (!string.IsNullOrWhiteSpace(step.WindowTitle))
            {
                return new StepTitleResult(Loc.Format("Title_EnterTextIn", step.WindowTitle), "text entry window title fallback");
            }

            if (!string.IsNullOrWhiteSpace(step.ProcessName))
            {
                return new StepTitleResult(Loc.Format("Title_EnterTextIn", step.ProcessName), "text entry process name fallback");
            }

            return new StepTitleResult(Loc.Get("Title_EnterText"), "text entry fallback");
        }

        var name = Clean(step.ElementName);
        var controlType = Clean(step.ControlType);
        var verb = GetPointerVerb(step.ActionType);

        if (step.UsefulElementFound && !string.IsNullOrWhiteSpace(name))
        {
            if (IsControlType(controlType, "Button"))
            {
                return new StepTitleResult(Loc.Format("Title_PointerButton", verb, Quote(name)), $"useful Button element name for {step.ActionType}");
            }

            if (IsControlType(controlType, "MenuItem"))
            {
                return step.ActionType == StepActionType.Click
                    ? new StepTitleResult(Loc.Format("Title_OpenMenu", Quote(name)), "useful MenuItem element name")
                    : new StepTitleResult(Loc.Format("Title_PointerMenu", verb, Quote(name)), $"useful MenuItem element name for {step.ActionType}");
            }

            if (IsControlType(controlType, "Edit"))
            {
                return new StepTitleResult(Loc.Format("Title_PointerField", verb, Quote(name)), $"useful Edit element name for {step.ActionType}");
            }

            return new StepTitleResult(Loc.Format("Title_PointerItem", verb, Quote(name)), $"useful UI Automation element name for {step.ActionType}");
        }

        if (!string.IsNullOrWhiteSpace(step.WindowTitle))
        {
            return new StepTitleResult(Loc.Format("Title_PointerIn", verb, step.WindowTitle), "window title fallback");
        }

        if (!string.IsNullOrWhiteSpace(step.ProcessName))
        {
            return new StepTitleResult(Loc.Format("Title_PointerIn", verb, step.ProcessName), "process name fallback");
        }

        return new StepTitleResult(Loc.Format("Title_PointerAtPosition", verb, step.ClickX, step.ClickY), "coordinate fallback");
    }

    private static string GetPointerVerb(StepActionType actionType)
    {
        return actionType switch
        {
            StepActionType.RightClick => Loc.Get("Verb_RightClick"),
            StepActionType.DoubleClick => Loc.Get("Verb_DoubleClick"),
            _ => Loc.Get("Verb_Click")
        };
    }

    private static string Quote(string value)
    {
        return value.Contains('\'') ? $"\"{value}\"" : $"'{value}'";
    }

    private static bool IsControlType(string? actual, string expected)
    {
        return actual?.Contains(expected, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

public sealed record StepTitleResult(string Title, string Reason);
