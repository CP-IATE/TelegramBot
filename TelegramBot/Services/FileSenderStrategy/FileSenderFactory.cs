namespace TelegramBot.Services.FileSenderStrategy;

public class FileSenderFactory
{
    public static IFileSender GetFileSender(string type)
    {
        return type switch
        {
            "jpg" => new PhotoFileSender(),
            "mp3" => new AudioFileSender(),
            "mp4" => new VideoFileSender(),
            "oga" => new VoiceFileSender(),
            _ => new GeneralFileSender()
        };
    }
}