using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Persistence.DTOs;

namespace TelegramBot.Services.FileSenderStrategy;

public class MediaGroupFileSender : IFileSender
{
    public async Task SendFileAsync(List<AttachmentDto> attachments, long chatId, ITelegramBotClient botClient, string? caption = null)
    {
        var mediaGroup = new List<IAlbumInputMedia>();

        foreach (var attachment in attachments)
        {
            var fileStream = new MemoryStream(Convert.FromBase64String(attachment.data));
            IAlbumInputMedia inputMedia = attachment.type.Split('.').Last() switch
            {
                "jpg" or "jpeg" or "png" => new InputMediaPhoto(new InputFileStream(fileStream, $"{Guid.NewGuid()}.{attachment.type}")){Caption = ""},
                "mp4" => new InputMediaVideo(new InputFileStream(fileStream, $"{Guid.NewGuid()}.{attachment.type}")){Caption = ""},
                _ => new InputMediaDocument(new InputFileStream(fileStream, $"{Guid.NewGuid()}.{attachment.type}")){Caption = ""}
                    
            };
            if (mediaGroup.Count == 0)
            {
                if (inputMedia is InputMediaPhoto photo)
                {
                    photo.Caption = caption;
                    photo.ParseMode = ParseMode.MarkdownV2;
                }
                if (inputMedia is InputMediaVideo video)
                {
                    video.Caption = caption;
                    video.ParseMode = ParseMode.MarkdownV2;
                }
                if (inputMedia is InputMediaDocument document)
                {
                    document.Caption = caption;
                    document.ParseMode = ParseMode.MarkdownV2;
                }
            }
            mediaGroup.Add(inputMedia);
            
        }
        await botClient.SendMediaGroup(chatId, mediaGroup);
    }
}