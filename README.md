# natti-chat-bot
A welcoming bot for t.me//natti_jun_front. Currently greets every new user with a DiscoElysium sticker.

## Installation

The easiest way to run this bot is pulling a docker image:

```bash
sudo docker pull eveloth/natti-chat-bot:{specify-tag}

sudo docker run \
  --name natti-chat-bot \
  -d \
  -e "BotConfiguration:BotToken={your-token}" \
  -e "BotConfiguration:HostAddress={your-domain}" \
  -e "BotConfiguration:SecretToken={e.g.-random-guid}" \
  -p 443:443 \
  eveloth/natti-chat-bot:{specify-tag}
```
**NOTE:** you MUST have an SSL certificate! Otherwise webhook won't be set. See: https://core.telegram.org/bots/webhooks

This bot is configured to run behind nginx as well. You should user docker port 80 for that (e.g. -p 5000:80).

Learn nginx basics here: https://www.freecodecamp.org/news/the-nginx-handbook/

Learn how to host ASP.NET Core web apps behind nginx: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-7.0
