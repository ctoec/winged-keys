# Winged Keys

Winged Keys is our hand-rolled .NET application for authentication and identity management.  Based largely on .NET's [IdentityServer4](https://github.com/IdentityServer/IdentityServer4), it handles things like user sign-up, user sign-in and general access control across the rest of our OEC applications.

## Development

### Local
Local development on OSX right now isn't possible, as one of our dependencies (LocalDB) isn't currently supported on OSX.  For the time being, development from directly within Docker is probably your best bet.

### Docker

1. Install [Docker](https://docs.docker.com/install/) (and maybe [Docker Compose](https://docs.docker.com/compose/install/), but it comes included with Docker Desktop for Mac).

1. Spin up the application!
    ```bash
    docker-compose up
    ```