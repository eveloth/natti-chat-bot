namespace NattiChatBot.Options;

public class ChatConfigOptions
{
    public const string ChatConfig = "ChatConfig";

    public string StickerId { get; set; } = default!;
    public List<string> BannedWords { get; set; }
}