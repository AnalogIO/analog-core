# Analog Coffee Card API

![Build and test solution](https://github.com/AnalogIO/analog-core/workflows/Build%20and%20test%20solution/badge.svg) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AnalogIO_analog-core&metric=alert_status)](https://sonarcloud.io/dashboard?id=AnalogIO_analog-core)

**Contact** AnalogIO at *support [at] analogio.dk*

Analog Coffee Card API handles clip card sale, user management and statistics for coffee clip cards for the apps in Cafe Analog. The API is setup as a classic ASP.NET core solution, currently running .net core 2.2.

## Configuration and running the solution

In order to build the solution, the following prerequisuites are expected to be present in the root of the `CoffeeCard` project folder:

- Application configuration (`appsettings.json`)
- MobilePay cerifitcate for backend communications

## Contributing

We are invite everyone to report issues and create pull requests to the repository. In order to align our implementations, we try to follow the principles from this guide : <https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc?view=aspnetcore-2.2>.

### Git branch structure

We follow a [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) inspired setup. The branch structure is as following :

- `develop` **(Main  branch)** The develop branch is the main branches. All new branches must check out from here into feature branches and merged back into develop. The contents of the development branch would usually reflect what is deployed to the test environment.
- `production` The production branch reflects the current deployment in production. The production branch is merged with the develop branch every time a new version deployed to production.
- `feature/{author}/{feature-name}` New features are developed on feature branches following the *feature / author name / feature name branch* structure.

### Merging with the develop branch

**A pull request must be created and approved before merging with develop!**

We use a **rebase** strategy when merging to develop. When a feature has been finished in development, the feature branch must be rebased with develop before merging in order to avoid ugly merge commits.

A rebase merge to develop can be done in command line

```bash
git fetch
git checkout feature/author/feature-name # checkout the feature branch
git rebase origin/develop # rebase with remote develop branch
# resolve any conflicts
git checkout develop
git merge feature/author/feature-name
git push
```
