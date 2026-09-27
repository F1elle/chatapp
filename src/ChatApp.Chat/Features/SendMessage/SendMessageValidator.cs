namespace ChatApp.Chat.Features.SendMessage;

public static class SendMessageValidator // TODO: FluentValidation later
{
    public static bool Validate(SendMessageCommand command)
    {
        if (command.Content == null || command.Content.Length == 0) // TODO: allow message to be empty only if it has attachments;
        { //no attachments yet implemented so no empty messages allowed :D
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
