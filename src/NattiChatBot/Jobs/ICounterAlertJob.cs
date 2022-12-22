namespace NattiChatBot.Jobs;

public interface ICounterAlertJob
{
    Task EnableCounters();
    Task DisableCounters();
}