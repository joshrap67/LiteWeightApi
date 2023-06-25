# Overview

This REST API provides functionality for the mobile
application [LiteWeight](https://play.google.com/store/apps/details?id=com.joshrap.liteweight&hl=en_US&gl=US). While
that is the primary consumer, you can still consume this API if you follow the [authentication steps](#authentication).

## Authentication

All endpoints in this API require a properly issued id token (JWT). This JWT can only be received if you have an
Firebase account registered for LiteWeight. This can be received outside the app by utilizing the Firebase identity API
endpoint https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword

The authentication scheme is a Bearer token one: `Bearer <token>`

## Authorization

The token used in every request specifies a user id. This user id corresponds to the user id of the authenticated user
in firebase. Thus, the only data that can be modified in the request is the data that belongs to the authenticated user.
Any attempt to modify the data of other users will result in a Forbidden response.

Additionally, this API requires that the authenticated user has a verified email. If the email is not yet verified, then
a forbidden response will be returned.

## Error Types

Some methods return 400 responses. These 400 responses have explicitly defined codes that define the type of error that
occurred, e.g. "UserNotFound". This can facilitate better error handling for the consumer of this API.

## Push Notifications
For some methods, successful execution will result in a push notification being sent. The push notification is sent via Firebase Cloud Messaging, and the recipient device token is determined from the context of the method. E.g. for Send Friend request the device token is that of the recipient of the friend request.