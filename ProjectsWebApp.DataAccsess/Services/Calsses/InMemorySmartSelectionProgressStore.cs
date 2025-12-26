using System;
using System.Collections.Concurrent;
using ProjectsWebApp.DataAccsess.Services.Interfaces;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public sealed class InMemorySmartSelectionProgressStore : ISmartSelectionProgressStore
    {
        private readonly ConcurrentDictionary<string, SmartSelectionProgressDto> _map = new();

        public void Set(string opId, SmartSelectionProgressDto progress)
        {
            if (string.IsNullOrWhiteSpace(opId)) return;
            _map[opId] = progress with { UpdatedAt = DateTimeOffset.UtcNow };
        }

        public SmartSelectionProgressDto? Get(string opId)
        {
            if (string.IsNullOrWhiteSpace(opId)) return null;
            _map.TryGetValue(opId, out var p);
            return p;
        }

        public void Remove(string opId)
        {
            if (string.IsNullOrWhiteSpace(opId)) return;
            _map.TryRemove(opId, out _);
        }
    }

    public sealed class SmartSelectionProgressNotifier : ISmartSelectionProgressNotifier
    {
        private readonly ISmartSelectionProgressStore _store;
        public SmartSelectionProgressNotifier(ISmartSelectionProgressStore store) => _store = store;

        public void Report(string opId, SmartSelectionStage stage, string step, string message, int index, int total)
        {
            if (string.IsNullOrWhiteSpace(opId)) return;
            var dto = new SmartSelectionProgressDto(
                OpId: opId,
                Stage: stage,
                Step: step,
                Message: message ?? string.Empty,
                Index: index,
                Total: total,
                UpdatedAt: DateTimeOffset.UtcNow);
            _store.Set(opId, dto);
        }
    }
}
