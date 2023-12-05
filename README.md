# Serendipity
https://serendipity.fly.dev/

Serendipity is a social media platform designed for Spotify users. Serendipity connect connects users with other users listening to the same song. Users can view each other's profiles, follow, and message in real-time. The following is a brief outline of the main technologies used.


## Architecture
### Backend
**• Framework:** ASP.NET Core

**• Authentication:** Incorporates JWT-based authentication, uses Microsoft.AspNetCore.Identity for managing user identities and security.

**• Entity Framework:** Used for ORM, integrated with ASP.NET Core Identity for user management.

**• REST Based API:** Stateless manipulation of resources.

**• SignalR:** Employs real-time communication for user presence, messaging, and Spotify data updates over websockets.

### Frontend
**• Framework:** Angular

**• Styling:** Angular Bootstrap for responsive and modern UI designs.

**• Programming Language:** TypeScript.

**• Real-time Interaction:** Integrated with SignalR hubs for immediate updates and interactivity.

### APIs
**• Spotify API:** Polls user's current music data.

**• Cloudinary API:** Manages media storage efficiently.

### Deployment
**• Containerization:** Docker for consistent deployment environments.

**• Hosting:** Fly.io for cloud-based hosting.

**• Database:** PostgreSQL for data management.


## Known Issues:
**Spotify Users:** Until Spotify grants Serendipity full deployment, it remains in development mode. To use the app, email the email address associated with your Spotify account to noahplotkin1@gmail.com to be added to the allowed users list, which will allow you to log in with Spotify.

**Bootstrap Datepicker:** The Bootstrap datepicker lists extra nonsensical dates alongside the left hand column of dates in the datepicker.

