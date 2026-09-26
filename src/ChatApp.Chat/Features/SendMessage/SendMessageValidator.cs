namespace ChatApp.Chat.Features.SendMessage;

public class SendMessageValidator // TODO: FluentValidation later
{
    public bool Validate(SendMessageCommand command)
    {
        if (command.Content == null || command.Content.Length == 0) // TODO: allow message to be empty only if it has attachments
        {
            return false;
        }

        if (command.Content.Length > 4096) // TODO: move out to config
        {
            return false;
        }

        // var allowedTypes = new List<MessageType>
        // {
        //     MessageType.Text,
        //     MessageType.WithMediaAttachments,
        //     MessageType.System,
        // };

        // if (!allowedTypes.Contains(command.Type))
        // {
        //     return false;
        // }

        return true;
    }
}
