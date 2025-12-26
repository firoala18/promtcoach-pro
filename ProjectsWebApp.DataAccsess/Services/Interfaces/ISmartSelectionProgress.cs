using System;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public enum SmartSelectionStage
    {
        Start = 0,
        LexiconLoading = 1,
        LexiconTrimmed = 2,
        SelectionSending = 3,
        SelectionReceived = 4,
        Resolved = 5,
        EvaluationSending = 6,
        EvaluationReceived = 7,
        Done = 8
    }

    public record SmartSelectionProgressDto
    (
        string OpId,
        SmartSelectionStage Stage,
        string Step,
        string Message,
        int Index,
        int Total,
        DateTimeOffset UpdatedAt
    );

    public interface ISmartSelectionProgressStore
    {
        void Set(string opId, SmartSelectionProgressDto progress);
        SmartSelectionProgressDto? Get(string opId);
        void Remove(string opId);
    }

    public interface ISmartSelectionProgressNotifier
    {
        void Report(string opId, SmartSelectionStage stage, string step, string message, int index, int total);
    }
}
