using NattiChatBot.Domain.Enums;

namespace NattiChatBot.Contracts.Requests;

public record TokenRequest(DateTime ExpiresAt, string GrantedTo, AccessType AccessType);