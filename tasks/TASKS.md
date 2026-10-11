## priority
- [ ] FIX: cursor in GetUserChatsRoute cannot work properly with GET method
- [ ] signalr hub - groups, backplane, probably remove RedisChatPersistence

## scope
- [ ] direct chat
- [ ] Endpoints
    - [x] send message endpoint
    - [x] remove hub registration from ChatEndpoints
    - [ ] finish the rest

## plans
- [ ] create message id on front end in roder to handle message send retries
- [ ] leave chat
- [ ] media
- [ ] indexes
- [ ] update user profile

## done
- [x] FIX: chatId, string content in SendMessageRoute are taken from query
- [x] move all the servies to autoinject
- [x] user - usersnapshot sync, rabbitmq
- [x] Rebus for rabbitmq (already installed; set it up)
- [x] user profile updated message
- [x] SendMessage
- [x] SendMessage fixes
- [x] db schema
- [x] problemdetails
    - [x] register problemdetails
- [x] user signed up
- [x] outbox
    - [x] handle message duplication in message handler
    - [x] add outbox to user service
    - [x] add outbox bus
