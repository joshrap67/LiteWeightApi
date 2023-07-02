# LiteWeight API

This web service exposes a REST API intended to be consumed by the Android Application [Liteweight](https://github.com/joshrap67/LiteWeight) (compatible with Android versions >= 3.x.x)

[API References](https://storage.googleapis.com/liteweight-api-documentation/apiDocs.html)

Refer to the [Wiki](https://github.com/joshrap67/LiteWeightApi/wiki) for details on the implementation of the service.

## Prerequisites

.Net 7 is used for the C# projects in this repository.

Firebase credentials must be installed locally.

Docker must be installed in order to build the container image to deploy.

Below environment variables must be set

- LiteWeight_Firebase__ProjectId
- LiteWeight_Firestore__ComplaintsCollection
- LiteWeight_Firestore__ReceivedWorkoutsCollection
- LiteWeight_Firestore__UsersCollection
- LiteWeight_Firestore__WorkoutsCollection
- LiteWeight_Jwt__AuthorityUrl


## Deployment

To deploy the documentation, simply run the publish powershell script. This requires google cloud credentials and the swashbuckle CLI NuGet package globally installed.

## Authors

- Joshua Rapoport - *Creator and Lead Software Developer*
