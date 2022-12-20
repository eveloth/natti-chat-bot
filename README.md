# natti-chat-bot
A welcoming bot for t.me//natti_jun_front. Currently greets every new user with a DiscoElysium sticker.

## Installation

The easiest way to run this bot is pulling a docker image:

```bash
sudo docker pull eveloth/natti-chat-bot:{specify-tag}

sudo docker run \
  --name natti-chat-bot \
  -d \
  -e "BotConfiguration:BotToken={your-token} \
  -e "BotConfiguration:HostAddress={your-domain} \
  -e "BotConfiguration:SecretToken={e.g.-random-guid} \
  -p 443:443 \
  eveloth/natti-chat-bot:{specify-tag}
```
