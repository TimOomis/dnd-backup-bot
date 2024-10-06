using CharacterSheets.Adapters.Discord.Configuration;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.UseCases.Interfaces;
using Discord;
using Discord.Interactions;

namespace CharacterSheets.Adapters.Discord.Commands;

public class BackupCommand(IGetPartyCharacterSheetsUseCase getPartyCharacterSheetsUseCase, DiscordSettings settings) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("backup", "Backs up character sheets")]
    public async Task Backup()
    {
        await DeferAsync(options: new RequestOptions
        {
            Timeout = 30, // Seconds
        });

        var sheets = await getPartyCharacterSheetsUseCase.Execute();
        _ = sheets;

        var attachments = sheets.Select(CreateAttachment).ToList();

        await Context.Channel.SendMessageAsync(embed: CreateEmbed());

        await FollowupWithFilesAsync(attachments);

        // Clean up streams properly.
        foreach (var attachment in attachments)
        {
            await attachment.Stream.DisposeAsync();
        }
    }

    private Embed CreateEmbed()
    {
        var embedBuilder = new EmbedBuilder();

        embedBuilder.WithColor(Color.Green);
        embedBuilder.WithTitle("Archival completed!");
        embedBuilder.WithDescription("""
            'Worry not, brave adventurers. Your scrolls, etched with the tales of your deeds and fates yet untold, are now secure within the Vault of Heroes
            Here, in the deepest chambers of the archive, not even the passage of time shall touch them. Go forth, and may your next quest be as legendary as the last!'
            """);

        embedBuilder.WithImageUrl(settings.ImageUrl);

        return embedBuilder.Build();
    }

    private FileAttachment CreateAttachment(CharacterSheet sheet)
    {
        var stream = new MemoryStream(sheet.Data)
        {
            Position = 0
        };

        return new FileAttachment(
            stream: stream,
            fileName: sheet.FileName,
            description: $"Character sheet for {sheet.CharacterName}, saved at <t:{DateTime.UtcNow.Ticks}:R>");
    }
}
