using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.SendMessage;

public class SendMessageValidator // TODO: FluentValidation later
{
    public bool Validate(SendMessageRequest request)
    {
        if (request.Content == null || request.Content.Length == 0) // TODO: allow message to be empty only if it has attachments
        {
            return false;
        }

        if (request.Content.Length > 4096)  // TODO: move out to config
        {
            return false;
        }

        var allowedTypes = new List<MessageType>
        {
            MessageType.Text,
            MessageType.WithMediaAttachments,
            MessageType.System,
            MessageType.WithFileAttachments
        };

        if (!allowedTypes.Contains(request.Type))
        {
            return false;
        }

        return true;
    }
}