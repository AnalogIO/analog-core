## Contributing

We are inviting everyone to report issues and create pull requests to the repository. In order to align our implementations, we try to follow the principles from this guide: <https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc?view=aspnetcore-3.1>.

### Git branch structure

We follow a [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) inspired setup. The branch structure is as following :

- `develop` **(Main  branch)** The develop branch is the main branches. All new branches must check out from here into feature branches and merged back into develop. The contents of the development branch would usually reflect what is deployed to the test environment.
- `production` The production branch reflects the current deployment in production. The production branch is merged with the develop branch every time a new version is deployed to production.
- `feature/{author}/{feature-name}` New features are developed on feature branches following the *feature / author name / feature name branch* structure.

### Merging with the develop branch

**A pull request must be created and approved before merging with develop!**

We use a **rebase** strategy when merging to develop. When a feature has been finished in development, the feature branch must be rebased with develop before merging in order to avoid ugly merge commits.

A rebase-merge to develop can be done in command line

```bash
git fetch
git checkout feature/author/feature-name # checkout the feature branch
git rebase origin/develop # rebase with remote develop branch
# resolve any conflicts
git checkout develop
git merge feature/author/feature-name
git push
```