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

## User Setup
With Winged-Keys being our IAM service, using it to get set up with account information and credentials to our various environments is a relatively simple process.

1. Spin up the application in a local Docker container, following the instructions specified above.
1. Once up, navigate to the login page (https://0.0.0.0:5050/Account/Login) and log in with the global Harry Potter credentials (just ask if you don't already have them).
1. Upon login, navigate to the account creation page (https://0.0.0.0:5050/Admin/NewAccount).
1. Fill out all of the account details on the form within that page, and submit the form.
1. Once created successfully, take a look at the `AspNetUsers` table in the winged-keys database in Docker.  You should see your newly created account details in there.
1. Copy all of the applicable data points into the [Hedwig](https://github.com/skylight-hq/ctoec-devops/blob/master/sql/hedwig/setup_team_skylight_users_hedwig.sql) and [Winged Keys](https://github.com/skylight-hq/ctoec-devops/blob/master/sql/wingedkeys/setup_team_skylight_users_wingedkeys.sql) user setup SQL scripts in the `ctoec-devops` repo.
1. Congratulations!  The next time infrastructure is deployed to a particular environment, your user data will be included in that deploy.  Please note that this will be your account information **across environments**, so be sure to keep track of it.

## Deploy
Deployment of all stages is currently handled through Azure DevOps release pipelines.

### Cross-Origin Resource Sharing
Whenever a new application is added to the OEC ECE ecosystem that requires authentication through winged-keys, that application's URL will need to be **manually added** to the `ClientUrls` environment variable in the corresponding Azure DevOps release pipelines across all applicable stages.  This will ensure winged-keys generates `Access-Control-Allow-Origin` headers to be attached to corresponding responses, necessary for actual access to winged-keys across domains.
