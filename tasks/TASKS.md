## priority
- [ ] direct chat

## scope
- [ ] signalr hub - groups, backplane, probably remove RedisChatPersistence
- [ ] Endpoints
    - [x] send message endpoint
    - [x] remove hub registration from ChatEndpoints
    - [ ] finish the rest

## plans
- [ ] leave chat
- [ ] media
- [ ] indexes
- [ ] update user profile

## done
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
