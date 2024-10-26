/*
// BotProcess.cs / pmcenter project / https://github.com/Elepover/pmcenter
// Main processing logic of pmcenter.
// Copyright (C) The pmcenter authors. Licensed under the Apache License (Version 2.0).
*/

using pmcenter.Commands;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static pmcenter.Methods.Logging;

namespace pmcenter
{
    public static partial class BotProcess
    {
        private static readonly CommandRouter commandManager = new CommandRouter();

        static BotProcess()
        {
            commandManager.RegisterCommand(new StartCommand());
            commandManager.RegisterCommand(new HelpCommand());

            commandManager.RegisterCommand(new InfoCommand());
            commandManager.RegisterCommand(new BanCommand());
            commandManager.RegisterCommand(new PardonCommand());

            commandManager.RegisterCommand(new AutoSaveCommand());
            commandManager.RegisterCommand(new BackupConfCommand());
            commandManager.RegisterCommand(new BanIdCommand());
            commandManager.RegisterCommand(new CatConfigCommand());
            commandManager.RegisterCommand(new ChatCommand());
            commandManager.RegisterCommand(new CheckUpdateCommand());
            commandManager.RegisterCommand(new ClearMessageLinksCommand());
            commandManager.RegisterCommand(new DetectPermissionCommand());
            commandManager.RegisterCommand(new DonateCommand());
            commandManager.RegisterCommand(new EditConfCommand());
            commandManager.RegisterCommand(new GetStatsCommand());
            commandManager.RegisterCommand(new PardonIdCommand());
            commandManager.RegisterCommand(new PerformCommand());
            commandManager.RegisterCommand(new PingCommand());
            commandManager.RegisterCommand(new ReadConfigCommand());
            commandManager.RegisterCommand(new ResetConfCommand());
            commandManager.RegisterCommand(new RestartCommand());
            commandManager.RegisterCommand(new RetractCommand());
            commandManager.RegisterCommand(new SaveConfigCommand());
            commandManager.RegisterCommand(new StatusCommand());
            commandManager.RegisterCommand(new StopChatCommand());
            commandManager.RegisterCommand(new SwitchBwCommand());
            commandManager.RegisterCommand(new SwitchFwCommand());
            commandManager.RegisterCommand(new SwitchLangCodeCommand());
            commandManager.RegisterCommand(new SwitchLangCommand());
            commandManager.RegisterCommand(new SwitchNotificationCommand());
            commandManager.RegisterCommand(new TestNetworkCommand());
            commandManager.RegisterCommand(new UpdateCommand());
            commandManager.RegisterCommand(new UptimeCommand());
        }

        public static async Task OnUpdate(Update update)
        {
            try
            {
                if (Vars.CurrentConf.DetailedMsgLogging && update.Type == UpdateType.Message)
                    Log($"OnUpdate() triggered: UpdType: {update.Type}, UpdID: {update.Id}, ChatId: {update.Message.Chat.Id}, Username: {update.Message.Chat.Username}, FromID: {update.Message.From.Id}, FromUsername: {update.Message.From.Username}", "BOT-DETAILED", LogLevel.Info);

                switch (update.Type)
                {
                    case UpdateType.Message: await MessageRoute(update); break;
                    case UpdateType.CallbackQuery: await CallbackQueryRoute(update); break;
                    default:
                        if (Vars.CurrentConf.DetailedMsgLogging)
                            Log($"Ditching unknown update type ({update.Type})...", "BOT-DETAILED");
                        return;
                }
            }
            catch (Exception ex)
            {
                Log($"General error while processing incoming update: {ex}", "BOT", LogLevel.Error);
                if (Vars.CurrentConf.CatchAllExceptions)
                {
                    try
                    {

                        _ = await Vars.Bot.SendTextMessageAsync(
                            chatId: Vars.CurrentConf.OwnerUID,
                            text: Vars.CurrentLang.Message_GeneralFailure.Replace("$1", ex.ToString()),
                            parseMode: ParseMode.Markdown,
                            protectContent: false,
                            disableNotification: Vars.CurrentConf.DisableNotifications
                        ).ConfigureAwait(false);
                    }
                    catch (Exception iEx)
                    {
                        Log($"Failed to catch exception to owner: {iEx}", "BOT", LogLevel.Error);
                    }
                }
            }
        }

        public static async Task OnError(Exception exception, HandleErrorSource source)
        {
            Log($"polling update failure: {exception.Message}, source: {source.ToString()}");
        }
    }
}
